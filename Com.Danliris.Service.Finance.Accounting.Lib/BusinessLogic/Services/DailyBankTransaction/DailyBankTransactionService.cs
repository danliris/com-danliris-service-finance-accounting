using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Helpers;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.DailyBankTransaction
{
    public class DailyBankTransactionService : IDailyBankTransactionService
    {
        private const string UserAgent = "Service";
        private readonly FinanceDbContext _DbContext;
        private readonly DbSet<DailyBankTransactionModel> _DbSet;
        private readonly DbSet<BankTransactionMonthlyBalance> _DbMonthlyBalance;
        public DailyBankTransactionService(FinanceDbContext dbContext)
        {
            _DbContext = dbContext;
            _DbSet = dbContext.Set<DailyBankTransactionModel>();
            _DbMonthlyBalance = dbContext.Set<BankTransactionMonthlyBalance>();
        }

        public async Task<int> Create(DailyBankTransactionModel model, string username)
        {
            int Created = 0;

            using (var transaction = _DbContext.Database.BeginTransaction())
            {
                try
                {
                    do
                    {
                        model.Code = CodeGenerator.Generate();
                    }
                    while (_DbSet.Any(d => d.Code.Equals(model.Code)));

                    //var previousDocument = await _DbSet.Where(w => w.AccountBankId.Equals(model.AccountBankId)).OrderByDescending(o => o.CreatedUtc).FirstOrDefaultAsync();

                    UpdateRemainingBalance(model, username);

                    //if (previousDocument != null)
                    //{
                    //    model.BeforeNominal = previousDocument.AfterNominal;
                    //    if (model.Status.ToUpper().Equals("IN"))
                    //    {
                    //        model.AfterNominal = model.BeforeNominal + model.Nominal;
                    //    }
                    //    else if (model.Status.ToUpper().Equals("OUT"))
                    //    {
                    //        model.AfterNominal = model.BeforeNominal - model.Nominal;
                    //    }
                    //}
                    //else
                    //{
                    //    if (model.Status.ToUpper().Equals("IN"))
                    //    {
                    //        model.AfterNominal += model.Nominal;
                    //    }
                    //    else if (model.Status.ToUpper().Equals("OUT"))
                    //    {
                    //        model.AfterNominal -= model.Nominal;
                    //    }
                    //}

                    EntityExtension.FlagForCreate(model, username, UserAgent);

                    _DbSet.Add(model);
                    Created = await _DbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

            return Created;
        }

        private void UpdateRemainingBalance(DailyBankTransactionModel model, string username)
        {
            var Month = model.Date.Month;
            var Year = model.Date.Year;
            var ActualBalance = _DbMonthlyBalance.Where(w => w.Month.Equals(Month) && w.Year.Equals(Year)).FirstOrDefault();

            if (ActualBalance == null)
            {
                var NewMonthBalance = new BankTransactionMonthlyBalance
                {
                    Month = Month,
                    Year = Year,
                    InitialBalance = model.Nominal,
                    RemainingBalance = model.Nominal
                };

                EntityExtension.FlagForCreate(NewMonthBalance, username, UserAgent);
                _DbMonthlyBalance.Add(NewMonthBalance);
            }
            else
            {
                var NextMonthBalance = GetNextMonthBalance(Month, Year);
                if (NextMonthBalance != null)
                {
                    if (model.Status.ToUpper().Equals("IN"))
                    {
                        NextMonthBalance.InitialBalance += model.Nominal;
                    }
                    else
                    {
                        NextMonthBalance.InitialBalance -= model.Nominal;
                    }
                    EntityExtension.FlagForUpdate(NextMonthBalance, username, UserAgent);
                    _DbMonthlyBalance.Update(NextMonthBalance);
                }

                if (model.Status.ToUpper().Equals("IN"))
                {
                    ActualBalance.RemainingBalance += model.Nominal;
                }
                else
                {
                    ActualBalance.RemainingBalance -= model.Nominal;
                }

                EntityExtension.FlagForUpdate(ActualBalance, username, UserAgent);
                _DbMonthlyBalance.Update(ActualBalance);
            }
        }

        private BankTransactionMonthlyBalance GetNextMonthBalance(int month, int year)
        {
            if (month == 12)
            {
                return _DbMonthlyBalance.Where(w => w.Month.Equals(1) && w.Year.Equals(year + 1)).FirstOrDefault();
            }
            else
            {
                return _DbMonthlyBalance.Where(w => w.Month.Equals(month + 1) && w.Year.Equals(year)).FirstOrDefault();
            }
        }

        public MemoryStream GenerateExcel(string bankId, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int clientTimeZoneOffset)
        {
            var Query = GetQuery(bankId, dateFrom, dateTo, clientTimeZoneOffset);

            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Referensi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jenis Referensi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Currency", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Debit", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kredit", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "After", DataType = typeof(double) });

            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", 0, 0, 0); // to allow column name to be generated properly for empty data as template
            else
            {
                var previous = new DailyBankTransactionModel();
                foreach (var item in Query)
                {
                    result.Rows.Add(item.Date.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID")), item.Remark, item.ReferenceNo, item.ReferenceType, item.AccountBankCurrencyCode, item.Status.ToUpper().Equals("IN") ? item.Nominal : 0, item.Status.ToUpper().Equals("OUT") ? item.Nominal : 0, item.AfterNominal);
                    previous = item;
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Mutasi") }, true);
        }

        private IQueryable<DailyBankTransactionModel> GetQuery(string bankId, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int clientTimeZoneOffset)
        {
            DateTimeOffset DateFrom = dateFrom == null ? dateTo == null ? DateTimeOffset.Now.AddDays(-30) : dateTo.Value.AddHours(clientTimeZoneOffset * -1).AddDays(-30) : dateFrom.Value.AddHours(clientTimeZoneOffset * -1);
            DateTimeOffset DateTo = dateTo == null ? dateFrom == null ? DateTimeOffset.Now : dateFrom.Value.AddHours(clientTimeZoneOffset * -1).AddDays(DateTimeOffset.Now.Subtract(dateFrom.Value.AddHours(clientTimeZoneOffset * -1)).TotalDays) : dateTo.Value.AddHours(clientTimeZoneOffset * -1);

            var Query = (from transaction in _DbContext.DailyBankTransactions
                         where
                         transaction.IsDeleted == false
                         && string.IsNullOrWhiteSpace(bankId) ? true : bankId.Equals(transaction.AccountBankId)
                         && transaction.Date >= DateFrom
                         && transaction.Date <= DateTo
                         orderby transaction.Date, transaction.CreatedUtc
                         select new DailyBankTransactionModel
                         {
                             Id = transaction.Id,
                             Date = transaction.Date,
                             Remark = $"{transaction.SupplierName ?? transaction.BuyerName}\n{transaction.Remark}",
                             ReferenceNo = transaction.ReferenceNo,
                             ReferenceType = transaction.ReferenceType,
                             AccountBankCurrencyCode = transaction.AccountBankCurrencyCode,
                             BeforeNominal = transaction.BeforeNominal,
                             AfterNominal = transaction.AfterNominal,
                             Nominal = transaction.Nominal,
                             Status = transaction.Status,
                         });

            return Query;
        }

        public ReadResponse<DailyBankTransactionModel> GetReport(string bankId, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int clientTimeZoneOffset)
        {
            IQueryable<DailyBankTransactionModel> Query = GetQuery(bankId, dateFrom, dateTo, clientTimeZoneOffset);

            var Test = Query.ToList();
            List<DailyBankTransactionModel> Result = Query.ToList();

            Dictionary<string, string> order = new Dictionary<string, string>();

            return new ReadResponse<DailyBankTransactionModel>(Result, Result.Count, order, new List<string>());
        }

        public ReadResponse<DailyBankTransactionModel> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<DailyBankTransactionModel> Query = _DbSet;

            Query = Query
                .Select(s => new DailyBankTransactionModel
                {
                    Id = s.Id,
                    CreatedUtc = s.CreatedUtc,
                    Code = s.Code,
                    LastModifiedUtc = s.LastModifiedUtc,
                    AccountBankName = s.AccountBankName,
                    AccountBankAccountName = s.AccountBankAccountName,
                    AccountBankAccountNumber = s.AccountBankAccountNumber,
                    AccountBankCode = s.AccountBankCode,
                    AccountBankCurrencyCode = s.AccountBankCurrencyCode,
                    AccountBankCurrencyId = s.AccountBankCurrencyId,
                    AccountBankCurrencySymbol = s.AccountBankCurrencySymbol,
                    AccountBankId = s.AccountBankId,
                    Date = s.Date,
                    ReferenceNo = s.ReferenceNo,
                    ReferenceType = s.ReferenceType,
                    Status = s.Status,
                    SourceType = s.SourceType
                });

            List<string> searchAttributes = new List<string>()
            {
                "Code", "ReferenceNo", "ReferenceType","AccountBankName", "AccountBankCurrencyCode", "Status", "SourceType"
            };

            Query = QueryHelper<DailyBankTransactionModel>.Search(Query, searchAttributes, Keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            Query = QueryHelper<DailyBankTransactionModel>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<DailyBankTransactionModel>.Order(Query, OrderDictionary);

            Pageable<DailyBankTransactionModel> pageable = new Pageable<DailyBankTransactionModel>(Query, Page - 1, Size);
            List<DailyBankTransactionModel> Data = pageable.Data.ToList();

            List<DailyBankTransactionModel> list = new List<DailyBankTransactionModel>();
            list.AddRange(
               Data.Select(s => new DailyBankTransactionModel
               {
                   Id = s.Id,
                   CreatedUtc = s.CreatedUtc,
                   Code = s.Code,
                   LastModifiedUtc = s.LastModifiedUtc,
                   AccountBankName = s.AccountBankName,
                   AccountBankAccountName = s.AccountBankAccountName,
                   AccountBankAccountNumber = s.AccountBankAccountNumber,
                   AccountBankCode = s.AccountBankCode,
                   AccountBankCurrencyCode = s.AccountBankCurrencyCode,
                   AccountBankCurrencyId = s.AccountBankCurrencyId,
                   AccountBankCurrencySymbol = s.AccountBankCurrencySymbol,
                   AccountBankId = s.AccountBankId,
                   Date = s.Date,
                   ReferenceNo = s.ReferenceNo,
                   ReferenceType = s.ReferenceType,
                   Status = s.Status,
                   SourceType = s.SourceType
               }).ToList()
            );

            int TotalData = pageable.TotalCount;

            return new ReadResponse<DailyBankTransactionModel>(list, TotalData, OrderDictionary, new List<string>());
        }

        public async Task<DailyBankTransactionModel> ReadById(int Id)
        {
            return await _DbSet.Where(w => w.Id.Equals(Id)).FirstOrDefaultAsync();
        }
    }
}
