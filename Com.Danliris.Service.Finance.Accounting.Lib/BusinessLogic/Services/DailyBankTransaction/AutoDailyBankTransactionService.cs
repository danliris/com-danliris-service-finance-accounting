using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRequestDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.DPPVATBankExpenditureNote;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentInvoicePurchasingDisposition;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.OthersExpenditureProofDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.PaymentDispositionNote;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.NewIntegrationViewModel;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.DailyBankTransaction
{
    public class AutoDailyBankTransactionService : IAutoDailyBankTransactionService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDailyBankTransactionService _dailyBankTransactionService;
        private readonly FinanceDbContext _dbContext;

        public AutoDailyBankTransactionService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _dailyBankTransactionService = serviceProvider.GetService<IDailyBankTransactionService>();
            _dbContext = serviceProvider.GetService<FinanceDbContext>();
        }

        public async Task<int> AutoCreateVbApproval(List<ApprovalVBAutoJournalDto> dtos) 
        {
            var result = 0;
            foreach(var dto in dtos)
            {
                result += await AutoCreateVbApproval(dto);
            }
            return result;
        }
        public async Task<int> AutoCreateVbApproval(ApprovalVBAutoJournalDto dto)
        {
            var dailyBankTransactionModel = new DailyBankTransactionModel()
            {
                AccountBankAccountName = dto.Bank.AccountName,
                AccountBankAccountNumber = dto.Bank.AccountNumber,
                AccountBankCode = dto.Bank.BankCode,
                AccountBankCurrencyCode = dto.Bank.Currency.Code,
                AccountBankCurrencyId = (int)dto.Bank.Currency.Id,
                AccountBankCurrencySymbol = dto.Bank.Currency.Symbol,
                AccountBankId = dto.Bank.Id,
                AccountBankName = dto.Bank.BankName,
                Date = dto.VbRequestDocument.Date,
                Nominal = dto.VbRequestDocument.Amount,
                CurrencyRate = (decimal)dto.VbRequestDocument.CurrencyRate,
                ReferenceNo = dto.VbRequestDocument.DocumentNo,
                ReferenceType = "Approval VB Inklaring",
                SourceType = "Approval VB Inklaring",
                SupplierCode = dto.VbRequestDocument.SuppliantUnitCode,
                SupplierId = dto.VbRequestDocument.SuppliantUnitId,
                SupplierName = dto.VbRequestDocument.SuppliantUnitName,
                Status = "Operasional",
                IsPosted = true
            };
            return await _dailyBankTransactionService.CreateAsync(dailyBankTransactionModel);
        }

        public Task<int> AutoCreateFromGarmentInvoicePurchasingDisposition(GarmentInvoicePurchasingDispositionModel model)
        {
            var nominal = model.Items.Sum(item => (decimal)item.TotalPaid * (decimal)model.CurrencyRate);
            var dailyBankTransactionModel = new DailyBankTransactionModel()
            {
                AccountBankAccountName = model.BankAccountName,
                AccountBankAccountNumber = model.BankAccountNo,
                AccountBankCode = model.BankCode,
                AccountBankCurrencyCode = model.CurrencyCode,
                AccountBankCurrencyId = model.CurrencyId,
                AccountBankCurrencySymbol = model.CurrencySymbol,
                AccountBankId = model.BankId,
                AccountBankName = model.BankName,
                Date = model.InvoiceDate,
                Nominal = nominal,
                CurrencyRate = (decimal)model.CurrencyRate,
                ReferenceNo = model.InvoiceNo,
                ReferenceType = "Bukti Pembayaran Disposisi Job Garment",
                Remark = model.CurrencyCode != "IDR" ? $"Pembayaran atas {model.BankCurrencyCode} dengan nominal {string.Format("{0:n}", nominal)} dan kurs {model.CurrencyCode}" : "",
                SourceType = model.PaymentType,
                SupplierCode = model.SupplierCode,
                SupplierId = model.SupplierId,
                SupplierName = model.SupplierName,
                Status = "OUT",
                IsPosted = true
            };

            if (model.BankCurrencyCode != "IDR")
            {
                dailyBankTransactionModel.Nominal = model.Items.Sum(item => (decimal)item.TotalPaid);
                dailyBankTransactionModel.NominalValas = nominal;
            }

            return _dailyBankTransactionService.CreateAsync(dailyBankTransactionModel);
        }

        public Task<int> AutoRevertFromGarmentInvoicePurchasingDisposition(GarmentInvoicePurchasingDispositionModel model)
        {
            return _dailyBankTransactionService.DeleteByReferenceNoAsync(model.InvoiceNo);
        }

        public Task<int> AutoCreateFromPaymentDisposition(PaymentDispositionNoteModel model)
        {
            var nominal = model.Items.Sum(item => (decimal)item.PayToSupplier);
            var dailyBankTransactionModel = new DailyBankTransactionModel()
            {
                AccountBankAccountName = model.BankAccountName,
                AccountBankAccountNumber = model.BankAccountNumber,
                AccountBankCode = model.BankCode,
                AccountBankCurrencyCode = model.BankCurrencyCode,
                AccountBankCurrencyId = model.BankCurrencyId,
                AccountBankCurrencySymbol = model.BankCurrencyCode,
                AccountBankId = model.BankId,
                AccountBankName = model.BankName,
                Date = model.PaymentDate,
                Nominal = nominal,
                CurrencyRate = (decimal)model.CurrencyRate,
                ReferenceNo = model.PaymentDispositionNo,
                ReferenceType = "Pembayaran Disposisi",
                Remark = model.CurrencyCode != "IDR" ? $"Pembayaran atas {model.BankCurrencyCode} dengan nominal {string.Format("{0:n}", nominal)} dan kurs {model.CurrencyCode}" : "",
                SourceType = model.TransactionType,
                SupplierCode = model.SupplierCode,
                SupplierId = model.SupplierId,
                SupplierName = model.SupplierName,
                Status = "OUT",
                IsPosted = true
            };

            if (model.BankCurrencyCode != "IDR")
            {
                dailyBankTransactionModel.Nominal = model.Items.Sum(item => (decimal)item.PayToSupplier);
                dailyBankTransactionModel.NominalValas = nominal;
            }

            return _dailyBankTransactionService.CreateAsync(dailyBankTransactionModel);
        }

        public Task<int> AutoRevertFromPaymentDisposition(PaymentDispositionNoteModel model)
        {
            //var nominal = model.Items.Sum(item => (decimal)item.TotalPaid * (decimal)model.CurrencyRate);
            //var dailyBankTransactionModel = new DailyBankTransactionModel()
            //{
            //    AccountBankAccountName = model.BankAccountName,
            //    AccountBankAccountNumber = model.BankAccountNumber,
            //    AccountBankCode = model.BankCode,
            //    AccountBankCurrencyCode = model.BankCurrencyCode,
            //    AccountBankCurrencyId = model.BankCurrencyId,
            //    AccountBankCurrencySymbol = model.BankCurrencyCode,
            //    AccountBankId = model.BankId,
            //    AccountBankName = model.BankName,
            //    Date = model.PaymentDate,
            //    Nominal = nominal,
            //    ReferenceNo = model.PaymentDispositionNo,
            //    ReferenceType = "Pembayaran Disposisi",
            //    Remark = model.CurrencyCode != "IDR" ? $"Pembayaran atas {model.BankCurrencyCode} dengan nominal {string.Format("{0:n}", nominal)} dan kurs {model.CurrencyCode}" : "",
            //    SourceType = model.TransactionType,
            //    SupplierCode = model.SupplierCode,
            //    SupplierId = model.SupplierId,
            //    SupplierName = model.SupplierName,
            //    Status = "IN"
            //};
            return _dailyBankTransactionService.DeleteByReferenceNoAsync(model.PaymentDispositionNo);
        }

        private async Task<AccountBank> GetAccountBank(int accountBankId)
        {
            var http = _serviceProvider.GetService<IHttpClientService>();

            var response = await http.GetAsync(APIEndpoint.Core + $"master/account-banks/{accountBankId}");

            var responseString = await response.Content.ReadAsStringAsync();
            var jsonSerializationSetting = new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Ignore };

            var result = JsonConvert.DeserializeObject<APIDefaultResponse<AccountBank>>(responseString, jsonSerializationSetting);

            return result.data;
        }

        public async Task<int> AutoCreateFromOthersExpenditureProofDocument(OthersExpenditureProofDocumentModel model, List<OthersExpenditureProofDocumentItemModel> itemModels)
        {
            var accountBank = await GetAccountBank(model.AccountBankId);

            var total = itemModels.Sum(element => element.Debit);

            var dailyBankTransactionModel = new DailyBankTransactionModel()
            {
                AccountBankAccountName = accountBank.AccountName,
                AccountBankAccountNumber = accountBank.AccountNumber,
                AccountBankCode = accountBank.BankCode,
                AccountBankCurrencyCode = accountBank.Currency.Code,
                AccountBankCurrencyId = accountBank.Currency.Id,
                AccountBankCurrencySymbol = accountBank.Currency.Symbol,
                AccountBankId = accountBank.Id,
                AccountBankName = accountBank.BankName,
                Date = model.Date,
                Nominal = itemModels.Sum(item => item.Debit),
                CurrencyRate = (decimal)model.CurrencyRate,
                ReferenceNo = model.DocumentNo,
                Remark = $"{model.Remark}\n\nPembayaran atas {accountBank.Currency.Code} dengan nominal {string.Format("{0:n}", total)}",
                SourceType = model.Type,
                Status = "OUT",
                IsPosted = true
            };

            if (accountBank.Currency.Code != "IDR")
                dailyBankTransactionModel.NominalValas = itemModels.Sum(item => item.Debit) * (decimal)model.CurrencyRate;

            return await _dailyBankTransactionService.CreateAsync(dailyBankTransactionModel);
        }

        public async Task<int> AutoRevertFromOthersExpenditureProofDocument(OthersExpenditureProofDocumentModel model, List<OthersExpenditureProofDocumentItemModel> itemModels)
        {
            var accountBank = await GetAccountBank(model.AccountBankId);
            var dailyBankTransactionModel = new DailyBankTransactionModel()
            {
                AccountBankAccountName = accountBank.AccountName,
                AccountBankAccountNumber = accountBank.AccountNumber,
                AccountBankCode = accountBank.BankCode,
                AccountBankCurrencyCode = accountBank.Currency.Code,
                AccountBankCurrencyId = accountBank.Currency.Id,
                AccountBankCurrencySymbol = accountBank.Currency.Symbol,
                AccountBankId = accountBank.Id,
                AccountBankName = accountBank.BankName,
                Date = model.Date,
                Nominal = itemModels.Sum(item => item.Debit),
                ReferenceNo = model.DocumentNo,
                Remark = "Pembayaran Lain - lain",
                SourceType = model.Type,
                Status = "IN"
            };
            return await _dailyBankTransactionService.CreateAsync(dailyBankTransactionModel);
        }

        public async Task<int> AutoCreateFromGarmentBankExpenditureDPPVAT(DPPVATBankExpenditureNoteModel model)
        {
            var accountBank = await GetAccountBank(model.BankAccountId);
            var dailyBankTransactionModel = new DailyBankTransactionModel()
            {
                AccountBankAccountName = accountBank.AccountName,
                AccountBankAccountNumber = accountBank.AccountNumber,
                AccountBankCode = accountBank.BankCode,
                AccountBankCurrencyCode = accountBank.Currency.Code,
                AccountBankCurrencyId = accountBank.Currency.Id,
                AccountBankCurrencySymbol = accountBank.Currency.Symbol,
                AccountBankId = accountBank.Id,
                AccountBankName = accountBank.BankName,
                Date = model.Date,
                Nominal = (decimal)model.Amount,
                ReferenceNo = model.DocumentNo,
                Remark = "Pembayaran Lain - lain",
                SourceType = "OPERASIONAL",
                Status = "IN"
            };
            return await _dailyBankTransactionService.CreateAsync(dailyBankTransactionModel);
        }

        public async Task<int> AutoCreateFromClearenceVB(List<int> vbRealizationIds, AccountBankViewModel accountBank)
        {
            var realizations = _dbContext.VBRealizationDocuments.Where(entity => vbRealizationIds.Contains(entity.Id) && entity.Type == VBRequestDocument.VBType.WithPO).ToList();

            var result = 0;
            foreach (var realization in realizations)
            {
                var realizationItems = _dbContext.VBRealizationDocumentUnitCostsItems.Where(entity => entity.VBRealizationDocumentId == realization.Id).ToList();
                var dailyBankTransactionModel = new DailyBankTransactionModel()
                {
                    AccountBankAccountName = accountBank.AccountName,
                    AccountBankAccountNumber = accountBank.AccountNumber,
                    AccountBankCode = accountBank.BankCode,
                    AccountBankCurrencyCode = accountBank.Currency.Code,
                    AccountBankCurrencyId = (int)accountBank.Currency.Id,
                    AccountBankCurrencySymbol = accountBank.Currency.Symbol,
                    AccountBankId = accountBank.Id,
                    AccountBankName = accountBank.BankName,
                    Date = realization.Date,
                    Nominal = realizationItems.Sum(item => item.Amount),
                    CurrencyRate = (decimal)realization.CurrencyRate,
                    ReferenceNo = realization.DocumentNo,
                    ReferenceType = "Clearence VB",
                    Remark = $"{realization.Remark}\n\nPembayaran atas {accountBank.Currency.Code} dengan nominal {string.Format("{0:n}", realizationItems.Sum(item => item.Amount))}",
                    SourceType = "OPERASIONAL",
                    Status = "OUT",
                    IsPosted = true
                };

                if (accountBank.Currency.Code != "IDR")
                    dailyBankTransactionModel.NominalValas = realizationItems.Sum(item => item.Amount) * (decimal)realization.CurrencyRate;

                result += await _dailyBankTransactionService.CreateAsync(dailyBankTransactionModel);
            }


            return result;
        }
    }
}