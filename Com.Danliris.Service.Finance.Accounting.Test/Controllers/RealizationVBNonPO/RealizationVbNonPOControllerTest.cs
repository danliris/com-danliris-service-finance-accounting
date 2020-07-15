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
    }
}
