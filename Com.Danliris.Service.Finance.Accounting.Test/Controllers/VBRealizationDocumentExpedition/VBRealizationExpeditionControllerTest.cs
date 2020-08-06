using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRealizationDocumentExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRealizationDocumentExpedition;
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

namespace Com.Danliris.Service.Finance.Accounting.Test.Controllers.VBRealizationDocumentExpedition
{
    public class VBRealizationExpeditionControllerTest
    {

        protected VBRealizationExpeditionController GetController(Mock<IServiceProvider> serviceProvider)
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

            VBRealizationExpeditionController controller = new VBRealizationExpeditionController(serviceProvider.Object);
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

        protected ServiceValidationException GetServiceValidationException()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            var validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(vBRealizationIdListDto, serviceProvider.Object, null);
            return new ServiceValidationException(validationContext, validationResults);
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        public VBRealizationIdListDto vBRealizationIdListDto
        {
            get
            {
                return new VBRealizationIdListDto()
                {
                    VBRealizationIds = new List<int>()
                    {
                        1
                    }
                };
            }
        }

        VBRealizationExpeditionRejectDto rejectDto
        {
            get
            {
                return new VBRealizationExpeditionRejectDto()
                {
                    Reason= "Reason"
                };
            }
        }
        [Fact]
        public void GetVbRealizationToVerification_Return_OK()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            var service = new Mock<IVBRealizationDocumentExpeditionService>();
            Dictionary<string, string> order = new Dictionary<string, string>();
            
            service.Setup(s => s.ReadRealizationToVerification(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset?>(), It.IsAny<string>(), It.IsAny<int>())).Returns(new ReadResponse<VBRealizationDocumentExpeditionModel>(new List<VBRealizationDocumentExpeditionModel>(),1, order,new List<string>()));

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService)))
               .Returns(service.Object);

            IActionResult response =  GetController(serviceProviderMock).GetVbRealizationToVerification(0, 0, null, null, 0);
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public void GetVbRealizationToVerification_Return_InternalServerError()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            var service = new Mock<IVBRealizationDocumentExpeditionService>();
            Dictionary<string, string> order = new Dictionary<string, string>();

            service.Setup(s => s.ReadRealizationToVerification(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset?>(), It.IsAny<string>(), It.IsAny<int>())).Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService)))
               .Returns(service.Object);

            IActionResult response = GetController(serviceProviderMock).GetVbRealizationToVerification(0, 0, null, null, 0);
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void Get_Return_OK()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            var service = new Mock<IVBRealizationDocumentExpeditionService>();
            Dictionary<string, string> order = new Dictionary<string, string>();

            service.Setup(s => s.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(new ReadResponse<VBRealizationDocumentExpeditionModel>(new List<VBRealizationDocumentExpeditionModel>(), 1, order, new List<string>()));

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService)))
               .Returns(service.Object);

            IActionResult response = GetController(serviceProviderMock).Get();
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public void Get_Return_InternalServerError()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            var service = new Mock<IVBRealizationDocumentExpeditionService>();
            
            service.Setup(s => s.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService)))
               .Returns(service.Object);

            IActionResult response = GetController(serviceProviderMock).Get();
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public async Task Post_Return_Created()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            var service = new Mock<IVBRealizationDocumentExpeditionService>();

            service.Setup(s => s.SubmitToVerification(It.IsAny<List<int>>())).ReturnsAsync(1);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService)))
               .Returns(service.Object);

            IActionResult response =await  GetController(serviceProviderMock).Post(vBRealizationIdListDto);
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.Created, statusCode);
        }
        

             [Fact]
        public async Task Post_Throws_ServiceValidationExeption()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            var service = new Mock<IVBRealizationDocumentExpeditionService>();

            service.Setup(s => s.SubmitToVerification(It.IsAny<List<int>>())).ThrowsAsync( GetServiceValidationException());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService)))
               .Returns(service.Object);

            IActionResult response = await GetController(serviceProviderMock).Post(vBRealizationIdListDto);
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public async Task Post_Return_InternalServerError()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            var service = new Mock<IVBRealizationDocumentExpeditionService>();
         
            service.Setup(s => s.SubmitToVerification(It.IsAny<List<int>>())).ThrowsAsync(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService)))
               .Returns(service.Object);

            IActionResult response = await GetController(serviceProviderMock).Post(vBRealizationIdListDto);
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }


        [Fact]
        public async Task Verify_Return_Created()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            var service = new Mock<IVBRealizationDocumentExpeditionService>();
           
            service.Setup(s => s.VerifiedToCashier(It.IsAny<int>())).ReturnsAsync(1);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService)))
               .Returns(service.Object);

            IActionResult response = await GetController(serviceProviderMock).Verify(1);
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.Created, statusCode);
        }


        [Fact]
        public async Task Verify_Return_InternalServerError()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            var service = new Mock<IVBRealizationDocumentExpeditionService>();

            service.Setup(s => s.VerifiedToCashier(It.IsAny<int>())).ThrowsAsync(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService)))
               .Returns(service.Object);

            IActionResult response = await GetController(serviceProviderMock).Verify(1);
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }
        [Fact]
        public async Task reject_Return_Created()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            var service = new Mock<IVBRealizationDocumentExpeditionService>();

            service.Setup(s => s.Reject(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(1);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService)))
               .Returns(service.Object);

            IActionResult response = await GetController(serviceProviderMock).reject(1, rejectDto);
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.Created, statusCode);
        }

        [Fact]
        public async Task reject_Throws_ServiceValidationException()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            var service = new Mock<IVBRealizationDocumentExpeditionService>();

            service.Setup(s => s.Reject(It.IsAny<int>(), It.IsAny<string>())).ThrowsAsync(GetServiceValidationException());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService)))
               .Returns(service.Object);

            IActionResult response = await GetController(serviceProviderMock).reject(1, rejectDto);
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }


        [Fact]
        public async Task reject_Return_InternalServerError()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            var service = new Mock<IVBRealizationDocumentExpeditionService>();

            service.Setup(s => s.Reject(It.IsAny<int>(), It.IsAny<string>())).ThrowsAsync(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService)))
               .Returns(service.Object);

            IActionResult response = await GetController(serviceProviderMock).reject(1, rejectDto);
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }
    }
}
