using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentFinance.Memorial;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentFinance.MemorialDetail;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Reports;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Reports.ExportSalesDebtorReport;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.GarmentFinance.BankCashReceiptDetail;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentFinance.BankCashReceiptDetail;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentFinance.MemorialDetail;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.CacheService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.Rreports.ExportSalesDebtorReportController;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.GarmentFinance.BankCashReceiptDetail;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.GarmentFinance.Memorial;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.GarmentFinance.MemorialDetail;
using Com.Danliris.Service.Finance.Accounting.Test.Helpers;
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
using static Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Reports.ExportSalesDebtorReport.GarmentShippingPackingList;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.Reports.ExportSalesDebtorReport
{
    public class ExportSalesDebtorReportSeviceTest
    {
        private const string ENTITY = "ExportSalesDebtorReportService";
        //private PurchasingDocumentAcceptanceDataUtil pdaDataUtil;
        //private readonly IIdentityService identityService;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }

        private FinanceDbContext _dbContext(string testName)
        {
            DbContextOptionsBuilder<FinanceDbContext> optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            FinanceDbContext dbContext = new FinanceDbContext(optionsBuilder.Options);

            return dbContext;
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


        private BankCashReceiptDetailDataUtil _dataUtilBankCash(BankCashReceiptDetailService service)
        {
            return new BankCashReceiptDetailDataUtil(service);
        }
        private GarmentFinanceMemorialDetailDataUtil _dataUtilMemo(GarmentFinanceMemorialDetailService service, string testname)
        {
            var memorialService = new GarmentFinanceMemorialService(GetServiceProvider().Object, service.DbContext);
            var memorialDataUtil = new GarmentFinanceMemorialDataUtil(memorialService);
            return new GarmentFinanceMemorialDetailDataUtil(service, memorialDataUtil);
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
        private string GetCurrentAsyncMethod([CallerMemberName] string methodName = "")
        {
            var method = new StackTrace()
                .GetFrames()
                .Select(frame => frame.GetMethod())
                .FirstOrDefault(item => item.Name == methodName);

            return method.Name;

        }
        
        [Fact]
        public async Task Should_Success_Get_All_Data()
        {
            var serviceProviderMock = GetServiceProvider();

            var httpClientService = new Mock<IHttpClientService>();

            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService)))
                .Returns(httpClientService.Object);

            var dbContext = GetDbContext(GetCurrentAsyncMethod());
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(FinanceDbContext)))
                .Returns(dbContext);
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test", TimezoneOffset = 7 });
            
            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"amount\":1000,\"buyerCodeAgent\":\"buyerCode\",\"buyerNameAgent\":\"buyer\",\"truckingDate\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"amount\",\"buyerCodeAgent\",\"truckingDate\"]}}");

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-shipping/invoices/exportSalesDebtor"))))
                .ReturnsAsync(message);
            

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(httpClientService.Object);

            GarmentFinanceMemorialDetailService serviceMemo = new  GarmentFinanceMemorialDetailService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            GarmentFinanceMemorialDetailModel modelMemo =  _dataUtilMemo(serviceMemo, GetCurrentMethod()).GetNewData();
            BankCashReceiptDetailService serviceBankCash = new BankCashReceiptDetailService(serviceProviderMock.Object);
            BankCashReceiptDetailModel cashReceiptDetailModel = await _dataUtilBankCash(serviceBankCash).GetTestData();
            ExportSalesDebtorReportService service = new ExportSalesDebtorReportService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
 
            var response = service.GetMonitoring(DateTimeOffset.Now.Month,DateTimeOffset.Now.Year,7);
            Assert.NotNull(response);
        }

        [Fact]
        public async Task Should_Success_Get_All_DataExcel()
        {
            var serviceProviderMock = GetServiceProvider();

            var httpClientService = new Mock<IHttpClientService>();

            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService)))
                .Returns(httpClientService.Object);

            var dbContext = GetDbContext(GetCurrentAsyncMethod());
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(FinanceDbContext)))
                .Returns(dbContext);
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test", TimezoneOffset = 7 });

            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"amount\":1000,\"buyerCodeAgent\":\"buyerCode\",\"buyerNameAgent\":\"buyer\",\"truckingDate\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"amount\",\"buyerCodeAgent\",\"truckingDate\"]}}");

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("garment-shipping/invoices/exportSalesDebtor"))))
                .ReturnsAsync(message);


            serviceProviderMock
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(httpClientService.Object);

            GarmentFinanceMemorialDetailService serviceMemo = new GarmentFinanceMemorialDetailService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            GarmentFinanceMemorialDetailModel modelMemo = _dataUtilMemo(serviceMemo, GetCurrentMethod()).GetNewData();
            BankCashReceiptDetailService serviceBankCash = new BankCashReceiptDetailService(serviceProviderMock.Object);
            BankCashReceiptDetailModel cashReceiptDetailModel = await _dataUtilBankCash(serviceBankCash).GetTestData();
            ExportSalesDebtorReportService service = new ExportSalesDebtorReportService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var response = service.GenerateExcel(DateTimeOffset.Now.Month, DateTimeOffset.Now.Year);
            Assert.NotNull(response);
        }

    }
}
