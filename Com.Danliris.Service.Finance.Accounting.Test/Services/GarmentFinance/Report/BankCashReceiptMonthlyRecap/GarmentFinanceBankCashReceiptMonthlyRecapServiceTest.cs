using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentFinance.Reports.BankCashReceiptMonthlyRecap;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.GarmentFinance.BankCashReceipt;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.GarmentFinance.BankCashReceiptDetail;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentFinance.BankCashReceiptDetail;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.GarmentFinance.BankCashReceipt;
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

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.GarmentFinance.Report.BankCashReceiptMonthlyRecap
{
    public class GarmentFinanceBankCashReceiptMonthlyRecapServiceTest
    {
        private const string ENTITY = "BankCashReceiptMonthlyRecapService";
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
            var dbContext = GetDbContext(GetCurrentAsyncMethod());
            var serviceProviderMock = GetServiceProvider();

            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(FinanceDbContext)))
                .Returns(dbContext);

            var bankCashReceiptService = new BankCashReceiptService(serviceProviderMock.Object);
            var bankCashReceiptDataUtil = new BankCashReceiptDataUtil(bankCashReceiptService);
            return new BankCashReceiptDetailDataUtil(service, bankCashReceiptDataUtil);
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

            var dbContext = GetDbContext(GetCurrentAsyncMethod());
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(FinanceDbContext)))
                .Returns(dbContext);

            BankCashReceiptDetailService serviceBankCash = new BankCashReceiptDetailService(serviceProviderMock.Object);
            BankCashReceiptDetailModel cashReceiptDetailModel = await _dataUtilBankCash(serviceBankCash).GetTestData();
            GarmentFinanceBankCashReceiptMonthlyRecapService service = new GarmentFinanceBankCashReceiptMonthlyRecapService(serviceProviderMock.Object, _dbContext(GetCurrentMethod()));


            var response = service.GetMonitoring(DateTimeOffset.Now, DateTimeOffset.Now,  7);
            Assert.NotNull(response);
        }

        [Fact]
        public async Task Should_Success_Get_All_DataExcel()
        {
            var serviceProviderMock = GetServiceProvider();

            var dbContext = GetDbContext(GetCurrentAsyncMethod());
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(FinanceDbContext)))
                .Returns(dbContext);

            BankCashReceiptDetailService serviceBankCash = new BankCashReceiptDetailService(serviceProviderMock.Object);
            BankCashReceiptDetailModel cashReceiptDetailModel = await _dataUtilBankCash(serviceBankCash).GetTestData();
            GarmentFinanceBankCashReceiptMonthlyRecapService service = new GarmentFinanceBankCashReceiptMonthlyRecapService(serviceProviderMock.Object, _dbContext(GetCurrentMethod()));


            var response = service.GenerateExcel(DateTimeOffset.Now, DateTimeOffset.Now, 7);
            Assert.NotNull(response);
        }
    }
}
