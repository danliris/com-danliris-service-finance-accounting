using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.ValidateService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.RealizationVBNonPO;
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

namespace Com.Danliris.Service.Finance.Accounting.Test.Controllers.RealizationVBNonPO
{
    public class RealizationVbNonPOControllerTest
    {
        protected RealizationVbNonPOController GetController(Mock<IServiceProvider> serviceProvider)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                    new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            serviceProvider
              .Setup(s => s.GetService(typeof(IIdentityService)))
              .Returns(new IdentityService() { TimezoneOffset = 1, Token = "token", Username = "username" });

            var validateService = new Mock<IValidateService>();
            serviceProvider
              .Setup(s => s.GetService(typeof(IValidateService)))
              .Returns(validateService.Object);

            Mock<IMapper> mapper = new Mock<IMapper>();
            serviceProvider
             .Setup(s => s.GetService(typeof(IMapper)))
             .Returns(mapper.Object);

            RealizationVbNonPOController controller = new RealizationVbNonPOController(serviceProvider.Object);
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

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        protected ServiceValidationException GetServiceValidationException()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            var validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(realizationVbNonPOViewModel, serviceProvider.Object, null);
            return new ServiceValidationException(validationContext, validationResults);
        }

        public RealizationVbNonPOViewModel realizationVbNonPOViewModel
        {
            get
            {
                return new RealizationVbNonPOViewModel()
                {
                    Id = 1,
                    VBRealizationNo = "VBRealizationNo",
                    Date = DateTimeOffset.Now,
                    numberVB = new DetailRequestNonPO()
                    {
                        Amount = 123,
                        CreateBy = "CreateBy",
                        CurrencyCode = "IDR",
                        CurrencyRate = 123,
                        Date = DateTimeOffset.Now,
                        DateEstimate = DateTimeOffset.Now,
                        UnitCode = "UnitCode",
                        UnitId = 1,
                        UnitLoad = "UnitLoad",
                        UnitName = "UnitName",
                        VBNo = "VBNo",
                        VBRequestCategory = "NONPO"

                    },
                    Items = new List<VbNonPORequestDetailViewModel>()
                    {
                        new VbNonPORequestDetailViewModel()
                        {
                            DateDetail = DateTimeOffset.Now,
                            Remark = "Remark",
                            Amount = 123,
                            isGetPPn = true
                        }
                    }
                };
            }
        }

