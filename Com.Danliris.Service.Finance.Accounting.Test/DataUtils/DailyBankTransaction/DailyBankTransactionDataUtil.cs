using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.DailyBankTransaction;
using System;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.DailyBankTransaction
{
    public class DailyBankTransactionDataUtil
    {
        private readonly DailyBankTransactionService Service;

        public DailyBankTransactionDataUtil(DailyBankTransactionService service)
        {
            Service = service;
        }

        public DailyBankTransactionModel GetNewData()
        {
            DailyBankTransactionModel TestData = new DailyBankTransactionModel()
            {
                AccountBankAccountName = "AccountName",
                AccountBankAccountNumber = "AccountNumber",
                AccountBankCode = "BankCode",
                AccountBankCurrencyCode = "CurrencyCode",
                AccountBankCurrencyId = "CurrencyId",
                AccountBankCurrencySymbol = "CurrencySymbol",
                AccountBankId = "BankId",
                AccountBankName = "BankName",
                AfterNominal = 0,
                BeforeNominal = 0,
                BuyerCode = "BuyerCode",
                BuyerId = "BuyerId",
                BuyerName = "BuyerName",
                Date = DateTimeOffset.UtcNow,
                Nominal = 1000,
                ReferenceNo = "ReferenceNo",
                ReferenceType = "ReferenceType",
                Remark = "Remark",
                SourceType = "Operasional",
                Status = "IN",
                SupplierCode = "SupplierCode",
                SupplierName = "SupplierName",
                SupplierId = "SupplierId"
            };

            return TestData;
        }

        public async Task<DailyBankTransactionModel> GetTestDataIn()
        {
            DailyBankTransactionModel model = GetNewData();
            await Service.CreateAsync(model);
            return await Service.ReadByIdAsync(model.Id);
        }

        public async Task<DailyBankTransactionModel> GetTestDataOut()
        {
            DailyBankTransactionModel model = GetNewData();
            model.Status = "OUT";
            await Service.CreateAsync(model);
            return await Service.ReadByIdAsync(model.Id);
        }
    }
}
