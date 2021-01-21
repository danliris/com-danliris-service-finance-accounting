using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.BudgetCashflow;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.BudgetCashflow;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.CacheService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.ValidateService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Controllers.BudgetCashflow
{
 public   class BudgetCashflowControllerTest
    {
        protected BudgetCashflowController GetController(Mock<IServiceProvider> serviceProvider)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            BudgetCashflowController controller = new BudgetCashflowController(serviceProvider.Object);
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
        public void PostCashflowUnit_Succes_Created()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.CreateBudgetCashflowUnit(It.IsAny<CashflowUnitFormDto>()))
                .Returns(1);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).PostCashflowUnit(new CashflowUnitFormDto());

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.Created, statusCode);
        }

        [Fact]
        public void PostCashflowUnit_Throws_ValidationException()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            var formDto = new CashflowUnitFormDto();
            service
                .Setup(s => s.CreateBudgetCashflowUnit(It.IsAny<CashflowUnitFormDto>()))
                .Throws(GetServiceValidationException(formDto));

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).PostCashflowUnit(formDto);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public void PostCashflowUnit_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            var formDto = new CashflowUnitFormDto();
            service
                .Setup(s => s.CreateBudgetCashflowUnit(It.IsAny<CashflowUnitFormDto>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).PostCashflowUnit(formDto);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void UpdateCashflowUnit_Succes_Return_NoContent()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.UpdateBudgetCashflowUnit(It.IsAny<CashflowUnitFormDto>()))
                .Returns(1);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).UpdateCashflowUnit(new CashflowUnitFormDto());

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCode);
        }

        [Fact]
        public void UpdateCashflowUnit_Throws_ServiceValidationException()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            var formDto = new CashflowUnitFormDto();
            service
                .Setup(s => s.UpdateBudgetCashflowUnit(It.IsAny<CashflowUnitFormDto>()))
                .Throws(GetServiceValidationException(formDto));

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).UpdateCashflowUnit(formDto);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }


        [Fact]
        public void UpdateCashflowUnit_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            var formDto = new CashflowUnitFormDto();
            service
                .Setup(s => s.UpdateBudgetCashflowUnit(It.IsAny<CashflowUnitFormDto>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).UpdateCashflowUnit(formDto);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public async Task Get_Return_OK()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.GetBudgetCashflowUnit(It.IsAny<int>(), It.IsAny<DateTimeOffset>()))
                .ReturnsAsync(new List<BudgetCashflowItemDto>());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            //Act
            IActionResult response =await GetController(serviceProviderMock).Get(1, DateTimeOffset.Now);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public async Task Get_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();

            service
                .Setup(s => s.GetBudgetCashflowUnit(It.IsAny<int>(), It.IsAny<DateTimeOffset>()))
                .ThrowsAsync(new Exception("",new Exception()) { 
                
                });

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            //Act
            IActionResult response = await GetController(serviceProviderMock).Get(1, DateTimeOffset.Now);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public async Task GeneratePdf_Success_Return_FilePdf()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            BudgetCashflowItemDto budgetCashflowItemDto = new BudgetCashflowItemDto(1,"",false);
            
            service
                .Setup(s => s.GetBudgetCashflowUnit(It.IsAny<int>(), It.IsAny<DateTimeOffset>()))
                .ReturnsAsync(new List<BudgetCashflowItemDto>() { budgetCashflowItemDto });


            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };
           
            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            //Act
            IActionResult response = await GetController(serviceProviderMock).GeneratePdf(1, DateTimeOffset.Now);

            //Assert
            Assert.NotNull(response);
            Assert.Equal("application/pdf", response.GetType().GetProperty("ContentType").GetValue(response, null));
        }


        [Fact]
        public async Task GeneratePdf_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            BudgetCashflowItemDto budgetCashflowItemDto = new BudgetCashflowItemDto(1, "", false);

            service
                .Setup(s => s.GetBudgetCashflowUnit(It.IsAny<int>(), It.IsAny<DateTimeOffset>()))
                .ThrowsAsync(new Exception());


            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            //Act
            IActionResult response = await GetController(serviceProviderMock).GeneratePdf(1, DateTimeOffset.Now);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public async Task GetDivision_Return_OK()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            BudgetCashflowItemDto budgetCashflowItemDto = new BudgetCashflowItemDto(1, "", false);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            service
              .Setup(s => s.GetBudgetCashflowDivision(It.IsAny<int>(), It.IsAny<DateTimeOffset>()))
              .ReturnsAsync(new BudgetCashflowDivision(new List<string>(), new List<BudgetCashflowItemDto>()));


            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);


            //Act
            IActionResult response = await GetController(serviceProviderMock).GetDivision(1, DateTimeOffset.Now);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public async Task GetDivision_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            BudgetCashflowItemDto budgetCashflowItemDto = new BudgetCashflowItemDto(1, "", false);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            service
              .Setup(s => s.GetBudgetCashflowDivision(It.IsAny<int>(), It.IsAny<DateTimeOffset>()))
              .ThrowsAsync(new Exception());


            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);


            //Act
            IActionResult response = await GetController(serviceProviderMock).GetDivision(1, DateTimeOffset.Now);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }


        [Fact]
        public async Task GetItems_Return_OK()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            BudgetCashflowItemDto budgetCashflowItemDto = new BudgetCashflowItemDto(1, "", false);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            BudgetCashflowUnitModel budgetCashflowUnit = new BudgetCashflowUnitModel(); 
            service
              .Setup(s => s.GetBudgetCashflowUnit(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>()))
              .Returns(new List<BudgetCashflowUnitItemDto>());


            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);


            //Act
            IActionResult response =  GetController(serviceProviderMock).GetItems(1,1, DateTimeOffset.Now);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public async Task GetItems_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            BudgetCashflowItemDto budgetCashflowItemDto = new BudgetCashflowItemDto(1, "", false);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            BudgetCashflowUnitModel budgetCashflowUnit = new BudgetCashflowUnitModel();
            service
              .Setup(s => s.GetBudgetCashflowUnit(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset>()))
              .Throws(new Exception());


            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);


            //Act
            IActionResult response =  GetController(serviceProviderMock).GetItems(1, 1, DateTimeOffset.Now);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }


        [Fact]
        public void  PostInitialCashBalance_Return_Created()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            BudgetCashflowItemDto budgetCashflowItemDto = new BudgetCashflowItemDto(1, "", false);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            CashBalanceFormDto form = new CashBalanceFormDto();
            service
              .Setup(s => s.CreateInitialCashBalance(It.IsAny<CashBalanceFormDto>()))
              .Returns(1);


            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);


            //Act
            IActionResult response = GetController(serviceProviderMock).PostInitialCashBalance(form);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.Created, statusCode);
        }


        [Fact]
        public void PostInitialCashBalance_Throws_ValidationException()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            BudgetCashflowItemDto budgetCashflowItemDto = new BudgetCashflowItemDto(1, "", false);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            CashBalanceFormDto form = new CashBalanceFormDto();
            service
              .Setup(s => s.CreateInitialCashBalance(It.IsAny<CashBalanceFormDto>()))
              .Throws(GetServiceValidationException(form));


            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);


            //Act
            IActionResult response = GetController(serviceProviderMock).PostInitialCashBalance(form);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public void PostInitialCashBalance_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            BudgetCashflowItemDto budgetCashflowItemDto = new BudgetCashflowItemDto(1, "", false);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            CashBalanceFormDto form = new CashBalanceFormDto();
            service
              .Setup(s => s.CreateInitialCashBalance(It.IsAny<CashBalanceFormDto>()))
              .Throws(new Exception());


            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);


            //Act
            IActionResult response = GetController(serviceProviderMock).PostInitialCashBalance(form);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void UpdateInitialCashBalance_Return_NoContent()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            BudgetCashflowItemDto budgetCashflowItemDto = new BudgetCashflowItemDto(1, "", false);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            CashBalanceFormDto form = new CashBalanceFormDto();
            service
              .Setup(s => s.UpdateInitialCashBalance(It.IsAny<CashBalanceFormDto>()))
              .Returns(1);


            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);


            //Act
            IActionResult response = GetController(serviceProviderMock).UpdateInitialCashBalance(form);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCode);
        }

        [Fact]
        public void UpdateInitialCashBalance_Throws_ValidationException()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            BudgetCashflowItemDto budgetCashflowItemDto = new BudgetCashflowItemDto(1, "", false);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            CashBalanceFormDto form = new CashBalanceFormDto();
            service
              .Setup(s => s.UpdateInitialCashBalance(It.IsAny<CashBalanceFormDto>()))
              .Throws(GetServiceValidationException(form));


            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);


            //Act
            IActionResult response = GetController(serviceProviderMock).UpdateInitialCashBalance(form);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }


        [Fact]
        public void UpdateInitialCashBalance_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            BudgetCashflowItemDto budgetCashflowItemDto = new BudgetCashflowItemDto(1, "", false);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            CashBalanceFormDto form = new CashBalanceFormDto();
            service
              .Setup(s => s.UpdateInitialCashBalance(It.IsAny<CashBalanceFormDto>()))
              .Throws(new Exception());


            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);


            //Act
            IActionResult response = GetController(serviceProviderMock).UpdateInitialCashBalance(form);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }


        [Fact]
        public void GetInitialCashBalanceItems_Return_OK()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            BudgetCashflowItemDto budgetCashflowItemDto = new BudgetCashflowItemDto(1, "", false);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            CashBalanceFormDto form = new CashBalanceFormDto();
            service
              .Setup(s => s.GetInitialCashBalance(It.IsAny<int>(), It.IsAny<DateTimeOffset>()))
              .Returns(new List<BudgetCashflowUnitItemDto>());


            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);


            //Act
            IActionResult response = GetController(serviceProviderMock).GetInitialCashBalanceItems(1,DateTimeOffset.Now);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }


        [Fact]
        public void GetInitialCashBalanceItems_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            BudgetCashflowItemDto budgetCashflowItemDto = new BudgetCashflowItemDto(1, "", false);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            CashBalanceFormDto form = new CashBalanceFormDto();
            service
              .Setup(s => s.GetInitialCashBalance(It.IsAny<int>(), It.IsAny<DateTimeOffset>()))
              .Throws(new Exception());


            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);


            //Act
            IActionResult response = GetController(serviceProviderMock).GetInitialCashBalanceItems(1, DateTimeOffset.Now);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }


        [Fact]
        public void PostRealCashBalance_Return_Created()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            BudgetCashflowItemDto budgetCashflowItemDto = new BudgetCashflowItemDto(1, "", false);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            CashBalanceFormDto form = new CashBalanceFormDto();
            service
              .Setup(s => s.CreateRealCashBalance(It.IsAny<CashBalanceFormDto>()))
              .Returns(1);


            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);


            //Act
            IActionResult response = GetController(serviceProviderMock).PostRealCashBalance(form);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.Created, statusCode);
        }

        [Fact]
        public void PostRealCashBalance_Throws_ValidationException()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            BudgetCashflowItemDto budgetCashflowItemDto = new BudgetCashflowItemDto(1, "", false);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            CashBalanceFormDto form = new CashBalanceFormDto();
            service
              .Setup(s => s.CreateRealCashBalance(It.IsAny<CashBalanceFormDto>()))
              .Throws(GetServiceValidationException(form));


            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);


            //Act
            IActionResult response = GetController(serviceProviderMock).PostRealCashBalance(form);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }


        [Fact]
        public void PostRealCashBalance_Throws_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            BudgetCashflowItemDto budgetCashflowItemDto = new BudgetCashflowItemDto(1, "", false);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            CashBalanceFormDto form = new CashBalanceFormDto();
            service
              .Setup(s => s.CreateRealCashBalance(It.IsAny<CashBalanceFormDto>()))
              .Throws(new Exception());


            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);


            //Act
            IActionResult response = GetController(serviceProviderMock).PostRealCashBalance(form);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }


        [Fact]
        public void UpdateRealCashBalance_Return_NoContent()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            BudgetCashflowItemDto budgetCashflowItemDto = new BudgetCashflowItemDto(1, "", false);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            CashBalanceFormDto form = new CashBalanceFormDto();
            service
              .Setup(s => s.UpdateRealCashBalance(It.IsAny<CashBalanceFormDto>()))
              .Returns(1);


            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);


            //Act
            IActionResult response = GetController(serviceProviderMock).UpdateRealCashBalance(form);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCode);
        }


        [Fact]
        public void UpdateRealCashBalance_Throws_ValidationException()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            BudgetCashflowItemDto budgetCashflowItemDto = new BudgetCashflowItemDto(1, "", false);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            CashBalanceFormDto form = new CashBalanceFormDto();
            service
              .Setup(s => s.UpdateRealCashBalance(It.IsAny<CashBalanceFormDto>()))
              .Throws(GetServiceValidationException(form));


            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);


            //Act
            IActionResult response = GetController(serviceProviderMock).UpdateRealCashBalance(form);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public void UpdateRealCashBalance_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            BudgetCashflowItemDto budgetCashflowItemDto = new BudgetCashflowItemDto(1, "", false);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            CashBalanceFormDto form = new CashBalanceFormDto();
            service
              .Setup(s => s.UpdateRealCashBalance(It.IsAny<CashBalanceFormDto>()))
              .Throws(new Exception());


            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);


            //Act
            IActionResult response = GetController(serviceProviderMock).UpdateRealCashBalance(form);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }


        [Fact]
        public void GetRealCashBalanceItems_Return_OK()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            BudgetCashflowItemDto budgetCashflowItemDto = new BudgetCashflowItemDto(1, "", false);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            service
              .Setup(s => s.GetRealCashBalance(It.IsAny<int>(), It.IsAny<DateTimeOffset>()))
              .Returns(new List<BudgetCashflowUnitItemDto>());


            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);


            //Act
            IActionResult response = GetController(serviceProviderMock).GetRealCashBalanceItems(1,DateTimeOffset.Now);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }


        [Fact]
        public void GetRealCashBalanceItems_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IBudgetCashflowService>();
            BudgetCashflowItemDto budgetCashflowItemDto = new BudgetCashflowItemDto(1, "", false);

            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string json_data = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(s => s.GetString(It.IsAny<string>())).Returns(json_data);
            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);

            service
              .Setup(s => s.GetRealCashBalance(It.IsAny<int>(), It.IsAny<DateTimeOffset>()))
              .Throws(new Exception());


            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IBudgetCashflowService)))
               .Returns(service.Object);


            //Act
            IActionResult response = GetController(serviceProviderMock).GetRealCashBalanceItems(1, DateTimeOffset.Now);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

    }
}
