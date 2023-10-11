//using Com.Danliris.Service.Finance.Accounting.Lib;
//using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.DPPVATBankExpenditureNote;
//using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentFinance.Reports.ValasDPPVATBankExpenditureNoteMonthlyRecap;
//using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
//using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
//using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.DPPVATBankExpenditureNote;
//using Com.Danliris.Service.Finance.Accounting.Test.Helpers;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Diagnostics;
//using Microsoft.Extensions.DependencyInjection;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Threading.Tasks;
//using Xunit;

//namespace Com.Danliris.Service.Finance.Accounting.Test.Services.GarmentFinance.Report.LocalDPPVATBankExpenditureNoteMonthlyRecap
//{
//    public class ValasDPPVATBankExpenditureNoteMonthlyRecapServiceTest
//    {
//        private string GetCurrentAsyncMethod([CallerMemberName] string methodName = "")
//        {
//            var method = new StackTrace()
//                .GetFrames()
//                .Select(frame => frame.GetMethod())
//                .FirstOrDefault(item => item.Name == methodName);

//            return method.Name;

//        }

//        private FinanceDbContext GetDbContext(string testName)
//        {
//            var serviceProvider = new ServiceCollection()
//                .AddEntityFrameworkInMemoryDatabase()
//                .BuildServiceProvider();

//            var optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();
//            optionsBuilder
//                .UseInMemoryDatabase(testName)
//                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
//                .UseInternalServiceProvider(serviceProvider);

//            return new FinanceDbContext(optionsBuilder.Options);
//        }

//        private Mock<IServiceProvider> GetServiceProvider()
//        {
//            var serviceProvider = new Mock<IServiceProvider>();

//            serviceProvider
//                .Setup(x => x.GetService(typeof(IHttpClientService)))
//                .Returns(new HttpClientTestService());

//            serviceProvider
//                .Setup(x => x.GetService(typeof(IIdentityService)))
//                .Returns(new IdentityService() { Token = "Token", Username = "Test", TimezoneOffset = 7 });


//            return serviceProvider;
//        }

//        private DPPVATBankExpenditureNoteDataUtil _dataUtil(DPPVATBankExpenditureNoteService service, string testname)
//        {
//            var dbContext = GetDbContext(GetCurrentAsyncMethod());
//            var serviceProviderMock = GetServiceProvider();

//            serviceProviderMock
//                .Setup(serviceProvider => serviceProvider.GetService(typeof(FinanceDbContext)))
//                .Returns(dbContext);

//            var dppvatexpenditurenoteService = new DPPVATBankExpenditureNoteService(serviceProviderMock.Object);
//            var dppvatexpenditurenoteServiceDataUtil = new DPPVATBankExpenditureNoteDataUtil(dppvatexpenditurenoteService);
//            return new DPPVATBankExpenditureNoteDataUtil(service);
//        }

//        private DPPVATBankExpenditureNoteDataUtil _dataUtilReceipt(DPPVATBankExpenditureNoteService service, string testname)
//        {
//            return new DPPVATBankExpenditureNoteDataUtil(service);
//        }

//        [Fact]
//        public async Task Should_Success_Get_All_Data()
//        {
//            var dbContext = GetDbContext(GetCurrentAsyncMethod());
//            var serviceProviderMock = GetServiceProvider();

//            var httpClientService = new Mock<IHttpClientService>();

//            serviceProviderMock
//                .Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService)))
//                .Returns(httpClientService.Object);

//            serviceProviderMock
//                .Setup(serviceProvider => serviceProvider.GetService(typeof(FinanceDbContext)))
//                .Returns(dbContext);

//            var service = new DPPVATBankExpenditureNoteService(serviceProviderMock.Object);

//            var dto = await _dataUtil(service, GetCurrentAsyncMethod()).GetTestData();
//            ValasDPPVATBankExpenditureNoteMonthlyRecapService serviceReport = new ValasDPPVATBankExpenditureNoteMonthlyRecapService(serviceProviderMock.Object, dbContext);

//            var response = serviceReport.GetMonitoring(DateTime.Now.AddDays(-3), DateTime.Now.AddDays(3), 7);
//            Assert.NotNull(response);
//        }

//        [Fact]
//        public async Task Should_Success_Get_All_Data_Null_Date()
//        {
//            var dbContext = GetDbContext(GetCurrentAsyncMethod());
//            var serviceProviderMock = GetServiceProvider();

//            var httpClientService = new Mock<IHttpClientService>();

//            serviceProviderMock
//                .Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService)))
//                .Returns(httpClientService.Object);

//            serviceProviderMock
//                .Setup(serviceProvider => serviceProvider.GetService(typeof(FinanceDbContext)))
//                .Returns(dbContext);

//            var service = new DPPVATBankExpenditureNoteService(serviceProviderMock.Object);

