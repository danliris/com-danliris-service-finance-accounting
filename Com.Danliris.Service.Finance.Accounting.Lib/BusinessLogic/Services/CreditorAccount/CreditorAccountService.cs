using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.CreditorAccount;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.CreditorAccount;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CreditorAccount;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.Purchasing.BankExpenditureNote;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.CreditorAccount
{
    public class CreditorAccountService : ICreditorAccountService
    {
        private const string UserAgent = "finance-service";
        protected DbSet<CreditorAccountModel> DbSet;
        protected IIdentityService IdentityService;
        public FinanceDbContext DbContext;

        public CreditorAccountService(IServiceProvider serviceProvider, FinanceDbContext dbContext)
        {
            DbContext = dbContext;
            DbSet = dbContext.Set<CreditorAccountModel>();
            IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        public void CreateModel(CreditorAccountModel model)
        {
            EntityExtension.FlagForCreate(model, IdentityService.Username, UserAgent);
            DbSet.Add(model);
        }

        public async Task<int> CreateAsync(CreditorAccountModel model)
        {
            CreateModel(model);
            return await DbContext.SaveChangesAsync();
        }

        public async Task DeleteModel(int id)
        {
            CreditorAccountModel model = await ReadModelById(id);
            EntityExtension.FlagForDelete(model, IdentityService.Username, UserAgent, true);
            DbSet.Update(model);
        }

        public async Task<int> DeleteAsync(int id)
        {
            await DeleteModel(id);
            return await DbContext.SaveChangesAsync();
        }

        public ReadResponse<CreditorAccountModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<CreditorAccountModel> query = DbSet;

            List<string> searchAttributes = new List<string>()
            {
                "Code", "Name"
            };

            query = QueryHelper<CreditorAccountModel>.Search(query, searchAttributes, keyword);

            Dictionary<string, object> filterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            query = QueryHelper<CreditorAccountModel>.Filter(query, filterDictionary);

            List<string> selectedFields = new List<string>()
                {
                    "Id", "Name", "Code", "Path", "Nature", "CashAccount", "ReportType", "LastModifiedUtc"
                };

            Dictionary<string, string> orderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            query = QueryHelper<CreditorAccountModel>.Order(query, orderDictionary);

            query = query.Select(x => new CreditorAccountModel()
            {
                Id = x.Id,
                BankExpenditureNoteDate = x.BankExpenditureNoteDate,
                BankExpenditureNoteId = x.BankExpenditureNoteId,
                BankExpenditureNoteNo = x.BankExpenditureNoteNo,
                FinalBalance = x.FinalBalance,
                InvoiceNo = x.InvoiceNo,
                MemoDate = x.MemoDate,
                MemoNo = x.MemoNo,
                SupplierCode = x.SupplierCode,
                SupplierName = x.SupplierName,
                UnitReceiptNoteDate = x.UnitReceiptNoteDate,
                UnitReceiptNoteNo = x.UnitReceiptNoteNo,
                LastModifiedUtc = x.LastModifiedUtc
            });

            Pageable<CreditorAccountModel> pageable = new Pageable<CreditorAccountModel>(query, page - 1, size);
            List<CreditorAccountModel> data = pageable.Data.ToList();
            int totalData = pageable.TotalCount;

            return new ReadResponse<CreditorAccountModel>(data, totalData, orderDictionary, selectedFields);
        }

        public Task<CreditorAccountModel> ReadModelById(int id)
        {
            return DbSet.FirstOrDefaultAsync(d => d.Id.Equals(id) && d.IsDeleted.Equals(false));
        }

        public async Task<CreditorAccountModel> ReadByIdAsync(int id)
        {
            return await ReadModelById(id);
        }

        public async Task<int> UpdateAsync(int id, CreditorAccountModel model)
        {
            UpdateModel(id, model);
            return await DbContext.SaveChangesAsync();
        }

        public void UpdateModel(int id, CreditorAccountModel model)
        {
            EntityExtension.FlagForUpdate(model, IdentityService.Username, UserAgent);
            DbSet.Update(model);
        }

        public MemoryStream GenerateExcel(string suplierName, int month, int year, int offSet)
        {
            throw new NotImplementedException();
        }

        public ReadResponse<CreditorAccountViewModel> GetReport(int page, int size, string suplierName, int month, int year, int offSet)
        {
            var queries = GetReport(suplierName, month, year, offSet);

            Pageable<CreditorAccountViewModel> pageable = new Pageable<CreditorAccountViewModel>(queries, page - 1, size);
            List<CreditorAccountViewModel> data = pageable.Data.ToList();

            return new ReadResponse<CreditorAccountViewModel>(queries, pageable.TotalCount, new Dictionary<string, string>(), new List<string>());
        }

        public async Task<BankExpenditureNoteViewModel> GetBankExpenditureNote(long bankExpenditureNoteId)
        {
            using (var client = new HttpClient() { Timeout = Timeout.InfiniteTimeSpan })
            {
                string relativePath = "bank-expenditure-notes/" + bankExpenditureNoteId;
                Uri serverUri = new Uri(APIEndpoint.Purchasing);
                Uri relativePathUri = new Uri(relativePath, UriKind.Relative);

                var uri = new Uri(serverUri, relativePathUri);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", IdentityService.Token);
                var response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                var stringContent = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<APIWrapper>(stringContent);
                return JsonConvert.DeserializeObject<BankExpenditureNoteViewModel>(data.Data);
            }
        }

        public List<CreditorAccountViewModel> GetReport(string suplierName, int month, int year, int offSet)
        {
            IQueryable<CreditorAccountModel> query = DbContext.CreditorAccounts.AsQueryable();
            List<CreditorAccountViewModel> result = new List<CreditorAccountViewModel>();

            query = query.Where(x => x.SupplierName == suplierName &&
                                        ((x.FinalBalance != 0 && x.UnitReceiptNoteDate.HasValue &&
                                            x.UnitReceiptNoteDate.Value.Year < year || (x.UnitReceiptNoteDate.Value.Year == year && x.UnitReceiptNoteDate.Value.Month < month)) ||
                                        (x.FinalBalance == 0 && (x.UnitReceiptNoteDate.HasValue && x.UnitReceiptNoteDate.Value.Month == month && x.UnitReceiptNoteDate.Value.Year == year) ||
                                            (x.BankExpenditureNoteDate.HasValue && x.BankExpenditureNoteDate.Value.Month == month && x.BankExpenditureNoteDate.Value.Year == year) ||
                                            (x.MemoDate.HasValue && x.MemoDate.Value.Month == month && x.MemoDate.Value.Year == year))));

            //if (!string.IsNullOrEmpty(suplierName))
            //    query = query.Where(x => x.SupplierName == suplierName);

            //if (month != -1 & year != -1)
            //{
            //    query = query.Where(x => (x.UnitReceiptNoteDate.HasValue && x.UnitReceiptNoteDate.Value.Month == month && x.UnitReceiptNoteDate.Value.Year == year) ||
            //                                (x.BankExpenditureNoteDate.HasValue && x.BankExpenditureNoteDate.Value.Month == month && x.BankExpenditureNoteDate.Value.Year == year) ||
            //                                (x.MemoDate.HasValue && x.MemoDate.Value.Month == month && x.MemoDate.Value.Year == year));
            //}

            foreach (var item in query.OrderBy(x => x.UnitReceiptNoteDate.GetValueOrDefault()).ToList())
            {
                if (!string.IsNullOrEmpty(item.UnitReceiptNoteNo))
                {
                    CreditorAccountViewModel vm = new CreditorAccountViewModel
                    {
                        UnitReceiptNoteNo = item.UnitReceiptNoteNo,
                        Date = item.UnitReceiptNoteDate.Value,
                        InvoiceNo = item.InvoiceNo,
                        DPP = item.UnitReceiptNoteDPP,
                        PPN = item.UnitReceiptNotePPN,
                        Total = item.UnitReceiptMutation,
                        Mutation = item.UnitReceiptMutation,

                    };

                    result.Add(vm);
                }

                if (!string.IsNullOrEmpty(item.BankExpenditureNoteNo))
                {
                    CreditorAccountViewModel vm = new CreditorAccountViewModel
                    {
                        BankExpenditureNoteNo = item.BankExpenditureNoteNo,
                        Date = item.BankExpenditureNoteDate.Value,
                        InvoiceNo = item.InvoiceNo,
                        DPP = item.BankExpenditureNoteDPP,
                        PPN = item.BankExpenditureNotePPN,
                        Total = item.BankExpenditureNoteMutation * -1,
                        Mutation = item.BankExpenditureNoteMutation,

                    };

                    result.Add(vm);

                }
                if (!string.IsNullOrEmpty(item.MemoNo))
                {
                    CreditorAccountViewModel vm = new CreditorAccountViewModel
                    {
                        MemoNo = item.MemoNo,
                        Date = item.MemoDate.Value,
                        InvoiceNo = item.InvoiceNo,
                        DPP = item.MemoDPP,
                        PPN = item.MemoPPN,
                        Total = item.MemoMutation,
                        Mutation = item.MemoMutation,

                    };

                    result.Add(vm);
                }
            }

            return result;
        }

        public async Task<int> UpdateFromUnitReceiptNoteAsync(string supplierCode, string unitReceiptNote, string invoiceNo, CreditorAccountModel model)
        {
            var data = await DbSet.FirstOrDefaultAsync(x => x.SupplierCode == supplierCode && x.UnitReceiptNoteNo == unitReceiptNote && x.InvoiceNo == invoiceNo);

            if (data == null)
                throw new NullReferenceException();

            data.UnitReceiptNotePPN = model.UnitReceiptNotePPN;
            data.UnitReceiptNoteNo = model.UnitReceiptNoteNo;
            data.UnitReceiptNoteDPP = model.UnitReceiptNoteDPP;
            data.UnitReceiptNoteDate = model.UnitReceiptNoteDate;
            data.UnitReceiptMutation = model.UnitReceiptMutation;
            data.SupplierName = model.SupplierName;
            data.SupplierCode = model.SupplierCode;
            data.InvoiceNo = model.InvoiceNo;
            data.FinalBalance = data.UnitReceiptMutation + data.BankExpenditureNoteMutation + data.MemoMutation;


            UpdateModel(data.Id, data);
            return await DbContext.SaveChangesAsync();
        }
    }
}
