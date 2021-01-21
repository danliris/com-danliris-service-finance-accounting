using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.BudgetCashflow;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.BudgetCashflow;
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

namespace Com.Danliris.Service.Finance.Accounting.Test.Controllers.BudgetCashflowMaster
{
   public class BudgetCashflowMasterControllerTest
    {
        protected BudgetCashflowMasterController GetController(Mock<IServiceProvider> serviceProvider)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            BudgetCashflowMasterController controller = new BudgetCashflowMasterController(serviceProvider.Object);
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
        public void PostCashflowType_Succes_Created()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.CreateBudgetCashflowType(It.IsAny<CashflowTypeFormDto>()))
                .Returns(1);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            IActionResult response =  GetController(serviceProviderMock).PostCashflowType(new CashflowTypeFormDto());

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.Created, statusCode);
        }


        [Fact]
        public void PostCashflowType_Throws_ValidationException()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            CashflowTypeFormDto formDto = new CashflowTypeFormDto();
            service
                .Setup(s => s.CreateBudgetCashflowType(It.IsAny<CashflowTypeFormDto>()))
                .Throws(GetServiceValidationException(formDto));

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).PostCashflowType(new CashflowTypeFormDto());

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public void PostCashflowType_Throws_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            CashflowTypeFormDto formDto = new CashflowTypeFormDto();
            service
                .Setup(s => s.CreateBudgetCashflowType(It.IsAny<CashflowTypeFormDto>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).PostCashflowType(new CashflowTypeFormDto());

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }


        [Fact]
        public void GetCashflowType_Succes_OK()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.GetBudgetCashflowTypes(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ReadResponse<BudgetCashflowTypeModel>(new List<BudgetCashflowTypeModel>(),1,new Dictionary<string, string>(),new List<string>()));

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).GetCashflowType("",1,10);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }


        [Fact]
        public void GetCashflowType_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.GetBudgetCashflowTypes(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).GetCashflowType("", 1, 10);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void PutCashflowType_Succes_OK()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.UpdateBudgetCashflowType(It.IsAny<int>(), It.IsAny<CashflowTypeFormDto>()))
                .Returns(1);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            CashflowTypeFormDto formDto = new CashflowTypeFormDto();
            IActionResult response = GetController(serviceProviderMock).PutCashflowType(1, formDto);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCode);
        }

        [Fact]
        public void PutCashflowType_Succes_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.UpdateBudgetCashflowType(It.IsAny<int>(), It.IsAny<CashflowTypeFormDto>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            CashflowTypeFormDto formDto = new CashflowTypeFormDto();
            IActionResult response = GetController(serviceProviderMock).PutCashflowType(1, formDto);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void PutCashflowType_Throws_ValidationException()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            CashflowTypeFormDto formDto = new CashflowTypeFormDto();
            service
                .Setup(s => s.UpdateBudgetCashflowType(It.IsAny<int>(), It.IsAny<CashflowTypeFormDto>()))
                .Throws(GetServiceValidationException(formDto));

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
           
            IActionResult response = GetController(serviceProviderMock).PutCashflowType(1, formDto);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public void DeleteCashflowType_Return_NoContent()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.DeleteBudgetCashflowType(It.IsAny<int>()))
                .Returns(1);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            CashflowTypeFormDto formDto = new CashflowTypeFormDto();
            IActionResult response = GetController(serviceProviderMock).DeleteCashflowType(1);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCode);
        }

        [Fact]
        public void DeleteCashflowType_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.DeleteBudgetCashflowType(It.IsAny<int>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            CashflowTypeFormDto formDto = new CashflowTypeFormDto();
            IActionResult response = GetController(serviceProviderMock).DeleteCashflowType(1);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void GetCashflowTypeById_Return_NoContent()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.GetBudgetCashflowTypeById(It.IsAny<int>()))
                .Returns(new BudgetCashflowTypeModel());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            CashflowTypeFormDto formDto = new CashflowTypeFormDto();
            IActionResult response = GetController(serviceProviderMock).GetCashflowTypeById(1);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }


        [Fact]
        public void GetCashflowTypeById_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.GetBudgetCashflowTypeById(It.IsAny<int>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            CashflowTypeFormDto formDto = new CashflowTypeFormDto();
            IActionResult response = GetController(serviceProviderMock).GetCashflowTypeById(1);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void PostCashflowCategory_Return_Created()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.CreateBudgetCashflowCategory(It.IsAny<CashflowCategoryFormDto>()))
                .Returns(1);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            CashflowTypeFormDto formDto = new CashflowTypeFormDto();
            IActionResult response = GetController(serviceProviderMock).PostCashflowCategory(new CashflowCategoryFormDto());

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.Created, statusCode);
        }


        [Fact]
        public void PostCashflowCategory_Throws_ValidationException()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            CashflowTypeFormDto formDto = new CashflowTypeFormDto();
            service
                .Setup(s => s.CreateBudgetCashflowCategory(It.IsAny<CashflowCategoryFormDto>()))
                .Throws(GetServiceValidationException(formDto));

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
          
            IActionResult response = GetController(serviceProviderMock).PostCashflowCategory(new CashflowCategoryFormDto());

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public void PostCashflowCategory_Throws_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            CashflowTypeFormDto formDto = new CashflowTypeFormDto();
            service
                .Setup(s => s.CreateBudgetCashflowCategory(It.IsAny<CashflowCategoryFormDto>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act

            IActionResult response = GetController(serviceProviderMock).PostCashflowCategory(new CashflowCategoryFormDto());

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }


        [Fact]
        public void GetCashflowCategories_Return_OK()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.GetBudgetCashflowCategories(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ReadResponse<BudgetCashflowCategoryModel>(new List<BudgetCashflowCategoryModel>(),1,new Dictionary<string, string>(),new List<string>()));

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            CashflowTypeFormDto formDto = new CashflowTypeFormDto();
            IActionResult response = GetController(serviceProviderMock).GetCashflowCategories(1,"",1,10);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }


        [Fact]
        public void GetCashflowCategories_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.GetBudgetCashflowCategories(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            CashflowTypeFormDto formDto = new CashflowTypeFormDto();
            IActionResult response = GetController(serviceProviderMock).GetCashflowCategories(1, "", 1, 10);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void PutCashflowCategories_Return_NoContent()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.UpdateBudgetCashflowCategory(It.IsAny<int>(),It.IsAny<CashflowCategoryFormDto>()))
                .Returns(1);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            CashflowTypeFormDto formDto = new CashflowTypeFormDto();
            IActionResult response = GetController(serviceProviderMock).PutCashflowCategories(1,new CashflowCategoryFormDto());

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCode);
        }

        [Fact]
        public void PutCashflowCategories_Throws_ValidationException()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            CashflowTypeFormDto formDto = new CashflowTypeFormDto();
            service
                .Setup(s => s.UpdateBudgetCashflowCategory(It.IsAny<int>(), It.IsAny<CashflowCategoryFormDto>()))
                .Throws(GetServiceValidationException(formDto));

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
           
            IActionResult response = GetController(serviceProviderMock).PutCashflowCategories(1, new CashflowCategoryFormDto());

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public void PutCashflowCategories_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            CashflowTypeFormDto formDto = new CashflowTypeFormDto();
            service
                .Setup(s => s.UpdateBudgetCashflowCategory(It.IsAny<int>(), It.IsAny<CashflowCategoryFormDto>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act

            IActionResult response = GetController(serviceProviderMock).PutCashflowCategories(1, new CashflowCategoryFormDto());

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }


        [Fact]
        public void DeleteCashflowCategory_Return_NoContent()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.DeleteBudgetCashflowCategories(It.IsAny<int>()))
                .Returns(1);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            CashflowTypeFormDto formDto = new CashflowTypeFormDto();
            IActionResult response = GetController(serviceProviderMock).DeleteCashflowCategory(1);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCode);
        }

        [Fact]
        public void GetCashflowCategoryById_Return_OK()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.GetBudgetCashflowCategoryById(It.IsAny<int>()))
                .Returns(new BudgetCashflowCategoryModel());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            CashflowTypeFormDto formDto = new CashflowTypeFormDto();
            IActionResult response = GetController(serviceProviderMock).GetCashflowCategoryById(1);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public void GetCashflowCategoryById_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.GetBudgetCashflowCategoryById(It.IsAny<int>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            CashflowTypeFormDto formDto = new CashflowTypeFormDto();
            IActionResult response = GetController(serviceProviderMock).GetCashflowCategoryById(1);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }


        [Fact]
        public void DeleteCashflowCategory_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.DeleteBudgetCashflowCategories(It.IsAny<int>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            CashflowTypeFormDto formDto = new CashflowTypeFormDto();
            IActionResult response = GetController(serviceProviderMock).DeleteCashflowCategory(1);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }


        [Fact]
        public void PostCashflowSubCategory_Return_Create()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.CreateBudgetCashflowSubCategory(It.IsAny<CashflowSubCategoryFormDto>()))
                .Returns(1);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            CashflowSubCategoryFormDto formDto = new CashflowSubCategoryFormDto();
            IActionResult response = GetController(serviceProviderMock).PostCashflowSubCategory(formDto);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.Created, statusCode);
        }


        [Fact]
        public void PostCashflowSubCategory_Throws_ValidationException()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            CashflowSubCategoryFormDto formDto = new CashflowSubCategoryFormDto();
            service
                .Setup(s => s.CreateBudgetCashflowSubCategory(It.IsAny<CashflowSubCategoryFormDto>()))
                .Throws(GetServiceValidationException(formDto));

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            
            IActionResult response = GetController(serviceProviderMock).PostCashflowSubCategory(formDto);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public void PostCashflowSubCategory_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.CreateBudgetCashflowSubCategory(It.IsAny<CashflowSubCategoryFormDto>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            CashflowSubCategoryFormDto formDto = new CashflowSubCategoryFormDto();
            IActionResult response = GetController(serviceProviderMock).PostCashflowSubCategory(formDto);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void GetCashflowSubCategories_Return_OK()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.GetBudgetCashflowSubCategories(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ReadResponse<BudgetCashflowSubCategoryModel>(new List<BudgetCashflowSubCategoryModel>(),1,new Dictionary<string, string>(),new List<string>()) {});

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).GetCashflowSubCategories(1,"",1,10);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public void GetCashflowSubCategories_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.GetBudgetCashflowSubCategories(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).GetCashflowSubCategories(1, "", 1, 10);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void PutCashflowSubCategories_Success_Return_NoContent()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.UpdateBudgetCashflowSubCategory( It.IsAny<int>(), It.IsAny<CashflowSubCategoryFormDto>()))
                .Returns(1);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            CashflowSubCategoryFormDto form = new CashflowSubCategoryFormDto();
            IActionResult response = GetController(serviceProviderMock).PutCashflowSubCategories(1, form);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCode);
        }


        [Fact]
        public void PutCashflowSubCategories_Throws_ValidationException()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            CashflowSubCategoryFormDto form = new CashflowSubCategoryFormDto();
            service
                .Setup(s => s.UpdateBudgetCashflowSubCategory(It.IsAny<int>(), It.IsAny<CashflowSubCategoryFormDto>()))
                .Throws(GetServiceValidationException(form));

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            
            IActionResult response = GetController(serviceProviderMock).PutCashflowSubCategories(1, form);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public void PutCashflowSubCategories_Throws_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            CashflowSubCategoryFormDto form = new CashflowSubCategoryFormDto();
            service
                .Setup(s => s.UpdateBudgetCashflowSubCategory(It.IsAny<int>(), It.IsAny<CashflowSubCategoryFormDto>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act

            IActionResult response = GetController(serviceProviderMock).PutCashflowSubCategories(1, form);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }


        [Fact]
        public void DeleteCashflowSubCategory_Success_Return_NoContent()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.DeleteBudgetCashflowSubCategories(It.IsAny<int>()))
                .Returns(1);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            CashflowSubCategoryFormDto form = new CashflowSubCategoryFormDto();
            IActionResult response = GetController(serviceProviderMock).DeleteCashflowSubCategory(1);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCode);
        }


        [Fact]
        public void DeleteCashflowSubCategory_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.DeleteBudgetCashflowSubCategories(It.IsAny<int>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            CashflowSubCategoryFormDto form = new CashflowSubCategoryFormDto();
            IActionResult response = GetController(serviceProviderMock).DeleteCashflowSubCategory(1);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void GetCashflowSubCategoryById_Success_Return_OK()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            var model = new BudgetCashflowSubCategoryModel("", 1, 1, new List<int>() { 1 }, true,new ReportType(),false);
            
            service
                .Setup(s => s.GetBudgetCashflowSubCategoryById(It.IsAny<int>()))
                .Returns(new BudgetCashflowSubCategoryTypeDto(model));

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            CashflowSubCategoryFormDto form = new CashflowSubCategoryFormDto();
            IActionResult response = GetController(serviceProviderMock).GetCashflowSubCategoryById(1);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }


        [Fact]
        public void GetCashflowSubCategoryById_Success_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.GetBudgetCashflowSubCategoryById(It.IsAny<int>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            CashflowSubCategoryFormDto form = new CashflowSubCategoryFormDto();
            IActionResult response = GetController(serviceProviderMock).GetCashflowSubCategoryById(1);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void Get_Success_Return_OK()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.GetBudgetCashflowMasterLayout(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ReadResponse<BudgetCashflowMasterDto>(new List<BudgetCashflowMasterDto>(),1,new Dictionary<string, string>(),new List<string>()));

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            CashflowSubCategoryFormDto form = new CashflowSubCategoryFormDto();
            IActionResult response = GetController(serviceProviderMock).Get("",1,10);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public void Get_Success_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.GetBudgetCashflowMasterLayout(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            //Act
            CashflowSubCategoryFormDto form = new CashflowSubCategoryFormDto();
            IActionResult response = GetController(serviceProviderMock).Get("", 1, 10);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }
    }
}
