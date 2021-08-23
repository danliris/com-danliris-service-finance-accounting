using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.GarmentFinance.BankCashReceiptDetail;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentFinance.BankCashReceiptDetail;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.GarmentFinance.BankCashReceiptDetail;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.GarmentFinance.BankCashReceiptDetail;
using Com.Danliris.Service.Finance.Accounting.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.GarmentFinance.BankCashReceiptDetail
{
    public class BankCashReceiptDetailServiceTest
    {
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
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test", TimezoneOffset = 7 });


            return serviceProvider;
        }

        private BankCashReceiptDetailDataUtil _dataUtil(BankCashReceiptDetailService service, string testname)
        {
            return new BankCashReceiptDetailDataUtil(service);
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {

            //Arrange
            var dbContext = GetDbContext(GetCurrentAsyncMethod());
            var serviceProviderMock = GetServiceProvider();

            var httpClientService = new Mock<IHttpClientService>();

            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService)))
                .Returns(httpClientService.Object);

            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(FinanceDbContext)))
                .Returns(dbContext);

            var service = new BankCashReceiptDetailService(serviceProviderMock.Object);

            var model = _dataUtil(service, GetCurrentAsyncMethod()).GetNewData();
            //Act
            var Response = await service.CreateAsync(model);

            //Assert
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Get_Data()
        {
            //Arrange
            var dbContext = GetDbContext(GetCurrentAsyncMethod());
            var serviceProviderMock = GetServiceProvider();

            var httpClientService = new Mock<IHttpClientService>();

            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService)))
                .Returns(httpClientService.Object);

            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(FinanceDbContext)))
                .Returns(dbContext);

            var service = new BankCashReceiptDetailService(serviceProviderMock.Object);

            var dto = await _dataUtil(service, GetCurrentAsyncMethod()).GetTestData();
            //Act
            var Response = service.Read(1, 25, "{}", null, null, "{}");

            //Assert
            Assert.NotNull(Response);
            Assert.NotEqual(0, Response.Count);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            //Arrange
            var dbContext = GetDbContext(GetCurrentAsyncMethod());
            var serviceProviderMock = GetServiceProvider();

            var httpClientService = new Mock<IHttpClientService>();

            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService)))
                .Returns(httpClientService.Object);

            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(FinanceDbContext)))
                .Returns(dbContext);

            var service = new BankCashReceiptDetailService(serviceProviderMock.Object);

            var dto = await _dataUtil(service, GetCurrentAsyncMethod()).GetTestData();
            //Act
            var Response = service.ReadByIdAsync(dto.Id);

            //Assert
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Delete_Data()
        {
            //Arrange
            var dbContext = GetDbContext(GetCurrentAsyncMethod());
            var serviceProviderMock = GetServiceProvider();

            var httpClientService = new Mock<IHttpClientService>();

            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService)))
                .Returns(httpClientService.Object);

            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(FinanceDbContext)))
                .Returns(dbContext);

            var service = new BankCashReceiptDetailService(serviceProviderMock.Object);

            var model = await _dataUtil(service, GetCurrentAsyncMethod()).GetTestData();
            //Act
            var newModel = await service.ReadByIdAsync(model.Id);
            var Response = await service.DeleteAsync(newModel.Id);

            //Assert
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Update_Data()
        {
            //Arrange
            var dbContext = GetDbContext(GetCurrentAsyncMethod());
            var serviceProviderMock = GetServiceProvider();

            var httpClientService = new Mock<IHttpClientService>();

            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService)))
                .Returns(httpClientService.Object);

            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(FinanceDbContext)))
                .Returns(dbContext);

            var service = new BankCashReceiptDetailService(serviceProviderMock.Object);

            var model = await _dataUtil(service, GetCurrentAsyncMethod()).GetTestData();
            var model2 = await _dataUtil(service, GetCurrentAsyncMethod()).GetTestData();

            BankCashReceiptDetailModel newModel2 = new BankCashReceiptDetailModel();
            newModel2.Id = model2.Id;
            //Act
            var newModel = await service.ReadByIdAsync(model.Id);
            var Response1 = await service.UpdateAsync(newModel.Id, newModel);
            Assert.NotEqual(0, Response1);

            newModel2.Items = new List<BankCashReceiptDetailItemModel> { model2.Items.First() };
            newModel2.OtherItems = new List<BankCashReceiptDetailOtherItemModel> { model2.OtherItems.First() };
            var Response = await service.UpdateAsync(model2.Id, newModel2);
            Assert.NotEqual(0, Response);

            BankCashReceiptDetailItemModel newItem = new BankCashReceiptDetailItemModel
            {
                BankCashReceiptDetailId = 1,
                Amount = 2,
            };

            BankCashReceiptDetailOtherItemModel newOtherItem = new BankCashReceiptDetailOtherItemModel
            {
                BankCashReceiptDetailId = 1,
                Amount = 2,
            };

            newModel2.Items.Add(newItem);
            newModel2.OtherItems.Add(newOtherItem);
            var Response3 = await service.UpdateAsync(model2.Id, newModel2);
            Assert.NotEqual(0, Response);

            // delete item and other item
            BankCashReceiptDetailModel newModel3 = new BankCashReceiptDetailModel();
            newModel3.Id = model2.Id;

            newModel3.Items = new List<BankCashReceiptDetailItemModel>();
            newModel3.OtherItems = new List<BankCashReceiptDetailOtherItemModel>();
            var Response4 = await service.UpdateAsync(model2.Id, newModel3);
            Assert.NotEqual(0, Response4);

        }

        [Fact]
        public void Should_Success_Validate_All_Null_Data()
        {
            BankCashReceiptDetailViewModel vm = new BankCashReceiptDetailViewModel();

            Assert.True(vm.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_Null_Items_Data()
        {
            BankCashReceiptDetailViewModel vm = new BankCashReceiptDetailViewModel();
            vm.Items = new List<BankCashReceiptDetailItemViewModel>
            {
                new BankCashReceiptDetailItemViewModel()
                {
                    Id=0,
                }
            };
            vm.OtherItems = new List<BankCashReceiptDetailOtherItemViewModel>
            {
                new BankCashReceiptDetailOtherItemViewModel()
                {
                    Id=0,
                }
            };

            Assert.True(vm.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_All_With_Data()
        {
            BankCashReceiptDetailViewModel vm = new BankCashReceiptDetailViewModel();
            vm.BankCashReceiptNo = "";
            vm.Items = new List<BankCashReceiptDetailItemViewModel>
            {
                new BankCashReceiptDetailItemViewModel()
                {
                    InvoiceId = 1,
                    InvoiceNo = "",
                    BuyerAgent = new Lib.ViewModels.NewIntegrationViewModel.BuyerViewModel
                    {
                        Code = ""
                    },
                    Currency = new Lib.ViewModels.NewIntegrationViewModel.CurrencyViewModel
                    {
                        Id = 0,
                        Code = "Test"
                    },
                    Amount = -1
                },
                new BankCashReceiptDetailItemViewModel()
                {
                    InvoiceId = 0,
                    InvoiceNo = "TEST",
                    Currency = new Lib.ViewModels.NewIntegrationViewModel.CurrencyViewModel
                    {
                        Id = 1,
                        Code = ""
                    }
                }
            };
            vm.OtherItems = new List<BankCashReceiptDetailOtherItemViewModel>
            {
                new BankCashReceiptDetailOtherItemViewModel()
                {
                    Account = new Lib.ViewModels.NewIntegrationViewModel.ChartOfAccountViewModel
                    {
                        Id = "",
                        Code = "Test"
                    },
                    Currency = new Lib.ViewModels.NewIntegrationViewModel.CurrencyViewModel
                    {
                        Id = 0
                    },
                    Amount = -1
                },
                new BankCashReceiptDetailOtherItemViewModel()
                {
                    Account = new Lib.ViewModels.NewIntegrationViewModel.ChartOfAccountViewModel
                    {
                        Id = "0",
                        Code = "Test"
                    },
                    Currency = new Lib.ViewModels.NewIntegrationViewModel.CurrencyViewModel
                    {
                        Id = 1,
                        Code = ""
                    }
                },
                new BankCashReceiptDetailOtherItemViewModel()
                {
                    Account = new Lib.ViewModels.NewIntegrationViewModel.ChartOfAccountViewModel
                    {
                        Id = "1",
                        Code = ""
                    },
                    Currency = new Lib.ViewModels.NewIntegrationViewModel.CurrencyViewModel
                    {
                        Id = 0,
                        Code = "Test"
                    }
                }
            };

            Assert.True(vm.Validate(null).Count() > 0);
        }
    }
}