//            var dto = await _dataUtil(service, GetCurrentAsyncMethod()).GetTestData();
//            ValasDPPVATBankExpenditureNoteMonthlyRecapService serviceReport = new ValasDPPVATBankExpenditureNoteMonthlyRecapService(serviceProviderMock.Object, dbContext);

//            var response = serviceReport.GetMonitoring(null, null, 7);
//            Assert.NotNull(response);
//        }

//        [Fact]
//        public async Task Should_Success_Get_All_DataExcel()
//        {
//            var dbContext = GetDbContext(GetCurrentAsyncMethod());
//            var serviceProviderMock = GetServiceProvider();

//            var httpClientService = new Mock<IHttpClientService>();

//            serviceProviderMock
//                .Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService)))
//                .Returns(httpClientService.Object);

//            serviceProviderMock
//                .Setup(serviceProvider => serviceProvider.GetService(typeof(FinanceDbContext)))
//                .Returns(dbContext);

//            var service = new DPPVATBankExpenditureNoteService(serviceProviderMock.Object);

//            var dto = await _dataUtil(service, GetCurrentAsyncMethod()).GetTestData();
//            ValasDPPVATBankExpenditureNoteMonthlyRecapService serviceReport = new ValasDPPVATBankExpenditureNoteMonthlyRecapService(serviceProviderMock.Object, dbContext);

//            var response = serviceReport.GenerateExcel(DateTime.Now.AddDays(-3), DateTime.Now.AddDays(3), 7);
//            Assert.NotNull(response);
//        }

//        [Fact]
//        public async Task Should_Success_Get_All_DataExcel_ZeroData()
//        {
//            var dbContext = GetDbContext(GetCurrentAsyncMethod());
//            var serviceProviderMock = GetServiceProvider();

//            var httpClientService = new Mock<IHttpClientService>();

//            serviceProviderMock
//                .Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService)))
//                .Returns(httpClientService.Object);

//            serviceProviderMock
//                .Setup(serviceProvider => serviceProvider.GetService(typeof(FinanceDbContext)))
//                .Returns(dbContext);

//            var service = new DPPVATBankExpenditureNoteService(serviceProviderMock.Object);

//            var dto = await _dataUtil(service, GetCurrentAsyncMethod()).GetTestData();
//            ValasDPPVATBankExpenditureNoteMonthlyRecapService serviceReport = new ValasDPPVATBankExpenditureNoteMonthlyRecapService(serviceProviderMock.Object, dbContext);

//            var response = serviceReport.GenerateExcel(DateTime.Now.AddDays(-3), DateTime.Now.AddDays(-3), 7);
//            Assert.NotNull(response);
//        }

//        [Fact]
//        public async Task Should_Success_Get_Detail_All_DataExcel()
//        {
//            var dbContext = GetDbContext(GetCurrentAsyncMethod());
//            var serviceProviderMock = GetServiceProvider();

//            var httpClientService = new Mock<IHttpClientService>();

//            serviceProviderMock
//                .Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService)))
//                .Returns(httpClientService.Object);

//            serviceProviderMock
//                .Setup(serviceProvider => serviceProvider.GetService(typeof(FinanceDbContext)))
//                .Returns(dbContext);

//            var service = new DPPVATBankExpenditureNoteService(serviceProviderMock.Object);

//            var dto = await _dataUtil(service, GetCurrentAsyncMethod()).GetTestData();
//            ValasDPPVATBankExpenditureNoteMonthlyRecapService serviceReport = new ValasDPPVATBankExpenditureNoteMonthlyRecapService(serviceProviderMock.Object, dbContext);

//            var response = serviceReport.GenerateDetailExcel(DateTime.Now.AddDays(-3), DateTime.Now.AddDays(3), 7);
//            Assert.NotNull(response);
//        }

//        [Fact]
//        public async Task Should_Success_Get_Detail_All_DataExcel_ZeroData()
//        {
//            var dbContext = GetDbContext(GetCurrentAsyncMethod());
//            var serviceProviderMock = GetServiceProvider();

//            var httpClientService = new Mock<IHttpClientService>();

//            serviceProviderMock
//                .Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService)))
//                .Returns(httpClientService.Object);

//            serviceProviderMock
//                .Setup(serviceProvider => serviceProvider.GetService(typeof(FinanceDbContext)))
//                .Returns(dbContext);

//            var service = new DPPVATBankExpenditureNoteService(serviceProviderMock.Object);

//            var dto = await _dataUtil(service, GetCurrentAsyncMethod()).GetTestData();
//            ValasDPPVATBankExpenditureNoteMonthlyRecapService serviceReport = new ValasDPPVATBankExpenditureNoteMonthlyRecapService(serviceProviderMock.Object, dbContext);

//            var response = serviceReport.GenerateDetailExcel(DateTime.Now.AddDays(-3), DateTime.Now.AddDays(-3), 7);
//            Assert.NotNull(response);
//        }
//    }
//}
