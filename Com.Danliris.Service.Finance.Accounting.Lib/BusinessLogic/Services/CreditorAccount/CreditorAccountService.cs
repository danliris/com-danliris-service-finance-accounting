using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.CreditorAccount;
using Com.Danliris.Service.Finance.Accounting.Lib.Helpers;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.CreditorAccount;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CreditorAccount;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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
            var data = GetReport(suplierName, month, year, offSet).Item1;

            DataTable dt = new DataTable();

            dt.Columns.Add(new DataColumn() { ColumnName = "Tanggal", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Nomor Bon Terima Unit", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Nomor Bukti Pengeluaran Bank", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Nomor Memo", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Nomor Invoice", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Nilai Invoice DPP", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Nilai Invoice PPN", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Nilai Invoice Total", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Mutasi", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Saldo Akhir", DataType = typeof(string) });

            if (data.Count == 0)
            {
                dt.Rows.Add("", "", "", "", "TOTAL", "", "", "IDR", "0");
            }
            else
            {
                double totalBalance = 0;
                foreach (var item in data)
                {
                    totalBalance += item.FinalBalance.GetValueOrDefault();
                    dt.Rows.Add(item.Date.HasValue ? item.Date.Value.AddHours(offSet).ToString("dd-MMM-yyyy") : null, item.UnitReceiptNoteNo, item.BankExpenditureNoteNo, item.MemoNo, item.InvoiceNo, item.DPP.GetValueOrDefault().ToString("#,##0"),
                        item.PPN.GetValueOrDefault().ToString("#,##0"), item.Total.GetValueOrDefault().ToString("#,##0"), item.Mutation.GetValueOrDefault().ToString("#,##0"), item.FinalBalance);
                }

                dt.Rows.Add("", "", "", "", "TOTAL", "", "", "IDR", totalBalance.ToString("#,##0"));
            }
            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(dt, "Kartu Hutang") }, true);
        }

        public (ReadResponse<CreditorAccountViewModel>, double) GetReport(int page, int size, string suplierName, int month, int year, int offSet)
        {
            var queries = GetReport(suplierName, month, year, offSet);

            Pageable<CreditorAccountViewModel> pageable = new Pageable<CreditorAccountViewModel>(queries.Item1, page - 1, size);
            List<CreditorAccountViewModel> data = pageable.Data.ToList();

            return (new ReadResponse<CreditorAccountViewModel>(queries.Item1, pageable.TotalCount, new Dictionary<string, string>(), new List<string>()), queries.Item2);
        }

        private List<CreditorAccountViewModel> GetPreviousMonthReport(IQueryable<CreditorAccountModel> supplierQuery, int month, int year, int offSet)
        {
            List<CreditorAccountViewModel> result = new List<CreditorAccountViewModel>();
            List<CreditorAccountModel> previousMonthCreditorAccount = new List<CreditorAccountModel>();
            var debtQuery = supplierQuery.Where(x => x.FinalBalance > 0 && x.UnitReceiptNoteDate.HasValue && x.UnitReceiptNoteDate.Value < new DateTimeOffset(year, month, 1, 0, 0, 0, x.UnitReceiptNoteDate.Value.Offset)).ToList();
            var paidQuery = supplierQuery.Where(x => x.FinalBalance == 0 && x.BankExpenditureNoteDate.HasValue && x.BankExpenditureNoteDate.Value.Month == month && x.BankExpenditureNoteDate.Value.Year == year && x.UnitReceiptNoteDate.HasValue && x.UnitReceiptNoteDate.Value < new DateTimeOffset(year, month, 1, 0, 0, 0, x.UnitReceiptNoteDate.Value.Offset)).ToList();

            previousMonthCreditorAccount.AddRange(debtQuery);
            previousMonthCreditorAccount.AddRange(paidQuery);

            foreach (var item in previousMonthCreditorAccount.OrderBy(x => x.UnitReceiptNoteDate.GetValueOrDefault()))
            {
                double unitReceiptMutaion = 0;
                double bankExpenditureMutation = 0;
                double memoMutation = 0;
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
                    unitReceiptMutaion = vm.Mutation.GetValueOrDefault();
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
                        Total = item.BankExpenditureNoteMutation,
                        Mutation = item.BankExpenditureNoteMutation * -1,

                    };
                    bankExpenditureMutation = vm.Mutation.GetValueOrDefault();
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
                    memoMutation = vm.Mutation.GetValueOrDefault();
                    result.Add(vm);
                }
            }

            if (result.Count > 0)
            {
                CreditorAccountViewModel totalPrevious = new CreditorAccountViewModel()
                {
                    FinalBalance = result.Sum(x => x.Mutation)
                };
                result.Add(totalPrevious);
            }

            return result;
        }


        public (List<CreditorAccountViewModel>, double) GetReport(string suplierName, int month, int year, int offSet)
        {
            IQueryable<CreditorAccountModel> supplierQuery = DbContext.CreditorAccounts.AsQueryable().Where(x => x.SupplierName == suplierName);

            var currentQuery = supplierQuery.Where(x => x.UnitReceiptNoteDate.HasValue && x.UnitReceiptNoteDate.Value.Month == month && x.UnitReceiptNoteDate.Value.Year == year);

            if (currentQuery.Count() == 0)
            {
                return (new List<CreditorAccountViewModel>(), 0);
            }
            var result = GetPreviousMonthReport(supplierQuery, month, year, offSet);
            foreach (var item in currentQuery.OrderBy(x => x.UnitReceiptNoteDate.GetValueOrDefault()).ToList())
            {
                double unitReceiptMutaion = 0;
                double bankExpenditureMutation = 0;
                double memoMutation = 0;
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
                    unitReceiptMutaion = vm.Mutation.GetValueOrDefault();
                    result.Add(vm);
                }

                if (!string.IsNullOrEmpty(item.BankExpenditureNoteNo))
                {
                    if (item.BankExpenditureNoteDate.HasValue && item.BankExpenditureNoteDate.Value.Month == month && item.BankExpenditureNoteDate.Value.Year == year)
                    {
                        CreditorAccountViewModel vm = new CreditorAccountViewModel
                        {
                            BankExpenditureNoteNo = item.BankExpenditureNoteNo,
                            Date = item.BankExpenditureNoteDate.Value,
                            InvoiceNo = item.InvoiceNo,
                            DPP = item.BankExpenditureNoteDPP,
                            PPN = item.BankExpenditureNotePPN,
                            Total = item.BankExpenditureNoteMutation,
                            Mutation = item.BankExpenditureNoteMutation * -1,

                        };
                        bankExpenditureMutation = vm.Mutation.GetValueOrDefault();
                        result.Add(vm);
                    }

                }
                if (!string.IsNullOrEmpty(item.MemoNo))
                {
                    if (item.MemoDate.HasValue && item.MemoDate.Value.Month == month && item.MemoDate.Value.Year == year)
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
                        memoMutation = vm.Mutation.GetValueOrDefault();
                        result.Add(vm);
                    }

                }

                CreditorAccountViewModel resultVM = new CreditorAccountViewModel()
                {
                    InvoiceNo = item.InvoiceNo,
                    Mutation = unitReceiptMutaion + bankExpenditureMutation + memoMutation,
                    FinalBalance = unitReceiptMutaion + bankExpenditureMutation + memoMutation,
                    Currency = item.CurrencyCode
                };
                result.Add(resultVM);
            }

            return (result, result.Sum(x => x.FinalBalance).GetValueOrDefault());
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
            data.CurrencyCode = viewModel.Currency;
            data.FinalBalance = data.UnitReceiptMutation + data.BankExpenditureNoteMutation + data.MemoMutation;


            UpdateModel(data.Id, data);
            return await DbContext.SaveChangesAsync();
        }

        public async Task<CreditorAccountUnitReceiptNotePostedViewModel> GetByUnitReceiptNote(string supplierCode, string unitReceiptNote, string invoiceNo)
        {
            CreditorAccountModel data;

            if (string.IsNullOrEmpty(invoiceNo))
                data = await DbSet.FirstOrDefaultAsync(x => x.SupplierCode == supplierCode && x.UnitReceiptNoteNo == unitReceiptNote);
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
                SupplierName = data.SupplierName,
                Currency = data.CurrencyCode
            };
        }

        public async Task<CreditorAccountBankExpenditureNotePostedViewModel> GetByBankExpenditureNote(string supplierCode, string bankExpenditureNote, string invoiceNo)
        {
            CreditorAccountModel data = await DbSet.FirstOrDefaultAsync(x => x.SupplierCode == supplierCode && x.BankExpenditureNoteNo == bankExpenditureNote && x.InvoiceNo == invoiceNo);

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

        //public async Task<CreditorAccountMemoPostedViewModel> GetByMemo(string supplierCode, string memoNo, string invoiceNo)
        //{
        //    CreditorAccountModel data= await DbSet.FirstOrDefaultAsync(x => x.SupplierCode == supplierCode && x.MemoNo == memoNo && x.InvoiceNo == invoiceNo);

        //    if (data == null)
        //        return null;

        //    return new CreditorAccountMemoPostedViewModel()
        //    {
        //        CreditorAccountId = data.Id,
        //        Code = data.MemoNo,
        //        Date = data.MemoDate.Value,
        //        DPP = data.MemoDPP,
        //        PPN = data.MemoPPN,
        //        InvoiceNo = data.InvoiceNo,
        //        SupplierCode = data.SupplierCode,
        //        SupplierName = data.SupplierName
        //    };
        //}

        public async Task<int> CreateFromBankExpenditureNoteAsync(CreditorAccountBankExpenditureNotePostedViewModel viewModel)
        {
            CreditorAccountModel model = await DbSet.FirstOrDefaultAsync(x => x.BankExpenditureNoteNo == null && x.SupplierCode == viewModel.SupplierCode && x.InvoiceNo == viewModel.InvoiceNo);

            if (model == null)

                //do nothing

                return 1;

            model.BankExpenditureNoteDate = viewModel.Date;
            model.BankExpenditureNoteId = viewModel.Id;
            model.BankExpenditureNoteMutation = viewModel.Mutation;
            model.BankExpenditureNoteNo = viewModel.Code;
            model.FinalBalance = model.UnitReceiptMutation + (model.BankExpenditureNoteMutation * -1) + model.MemoMutation;

            UpdateModel(model.Id, model);
            return await DbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateFromBankExpenditureNoteAsync(CreditorAccountBankExpenditureNotePostedViewModel viewModel)
        {
            CreditorAccountModel data = await DbSet.FirstOrDefaultAsync(x => x.SupplierCode == viewModel.SupplierCode && x.BankExpenditureNoteNo == viewModel.Code && x.InvoiceNo == viewModel.InvoiceNo);

            if (data == null)
                throw new NotFoundException();

            data.BankExpenditureNoteNo = viewModel.Code;
            data.BankExpenditureNoteDate = viewModel.Date;
            data.BankExpenditureNoteMutation = viewModel.Mutation;
            data.FinalBalance = data.UnitReceiptMutation + (data.BankExpenditureNoteMutation * -1) + data.MemoMutation;


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
                UnitReceiptNoteNo = viewModel.Code,
                CurrencyCode = viewModel.Currency
            };

            return await CreateAsync(model);
        }

        public async Task<int> DeleteFromUnitReceiptNoteAsync(int id)
        {
            CreditorAccountModel model = await DbSet.FirstOrDefaultAsync(x => x.Id == id);

            return await DeleteAsync(model.Id);
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

        public async Task<int> DeleteFromBankExpenditureNoteListAsync(string code)
        {
            var models = await DbSet.Where(x => x.BankExpenditureNoteNo == code).ToListAsync();
            foreach (var model in models)
            {
                model.BankExpenditureNoteDate = null;
                model.BankExpenditureNoteDPP = 0;
                model.BankExpenditureNoteId = 0;
                model.BankExpenditureNoteMutation = 0;
                model.BankExpenditureNoteNo = null;
                model.BankExpenditureNotePPN = 0;
                model.FinalBalance = model.UnitReceiptMutation + model.BankExpenditureNoteMutation + model.MemoMutation;
                await UpdateAsync(model.Id, model);
            }
            return 1;
        }

        public async Task<int> UpdateFromUnitPaymentOrderAsync(CreditorAccountUnitPaymentOrderPostedViewModel viewModel)
        {
            if (viewModel.CreditorAccounts != null)
            {
                foreach (var item in viewModel.CreditorAccounts)
                {
                    var creditorAccount = await DbContext.CreditorAccounts.FirstOrDefaultAsync(x => x.SupplierCode == item.SupplierCode && x.UnitReceiptNoteNo == item.Code);
                    creditorAccount.InvoiceNo = viewModel.InvoiceNo;
                }
                return await DbContext.SaveChangesAsync();
            }
            else
            {
                return 0;
            }

        }

        public async Task<int> CreateFromMemoAsync(CreditorAccountMemoPostedViewModel viewModel)
        {
            CreditorAccountModel model = await DbSet.FirstOrDefaultAsync(x => x.SupplierCode == viewModel.SupplierCode && x.InvoiceNo == viewModel.InvoiceNo);

            if (model == null)
                throw new NotFoundException();

            model.MemoDate = viewModel.Date;
            model.MemoDPP = viewModel.DPP;
            model.MemoPPN = viewModel.PPN;
            model.MemoNo = viewModel.Code;
            model.MemoMutation = viewModel.DPP + viewModel.PPN;
            model.FinalBalance = model.UnitReceiptMutation + model.BankExpenditureNoteMutation + model.MemoMutation;

            UpdateModel(model.Id, model);
            return await DbContext.SaveChangesAsync();
        }
    }
}
