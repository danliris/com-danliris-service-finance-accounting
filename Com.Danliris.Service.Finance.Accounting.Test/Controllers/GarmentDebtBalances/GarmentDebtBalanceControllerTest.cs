using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentDebtBalance;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.ValidateService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Controllers.GarmentDebtBalances
{
   public class GarmentDebtBalanceControllerTest
    {
        protected GarmentDebtBalanceController GetController(Mock<IServiceProvider> serviceProvider)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            GarmentDebtBalanceController controller = new GarmentDebtBalanceController(serviceProvider.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");
            return controller;
        }

        Mock<IServiceProvider> GetServiceProvider()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
              .Setup(s => s.GetService(typeof(IIdentityService)))
              .Returns(new IdentityService() { TimezoneOffset = 1, Token = "token", Username = "username" });

            var validateService = new Mock<IValidateService>();
            serviceProvider
              .Setup(s => s.GetService(typeof(IValidateService)))
              .Returns(validateService.Object);
            return serviceProvider;
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        protected ServiceValidationException GetServiceValidationException(dynamic dto)
        {
            var serviceProvider = new Mock<IServiceProvider>();
            var validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(dto, serviceProvider.Object, null);
            return new ServiceValidationException(validationContext, validationResults);
        }

        [Fact]
        public void PostCustoms_Succes_Created()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IGarmentDebtBalanceService>();

            service
                .Setup(s => s.CreateFromCustoms(It.IsAny<CustomsFormDto>()))
                .Returns(1);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IGarmentDebtBalanceService)))
               .Returns(service.Object);

            //Act
            CustomsFormDto form = new CustomsFormDto();
            IActionResult response =  GetController(serviceProviderMock).PostCustoms(form);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.Created, statusCode);
        }

        [Fact]
        public void PostCustoms_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IGarmentDebtBalanceService>();

            service
                .Setup(s => s.CreateFromCustoms(It.IsAny<CustomsFormDto>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IGarmentDebtBalanceService)))
               .Returns(service.Object);

            //Act
            CustomsFormDto form = new CustomsFormDto();
            IActionResult response = GetController(serviceProviderMock).PostCustoms(form);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void PutInvoice_Succes_Update_Return_NoContent()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IGarmentDebtBalanceService>();

            service
                .Setup(s => s.UpdateFromInvoice(It.IsAny<int>(),It.IsAny<InvoiceFormDto>()))
                .Returns(1);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IGarmentDebtBalanceService)))
               .Returns(service.Object);

            //Act
            InvoiceFormDto form = new InvoiceFormDto();
            IActionResult response = GetController(serviceProviderMock).PutInvoice(1,form);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCode);
        }


        [Fact]
        public void PutInvoice_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IGarmentDebtBalanceService>();

            service
                .Setup(s => s.UpdateFromInvoice(It.IsAny<int>(), It.IsAny<InvoiceFormDto>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IGarmentDebtBalanceService)))
               .Returns(service.Object);

            //Act
            InvoiceFormDto form = new InvoiceFormDto();
            IActionResult response = GetController(serviceProviderMock).PutInvoice(1, form);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void PutInternalNote_Succes_Update_Return_NoContent()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IGarmentDebtBalanceService>();

            service
                .Setup(s => s.UpdateFromInternalNote(It.IsAny<int>(), It.IsAny<InternalNoteFormDto>()))
                .Returns(1);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IGarmentDebtBalanceService)))
               .Returns(service.Object);

            //Act
            InternalNoteFormDto form = new InternalNoteFormDto();
            IActionResult response = GetController(serviceProviderMock).PutInternalNote(1, form);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCode);
        }


        [Fact]
        public void PutInternalNote_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IGarmentDebtBalanceService>();

            service
                .Setup(s => s.UpdateFromInternalNote(It.IsAny<int>(), It.IsAny<InternalNoteFormDto>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IGarmentDebtBalanceService)))
               .Returns(service.Object);

            //Act
            InternalNoteFormDto form = new InternalNoteFormDto();
            IActionResult response = GetController(serviceProviderMock).PutInternalNote(1, form);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void PutBankExpenditureNote_Succes_Update_Return_NoContent()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IGarmentDebtBalanceService>();

            service
                .Setup(s => s.UpdateFromBankExpenditureNote(It.IsAny<int>(), It.IsAny<BankExpenditureNoteFormDto>()))
                .Returns(1);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IGarmentDebtBalanceService)))
               .Returns(service.Object);

            //Act
            BankExpenditureNoteFormDto form = new BankExpenditureNoteFormDto();
            IActionResult response = GetController(serviceProviderMock).PutBankExpenditureNote(1, form);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCode);
        }

        [Fact]
        public void PutBankExpenditureNote_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IGarmentDebtBalanceService>();

            service
                .Setup(s => s.UpdateFromBankExpenditureNote(It.IsAny<int>(), It.IsAny<BankExpenditureNoteFormDto>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IGarmentDebtBalanceService)))
               .Returns(service.Object);

            //Act
            BankExpenditureNoteFormDto form = new BankExpenditureNoteFormDto();
            IActionResult response = GetController(serviceProviderMock).PutBankExpenditureNote(1, form);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void GetSummary_Return_OK()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IGarmentDebtBalanceService>();
            GarmentDebtBalanceSummaryDto dto = new GarmentDebtBalanceSummaryDto(1,"supplierCode","supplierName",true,1,"IDR",1,1,1,1,1,1,1,1);
            service
                .Setup(s => s.GetDebtBalanceSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new List<GarmentDebtBalanceSummaryDto>() {
                dto
                } );

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IGarmentDebtBalanceService)))
               .Returns(service.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).GetSummary(1, 1,DateTime.Now.Year,true,true);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public void GetSummary_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IGarmentDebtBalanceService>();
            GarmentDebtBalanceSummaryDto dto = new GarmentDebtBalanceSummaryDto(1, "supplierCode", "supplierName", true, 1, "IDR", 1, 1, 1, 1, 1, 1, 1, 1);
            service
                .Setup(s => s.GetDebtBalanceSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IGarmentDebtBalanceService)))
               .Returns(service.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).GetSummary(1, 1, DateTime.Now.Year, true, true);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void GetPdf_Return_PdfFile()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IGarmentDebtBalanceService>();
            GarmentDebtBalanceSummaryDto dto = new GarmentDebtBalanceSummaryDto(1, "supplierCode", "supplierName", true, 1, "IDR", 1, 1, 1, 1, 1, 1, 1, 1);
            service
                .Setup(s => s.GetDebtBalanceSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new List<GarmentDebtBalanceSummaryDto>() {
                dto
                });

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IGarmentDebtBalanceService)))
               .Returns(service.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).GetPdf(1, 1, DateTime.Now.Year, true, true);

            //Assert
            Assert.Equal("application/pdf", response.GetType().GetProperty("ContentType").GetValue(response, null));
        }

        [Fact]
        public void GetPdf_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IGarmentDebtBalanceService>();
            GarmentDebtBalanceSummaryDto dto = new GarmentDebtBalanceSummaryDto(1, "supplierCode", "supplierName", true, 1, "IDR", 1, 1, 1, 1, 1, 1, 1, 1);
            service
                .Setup(s => s.GetDebtBalanceSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IGarmentDebtBalanceService)))
               .Returns(service.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).GetPdf(1, 1, DateTime.Now.Year, true, true);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void GetXls_Return_XlsxFile()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IGarmentDebtBalanceService>();
            GarmentDebtBalanceSummaryDto dto = new GarmentDebtBalanceSummaryDto(1, "supplierCode", "supplierName", true, 1, "IDR", 1, 1, 1, 1, 1, 1, 1, 1);
            service
                .Setup(s => s.GetDebtBalanceSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new List<GarmentDebtBalanceSummaryDto>() {
                dto
                });

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IGarmentDebtBalanceService)))
               .Returns(service.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).GetXls(1, 1, DateTime.Now.Year, true, true);

            //Assert
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));
        }

        [Fact]
        public void GetXls_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IGarmentDebtBalanceService>();
            GarmentDebtBalanceSummaryDto dto = new GarmentDebtBalanceSummaryDto(1, "supplierCode", "supplierName", true, 1, "IDR", 1, 1, 1, 1, 1, 1, 1, 1);
            service
                .Setup(s => s.GetDebtBalanceSummary(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IGarmentDebtBalanceService)))
               .Returns(service.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).GetXls(1, 1, DateTime.Now.Year, true, true);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }


        [Fact]
        public void GetSummaryAndTotalByCurrency_Return_OK()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IGarmentDebtBalanceService>();
            GarmentDebtBalanceSummaryDto summaryDto = new GarmentDebtBalanceSummaryDto(1, "supplierCode", "supplierName", true, 1, "IDR", 1, 1, 1, 1, 1, 1, 1, 1);
            var summaryAndTotalCurrencydto = new GarmentDebtBalanceSummaryAndTotalCurrencyDto()
            {
                Data = new List<GarmentDebtBalanceSummaryDto>()
                {
                   summaryDto
                },
                GroupTotalCurrency = new List<GarmentDebtBalanceSummaryTotalByCurrencyDto>()
                {
                    new GarmentDebtBalanceSummaryTotalByCurrencyDto()
                }
            };
            service
                .Setup(s => s.GetDebtBalanceSummaryAndTotalCurrency(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(summaryAndTotalCurrencydto);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IGarmentDebtBalanceService)))
               .Returns(service.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).GetSummaryAndTotalByCurrency(1, 1, DateTime.Now.Year, true, true);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public void GetSummaryAndTotalByCurrency_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IGarmentDebtBalanceService>();

            service
                .Setup(s => s.GetDebtBalanceSummaryAndTotalCurrency(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IGarmentDebtBalanceService)))
               .Returns(service.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).GetSummaryAndTotalByCurrency(1, 1, DateTime.Now.Year, true, true);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }
    }
}
