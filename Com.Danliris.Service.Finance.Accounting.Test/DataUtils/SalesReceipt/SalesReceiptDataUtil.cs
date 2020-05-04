using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.SalesReceipt;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.SalesReceipt;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.NewIntegrationViewModel;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.SalesReceipt;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.SalesReceipt
{
    public class SalesReceiptDataUtil
    {
        private readonly SalesReceiptService Service;

        public SalesReceiptDataUtil(SalesReceiptService service)
        {
            Service = service;
        }

        public SalesReceiptModel GetNewData()
        {
            SalesReceiptModel TestData = new SalesReceiptModel()
            {
                Code = "code",
                AutoIncreament = 1,
                SalesReceiptNo = "SalesReceiptNo",
                SalesReceiptDate = DateTimeOffset.UtcNow,
                UnitId = 1,
                UnitName = "Dying",
                BuyerId = 1,
                BuyerName = "BuyerName",
                BuyerAddress = "BuyerAddress",
                OriginAccountNumber = "OriginAccountNumber",
                CurrencyId = 1,
                CurrencyCode = "CurrencyCode",
                CurrencySymbol = "CurrencySymbol",
                CurrencyRate = 1,
                BankId = 1,
                AccountName = "AccountName",
                AccountNumber = "AccountNumber",
                BankName = "BankName",
                BankCode = "BankCode",
                AdministrationFee = 1,
                TotalPaid = 1,
                SalesReceiptDetails = new List<SalesReceiptDetailModel>()
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
                }
            };

            return TestData;
        }

        public SalesReceiptViewModel GetDataToValidate()
        {
            SalesReceiptViewModel TestData = new SalesReceiptViewModel()
            {
                SalesReceiptDate = DateTimeOffset.UtcNow.AddDays(-1),
                Unit = new NewUnitViewModel()
                {
                    Id = 0,
                    Name = "",
                },
                Buyer = new NewBuyerViewModel()
                {
                    Id = 0,
                    Name = "",
                    Address = "",
                },
                OriginAccountNumber = "",
                Currency = new CurrencyViewModel()
                {
                    Id = 0,
                    Code = "",
                    Symbol = "",
                    Rate = 0,
                },
                Bank = new AccountBankViewModel()
                {
                    Id = 0,
                    AccountName = "",
                    AccountNumber = "",
                    BankName = "",
                    Code = "",
                },
                AdministrationFee = -1,
                TotalPaid = -1,
                SalesReceiptDetails = new List<SalesReceiptDetailViewModel>{
                        new SalesReceiptDetailViewModel{
                            Id = 0,
                            SalesInvoice = new SalesInvoiceViewModel()
                            {
                                Id = 0,
                                SalesInvoiceNo = "",
                                Currency = new CurrencyViewModel()
                                {
                                    Id = 0,
                                    Code = "",
                                    Symbol = "",
                                    Rate = 0,
                                },
                            },
                            DueDate = DateTimeOffset.UtcNow.AddDays(-1),
                            VatType = "",
                            Tempo = -1,
                            TotalPayment = -1,
                            TotalPaid = -1,
                            Paid = -1,
                            Nominal = -1,
                            Unpaid = -1,
                            IsPaidOff = false
                        }
                    }
            };

            return TestData;
        }

        public async Task<SalesReceiptModel> GetTestDataById()
        {
            SalesReceiptModel model = GetNewData();
            await Service.CreateAsync(model);
            return await Service.ReadByIdAsync(model.Id);
        }
    }
}
