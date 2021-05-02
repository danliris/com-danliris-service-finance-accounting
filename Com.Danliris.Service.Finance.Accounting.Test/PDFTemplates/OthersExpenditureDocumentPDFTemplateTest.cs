using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MasterCOA;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.OthersExpenditureProofDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.PDFTemplates;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.OthersExpenditureProofDocument;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.PDFTemplates
{
    public class OthersExpenditureDocumentPDFTemplateTest
    {
        public OthersExpenditureProofDocumentPDFViewModel othersExpenditureProofDocumentPDFViewModelNonIDR
        {
            get
            {
                return new OthersExpenditureProofDocumentPDFViewModel()
                {
                    AccountBankId = 1,
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
                            Code = "SGD",
                            Description = "Description",
                            Id = 1,
                            Rate = 1,
                            Symbol = "$"
                        },
                        Id = 1
                    },
                    CekBgNo = "CekBgNo",
                    Date = DateTimeOffset.Now,
                    DocumentNo = "DocumentNo",
                    Id = 1,
                    Items = new List<OthersExpenditureProofDocumentItemPDFViewModel>()
                    {
                        new OthersExpenditureProofDocumentItemPDFViewModel(new OthersExpenditureProofDocumentItemModel(), new List<COAModel>())
                        {
                            COA = new Lib.Models.MasterCOA.COAModel()
                            {
                                Active = true,
                                Balance = 100,
                                CashAccount = "CashAccount",
                                Code = "Code",
                                Code1 = "Code1",
                                Code2 = "Code2",
                                Code3 = "Code3",
                                Code4 = "Code4",
                                CreatedAgent = "CreatedAgent",
                                CreatedBy = "CreatedBy",
                                CreatedUtc = DateTime.Now,
                                DeletedAgent = "DeletedAgent",
                                DeletedBy = "DeletedBy",
                                DeletedUtc = DateTime.Now,
                                Header = "Header",
                                Id = 1,
                                IsDeleted = false,
                                LastModifiedAgent = "LastModifiedAgent",
                                LastModifiedBy = "LastModifiedBy",
                                LastModifiedUtc = DateTime.Now,
                                Name = "Name",
                                Nature = "Nature",
                                Path = "Path",
                                ReportType = "ReportType",
                                Subheader = "Subheader"
                            },
                            COAId = 1,
                            Debit = 100,
                            Id = 1,
                            Remark = "Remark"
                        }
                    },
                    Remark = "Remark",
                    Type = "Type"
                };
            }
        }

        [Fact]
        public void shouldSuccessPDFTemplateWithCurrencyNonIDR()
        {
            OthersExpenditureDocumentPDFTemplate PdfTemplate = new OthersExpenditureDocumentPDFTemplate();
            MemoryStream result = PdfTemplate.GeneratePdfTemplate(othersExpenditureProofDocumentPDFViewModelNonIDR, 7);
            Assert.NotNull(result);
        }
    }
}
