using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.BudgetCashflow;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.CacheService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.BudgetCashflow;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.BudgetCashflow
{
    public class BudgetCashflowServiceTest
    {
        private const string Entity = "GarmentPurchasingExpeditions";

        private string GetCurrentAsyncMethod([CallerMemberName] string methodName = "")
        {
            var method = new StackTrace()
                .GetFrames()
                .Select(frame => frame.GetMethod())
                .FirstOrDefault(item => item.Name == methodName);

            return method.Name;

        }

        private FinanceDbContext GetDbContext(string testName)
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .UseInternalServiceProvider(serviceProvider);

            return new FinanceDbContext(optionsBuilder.Options);
        }

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { TimezoneOffset = 0, Token = "token", Username = "username" });

            return serviceProviderMock;
        }


        private BudgetCashflowDataUtil GetDataUtil(BudgetCashflowService service)
        {
            return new BudgetCashflowDataUtil(service);
        }

        [Fact]
        public void Should_Success_CreateBudgetCashflowUnit()
        {
            //Arrange
            var dbContext = GetDbContext(GetCurrentAsyncMethod());
            var serviceProviderMock = GetServiceProvider();
            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataCurrencies= new List<CurrencyDto>()
            {
                new CurrencyDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string jsonCurrency = JsonConvert.SerializeObject(dataCurrencies);
            ICacheServiceMock.Setup(x => x.GetString(It.Is<string>(y => y.Contains("Currency")))).Returns(jsonCurrency);

            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string jsonUnit = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(x => x.GetString(It.Is<string>(y => y.Contains("Unit")))).Returns(jsonUnit);


            var dataDivisons = new List<DivisionDto>()
            {
                new DivisionDto()
                {
                    Code="Code"
                }
            };
            string jsonDivision = JsonConvert.SerializeObject(dataDivisons);
            ICacheServiceMock.Setup(x => x.GetString(It.Is<string>(y => y.Contains("Division")))).Returns(jsonDivision);

            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);


            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(FinanceDbContext)))
                .Returns(dbContext);

            var service = new BudgetCashflowService(serviceProviderMock.Object);
            var formDto = GetDataUtil(service).GetNewData_CashflowUnitFormDto();

            //Act
            var result =  service.CreateBudgetCashflowUnit(formDto);

            //Assert
            Assert.NotEqual(0, result);
        }


        [Fact]
        public void Should_Success_CashflowCategoryFormDto()
        {
            //Arrange
            var dbContext = GetDbContext(GetCurrentAsyncMethod());
            var serviceProviderMock = GetServiceProvider();
            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataCurrencies = new List<CurrencyDto>()
            {
                new CurrencyDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string jsonCurrency = JsonConvert.SerializeObject(dataCurrencies);
            ICacheServiceMock.Setup(x => x.GetString(It.Is<string>(y => y.Contains("Currency")))).Returns(jsonCurrency);

            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string jsonUnit = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(x => x.GetString(It.Is<string>(y => y.Contains("Unit")))).Returns(jsonUnit);


            var dataDivisons = new List<DivisionDto>()
            {
                new DivisionDto()
                {
                    Code="Code"
                }
            };
            string jsonDivision = JsonConvert.SerializeObject(dataDivisons);
            ICacheServiceMock.Setup(x => x.GetString(It.Is<string>(y => y.Contains("Division")))).Returns(jsonDivision);

            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);


            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(FinanceDbContext)))
                .Returns(dbContext);

            var service = new BudgetCashflowService(serviceProviderMock.Object);
            var formDto = GetDataUtil(service).GetNewData_CashflowCategoryFormDto();

            //Act
            var result = service.CreateBudgetCashflowCategory(formDto);

            //Assert
            Assert.NotEqual(0, result);
        }


        [Fact]
        public void Should_Success_CreateBudgetCashflowType()
        {
            //Arrange
            var dbContext = GetDbContext(GetCurrentAsyncMethod());
            var serviceProviderMock = GetServiceProvider();
            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataCurrencies = new List<CurrencyDto>()
            {
                new CurrencyDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string jsonCurrency = JsonConvert.SerializeObject(dataCurrencies);
            ICacheServiceMock.Setup(x => x.GetString(It.Is<string>(y => y.Contains("Currency")))).Returns(jsonCurrency);

            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string jsonUnit = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(x => x.GetString(It.Is<string>(y => y.Contains("Unit")))).Returns(jsonUnit);


            var dataDivisons = new List<DivisionDto>()
            {
                new DivisionDto()
                {
                    Code="Code"
                }
            };
            string jsonDivision = JsonConvert.SerializeObject(dataDivisons);
            ICacheServiceMock.Setup(x => x.GetString(It.Is<string>(y => y.Contains("Division")))).Returns(jsonDivision);

            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);


            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(FinanceDbContext)))
                .Returns(dbContext);

            var service = new BudgetCashflowService(serviceProviderMock.Object);
            var formDto = GetDataUtil(service).GetNewData_CashflowTypeFormDto();

            //Act
            var result = service.CreateBudgetCashflowType(formDto);

            //Assert
            Assert.NotEqual(0, result);
        }



        [Fact]
        public void Should_Success_CreateBudgetCashflowSubCategory()
        {
            //Arrange
            var dbContext = GetDbContext(GetCurrentAsyncMethod());
            var serviceProviderMock = GetServiceProvider();
            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataCurrencies = new List<CurrencyDto>()
            {
                new CurrencyDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string jsonCurrency = JsonConvert.SerializeObject(dataCurrencies);
            ICacheServiceMock.Setup(x => x.GetString(It.Is<string>(y => y.Contains("Currency")))).Returns(jsonCurrency);

            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string jsonUnit = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(x => x.GetString(It.Is<string>(y => y.Contains("Unit")))).Returns(jsonUnit);


            var dataDivisons = new List<DivisionDto>()
            {
                new DivisionDto()
                {
                    Code="Code"
                }
            };
            string jsonDivision = JsonConvert.SerializeObject(dataDivisons);
            ICacheServiceMock.Setup(x => x.GetString(It.Is<string>(y => y.Contains("Division")))).Returns(jsonDivision);

            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);


            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(FinanceDbContext)))
                .Returns(dbContext);

            var service = new BudgetCashflowService(serviceProviderMock.Object);
            var formDto = GetDataUtil(service).GetNewData_CashflowSubCategoryFormDto();

            //Act
            var result = service.CreateBudgetCashflowSubCategory(formDto);

            //Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public void Should_Success_CreateRealCashBalance()
        {
            //Arrange
            var dbContext = GetDbContext(GetCurrentAsyncMethod());
            var serviceProviderMock = GetServiceProvider();
            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataCurrencies = new List<CurrencyDto>()
            {
                new CurrencyDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string jsonCurrency = JsonConvert.SerializeObject(dataCurrencies);
            ICacheServiceMock.Setup(x => x.GetString(It.Is<string>(y => y.Contains("Currency")))).Returns(jsonCurrency);

            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string jsonUnit = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(x => x.GetString(It.Is<string>(y => y.Contains("Unit")))).Returns(jsonUnit);


            var dataDivisons = new List<DivisionDto>()
            {
                new DivisionDto()
                {
                    Code="Code"
                }
            };
            string jsonDivision = JsonConvert.SerializeObject(dataDivisons);
            ICacheServiceMock.Setup(x => x.GetString(It.Is<string>(y => y.Contains("Division")))).Returns(jsonDivision);

            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);


            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(FinanceDbContext)))
                .Returns(dbContext);

            var service = new BudgetCashflowService(serviceProviderMock.Object);
            var formDto = GetDataUtil(service).GetNewData_CashBalanceFormDto();

            //Act
            var result = service.CreateRealCashBalance(formDto);

            //Assert
            Assert.NotEqual(0, result);
        }


        [Fact]
        public void Should_Success_CreateInitialCashBalance()
        {
            //Arrange
            var dbContext = GetDbContext(GetCurrentAsyncMethod());
            var serviceProviderMock = GetServiceProvider();
            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataCurrencies = new List<CurrencyDto>()
            {
                new CurrencyDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string jsonCurrency = JsonConvert.SerializeObject(dataCurrencies);
            ICacheServiceMock.Setup(x => x.GetString(It.Is<string>(y => y.Contains("Currency")))).Returns(jsonCurrency);

            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string jsonUnit = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(x => x.GetString(It.Is<string>(y => y.Contains("Unit")))).Returns(jsonUnit);


            var dataDivisons = new List<DivisionDto>()
            {
                new DivisionDto()
                {
                    Code="Code"
                }
            };
            string jsonDivision = JsonConvert.SerializeObject(dataDivisons);
            ICacheServiceMock.Setup(x => x.GetString(It.Is<string>(y => y.Contains("Division")))).Returns(jsonDivision);

            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);


            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(FinanceDbContext)))
                .Returns(dbContext);

            var service = new BudgetCashflowService(serviceProviderMock.Object);
            var formDto = GetDataUtil(service).GetNewData_CashBalanceFormDto();

            //Act
            var result = service.CreateInitialCashBalance(formDto);

            //Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public void Should_Success_UpdateInitialCashBalance()
        {
            //Arrange
            var dbContext = GetDbContext(GetCurrentAsyncMethod());
            var serviceProviderMock = GetServiceProvider();
            Mock<ICacheService> ICacheServiceMock = new Mock<ICacheService>();
            var dataCurrencies = new List<CurrencyDto>()
            {
                new CurrencyDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string jsonCurrency = JsonConvert.SerializeObject(dataCurrencies);
            ICacheServiceMock.Setup(x => x.GetString(It.Is<string>(y => y.Contains("Currency")))).Returns(jsonCurrency);

            var dataUnits = new List<UnitDto>()
            {
                new UnitDto()
                {
                    Code="Code"
                }
            };

            //Tranform it to Json object
            string jsonUnit = JsonConvert.SerializeObject(dataUnits);
            ICacheServiceMock.Setup(x => x.GetString(It.Is<string>(y => y.Contains("Unit")))).Returns(jsonUnit);


            var dataDivisons = new List<DivisionDto>()
            {
                new DivisionDto()
                {
                    Code="Code"
                }
            };
            string jsonDivision = JsonConvert.SerializeObject(dataDivisons);
            ICacheServiceMock.Setup(x => x.GetString(It.Is<string>(y => y.Contains("Division")))).Returns(jsonDivision);

            serviceProviderMock
              .Setup(serviceProvider => serviceProvider.GetService(typeof(ICacheService)))
              .Returns(ICacheServiceMock.Object);


            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(FinanceDbContext)))
                .Returns(dbContext);

            var service = new BudgetCashflowService(serviceProviderMock.Object);
            var formDto = GetDataUtil(service).GetNewData_CashBalanceFormDto();

            //Act
            var result = service.UpdateInitialCashBalance(formDto);

            //Assert
            Assert.NotEqual(0, result);
        }


    }
}
