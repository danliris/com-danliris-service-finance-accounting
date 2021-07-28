using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.GarmentFinance.BankCashReceiptDetail;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentFinance.BankCashReceiptDetail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.GarmentFinance.BankCashReceiptDetail
{
    public class BankCashReceiptDetailDataUtil
    {
        private readonly BankCashReceiptDetailService Service;

        public BankCashReceiptDetailDataUtil(BankCashReceiptDetailService service)
        {
            Service = service;
        }

        public BankCashReceiptDetailModel GetNewData()
        {
            return new BankCashReceiptDetailModel
            {
                BankCashReceiptId = 1,
                BankCashReceiptDate = DateTimeOffset.Now,
                BankCashReceiptNo = "bankCashReceiptNo",
                Items = new List<BankCashReceiptDetailItemModel>
                {
                    new BankCashReceiptDetailItemModel()
                    {
                        Amount = 1,
                        BankCashReceiptDetailId = 1,
                        BuyerCode = "code",
                        BuyerId = 1,
                        BuyerName = "name",
                        CurrencyCode = "code",
                        CurrencyId = 1,
                        CurrencyRate = 1,
                        InvoiceId = 1,
                        InvoiceNo = "no",
                    }
                }
            };
        }

        public async Task<BankCashReceiptDetailModel> GetTestData()
        {
            BankCashReceiptDetailModel model = GetNewData();
            await Service.CreateAsync(model);
            return await Service.ReadByIdAsync(model.Id);
        }
    }
}
