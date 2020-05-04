using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.SalesReceipt;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.SalesReceipt;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.SalesReceipt
{
    public class SalesReceiptDataUtil : BaseDataUtil<SalesReceiptService, SalesReceiptModel>
    {
        public SalesReceiptDataUtil(SalesReceiptService service) : base(service)
        {
        }

        public override async Task<SalesReceiptModel> GetNewData()
        {
            var data = await base.GetNewData();

            data.Code = "code";
            data.AutoIncreament = 1;
            data.SalesReceiptNo = "SalesReceiptNo";
            data.SalesReceiptDate = DateTimeOffset.UtcNow;
            data.UnitId = 1;
            data.UnitName = "Dying";
            data.BuyerId = 1;
            data.BuyerName = "BuyerName";
            data.BuyerAddress = "BuyerAddress";
            data.OriginAccountNumber = "OriginAccountNumber";
            data.CurrencyId = 1;
            data.CurrencyCode = "CurrencyCode";
            data.CurrencySymbol = "CurrencySymbol";
            data.CurrencyRate = 1;
            data.BankId = 1;
            data.AccountName = "AccountName";
            data.AccountNumber = "AccountNumber";
            data.BankName = "BankName";
            data.BankCode = "BankCode";
            data.AdministrationFee = 1;
            data.TotalPaid = 1;

            data.SalesReceiptDetails = new List<SalesReceiptDetailModel>()
                {
                    new SalesReceiptDetailModel()
                    {
                        SalesInvoiceId = Convert.ToInt32(1),
                        SalesInvoiceNo = "SalesInvoiceNo",
                        DueDate = DateTimeOffset.UtcNow,
                        VatType = "PPN BUMN",
                        Tempo = 16,
                        CurrencyId = 1,
                        CurrencyCode = "IDR",
                        CurrencySymbol = "Rp",
                        CurrencyRate = 14000,
                        TotalPayment = 10000,
                        TotalPaid = 1000,
                        Paid = 1000,
                        Nominal = 1000,
                        Unpaid = 8000,
                        OverPaid = 0,
                        IsPaidOff = false,

                    }
                };
            return data;
        }
    }
}
