using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.SalesReceipt;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.SalesReceipt;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.NewIntegrationViewModel;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.SalesReceipt;
using Com.Danliris.Service.Finance.Accounting.Test.Controller.Utils;
using Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.SalesReceipt;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Controllers.SalesReceipt
{
    public class SalesReceiptControllerTest : BaseControllerTest<SalesReceiptController, SalesReceiptModel, SalesReceiptViewModel, ISalesReceiptService>
    {
        [Fact]
        public void Get_Sales_Receipt_PDF_Success()
        {
            var vm = new SalesReceiptViewModel()
            {
                SalesReceiptDate = DateTimeOffset.Now,
                Unit = new NewUnitViewModel()
                {
                    Name = "Name",
                },
                Buyer = new NewBuyerViewModel()
                {
                    Name = "Name",
                    Address = "Address",
                },
                Currency = new CurrencyViewModel()
                {
                    Code = "IDR",
                    Symbol = "Rp",
                    Rate = 14000,
                },
                Bank = new AccountBankViewModel()
                {
                    BankName = "BCA",
                },
                SalesReceiptDetails = new List<SalesReceiptDetailViewModel>()
                {
                    new SalesReceiptDetailViewModel()
                    {
                        VatType = "PPN BUMN",
                        SalesInvoice = new SalesInvoiceViewModel()
                        {
                            SalesInvoiceNo = "no",
                            Currency = new CurrencyViewModel()
                            {
                                Code = "IDR",
                                Symbol = "Rp",
                                Rate = 14000,
                            },
                        },
                    }
                }

            };
            var mocks = GetMocks();
            mocks.Service.Setup(x => x.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(Model);
            mocks.Mapper.Setup(s => s.Map<SalesReceiptViewModel>(It.IsAny<SalesReceiptModel>()))
                .Returns(vm);
            var controller = GetController(mocks);
            var response = controller.GetSalesReceiptPDF(1).Result;

            Assert.NotNull(response);

        }

        [Fact]
        public void Get_Sales_Receipt_PDF_NotFound()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(x => x.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(default(SalesReceiptModel));
            var controller = GetController(mocks);
            var response = controller.GetSalesReceiptPDF(1).Result;

            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.NotFound, statusCode);

        }

        [Fact]
        public void Get_Sales_Receipt_PDF_Exception()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(x => x.ReadByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception("error"));
            var controller = GetController(mocks);
            var response = controller.GetSalesReceiptPDF(1).Result;

            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);

        }
    }
}
