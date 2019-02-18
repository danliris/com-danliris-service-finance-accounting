using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Enums.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Helpers;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MasterCOA;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.JournalTransaction;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.JournalTransaction
{
    public class JournalTransactionService : IJournalTransactionService
    {
        private const string _UserAgent = "finance-service";
        protected DbSet<JournalTransactionModel> _DbSet;
        protected DbSet<JournalTransactionItemModel> _ItemDbSet;
        protected DbSet<COAModel> _COADbSet;
        private readonly IServiceProvider _serviceProvider;
        protected IIdentityService _IdentityService;
        public FinanceDbContext _DbContext;

        public JournalTransactionService(IServiceProvider serviceProvider, FinanceDbContext dbContext)
        {
            _DbContext = dbContext;
            _DbSet = dbContext.Set<JournalTransactionModel>();
            _ItemDbSet = dbContext.Set<JournalTransactionItemModel>();
            _COADbSet = dbContext.Set<COAModel>();
            _serviceProvider = serviceProvider;
            _IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        public async Task<int> CreateAsync(JournalTransactionModel model)
        {
            do
            {
                model.DocumentNo = CodeGenerator.Generate();
            }
            while (_DbSet.Any(d => d.DocumentNo.Equals(model.DocumentNo)));

            if (_DbSet.Any(d => d.ReferenceNo.Equals(model.ReferenceNo) && !d.IsDeleted && !d.IsReversed && !d.IsReverser))
            {
                var errorResult = new List<ValidationResult>()
                {
                    new ValidationResult("No. Referensi duplikat", new List<string> { "ReferenceNo" })
                };
                ValidationContext validationContext = new ValidationContext(model, _serviceProvider, null);
                throw new ServiceValidationException(validationContext, errorResult);
            }

            if (string.IsNullOrWhiteSpace(model.Status))
                model.Status = JournalTransactionStatus.Draft;

            EntityExtension.FlagForCreate(model, _IdentityService.Username, _UserAgent);
            foreach (var item in model.Items)
            {
                var coa = _COADbSet.FirstOrDefault(f => f.Id.Equals(item.COA.Id) || f.Code.Equals(item.COA.Code));
                item.COAId = coa.Id;
                item.COA = null;

                EntityExtension.FlagForCreate(item, _IdentityService.Username, _UserAgent);
                _ItemDbSet.Add(item);
            }

            _DbSet.Add(model);
            return await _DbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            var model = await ReadByIdAsync(id);
            EntityExtension.FlagForDelete(model, _IdentityService.Username, _UserAgent);
            foreach (var item in model.Items)
            {
                EntityExtension.FlagForDelete(item, _IdentityService.Username, _UserAgent);
                _ItemDbSet.Update(item);
            }

            _DbSet.Update(model);
            return await _DbContext.SaveChangesAsync();
        }

        public ReadResponse<JournalTransactionModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<JournalTransactionModel> Query = _DbSet;

            Query = Query
                .Select(s => new JournalTransactionModel
                {
                    Id = s.Id,
                    CreatedUtc = s.CreatedUtc,
                    CreatedBy = s.CreatedBy,
                    DocumentNo = s.DocumentNo,
                    Date = s.Date,
                    Description = s.Description,
                    ReferenceNo = s.ReferenceNo,
                    LastModifiedUtc = s.LastModifiedUtc,
                    Status = s.Status
                });

            List<string> searchAttributes = new List<string>()
            {
                "DocumentNo", "ReferenceNo", "Description", "Status"
            };

            Query = QueryHelper<JournalTransactionModel>.Search(Query, searchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<JournalTransactionModel>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<JournalTransactionModel>.Order(Query, OrderDictionary);

            Pageable<JournalTransactionModel> pageable = new Pageable<JournalTransactionModel>(Query, page - 1, size);
            List<JournalTransactionModel> Data = pageable.Data.ToList();

            List<JournalTransactionModel> list = new List<JournalTransactionModel>();
            list.AddRange(
               Data.Select(s => new JournalTransactionModel
               {
                   Id = s.Id,
                   CreatedUtc = s.CreatedUtc,
                   CreatedBy = s.CreatedBy,
                   DocumentNo = s.DocumentNo,
                   Date = s.Date,
                   Description = s.Description,
                   ReferenceNo = s.ReferenceNo,
                   LastModifiedUtc = s.LastModifiedUtc,
                   Status = s.Status
               }).ToList()
            );

            int TotalData = pageable.TotalCount;

            return new ReadResponse<JournalTransactionModel>(list, TotalData, OrderDictionary, new List<string>());
        }

        public async Task<JournalTransactionModel> ReadByIdAsync(int id)
        {
            var Result = await _DbSet.FirstOrDefaultAsync(d => d.Id.Equals(id) && !d.IsDeleted);
            Result.Items = await _ItemDbSet.Where(w => w.JournalTransactionId.Equals(id) && !w.IsDeleted).ToListAsync();
            foreach (var item in Result.Items)
            {
                var COA = await _COADbSet.FirstOrDefaultAsync(c => c.Id.Equals(item.COAId) && !c.IsDeleted);
                item.COA = COA;
            }
            return Result;
        }

        public async Task<int> UpdateAsync(int id, JournalTransactionModel model)
        {
            EntityExtension.FlagForUpdate(model, _IdentityService.Username, _UserAgent);
            List<int> journalItemIds = await _ItemDbSet.Where(w => w.JournalTransactionId.Equals(id) && !w.IsDeleted).Select(s => s.Id).ToListAsync();

            foreach (var journalItemId in journalItemIds)
            {
                var item = model.Items.FirstOrDefault(f => f.Id.Equals(journalItemId));
                if (item == null)
                {
                    var itemToDelete = await _ItemDbSet.FirstOrDefaultAsync(f => f.Id.Equals(journalItemId));
                    itemToDelete.COA = null;
                    EntityExtension.FlagForDelete(itemToDelete, _IdentityService.Username, _UserAgent);
                    _ItemDbSet.Update(itemToDelete);
                }
                else
                {
                    var coa = _COADbSet.FirstOrDefault(f => f.Id.Equals(item.COA.Id) || f.Code.Equals(item.COA.Code));
                    item.COAId = coa.Id;
                    item.COA = null;
                    EntityExtension.FlagForUpdate(item, _IdentityService.Username, _UserAgent);
                    _ItemDbSet.Update(item);
                }
            }

            foreach (var item in model.Items)
            {
                if (item.Id <= 0)
                {
                    item.COAId = item.COA.Id;
                    item.COA = null;
                    EntityExtension.FlagForCreate(item, _IdentityService.Username, _UserAgent);
                    _ItemDbSet.Add(item);
                }
            }

            _DbSet.Update(model);
            return await _DbContext.SaveChangesAsync();
        }

        private (List<JournalTransactionReportViewModel>, double, double) GetReport(DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int offSet)
        {
            _DbContext.ChartsOfAccounts.Load();
            IQueryable<JournalTransactionItemModel> query = _DbContext.JournalTransactionItems
                .Include(x => x.JournalTransaction);

            if (dateFrom == null && dateTo == null)
            {
                query = query
                    .Where(x => DateTimeOffset.UtcNow.AddDays(-30).Date <= x.JournalTransaction.Date.AddHours(offSet).Date
                        && x.JournalTransaction.Date.AddHours(offSet).Date <= DateTime.UtcNow.Date);
            }
            else if (dateFrom == null && dateTo != null)
            {
                query = query
                    .Where(x => dateTo.Value.AddDays(-30).Date <= x.JournalTransaction.Date.AddHours(offSet).Date
                        && x.JournalTransaction.Date.AddHours(offSet).Date <= dateTo.Value.Date);
            }
            else if (dateTo == null && dateFrom != null)
            {
                query = query
                    .Where(x => dateFrom.Value.Date <= x.JournalTransaction.Date.AddHours(offSet).Date
                        && x.JournalTransaction.Date.AddHours(offSet).Date <= dateFrom.Value.AddDays(30).Date);
            }
            else
            {
                query = query
                    .Where(x => dateFrom.Value.Date <= x.JournalTransaction.Date.AddHours(offSet).Date
                        && x.JournalTransaction.Date.AddHours(offSet).Date <= dateTo.Value.Date);
            }

            List<JournalTransactionReportViewModel> result = new List<JournalTransactionReportViewModel>();
            foreach (var item in query.OrderBy(x => x.JournalTransaction.Date).ToList())
            {
                JournalTransactionReportViewModel vm = new JournalTransactionReportViewModel
                {
                    Credit = item.Credit,
                    Date = item.JournalTransaction.Date,
                    Debit = item.Debit,
                    Remark = item.Remark
                };

                if (item.COA != null)
                {
                    vm.COACode = item.COA.Code;
                    vm.COAName = item.COA.Name;
                }
                result.Add(vm);

            }

            return (result, result.Sum(x => x.Debit.GetValueOrDefault()), result.Sum(x => x.Credit.GetValueOrDefault()));
        }

        public (ReadResponse<JournalTransactionReportViewModel>, double, double) GetReport(int page, int size, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int offSet)
        {
            var queries = GetReport(dateFrom, dateTo, offSet);

            Pageable<JournalTransactionReportViewModel> pageable = new Pageable<JournalTransactionReportViewModel>(queries.Item1, page - 1, size);
            List<JournalTransactionReportViewModel> data = pageable.Data.ToList();

            return (new ReadResponse<JournalTransactionReportViewModel>(data, pageable.TotalCount, new Dictionary<string, string>(), new List<string>()), queries.Item2, queries.Item3);
        }

        public MemoryStream GenerateExcel(DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int offSet)
        {
            var data = GetReport(dateFrom, dateTo, offSet);

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn() { ColumnName = "Date", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Nama Akun", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "No Akun", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Keterangan", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Debit", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Kredit", DataType = typeof(string) });

            if (data.Item1.Count == 0)
            {
                dt.Rows.Add("", "", "", "", "", "");
            }
            else
            {
                foreach (var item in data.Item1)
                {
                    dt.Rows.Add(item.Date.AddHours(offSet).ToString("dd MMM yyyy"), string.IsNullOrEmpty(item.COAName) ? "-" : item.COAName, string.IsNullOrEmpty(item.COACode) ? "-" : item.COACode,
                        string.IsNullOrEmpty(item.Remark) ? "-" : item.Remark, item.Debit.HasValue ? item.Debit.Value.ToString("#,##0.###0") : "0", item.Credit.HasValue ? item.Credit.Value.ToString("#,##0.###0") : "0");

                }
                dt.Rows.Add("", "", "", "TOTAL", data.Item2.ToString("#,##0.###0"), data.Item3.ToString("#,##0.###0"));
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(dt, "Jurnal Transaksi") }, true);
        }

        public async Task<int> ReverseJournalTransactionByReferenceNo(string referenceNo)
        {
            var transactionToReverse = _DbSet.FirstOrDefault(entity => entity.ReferenceNo.Equals(referenceNo) && !entity.IsReversed && !entity.IsReverser && !entity.IsDeleted);

            if (transactionToReverse == null)
            {
                throw new Exception("Transaction Not Found");
            }

            transactionToReverse.IsReversed = true;
            _DbSet.Update(transactionToReverse);

            var transactionToReverseItems = _ItemDbSet.Where(entity => entity.JournalTransactionId.Equals(transactionToReverse.Id) && !entity.IsDeleted).ToList();
            var reversingItems = new List<JournalTransactionItemModel>();
            foreach (var transactionToReverseItem in transactionToReverseItems)
            {
                var reversingItem = new JournalTransactionItemModel()
                {
                    COAId = transactionToReverseItem.COAId,
                    Credit = transactionToReverseItem.Debit,
                    Debit = transactionToReverseItem.Credit,
                    Remark = transactionToReverseItem.Remark
                };
                EntityExtension.FlagForCreate(reversingItem, _IdentityService.Username, _UserAgent);
                reversingItems.Add(reversingItem);
            }

            var reversingJournalTransaction = new JournalTransactionModel()
            {
                Date = DateTimeOffset.Now,
                Items = reversingItems,
                ReferenceNo = transactionToReverse.ReferenceNo,
                Description = $"Jurnal Pembalik {transactionToReverse.DocumentNo}"
            };

            do
            {
                reversingJournalTransaction.DocumentNo = CodeGenerator.Generate();
            }
            while (_DbSet.Any(d => d.DocumentNo.Equals(reversingJournalTransaction.DocumentNo)));
            reversingJournalTransaction.IsReverser = true;
            EntityExtension.FlagForCreate(reversingJournalTransaction, _IdentityService.Username, _UserAgent);
            _DbSet.Add(reversingJournalTransaction);

            return await _DbContext.SaveChangesAsync();
        }

        public async Task<SubLedgerReportViewModel> GetSubLedgerReport(int coaId, int month, int year, int timeoffset)
        {
            return await GetSubLedgerReportData(coaId, month, year, timeoffset);
        }

        private async Task<SubLedgerReportViewModel> GetSubLedgerReportData(int coaId, int month, int year, int timeoffset)
        {
            var postedJournals = _DbSet.Where(w => (!string.IsNullOrWhiteSpace(w.Status) && w.Status.Equals(JournalTransactionStatus.Posted)) && w.Date.AddHours(timeoffset).Month.Equals(month) && w.Date.AddHours(timeoffset).Year.Equals(year)).Select(s => new { s.Id, s.Date, s.ReferenceNo, s.DocumentNo }).ToList();
            var postedIds = postedJournals.Select(s => s.Id).ToList();
            var postedReferenceNos = postedJournals.Select(s => s.ReferenceNo).ToList();

            var bankPayments = await GetBankPayments(month, year, timeoffset);

            var entries = _ItemDbSet.Where(w => postedIds.Contains(w.JournalTransactionId) && w.COAId.Equals(coaId)).ToList();
            var closingDebitBalance = entries.Sum(s => s.Debit);
            var closingCreditBalance = entries.Sum(s => s.Credit);

            var initialDate = new DateTime(year, month, 1);
            var previousPostedJournalIds = _DbSet.Where(w => (!string.IsNullOrWhiteSpace(w.Status) && w.Status.Equals(JournalTransactionStatus.Posted)) && w.Date < initialDate).Select(s => s.Id).ToList();
            var previousDebitBalance = _ItemDbSet.Where(w => previousPostedJournalIds.Contains(w.JournalTransactionId) && w.COAId.Equals(coaId)).Sum(s => s.Debit);
            var previousCreditBalance = _ItemDbSet.Where(w => previousPostedJournalIds.Contains(w.JournalTransactionId) && w.COAId.Equals(coaId)).Sum(s => s.Credit);

            var result = new SubLedgerReportViewModel
            {
                InitialBalance = (decimal)previousDebitBalance - (decimal)previousCreditBalance,
                ClosingBalance = ((decimal)previousDebitBalance - (decimal)previousCreditBalance) + ((decimal)closingDebitBalance - (decimal)closingCreditBalance)
            };

            foreach (var entry in entries)
            {
                var header = postedJournals.FirstOrDefault(f => f.Id.Equals(entry.JournalTransactionId));
                var bankPayment = bankPayments.FirstOrDefault(f => f.DocumentNo.Equals(header?.ReferenceNo));
                var data = new SubLedgerReport()
                {
                    BankName = bankPayment?.BankName,
                    BGCheck = bankPayment?.BGCheckNumber,
                    Credit = (decimal)entry.Credit,
                    Debit = (decimal)entry.Debit,
                    Date = header.Date.AddHours(timeoffset).ToString("dd MMMM yyyy", CultureInfo.InvariantCulture),
                    No = header.DocumentNo,
                    Remark = entry.Remark
                };

                result.Info.Add(data);
            }

            return result;
        }

        private async Task<List<BankPayment>> GetBankPayments(int month, int year, int timeoffset)
        {
            var uri = APIEndpoint.Purchasing + $"bank-expenditure-notes/get-documentno/by-period?month={month}&year={year}&timeoffset={timeoffset}";

            IHttpClientService _httpClientService = (IHttpClientService)_serviceProvider.GetService(typeof(IHttpClientService));
            var response = await _httpClientService.GetAsync(uri);

            var result = new BankPaymentResult();
            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                result = JsonConvert.DeserializeObject<BankPaymentResult>(await response.Content.ReadAsStringAsync());
            }
            return result.data.ToList();
        }

        public async Task<SubLedgerXlsFormat> GetSubLedgerReportXls(int coaId, int month, int year, int timeoffset)
        {
            var result = new SubLedgerXlsFormat();
            var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month);

            var coa = _DbContext.ChartsOfAccounts.FirstOrDefault(f => f.Id.Equals(coaId));

            var data = await GetSubLedgerReportData(coaId, month, year, timeoffset);

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn() { ColumnName = "Tanggal", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "No. Bukti", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Bank", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Cek/BG", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Perkiraan", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Debit", DataType = typeof(decimal) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Kredit", DataType = typeof(decimal) });

            if (data.Info.Count == 0)
            {
                dt.Rows.Add("", "", "", "", "", 0, 0);
            }
            else
            {
                foreach (var item in data.Info)
                {
                    dt.Rows.Add(item.Date, item.No, item.BankName, item.BGCheck, item.Remark, item.Debit, item.Credit);
                }
            }

            result.Filename = $"Laporan Sub Ledger {coa.Name} Periode {monthName} {year}";
            result.Result = Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(dt, "Sub Ledger") }, true);

            return result;
        }

        public async Task<int> PostTransactionAsync(int id)
        {
            var model = _DbSet.FirstOrDefault(f => f.Id.Equals(id));

            model.Status = JournalTransactionStatus.Posted;
            EntityExtension.FlagForUpdate(model, _IdentityService.Username, _UserAgent);
            _DbSet.Update(model);
            return await _DbContext.SaveChangesAsync();
        }
    }

    public class BankPaymentResult
    {
        public BankPaymentResult()
        {
            data = new List<BankPayment>();
        }
        public IList<BankPayment> data { get; set; }
    }

    public class BankPayment
    {
        public string DocumentNo { get; set; }
        public string BankName { get; set; }
        public string BGCheckNumber { get; set; }
    }
}
