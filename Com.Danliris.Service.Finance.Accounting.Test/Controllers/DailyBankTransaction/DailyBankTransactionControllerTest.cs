using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.ValidateService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.IntegrationViewModel;
using Com.Danliris.Service.Finance.Accounting.Test.Helpers;
using Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.DailyBankTransaction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Controllers.DailyBankTransaction
{
    public class DailyBankTransactionControllerTest
    {
        private DailyBankTransactionViewModel ViewModel
        {
            get
            {
                return new DailyBankTransactionViewModel()
                {
                    Bank = new AccountBankViewModel() { currency = new CurrencyViewModel() },
                    Supplier = new SupplierViewModel(),
                    Buyer = new BuyerViewModel()
                };
            }
        }

        private DailyBankTransactionModel Model
        {
            get
            {
                return new DailyBankTransactionModel();
            }
        }

        private ServiceValidationException GetServiceValidationExeption()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(ViewModel, serviceProvider.Object, null);
            return new ServiceValidationException(validationContext, validationResults);
        }

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            return serviceProvider;
        }

        private DailyBankTransactionControllers GetController(Mock<IDailyBankTransactionService> serviceM, Mock<IValidateService> validateM, Mock<IMapper> mapper)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var servicePMock = GetServiceProvider();
            servicePMock
                .Setup(x => x.GetService(typeof(IValidateService)))
                .Returns(validateM.Object);

            DailyBankTransactionControllers controller = new DailyBankTransactionControllers(servicePMock.Object, serviceM.Object, mapper.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                    {
                        User = user.Object
                    }
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");

            return controller;
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        [Fact]
        public void Should_Success_Get_All_Data()
        {
            var mockService = new Mock<IDailyBankTransactionService>();
            mockService.Setup(x => x.Read(1, 25, "{}", null, "{}"))
                .Returns(new ReadResponse<DailyBankTransactionModel>(new List<DailyBankTransactionModel>(), 1, new Dictionary<string, string>(), new List<string>()));
            var mockMapper = new Mock<IMapper>();

            DailyBankTransactionControllers controller = new DailyBankTransactionControllers(GetServiceProvider().Object, mockService.Object, mockMapper.Object);
            var response = controller.Get(1, 25, "{}", null, "{}");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Not_Found_Get_Data_By_Id()
        {
            var mockService = new Mock<IDailyBankTransactionService>();
            mockService.Setup(x => x.ReadById(It.IsAny<int>()))
                .ReturnsAsync((DailyBankTransactionModel)null);

            var mockMapper = new Mock<IMapper>();

            DailyBankTransactionControllers controller = new DailyBankTransactionControllers(GetServiceProvider().Object, mockService.Object, mockMapper.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "test";

            var response = controller.GetById(It.IsAny<int>()).Result;
            Assert.Equal((int)HttpStatusCode.NotFound, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Data_By_Id()
        {
            var mockService = new Mock<IDailyBankTransactionService>();
            mockService.Setup(x => x.ReadById(It.IsAny<int>()))
               .Throws(new Exception());

            var mockMapper = new Mock<IMapper>();

            DailyBankTransactionControllers controller = new DailyBankTransactionControllers(GetServiceProvider().Object, mockService.Object, mockMapper.Object);
            var response = controller.GetById(It.IsAny<int>()).Result;
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Return_Bad_Request_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<DailyBankTransactionViewModel>())).Throws(GetServiceValidationExeption());

            var mockService = new Mock<IDailyBankTransactionService>();
            mockService.Setup(x => x.Create(It.IsAny<DailyBankTransactionModel>(), "unittestusername"))
               .ReturnsAsync(1);

            var mockMapper = new Mock<IMapper>();

            var controller = GetController(mockService, validateMock, mockMapper);

            var response = controller.Post(ViewModel).Result;
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Data_By_Id()
        {
            var mockService = new Mock<IDailyBankTransactionService>();
            mockService.Setup(x => x.ReadById(It.IsAny<int>()))
                .ReturnsAsync(Model);

            var mockMapper = new Mock<IMapper>();

            DailyBankTransactionControllers controller = new DailyBankTransactionControllers(GetServiceProvider().Object, mockService.Object, mockMapper.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "test";

            var response = controller.GetById(It.IsAny<int>()).Result;
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<DailyBankTransactionViewModel>())).Verifiable();

            var mockService = new Mock<IDailyBankTransactionService>();
            mockService.Setup(x => x.Create(It.IsAny<DailyBankTransactionModel>(), "unittestusername"))
               .ThrowsAsync(new Exception());

            var mockMapper = new Mock<IMapper>();

            var controller = GetController(mockService, validateMock, mockMapper);

            var response = controller.Post(this.ViewModel).Result;
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Report()
        {
            var mockService = new Mock<IDailyBankTransactionService>();
            mockService.Setup(x => x.GetReport(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>()))
                .Returns(new ReadResponse<DailyBankTransactionModel>(new List<DailyBankTransactionModel>(), 1, new Dictionary<string, string>(), new List<string>()));
            var mockMapper = new Mock<IMapper>();

            DailyBankTransactionControllers controller = new DailyBankTransactionControllers(GetServiceProvider().Object, mockService.Object, mockMapper.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "1";

            var response = controller.GetReport("", DateTimeOffset.Now, DateTimeOffset.Now);
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        //[Fact]
        //public void Should_Success_Get_Report_Excel()
        //{
        //    Mock<IDailyBankTransactionFacade> mockFacade = new Mock<IDailyBankTransactionFacade>();
        //    mockFacade.Setup(p => p.GenerateExcel(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>()))
        //        .Returns(new System.IO.MemoryStream());

        //    var mockMapper = new Mock<IMapper>();

        //    DailyBankTransactionControllers controller = new DailyBankTransactionControllers(GetServiceProvider().Object, mockFacade.Object, mockMapper.Object);
        //    controller.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = new DefaultHttpContext()
        //    };

        //    controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "1";

        //    var response = controller.GetReportXls("", null, null);
        //    Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        //}
    }
}