        [Fact]
        public async Task RealizationVbNonPORequestPDF_Return_NotFound()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var RealizationVbNonPOMock = new Mock<IRealizationVbNonPOService>();
            RealizationVbNonPOMock.Setup(s => s.ReadByIdAsync2(It.IsAny<int>())).ReturnsAsync(() => null);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService)))
               .Returns(RealizationVbNonPOMock.Object);

            IActionResult response = await GetController(serviceProviderMock).RealizationVbNonPORequestPDF(1);
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.NotFound, statusCode);
        }

        [Fact]
        public async Task RealizationVbNonPORequestPDF_Return_InternalServerError()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var RealizationVbNonPOMock = new Mock<IRealizationVbNonPOService>();
            RealizationVbNonPOMock.Setup(s => s.ReadByIdAsync2(It.IsAny<int>())).Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService)))
               .Returns(RealizationVbNonPOMock.Object);

            IActionResult response = await GetController(serviceProviderMock).RealizationVbNonPORequestPDF(1);
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void Get_Return_OK()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var RealizationVbNonPOMock = new Mock<IRealizationVbNonPOService>();
            Dictionary<string, string> order = new Dictionary<string, string>();
            order.Add("RequestVbName", "desc");

            var queryResult = new ReadResponse<RealizationVbList>(new List<RealizationVbList>(), 1, order, new List<string>() { "RequestVbName", "RequestVbName" });

            RealizationVbNonPOMock.Setup(s => s.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>())).Returns(queryResult);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService)))
               .Returns(RealizationVbNonPOMock.Object);

            IActionResult response = GetController(serviceProviderMock).Get(1, 25, "{}", new List<string>() { "RequestVbName" }, "", "{}");
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }


        [Fact]
        public void Get_Return_InternalServerError()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var RealizationVbNonPOMock = new Mock<IRealizationVbNonPOService>();

            RealizationVbNonPOMock.Setup(s => s.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService)))
               .Returns(RealizationVbNonPOMock.Object);

            IActionResult response = GetController(serviceProviderMock).Get(1, 25, "{}", new List<string>() { "RequestVbName" }, "", "{}");
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }


        [Fact]
        public async Task Post_Return_OK()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var RealizationVbNonPOMock = new Mock<IRealizationVbNonPOService>();

            RealizationVbNonPOMock.Setup(s => s.CreateAsync(It.IsAny<RealizationVbModel>(), It.IsAny<RealizationVbNonPOViewModel>())).ReturnsAsync(1);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService)))
               .Returns(RealizationVbNonPOMock.Object);

            IActionResult response = await GetController(serviceProviderMock).Post(realizationVbNonPOViewModel);
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.Created, statusCode);
        }

        [Fact]
        public async Task Post_Return_BadRequest()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var RealizationVbNonPOMock = new Mock<IRealizationVbNonPOService>();

            RealizationVbNonPOMock.Setup(s => s.CreateAsync(It.IsAny<RealizationVbModel>(), It.IsAny<RealizationVbNonPOViewModel>())).ThrowsAsync(GetServiceValidationException());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService)))
               .Returns(RealizationVbNonPOMock.Object);

            IActionResult response = await GetController(serviceProviderMock).Post(realizationVbNonPOViewModel);
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public async Task Post_Return_InternalServerError()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var RealizationVbNonPOMock = new Mock<IRealizationVbNonPOService>();

            RealizationVbNonPOMock.Setup(s => s.CreateAsync(It.IsAny<RealizationVbModel>(), It.IsAny<RealizationVbNonPOViewModel>())).ThrowsAsync(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService)))
               .Returns(RealizationVbNonPOMock.Object);

            IActionResult response = await GetController(serviceProviderMock).Post(realizationVbNonPOViewModel);
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public async Task GetById_Return_InternalServerError()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var RealizationVbNonPOMock = new Mock<IRealizationVbNonPOService>();

            RealizationVbNonPOMock.Setup(s => s.ReadByIdAsync2(It.IsAny<int>())).ThrowsAsync(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService)))
               .Returns(RealizationVbNonPOMock.Object);

            IActionResult response = await GetController(serviceProviderMock).GetById(1);
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public async Task GetById_Return_NotFound()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var RealizationVbNonPOMock = new Mock<IRealizationVbNonPOService>();

            RealizationVbNonPOMock.Setup(s => s.ReadByIdAsync2(It.IsAny<int>())).ReturnsAsync(() => null);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService)))
               .Returns(RealizationVbNonPOMock.Object);

            IActionResult response = await GetController(serviceProviderMock).GetById(1);
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.NotFound, statusCode);
        }


        [Fact]
        public async Task GetById_Return_OK()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var RealizationVbNonPOMock = new Mock<IRealizationVbNonPOService>();

            RealizationVbNonPOMock.Setup(s => s.ReadByIdAsync2(It.IsAny<int>())).ReturnsAsync(realizationVbNonPOViewModel);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService)))
               .Returns(RealizationVbNonPOMock.Object);

            IActionResult response = await GetController(serviceProviderMock).GetById(1);
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public async Task Put_Return_OK()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var RealizationVbNonPOMock = new Mock<IRealizationVbNonPOService>();

            RealizationVbNonPOMock.Setup(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<RealizationVbNonPOViewModel>())).ReturnsAsync(1);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService)))
               .Returns(RealizationVbNonPOMock.Object);

            IActionResult response = await GetController(serviceProviderMock).Put(1, realizationVbNonPOViewModel);
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCode);
        }

        [Fact]
        public async Task Put_Return_BadRequest()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var RealizationVbNonPOMock = new Mock<IRealizationVbNonPOService>();

            RealizationVbNonPOMock.Setup(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<RealizationVbNonPOViewModel>())).ReturnsAsync(1);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService)))
               .Returns(RealizationVbNonPOMock.Object);


            IActionResult response = await GetController(serviceProviderMock).Put(0, realizationVbNonPOViewModel);
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public async Task Put_Throws_ServiceValidationException()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var RealizationVbNonPOMock = new Mock<IRealizationVbNonPOService>();

            RealizationVbNonPOMock.Setup(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<RealizationVbNonPOViewModel>())).ThrowsAsync(GetServiceValidationException());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService)))
               .Returns(RealizationVbNonPOMock.Object);


            IActionResult response = await GetController(serviceProviderMock).Put(1, realizationVbNonPOViewModel);
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public async Task Put_Return_InternalServerError()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var RealizationVbNonPOMock = new Mock<IRealizationVbNonPOService>();

            RealizationVbNonPOMock.Setup(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<RealizationVbNonPOViewModel>())).ThrowsAsync(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService)))
               .Returns(RealizationVbNonPOMock.Object);


            IActionResult response = await GetController(serviceProviderMock).Put(1, realizationVbNonPOViewModel);
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public async Task Delete_Return_InternalServerError()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var RealizationVbNonPOMock = new Mock<IRealizationVbNonPOService>();

            RealizationVbNonPOMock.Setup(s => s.DeleteAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService)))
               .Returns(RealizationVbNonPOMock.Object);


            IActionResult response = await GetController(serviceProviderMock).Delete(1);
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public async Task Delete_Succees_Return_NoContent()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var RealizationVbNonPOMock = new Mock<IRealizationVbNonPOService>();

            RealizationVbNonPOMock.Setup(s => s.DeleteAsync(It.IsAny<int>())).ReturnsAsync(1);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService)))
               .Returns(RealizationVbNonPOMock.Object);


            IActionResult response = await GetController(serviceProviderMock).Delete(1);
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCode);
        }

        [Fact]
        public async Task Get_Sales_Receipt_PDF_Success_Currency_IDRAsync()
        {
            var vm = new RealizationVbNonPOViewModel()
            {
                Id = 1,
                VBRealizationNo = "VBRealizationNo",
                Date = DateTimeOffset.Now,
                numberVB = new DetailRequestNonPO()
                {
                    Amount = 123,
                    CreateBy = "CreateBy",
                    CurrencyCode = "IDR",
                    CurrencyRate = 123,
                    Date = DateTimeOffset.Now,
                    DateEstimate = DateTimeOffset.Now,
                    UnitCode = "UnitCode",
                    UnitId = 1,
                    UnitLoad = "UnitLoad",
                    UnitName = "UnitName",
                    VBNo = "VBNo",
                    VBRequestCategory = "NONPO"

                },
                Items = new List<VbNonPORequestDetailViewModel>()
                {
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now,
                        Remark = "Remark",
                        Amount = 123,
                        isGetPPn = true
                    }
                }

            };

            var serviceProviderMock = new Mock<IServiceProvider>();

            var serviceMock = new Mock<IRealizationVbNonPOService>();
            serviceMock
                .Setup(service => service.ReadByIdAsync2(It.IsAny<int>()))
                .ReturnsAsync(vm);
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);
            //var mapperMock = new Mock<IMapper>();
            //mapperMock
            //    .Setup(mapper => mapper.Map<VbNonPORequestViewModel>(It.IsAny<VbNonPORequestViewModel>()))
            //    .Returns(vm);
            //serviceProviderMock
            //    .Setup(serviceProvider => serviceProvider.GetService(typeof(IMapper))).Returns(mapperMock.Object);

            var controller = GetController(serviceProviderMock);

            var response = await controller.RealizationVbNonPORequestPDF(It.IsAny<int>());
            //var statusCode = GetStatusCode(response);

            Assert.NotNull(response);
        }

        [Fact]
        public async Task Get_Sales_Receipt_PDF_Success_Currency_USDAsync()
        {
            var vm = new RealizationVbNonPOViewModel()
            {
                VBRealizationNo = "VBRealizationNo",
                Date = DateTimeOffset.Now,
                numberVB = new DetailRequestNonPO()
                {
                    Amount = 123,
                    CreateBy = "CreateBy",
                    CurrencyCode = "USD",
                    CurrencyRate = 123,
                    Date = DateTimeOffset.Now,
                    DateEstimate = DateTimeOffset.Now,
                    UnitCode = "UnitCode",
                    UnitId = 1,
                    UnitLoad = "UnitLoad",
                    UnitName = "UnitName",
                    VBNo = "VBNo",
                    VBRequestCategory = "NONPO"

                },
                Items = new List<VbNonPORequestDetailViewModel>()
                {
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now,
                        Remark = "Remark",
                        Amount = 123,
                        isGetPPn = true
                    }
                }

            };

            var serviceProviderMock = new Mock<IServiceProvider>();

            var serviceMock = new Mock<IRealizationVbNonPOService>();
            serviceMock
                .Setup(service => service.ReadByIdAsync2(It.IsAny<int>()))
                .ReturnsAsync(vm);
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);

            var controller = GetController(serviceProviderMock);

            var response = await controller.RealizationVbNonPORequestPDF(It.IsAny<int>());

            Assert.NotNull(response);
        }

        [Fact]
        public async Task Get_Sales_Receipt_PDF_Success_SPINNING1()
        {
            var vm = new RealizationVbNonPOViewModel()
            {
                VBRealizationNo = "VBRealizationNo",
                Date = DateTimeOffset.Now,
                numberVB = new DetailRequestNonPO()
                {
                    Amount = 123,
                    CreateBy = "CreateBy",
                    CurrencyCode = "USD",
                    CurrencyRate = 123,
                    Date = DateTimeOffset.Now,
                    DateEstimate = DateTimeOffset.Now,
                    UnitCode = "UnitCode",
                    UnitId = 1,
                    UnitLoad = "SPINNING 1",
                    UnitName = "UnitName",
                    VBNo = "VBNo",
                    VBRequestCategory = "NONPO"

                },
                Items = new List<VbNonPORequestDetailViewModel>()
                {
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now,
                        Remark = "Remark",
                        Amount = 123,
                        isGetPPn = true
                    }
                }

            };

            var serviceProviderMock = new Mock<IServiceProvider>();

            var serviceMock = new Mock<IRealizationVbNonPOService>();
            serviceMock
                .Setup(service => service.ReadByIdAsync2(It.IsAny<int>()))
                .ReturnsAsync(vm);
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);

            var controller = GetController(serviceProviderMock);

            var response = await controller.RealizationVbNonPORequestPDF(It.IsAny<int>());

            Assert.NotNull(response);
        }

        [Fact]
        public async Task Get_Sales_Receipt_PDF_Success_SPINNING2()
        {
            var vm = new RealizationVbNonPOViewModel()
            {
                VBRealizationNo = "VBRealizationNo",
                Date = DateTimeOffset.Now,
                numberVB = new DetailRequestNonPO()
                {
                    Amount = 123,
                    CreateBy = "CreateBy",
                    CurrencyCode = "USD",
                    CurrencyRate = 123,
                    Date = DateTimeOffset.Now,
                    DateEstimate = DateTimeOffset.Now,
                    UnitCode = "UnitCode",
                    UnitId = 1,
                    UnitLoad = "SPINNING 2",
                    UnitName = "UnitName",
                    VBNo = "VBNo",
                    VBRequestCategory = "NONPO"

                },
                Items = new List<VbNonPORequestDetailViewModel>()
                {
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now,
                        Remark = "Remark",
                        Amount = 123,
                        isGetPPn = true
                    }
                }

            };

            var serviceProviderMock = new Mock<IServiceProvider>();

            var serviceMock = new Mock<IRealizationVbNonPOService>();
            serviceMock
                .Setup(service => service.ReadByIdAsync2(It.IsAny<int>()))
                .ReturnsAsync(vm);
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);

            var controller = GetController(serviceProviderMock);

            var response = await controller.RealizationVbNonPORequestPDF(It.IsAny<int>());

            Assert.NotNull(response);
        }

        [Fact]
        public async Task Get_Sales_Receipt_PDF_Success_SPINNING3()
        {
            var vm = new RealizationVbNonPOViewModel()
            {
                VBRealizationNo = "VBRealizationNo",
                Date = DateTimeOffset.Now,
                numberVB = new DetailRequestNonPO()
                {
                    Amount = 123,
                    CreateBy = "CreateBy",
                    CurrencyCode = "USD",
                    CurrencyRate = 123,
                    Date = DateTimeOffset.Now,
                    DateEstimate = DateTimeOffset.Now,
                    UnitCode = "UnitCode",
                    UnitId = 1,
                    UnitLoad = "SPINNING 3",
                    UnitName = "UnitName",
                    VBNo = "VBNo",
                    VBRequestCategory = "NONPO"

                },
                Items = new List<VbNonPORequestDetailViewModel>()
                {
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now,
                        Remark = "Remark",
                        Amount = 123,
                        isGetPPn = true
                    }
                }

            };

            var serviceProviderMock = new Mock<IServiceProvider>();

            var serviceMock = new Mock<IRealizationVbNonPOService>();
            serviceMock
                .Setup(service => service.ReadByIdAsync2(It.IsAny<int>()))
                .ReturnsAsync(vm);
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);

            var controller = GetController(serviceProviderMock);

            var response = await controller.RealizationVbNonPORequestPDF(It.IsAny<int>());

            Assert.NotNull(response);
        }

        [Fact]
        public async Task Get_Sales_Receipt_PDF_Success_WEAVING1()
        {
            var vm = new RealizationVbNonPOViewModel()
            {
                VBRealizationNo = "VBRealizationNo",
                Date = DateTimeOffset.Now,
                numberVB = new DetailRequestNonPO()
                {
                    Amount = 123,
                    CreateBy = "CreateBy",
                    CurrencyCode = "USD",
                    CurrencyRate = 123,
                    Date = DateTimeOffset.Now,
                    DateEstimate = DateTimeOffset.Now,
                    UnitCode = "UnitCode",
                    UnitId = 1,
                    UnitLoad = "WEAVING 1",
                    UnitName = "UnitName",
                    VBNo = "VBNo",
                    VBRequestCategory = "NONPO"

                },
                Items = new List<VbNonPORequestDetailViewModel>()
                {
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now,
                        Remark = "Remark",
                        Amount = 123,
                        isGetPPn = true
                    }
                }

            };

            var serviceProviderMock = new Mock<IServiceProvider>();

            var serviceMock = new Mock<IRealizationVbNonPOService>();
            serviceMock
                .Setup(service => service.ReadByIdAsync2(It.IsAny<int>()))
                .ReturnsAsync(vm);
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);

            var controller = GetController(serviceProviderMock);

            var response = await controller.RealizationVbNonPORequestPDF(It.IsAny<int>());

            Assert.NotNull(response);
        }

        [Fact]
        public async Task Get_Sales_Receipt_PDF_Success_WEAVING2()
        {
            var vm = new RealizationVbNonPOViewModel()
            {
                VBRealizationNo = "VBRealizationNo",
                Date = DateTimeOffset.Now,
                numberVB = new DetailRequestNonPO()
                {
                    Amount = 123,
                    CreateBy = "CreateBy",
                    CurrencyCode = "USD",
                    CurrencyRate = 123,
                    Date = DateTimeOffset.Now,
                    DateEstimate = DateTimeOffset.Now,
                    UnitCode = "UnitCode",
                    UnitId = 1,
                    UnitLoad = "WEAVING 2",
                    UnitName = "UnitName",
                    VBNo = "VBNo",
                    VBRequestCategory = "NONPO"

                },
                Items = new List<VbNonPORequestDetailViewModel>()
                {
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now,
                        Remark = "Remark",
                        Amount = 123,
                        isGetPPn = true
                    }
                }

            };

            var serviceProviderMock = new Mock<IServiceProvider>();

            var serviceMock = new Mock<IRealizationVbNonPOService>();
            serviceMock
                .Setup(service => service.ReadByIdAsync2(It.IsAny<int>()))
                .ReturnsAsync(vm);
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);

            var controller = GetController(serviceProviderMock);

            var response = await controller.RealizationVbNonPORequestPDF(It.IsAny<int>());

            Assert.NotNull(response);
        }

        [Fact]
        public async Task Get_Sales_Receipt_PDF_Success_PRINTING()
        {
            var vm = new RealizationVbNonPOViewModel()
            {
                VBRealizationNo = "VBRealizationNo",
                Date = DateTimeOffset.Now,
                numberVB = new DetailRequestNonPO()
                {
                    Amount = 123,
                    CreateBy = "CreateBy",
                    CurrencyCode = "USD",
                    CurrencyRate = 123,
                    Date = DateTimeOffset.Now,
                    DateEstimate = DateTimeOffset.Now,
                    UnitCode = "UnitCode",
                    UnitId = 1,
                    UnitLoad = "PRINTING",
                    UnitName = "UnitName",
                    VBNo = "VBNo",
                    VBRequestCategory = "NONPO"

                },
                Items = new List<VbNonPORequestDetailViewModel>()
                {
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now,
                        Remark = "Remark",
                        Amount = 123,
                        isGetPPn = true
                    }
                }

            };

            var serviceProviderMock = new Mock<IServiceProvider>();

            var serviceMock = new Mock<IRealizationVbNonPOService>();
            serviceMock
                .Setup(service => service.ReadByIdAsync2(It.IsAny<int>()))
                .ReturnsAsync(vm);
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);

            var controller = GetController(serviceProviderMock);

            var response = await controller.RealizationVbNonPORequestPDF(It.IsAny<int>());

            Assert.NotNull(response);
        }

        [Fact]
        public async Task Get_Sales_Receipt_PDF_Success_DYEING()
        {
            var vm = new RealizationVbNonPOViewModel()
            {
                VBRealizationNo = "VBRealizationNo",
                Date = DateTimeOffset.Now,
                numberVB = new DetailRequestNonPO()
                {
                    Amount = 123,
                    CreateBy = "CreateBy",
                    CurrencyCode = "USD",
                    CurrencyRate = 123,
                    Date = DateTimeOffset.Now,
                    DateEstimate = DateTimeOffset.Now,
                    UnitCode = "UnitCode",
                    UnitId = 1,
                    UnitLoad = "DYEING",
                    UnitName = "UnitName",
                    VBNo = "VBNo",
                    VBRequestCategory = "NONPO"

                },
                Items = new List<VbNonPORequestDetailViewModel>()
                {
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now,
                        Remark = "Remark",
                        Amount = 123,
                        isGetPPn = true
                    }
                }

            };

            var serviceProviderMock = new Mock<IServiceProvider>();

            var serviceMock = new Mock<IRealizationVbNonPOService>();
            serviceMock
                .Setup(service => service.ReadByIdAsync2(It.IsAny<int>()))
                .ReturnsAsync(vm);
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);

            var controller = GetController(serviceProviderMock);

            var response = await controller.RealizationVbNonPORequestPDF(It.IsAny<int>());

            Assert.NotNull(response);
        }

        [Fact]
        public async Task Get_Sales_Receipt_PDF_Success_KONFEKSI1A()
        {
            var vm = new RealizationVbNonPOViewModel()
            {
                VBRealizationNo = "VBRealizationNo",
                Date = DateTimeOffset.Now,
                numberVB = new DetailRequestNonPO()
                {
                    Amount = 123,
                    CreateBy = "CreateBy",
                    CurrencyCode = "USD",
                    CurrencyRate = 123,
                    Date = DateTimeOffset.Now,
                    DateEstimate = DateTimeOffset.Now,
                    UnitCode = "UnitCode",
                    UnitId = 1,
                    UnitLoad = "KONFEKSI 1A",
                    UnitName = "UnitName",
                    VBNo = "VBNo",
                    VBRequestCategory = "NONPO"

                },
                Items = new List<VbNonPORequestDetailViewModel>()
                {
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now,
                        Remark = "Remark",
                        Amount = 123,
                        isGetPPn = true
                    }
                }

            };

            var serviceProviderMock = new Mock<IServiceProvider>();

            var serviceMock = new Mock<IRealizationVbNonPOService>();
            serviceMock
                .Setup(service => service.ReadByIdAsync2(It.IsAny<int>()))
                .ReturnsAsync(vm);
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);

            var controller = GetController(serviceProviderMock);

            var response = await controller.RealizationVbNonPORequestPDF(It.IsAny<int>());

            Assert.NotNull(response);
        }

        [Fact]
        public async Task Get_Sales_Receipt_PDF_Success_KONFEKSI1B()
        {
            var vm = new RealizationVbNonPOViewModel()
            {
                VBRealizationNo = "VBRealizationNo",
                Date = DateTimeOffset.Now,
                numberVB = new DetailRequestNonPO()
                {
                    Amount = 123,
                    CreateBy = "CreateBy",
                    CurrencyCode = "USD",
                    CurrencyRate = 123,
                    Date = DateTimeOffset.Now,
                    DateEstimate = DateTimeOffset.Now,
                    UnitCode = "UnitCode",
                    UnitId = 1,
                    UnitLoad = "KONFEKSI 1B",
                    UnitName = "UnitName",
                    VBNo = "VBNo",
                    VBRequestCategory = "NONPO"

                },
                Items = new List<VbNonPORequestDetailViewModel>()
                {
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now,
                        Remark = "Remark",
                        Amount = 123,
                        isGetPPn = true
                    }
                }

            };

            var serviceProviderMock = new Mock<IServiceProvider>();

            var serviceMock = new Mock<IRealizationVbNonPOService>();
            serviceMock
                .Setup(service => service.ReadByIdAsync2(It.IsAny<int>()))
                .ReturnsAsync(vm);
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);

            var controller = GetController(serviceProviderMock);

            var response = await controller.RealizationVbNonPORequestPDF(It.IsAny<int>());

            Assert.NotNull(response);
        }

        [Fact]
        public async Task Get_Sales_Receipt_PDF_Success_KONFEKSI2A()
        {
            var vm = new RealizationVbNonPOViewModel()
            {
                VBRealizationNo = "VBRealizationNo",
                Date = DateTimeOffset.Now,
                numberVB = new DetailRequestNonPO()
                {
                    Amount = 123,
                    CreateBy = "CreateBy",
                    CurrencyCode = "USD",
                    CurrencyRate = 123,
                    Date = DateTimeOffset.Now,
                    DateEstimate = DateTimeOffset.Now,
                    UnitCode = "UnitCode",
                    UnitId = 1,
                    UnitLoad = "KONFEKSI 2A",
                    UnitName = "UnitName",
                    VBNo = "VBNo",
                    VBRequestCategory = "NONPO"

                },
                Items = new List<VbNonPORequestDetailViewModel>()
                {
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now,
                        Remark = "Remark",
                        Amount = 123,
                        isGetPPn = true
                    }
                }

            };

            var serviceProviderMock = new Mock<IServiceProvider>();

            var serviceMock = new Mock<IRealizationVbNonPOService>();
            serviceMock
                .Setup(service => service.ReadByIdAsync2(It.IsAny<int>()))
                .ReturnsAsync(vm);
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);

            var controller = GetController(serviceProviderMock);

            var response = await controller.RealizationVbNonPORequestPDF(It.IsAny<int>());

            Assert.NotNull(response);
        }

        [Fact]
        public async Task Get_Sales_Receipt_PDF_Success_KONFEKSI2B()
        {
            var vm = new RealizationVbNonPOViewModel()
            {
                VBRealizationNo = "VBRealizationNo",
                Date = DateTimeOffset.Now,
                numberVB = new DetailRequestNonPO()
                {
                    Amount = 123,
                    CreateBy = "CreateBy",
                    CurrencyCode = "USD",
                    CurrencyRate = 123,
                    Date = DateTimeOffset.Now,
                    DateEstimate = DateTimeOffset.Now,
                    UnitCode = "UnitCode",
                    UnitId = 1,
                    UnitLoad = "KONFEKSI 2B",
                    UnitName = "UnitName",
                    VBNo = "VBNo",
                    VBRequestCategory = "NONPO"

                },
                Items = new List<VbNonPORequestDetailViewModel>()
                {
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now,
                        Remark = "Remark",
                        Amount = 123,
                        isGetPPn = true
                    }
                }

            };

            var serviceProviderMock = new Mock<IServiceProvider>();

            var serviceMock = new Mock<IRealizationVbNonPOService>();
            serviceMock
                .Setup(service => service.ReadByIdAsync2(It.IsAny<int>()))
                .ReturnsAsync(vm);
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);

            var controller = GetController(serviceProviderMock);

            var response = await controller.RealizationVbNonPORequestPDF(It.IsAny<int>());

            Assert.NotNull(response);
        }

        [Fact]
        public async Task Get_Sales_Receipt_PDF_Success_KONFEKSI2C()
        {
            var vm = new RealizationVbNonPOViewModel()
            {
                VBRealizationNo = "VBRealizationNo",
                Date = DateTimeOffset.Now,
                numberVB = new DetailRequestNonPO()
                {
                    Amount = 0,
                    CreateBy = "CreateBy",
                    CurrencyCode = "USD",
                    CurrencyRate = 123,
                    Date = DateTimeOffset.Now,
                    DateEstimate = DateTimeOffset.Now,
                    UnitCode = "UnitCode",
                    UnitId = 1,
                    UnitLoad = "KONFEKSI 2C",
                    UnitName = "UnitName",
                    VBNo = "VBNo",
                    VBRequestCategory = "NONPO"

                },
                Items = new List<VbNonPORequestDetailViewModel>()
                {
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now,
                        Remark = "Remark",
                        Amount = 123,
                        isGetPPn = false
                    }
                }

            };

            var serviceProviderMock = new Mock<IServiceProvider>();

            var serviceMock = new Mock<IRealizationVbNonPOService>();
            serviceMock
                .Setup(service => service.ReadByIdAsync2(It.IsAny<int>()))
                .ReturnsAsync(vm);
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);

            var controller = GetController(serviceProviderMock);

            var response = await controller.RealizationVbNonPORequestPDF(It.IsAny<int>());

            Assert.NotNull(response);
        }

        [Fact]
        public async Task Get_Sales_Receipt_PDF_Success_UMUM()
        {
            var vm = new RealizationVbNonPOViewModel()
            {
                VBRealizationNo = "VBRealizationNo",
                Date = DateTimeOffset.Now,
                numberVB = new DetailRequestNonPO()
                {
                    Amount = 123,
                    CreateBy = "CreateBy",
                    CurrencyCode = "USD",
                    CurrencyRate = 123,
                    Date = DateTimeOffset.Now,
                    DateEstimate = DateTimeOffset.Now,
                    UnitCode = "UnitCode",
                    UnitId = 1,
                    UnitLoad = "UMUM",
                    UnitName = "UnitName",
                    VBNo = "VBNo",
                    VBRequestCategory = "NONPO"

                },
                Items = new List<VbNonPORequestDetailViewModel>()
                {
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now,
                        Remark = "Remark",
                        Amount = 0,
                        isGetPPn = true
                    }
                }

            };

            var serviceProviderMock = new Mock<IServiceProvider>();

            var serviceMock = new Mock<IRealizationVbNonPOService>();
            serviceMock
                .Setup(service => service.ReadByIdAsync2(It.IsAny<int>()))
                .ReturnsAsync(vm);
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IRealizationVbNonPOService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);

            var controller = GetController(serviceProviderMock);

            var response = await controller.RealizationVbNonPORequestPDF(It.IsAny<int>());

            Assert.NotNull(response);
        }
    }
}
