using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.CreditorAccount;
using Com.Danliris.Service.Finance.Accounting.Lib.Enums;
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
        
        public Task<CreditorAccountModel> ReadModelById(int id)
        {
            return DbSet.FirstOrDefaultAsync(d => d.Id.Equals(id) && d.IsDeleted.Equals(false));
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

        public async Task<int> UpdateFromUnitReceiptNoteAsync(CreditorAccountUnitReceiptNotePostedViewModel viewModel)
        {
            CreditorAccountModel data = await DbSet.FirstOrDefaultAsync(x => x.Id == viewModel.CreditorAccountId);
            if (data == null)
                throw new NotFoundException();

            data.UnitReceiptNotePPN = viewModel.PPN;
            data.UnitReceiptNoteNo = viewModel.Code;
            data.UnitReceiptNoteDPP = viewModel.DPP;
            data.UnitReceiptNoteDate = viewModel.Date;
            data.UnitReceiptMutation = viewModel.DPP + viewModel.PPN;
            data.SupplierName = viewModel.SupplierName;
            data.SupplierCode = viewModel.SupplierCode;
            data.InvoiceNo = viewModel.InvoiceNo;
            data.FinalBalance = data.UnitReceiptMutation + data.BankExpenditureNoteMutation + data.MemoMutation;


            UpdateModel(data.Id, data);
            return await DbContext.SaveChangesAsync();
        }
        
        public async Task<CreditorAccountUnitReceiptNotePostedViewModel> GetByUnitReceiptNote(string supplierCode, string unitReceiptNote, string invoiceNo)
        {
            CreditorAccountModel data;

            if (string.IsNullOrEmpty(invoiceNo))
                data = await DbSet.FirstOrDefaultAsync(x => x.SupplierCode == supplierCode && x.UnitReceiptNoteNo == unitReceiptNote && string.IsNullOrEmpty(x.InvoiceNo));
            else
                data = await DbSet.FirstOrDefaultAsync(x => x.SupplierCode == supplierCode && x.UnitReceiptNoteNo == unitReceiptNote && x.InvoiceNo == invoiceNo);

            if (data == null)
                return null;

            return new CreditorAccountUnitReceiptNotePostedViewModel()
            {
                CreditorAccountId = data.Id,
                Code = data.UnitReceiptNoteNo,
                Date = data.UnitReceiptNoteDate.Value,
                DPP = data.UnitReceiptNoteDPP,
                PPN = data.UnitReceiptNotePPN,
                InvoiceNo = data.InvoiceNo,
                SupplierCode = data.SupplierCode,
                SupplierName = data.SupplierName
            };
        }

        public async Task<CreditorAccountBankExpenditureNotePostedViewModel> GetByBankExpenditureNote(string supplierCode, string bankExpenditureNote, string invoiceNo)
        {
            CreditorAccountModel data= await DbSet.FirstOrDefaultAsync(x => x.SupplierCode == supplierCode && x.BankExpenditureNoteNo == bankExpenditureNote && x.InvoiceNo == invoiceNo);

            if (data == null)
                return null;

            return new CreditorAccountBankExpenditureNotePostedViewModel()
            {
                CreditorAccountId = data.Id,
                Mutation = data.BankExpenditureNoteMutation,
                Id = data.BankExpenditureNoteId,
                Code = data.BankExpenditureNoteNo,
                Date = data.BankExpenditureNoteDate.Value,
                InvoiceNo = data.InvoiceNo,
                SupplierCode = data.SupplierCode,
                SupplierName = data.SupplierName
            };
        }

        public async Task<CreditorAccountMemoPostedViewModel> GetByMemo(string supplierCode, string memoNo, string invoiceNo)
        {
            CreditorAccountModel data= await DbSet.FirstOrDefaultAsync(x => x.SupplierCode == supplierCode && x.MemoNo == memoNo && x.InvoiceNo == invoiceNo);

            if (data == null)
                return null;

            return new CreditorAccountMemoPostedViewModel()
            {
                CreditorAccountId = data.Id,
                Code = data.MemoNo,
                Date = data.MemoDate.Value,
                DPP = data.MemoDPP,
                PPN = data.MemoPPN,
                InvoiceNo = data.InvoiceNo,
                SupplierCode = data.SupplierCode,
                SupplierName = data.SupplierName
            };
        }

        public async Task<int> CreateFromBankExpenditureNoteAsync(CreditorAccountBankExpenditureNotePostedViewModel viewModel)
        {
            CreditorAccountModel model = await DbSet.FirstOrDefaultAsync(x => x.SupplierCode == viewModel.SupplierCode && x.InvoiceNo == viewModel.InvoiceNo);

            if (model == null)
                throw new NotFoundException();

            model.BankExpenditureNoteDate = viewModel.Date;
            model.BankExpenditureNoteId = viewModel.Id;
            model.BankExpenditureNoteMutation = viewModel.Mutation * -1;
            model.BankExpenditureNoteNo = viewModel.Code;
            model.FinalBalance = model.UnitReceiptMutation + model.BankExpenditureNoteMutation + model.MemoMutation;

            UpdateModel(model.Id, model);
            return await DbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateFromBankExpenditureNoteAsync(CreditorAccountBankExpenditureNotePostedViewModel viewModel)
        {
            CreditorAccountModel data = await DbSet.FirstOrDefaultAsync(x => x.Id == viewModel.CreditorAccountId);
            
            if (data == null)
                throw new NotFoundException();
            
            data.BankExpenditureNoteNo = viewModel.Code;
            data.BankExpenditureNoteDate = viewModel.Date;
            data.BankExpenditureNoteMutation = viewModel.Mutation * -1;
            data.FinalBalance = data.UnitReceiptMutation + data.BankExpenditureNoteMutation + data.MemoMutation;


            UpdateModel(data.Id, data);
            return await DbContext.SaveChangesAsync();
        }

        public async Task<int> CreateFromUnitReceiptNoteAsync(CreditorAccountUnitReceiptNotePostedViewModel viewModel)
        {
            CreditorAccountModel model = new CreditorAccountModel()
            {
                UnitReceiptNotePPN = viewModel.PPN,
                FinalBalance = viewModel.DPP + viewModel.PPN,
                InvoiceNo = viewModel.InvoiceNo,
                SupplierCode = viewModel.SupplierCode,
                SupplierName = viewModel.SupplierName,
                UnitReceiptMutation = viewModel.DPP + viewModel.PPN,
                UnitReceiptNoteDate = viewModel.Date,
                UnitReceiptNoteDPP = viewModel.DPP,
                UnitReceiptNoteNo = viewModel.Code
            };

            return await CreateAsync(model);
        }

        public async Task<int> DeleteFromUnitReceiptNoteAsync(int id)
        {
            CreditorAccountModel model = await DbSet.FirstOrDefaultAsync(x => x.Id == id);

            if(!string.IsNullOrEmpty(model.BankExpenditureNoteNo) || !string.IsNullOrEmpty(model.MemoNo))
            {
                return await DeleteAsync(model.Id);
            }
            else
            {
                model.UnitReceiptMutation = 0;
                model.UnitReceiptNoteDate = null;
                model.UnitReceiptNoteDPP = 0;
                model.UnitReceiptNoteNo = null;
                model.UnitReceiptNotePPN = 0;
                model.FinalBalance = model.UnitReceiptMutation + model.BankExpenditureNoteMutation + model.MemoMutation;

                return await UpdateAsync(model.Id, model);
            }
        }

        public async Task<int> DeleteFromBankExpenditureNoteAsync(int id)
        {
            CreditorAccountModel model = await DbSet.FirstOrDefaultAsync(x => x.Id == id);

            model.BankExpenditureNoteDate = null;
            model.BankExpenditureNoteDPP = 0;
            model.BankExpenditureNoteId = 0;
            model.BankExpenditureNoteMutation = 0;
            model.BankExpenditureNoteNo = null;
            model.BankExpenditureNotePPN = 0;
            model.FinalBalance = model.UnitReceiptMutation + model.BankExpenditureNoteMutation + model.MemoMutation;

            return await UpdateAsync(model.Id, model);
        }
    }
}
