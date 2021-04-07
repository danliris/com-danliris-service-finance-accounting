using Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.AccountingBook;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.ValidateService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using System.ComponentModel.DataAnnotations;
using Xunit;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.AccountingBook;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.AccountingBook;
using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.AccountingBook;
using System.Threading.Tasks;
using System.Net;

namespace Com.Danliris.Service.Finance.Accounting.Test.Controllers.AccountingBook
{
    public class AccountingBookControllerTest
    {
        private AccountingBookViewModel ViewModel
        {
            get { return new AccountingBookViewModel(); }
        }
        private AccountingBookController GetController(IServiceProvider serviceprovider)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username","unittestusername")
               };
            user.Setup(x => x.Claims).Returns(claims);

            //AccountingBookController controller = new AccountingBookController(serviceprovider.Object);
            var controller = (AccountingBookController)Activator.CreateInstance(typeof(AccountingBookController), serviceprovider);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "7";
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

        protected ServiceValidationException GetServiceValidationException()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            var validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(ViewModel, serviceProvider.Object, null);
            return new ServiceValidationException(validationContext, validationResults);
        }

        [Fact]
        public void Get_WithoutException_ReturnOK()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var serviceMock = new Mock<IAccountingBookService>();
            serviceMock
                .Setup(service => service.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ReadResponse<AccountingBookModel>(new List<AccountingBookModel>(), 1, new Dictionary<string, string>(), new List<string>()));
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IAccountingBookService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);
            var mapperMock = new Mock<IMapper>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IMapper))).Returns(mapperMock.Object);

            var controller = GetController(serviceProviderMock.Object);

            var response = controller.Get(select: new List<string>());
            var statusCode = GetStatusCode(response);

            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public void Get_WithException_ReturnInternalServerError()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var serviceMock = new Mock<IAccountingBookService>();
            serviceMock
                .Setup(service => service.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IAccountingBookService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);
            var mapperMock = new Mock<IMapper>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IMapper))).Returns(mapperMock.Object);

            var controller = GetController(serviceProviderMock.Object);

            var response = controller.Get(select: new List<string>());
            var statusCode = GetStatusCode(response);

            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public async Task   PostAccountingBook_Success_Created()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            //var serviceProviderMock = new Mock<IServiceProvider>();
            var serviceMock  = new Mock<IAccountingBookService>();

            serviceMock
                .Setup(s => s.CreateAsync(It.IsAny<AccountingBookModel>()))
                .ReturnsAsync(1);
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IAccountingBookService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            validateServiceMock
                .Setup(validateService => validateService.Validate(It.IsAny<AccountingBookModel>()))
                .Verifiable();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);
            var mapperMock = new Mock<IMapper>();
            mapperMock
                .Setup(mapper => mapper.Map<AccountingBookModel>(It.IsAny<AccountingBookViewModel>()))
                .Returns(It.IsAny<AccountingBookModel>());
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IMapper))).Returns(mapperMock.Object);

            var controller = GetController(serviceProviderMock.Object);

            var response = await controller.Post(new AccountingBookViewModel());
            var statusCode = GetStatusCode(response);

            Assert.Equal((int)HttpStatusCode.Created, statusCode);
        }

        [Fact]
        public async Task Post_WithException_ReturnBadRequest()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var serviceMock = new Mock<IAccountingBookService>();
            serviceMock
                .Setup(service => service.CreateAsync(It.IsAny<AccountingBookModel>()))
                .ReturnsAsync(1);
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IAccountingBookService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            validateServiceMock
                .Setup(validateService => validateService.Validate(It.IsAny<AccountingBookViewModel>()))
                .Throws(GetServiceValidationException());
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);
            var mapperMock = new Mock<IMapper>();
            mapperMock
                .Setup(mapper => mapper.Map<AccountingBookModel>(It.IsAny<AccountingBookViewModel>()))
                .Returns(It.IsAny<AccountingBookModel>());
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IMapper))).Returns(mapperMock.Object);

            var controller = GetController(serviceProviderMock.Object);

            var response = await controller.Post(new AccountingBookViewModel());
            var statusCode = GetStatusCode(response);

            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public async Task Post_WithException_ReturnInternalServerError()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var serviceMock = new Mock<IAccountingBookService>();
            serviceMock
                .Setup(service => service.CreateAsync(It.IsAny<AccountingBookModel>()))
                .ReturnsAsync(1);
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IAccountingBookService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            validateServiceMock
                .Setup(validateService => validateService.Validate(It.IsAny<AccountingBookViewModel>()))
                .Throws(new Exception());
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);
            var mapperMock = new Mock<IMapper>();
            mapperMock
                .Setup(mapper => mapper.Map<AccountingBookModel>(It.IsAny<AccountingBookViewModel>()))
                .Returns(It.IsAny<AccountingBookModel>());
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IMapper))).Returns(mapperMock.Object);

            var controller = GetController(serviceProviderMock.Object);

            var response = await controller.Post(new AccountingBookViewModel());
            var statusCode = GetStatusCode(response);

            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public async Task DeleteById_WithoutException_ReturnNoContent()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var serviceMock = new Mock<IAccountingBookService>();
            serviceMock
                .Setup(service => service.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync(1);
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IAccountingBookService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);
            var mapperMock = new Mock<IMapper>();
            mapperMock
                .Setup(mapper => mapper.Map<AccountingBookViewModel>(It.IsAny<AccountingBookModel>()))
                .Returns(new AccountingBookViewModel());
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IMapper))).Returns(mapperMock.Object);

            var controller = GetController(serviceProviderMock.Object);

            var response = await controller.Delete(It.IsAny<int>());
            var statusCode = GetStatusCode(response);

            Assert.Equal((int)HttpStatusCode.NoContent, statusCode);
        }

        [Fact]
        public async Task DeleteById_WithException_ReturnInternalServerError()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var serviceMock = new Mock<IAccountingBookService>();
            serviceMock
                .Setup(service => service.DeleteAsync(It.IsAny<int>()))
                .Throws(new Exception());
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IAccountingBookService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);
            var mapperMock = new Mock<IMapper>();
            mapperMock
                .Setup(mapper => mapper.Map<AccountingBookViewModel>(It.IsAny<AccountingBookModel>()))
                .Returns(new AccountingBookViewModel());
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IMapper))).Returns(mapperMock.Object);

            var controller = GetController(serviceProviderMock.Object);

            var response = await controller.Delete(It.IsAny<int>());
            var statusCode = GetStatusCode(response);

            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }
        #region put
        [Fact]
        public async Task Put_WithoutException_ReturnNoContent()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var serviceMock = new Mock<IAccountingBookService>();
            serviceMock
                .Setup(service => service.UpdateAsync(It.IsAny<int>(), It.IsAny<AccountingBookModel>()))
                .ReturnsAsync(1);
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IAccountingBookService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            validateServiceMock
                .Setup(validateService => validateService.Validate(It.IsAny<AccountingBookViewModel>()))
                .Verifiable();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);
            var mapperMock = new Mock<IMapper>();
            mapperMock
                .Setup(mapper => mapper.Map<AccountingBookModel>(It.IsAny<AccountingBookViewModel>()))
                .Returns(It.IsAny<AccountingBookModel>());
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IMapper))).Returns(mapperMock.Object);

            var controller = GetController(serviceProviderMock.Object);

            var response = await controller.Put(It.IsAny<int>(), new AccountingBookViewModel());
            var statusCode = GetStatusCode(response);

            Assert.Equal((int)HttpStatusCode.NoContent, statusCode);
        }

        [Fact]
        public async Task Put_WithValidationException_ReturnBadRequest()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var serviceMock = new Mock<IAccountingBookService>();
            serviceMock
                .Setup(service => service.UpdateAsync(It.IsAny<int>(), It.IsAny<AccountingBookModel>()))
                .ReturnsAsync(1);
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IAccountingBookService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            validateServiceMock
                .Setup(validateService => validateService.Validate(It.IsAny<AccountingBookViewModel>()))
                .Throws(GetServiceValidationException());
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);
            var mapperMock = new Mock<IMapper>();
            mapperMock
                .Setup(mapper => mapper.Map<AccountingBookModel>(It.IsAny<AccountingBookViewModel>()))
                .Returns(It.IsAny<AccountingBookModel>());
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IMapper))).Returns(mapperMock.Object);

            var controller = GetController(serviceProviderMock.Object);

            var response = await controller.Put(It.IsAny<int>(), new AccountingBookViewModel());
            var statusCode = GetStatusCode(response);

            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public async Task Put_WithValidationException_ReturnBadRequest_Diff_Id()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var serviceMock = new Mock<IAccountingBookService>();
            serviceMock
                .Setup(service => service.UpdateAsync(It.IsAny<int>(), It.IsAny<AccountingBookModel>()))
                .ReturnsAsync(1);
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IAccountingBookService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            validateServiceMock
                .Setup(validateService => validateService.Validate(It.IsAny<AccountingBookViewModel>()))
                .Verifiable();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);
            var mapperMock = new Mock<IMapper>();
            mapperMock
                .Setup(mapper => mapper.Map<AccountingBookModel>(It.IsAny<AccountingBookViewModel>()))
                .Returns(It.IsAny<AccountingBookModel>());
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IMapper))).Returns(mapperMock.Object);

            var controller = GetController(serviceProviderMock.Object);

            var response = await controller.Put(1, new AccountingBookViewModel() { Id = 0 });
            var statusCode = GetStatusCode(response);

            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public async Task Put_WithException_ReturnInternalServerError()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var serviceMock = new Mock<IAccountingBookService>();
            serviceMock
                .Setup(service => service.UpdateAsync(It.IsAny<int>(), It.IsAny<AccountingBookModel>()))
                .ReturnsAsync(1);
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IAccountingBookService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            validateServiceMock
                .Setup(validateService => validateService.Validate(It.IsAny<AccountingBookViewModel>()))
                .Throws(new Exception());
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);
            var mapperMock = new Mock<IMapper>();
            mapperMock
                .Setup(mapper => mapper.Map<AccountingBookModel>(It.IsAny<AccountingBookViewModel>()))
                .Returns(It.IsAny<AccountingBookModel>());
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IMapper))).Returns(mapperMock.Object);

            var controller = GetController(serviceProviderMock.Object);

            var response = await controller.Put(It.IsAny<int>(), new AccountingBookViewModel());
            var statusCode = GetStatusCode(response);

            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }
        #endregion

        [Fact]
        public async Task GetById_WithoutException_ReturnOk()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var serviceMock = new Mock<IAccountingBookService>();
            serviceMock
                .Setup(service => service.ReadByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new AccountingBookModel());
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IAccountingBookService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            validateServiceMock
                .Setup(validateService => validateService.Validate(It.IsAny<AccountingBookViewModel>()))
                .Verifiable();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);
            var mapperMock = new Mock<IMapper>();
            mapperMock
                .Setup(mapper => mapper.Map<AccountingBookModel>(It.IsAny<AccountingBookViewModel>()))
                .Returns(It.IsAny<AccountingBookModel>());
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IMapper))).Returns(mapperMock.Object);

            var controller = GetController(serviceProviderMock.Object);

            var response = await controller.GetById(It.IsAny<int>());
            var statusCode = GetStatusCode(response);

            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public async Task GetById_WithException_ReturnInternalServerError()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var serviceMock = new Mock<IAccountingBookService>();
            serviceMock
                .Setup(service => service.ReadByIdAsync(It.IsAny<int>()))
                .Throws(new Exception());
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IAccountingBookService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            validateServiceMock
                .Setup(validateService => validateService.Validate(It.IsAny<AccountingBookViewModel>()))
                .Throws(new Exception());
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);
            var mapperMock = new Mock<IMapper>();
            mapperMock
                .Setup(mapper => mapper.Map<AccountingBookModel>(It.IsAny<AccountingBookViewModel>()))
                .Returns(It.IsAny<AccountingBookModel>());
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IMapper))).Returns(mapperMock.Object);

            var controller = GetController(serviceProviderMock.Object);

            var response = await controller.GetById(It.IsAny<int>());
            var statusCode = GetStatusCode(response);

            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public async Task GetById_WithException_ReturnNotFound_NullId()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var serviceMock = new Mock<IAccountingBookService>();
            serviceMock
                .Setup(service => service.ReadByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((AccountingBookModel)null);
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IAccountingBookService))).Returns(serviceMock.Object);

            var validateServiceMock = new Mock<IValidateService>();
            validateServiceMock
                .Setup(validateService => validateService.Validate(It.IsAny<AccountingBookViewModel>()))
                .Throws(new Exception());
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(identityServiceMock.Object);
            var mapperMock = new Mock<IMapper>();
            mapperMock
                .Setup(mapper => mapper.Map<AccountingBookModel>(It.IsAny<AccountingBookViewModel>()))
                .Returns(It.IsAny<AccountingBookModel>());
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IMapper))).Returns(mapperMock.Object);

            var controller = GetController(serviceProviderMock.Object);

            var response = await controller.GetById(It.IsAny<int>());
            var statusCode = GetStatusCode(response);

            Assert.Equal((int)HttpStatusCode.NotFound, statusCode);
        }


    }
}
