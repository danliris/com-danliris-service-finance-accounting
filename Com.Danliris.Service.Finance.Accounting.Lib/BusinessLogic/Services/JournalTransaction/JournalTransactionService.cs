using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Helpers;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MasterCOA;
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
using System.Data;
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
        protected IIdentityService _IdentityService;
        public FinanceDbContext _DbContext;

        public JournalTransactionService(IServiceProvider serviceProvider, FinanceDbContext dbContext)
        {
            _DbContext = dbContext;
            _DbSet = dbContext.Set<JournalTransactionModel>();
            _ItemDbSet = dbContext.Set<JournalTransactionItemModel>();
            _COADbSet = dbContext.Set<COAModel>();
            _IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        public async Task<int> CreateAsync(JournalTransactionModel model)
        {
            do
            {
                model.DocumentNo = CodeGenerator.Generate();
            }
            while (_DbSet.Any(d => d.DocumentNo.Equals(model.DocumentNo)));

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
                });

            List<string> searchAttributes = new List<string>()
            {
                "DocumentNo", "ReferenceNo", "Description"
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

        private List<JournalTransactionReportViewModel> GetReport(int month, int year, int offset)
        {
            _DbContext.ChartsOfAccounts.Load();
            IQueryable<JournalTransactionItemModel> query = _DbContext.JournalTransactionItems
                .Include(x => x.JournalTransaction)
                .Where(x => x.JournalTransaction.Date.Month == month && x.JournalTransaction.Date.Year == year);
            
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
            return result;
        }

        public ReadResponse<JournalTransactionReportViewModel> GetReport(int page, int size, int month, int year, int offSet)
        {
            var queries = GetReport(month, year, offSet);

            Pageable<JournalTransactionReportViewModel> pageable = new Pageable<JournalTransactionReportViewModel>(queries, page - 1, size);
            List<JournalTransactionReportViewModel> data = pageable.Data.ToList();

            return new ReadResponse<JournalTransactionReportViewModel>(data, pageable.TotalCount, new Dictionary<string, string>(), new List<string>());
        }

        public MemoryStream GenerateExcel(int month, int year, int offSet)
        {
            var data = GetReport(month, year, offSet);

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn() { ColumnName = "Date", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Nama Akun", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "No Akun", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Keterangan", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Debit", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Kredit", DataType = typeof(string) });

            if(data.Count == 0)
            {
                dt.Rows.Add("", "", "", "", "", "");
            }
            else
            {
                foreach(var item in data)
                {
                    dt.Rows.Add(item.Date.AddHours(offSet).ToString("dd MMM yyyy"), string.IsNullOrEmpty(item.COAName) ? "-" : item.COAName, string.IsNullOrEmpty(item.COACode) ? "-" : item.COACode,
                        string.IsNullOrEmpty(item.Remark) ? "-" : item.Remark, item.Debit.HasValue ? item.Debit.Value.ToString("#,##0.###0") : "0", item.Credit.HasValue ? item.Credit.Value.ToString("#,##0.###0") : "0");

                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(dt, "Jurnal Transaksi") }, true);
        }
    }
}
