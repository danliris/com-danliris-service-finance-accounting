using Com.Danliris.Service.Finance.Accounting.Lib.PDFTemplates;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.GarmentFinance.BankCashReceipt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.PDFTemplates
{
    public class GarmentFinanceBankCashReceiptPdfTemplateTest
    {
        public BankCashReceiptViewModel viewModelIDR
        {
            get
            {
                return new BankCashReceiptViewModel()
                {
                    ReceiptNo = "receiptNo",
                    Amount = 1.00M,
                    Currency = new Lib.ViewModels.NewIntegrationViewModel.CurrencyViewModel()
                    {
                        Id = 1,
                        Code = "IDR",
                        Description = "description",
                        Rate = 1,
                        Symbol = "symbol",
                    },
                    NumberingCode = "numberingCode",
                    ReceiptDate = DateTimeOffset.Now,
                    Remarks = "remarks",
                    DebitCoa = new Lib.ViewModels.NewIntegrationViewModel.ChartOfAccountViewModel()
                    {
                        Id = "1",
                        Code = "code",
                        Name = "name",
                    },
                    IncomeType = "incomeType",
                    Bank = new Lib.ViewModels.NewIntegrationViewModel.AccountBankViewModel()
                    {
                        AccountCOA = "AccountCOA",
                        AccountName = "AccountName",
                        AccountNumber = "AccountNumber",
                        BankCode = "BankCode",
                        BankName = "BankName",
                        Code = "Code",
                        Currency = new Lib.ViewModels.NewIntegrationViewModel.CurrencyViewModel()
                        {
                            Code = "IDR",
                            Description = "Description",
                            Id = 1,
                            Rate = 1,
                            Symbol = "Rp"
                        },
                        Id = 1
                    },
                    Id = 1,
                    Items = new List<BankCashReceiptItemViewModel>()
                    {
                        new BankCashReceiptItemViewModel()
                        {
                            Id = 1,
                            AccAmount = new Lib.ViewModels.NewIntegrationViewModel.ChartOfAccountViewModel()
                            {
                                Id = "1",
                                Code = "code",
                                Name = "name",
                            },
                            AccNumber = new Lib.ViewModels.NewIntegrationViewModel.ChartOfAccountViewModel()
                            {
                                Id = "1",
                                Code = "code",
                                Name = "name",
                            },
                            AccSub = new Lib.ViewModels.NewIntegrationViewModel.ChartOfAccountViewModel()
                            {
                                Id = "1",
                                Code = "code",
                                Name = "name",
                            },
                            AccUnit = new Lib.ViewModels.NewIntegrationViewModel.ChartOfAccountViewModel()
                            {
                                Id = "1",
                                Code = "code",
                                Name = "name",
                            },
                            Amount = 1,
                            C1A = 1,
                            C1B = 1,
                            C2A = 1,
                            C2B = 1,
                            C2C = 1,
                            NoteNumber = "noteNumber",
                            Remarks = "remarks",
                            Summary = 1,
                        }
                    }
                };
            }
        }

        public BankCashReceiptViewModel viewModelDollar
        {
            get
            {
                return new BankCashReceiptViewModel()
                {
                    ReceiptNo = "receiptNo",
                    Amount = 1.00M,
                    Currency = new Lib.ViewModels.NewIntegrationViewModel.CurrencyViewModel()
                    {
                        Id = 1,
                        Code = "USD",
                        Description = "description",
                        Rate = 1,
                        Symbol = "symbol",
                    },
                    NumberingCode = "numberingCode",
                    ReceiptDate = DateTimeOffset.Now,
                    Remarks = "remarks",
                    DebitCoa = new Lib.ViewModels.NewIntegrationViewModel.ChartOfAccountViewModel()
                    {
                        Id = "1",
                        Code = "code",
                        Name = "name",
                    },
                    IncomeType = "incomeType",
                    Bank = new Lib.ViewModels.NewIntegrationViewModel.AccountBankViewModel()
                    {
                        AccountCOA = "AccountCOA",
                        AccountName = "AccountName",
                        AccountNumber = "AccountNumber",
                        BankCode = "BankCode",
                        BankName = "BankName",
                        Code = "Code",
                        Currency = new Lib.ViewModels.NewIntegrationViewModel.CurrencyViewModel()
                        {
                            Code = "USD",
                            Description = "Description",
                            Id = 1,
                            Rate = 1,
                            Symbol = "$"
                        },
                        Id = 1
                    },
                    Id = 1,
                    Items = new List<BankCashReceiptItemViewModel>()
                    {
                        new BankCashReceiptItemViewModel()
                        {
                            Id = 1,
                            AccAmount = new Lib.ViewModels.NewIntegrationViewModel.ChartOfAccountViewModel()
                            {
                                Id = "1",
                                Code = "code",
                                Name = "name",
                            },
                            AccNumber = new Lib.ViewModels.NewIntegrationViewModel.ChartOfAccountViewModel()
                            {
                                Id = "1",
                                Code = "code",
                                Name = "name",
                            },
                            AccSub = new Lib.ViewModels.NewIntegrationViewModel.ChartOfAccountViewModel()
                            {
                                Id = "1",
                                Code = "code",
                                Name = "name",
                            },
                            AccUnit = new Lib.ViewModels.NewIntegrationViewModel.ChartOfAccountViewModel()
                            {
                                Id = "1",
                                Code = "code",
                                Name = "name",
                            },
                            Amount = 1,
                            C1A = 1,
                            C1B = 1,
                            C2A = 1,
                            C2B = 1,
                            C2C = 1,
                            NoteNumber = "noteNumber",
                            Remarks = "remarks",
                            Summary = 1,
                        }
                    }
                };
            }
        }

        public BankCashReceiptViewModel viewModelIDRNonZero
        {
            get
            {
                return new BankCashReceiptViewModel()
                {
                    ReceiptNo = "receiptNo",
                    Amount = 1.12M,
                    Currency = new Lib.ViewModels.NewIntegrationViewModel.CurrencyViewModel()
                    {
                        Id = 1,
                        Code = "IDR",
                        Description = "description",
                        Rate = 1,
                        Symbol = "symbol",
                    },
                    NumberingCode = "numberingCode",
                    ReceiptDate = DateTimeOffset.Now,
                    Remarks = "remarks",
                    DebitCoa = new Lib.ViewModels.NewIntegrationViewModel.ChartOfAccountViewModel()
                    {
                        Id = "1",
                        Code = "code",
                        Name = "name",
                    },
                    IncomeType = "incomeType",
                    Bank = new Lib.ViewModels.NewIntegrationViewModel.AccountBankViewModel()
                    {
                        AccountCOA = "AccountCOA",
                        AccountName = "AccountName",
                        AccountNumber = "AccountNumber",
                        BankCode = "BankCode",
                        BankName = "BankName",
                        Code = "Code",
                        Currency = new Lib.ViewModels.NewIntegrationViewModel.CurrencyViewModel()
                        {
                            Code = "IDR",
                            Description = "Description",
                            Id = 1,
                            Rate = 1,
                            Symbol = "Rp"
                        },
                        Id = 1
                    },
                    Id = 1,
                    Items = new List<BankCashReceiptItemViewModel>()
                    {
                        new BankCashReceiptItemViewModel()
                        {
                            Id = 1,
                            AccAmount = new Lib.ViewModels.NewIntegrationViewModel.ChartOfAccountViewModel()
                            {
                                Id = "1",
                                Code = "code",
                                Name = "name",
                            },
                            AccNumber = new Lib.ViewModels.NewIntegrationViewModel.ChartOfAccountViewModel()
                            {
                                Id = "1",
                                Code = "code",
                                Name = "name",
                            },
                            AccSub = new Lib.ViewModels.NewIntegrationViewModel.ChartOfAccountViewModel()
                            {
                                Id = "1",
                                Code = "code",
                                Name = "name",
                            },
                            AccUnit = new Lib.ViewModels.NewIntegrationViewModel.ChartOfAccountViewModel()
                            {
                                Id = "1",
                                Code = "code",
                                Name = "name",
                            },
                            Amount = 1,
                            C1A = 1,
                            C1B = 1,
                            C2A = 1,
                            C2B = 1,
                            C2C = 1,
                            NoteNumber = "noteNumber",
                            Remarks = "remarks",
                            Summary = 1,
                        }
                    }
                };
            }
        }

        public BankCashReceiptViewModel viewModelDollarNonZero
        {
            get
            {
                return new BankCashReceiptViewModel()
                {
                    ReceiptNo = "receiptNo",
                    Amount = 2.12M,
                    Currency = new Lib.ViewModels.NewIntegrationViewModel.CurrencyViewModel()
                    {
                        Id = 1,
                        Code = "USD",
                        Description = "description",
                        Rate = 1,
                        Symbol = "symbol",
                    },
                    NumberingCode = "numberingCode",
                    ReceiptDate = DateTimeOffset.Now,
                    Remarks = "remarks",
                    DebitCoa = new Lib.ViewModels.NewIntegrationViewModel.ChartOfAccountViewModel()
                    {
                        Id = "1",
                        Code = "code",
                        Name = "name",
                    },
                    IncomeType = "incomeType",
                    Bank = new Lib.ViewModels.NewIntegrationViewModel.AccountBankViewModel()
                    {
                        AccountCOA = "AccountCOA",
                        AccountName = "AccountName",
                        AccountNumber = "AccountNumber",
                        BankCode = "BankCode",
                        BankName = "BankName",
                        Code = "Code",
                        Currency = new Lib.ViewModels.NewIntegrationViewModel.CurrencyViewModel()
                        {
                            Code = "USD",
                            Description = "Description",
                            Id = 1,
                            Rate = 1,
                            Symbol = "$"
                        },
                        Id = 1
                    },
                    Id = 1,
                    Items = new List<BankCashReceiptItemViewModel>()
                    {
                        new BankCashReceiptItemViewModel()
                        {
                            Id = 1,
                            AccAmount = new Lib.ViewModels.NewIntegrationViewModel.ChartOfAccountViewModel()
                            {
                                Id = "1",
                                Code = "code",
                                Name = "name",
                            },
                            AccNumber = new Lib.ViewModels.NewIntegrationViewModel.ChartOfAccountViewModel()
                            {
                                Id = "1",
                                Code = "code",
                                Name = "name",
                            },
                            AccSub = new Lib.ViewModels.NewIntegrationViewModel.ChartOfAccountViewModel()
                            {
                                Id = "1",
                                Code = "code",
                                Name = "name",
                            },
                            AccUnit = new Lib.ViewModels.NewIntegrationViewModel.ChartOfAccountViewModel()
                            {
                                Id = "1",
                                Code = "code",
                                Name = "name",
                            },
                            Amount = 1,
                            C1A = 1,
                            C1B = 1,
                            C2A = 1,
                            C2B = 1,
                            C2C = 1,
                            NoteNumber = "noteNumber",
                            Remarks = "remarks",
                            Summary = 1,
                        }
                    }
                };
            }
        }

        [Fact]
        public void shouldSuccessPDFTemplateWithCurrencyIDR()
        {
            GarmentFinanceBankCashReceiptPdfTemplate PdfTemplate = new GarmentFinanceBankCashReceiptPdfTemplate();
            MemoryStream result = PdfTemplate.GeneratePdfTemplate(viewModelIDR, 7);
            Assert.NotNull(result);
        }

        [Fact]
        public void shouldSuccessPDFTemplateWithCurrencyUSD()
        {
            GarmentFinanceBankCashReceiptPdfTemplate PdfTemplate = new GarmentFinanceBankCashReceiptPdfTemplate();
            MemoryStream result = PdfTemplate.GeneratePdfTemplate(viewModelDollar, 7);
            Assert.NotNull(result);
        }

        [Fact]
        public void shouldSuccessPDFTemplateWithCurrencyIDRNonZero()
        {
            GarmentFinanceBankCashReceiptPdfTemplate PdfTemplate = new GarmentFinanceBankCashReceiptPdfTemplate();
            MemoryStream result = PdfTemplate.GeneratePdfTemplate(viewModelIDRNonZero, 7);
            Assert.NotNull(result);
        }

        [Fact]
        public void shouldSuccessPDFTemplateWithCurrencyUSDNonZero()
        {
            GarmentFinanceBankCashReceiptPdfTemplate PdfTemplate = new GarmentFinanceBankCashReceiptPdfTemplate();
            MemoryStream result = PdfTemplate.GeneratePdfTemplate(viewModelDollarNonZero, 7);
            Assert.NotNull(result);
        }
    }
}
