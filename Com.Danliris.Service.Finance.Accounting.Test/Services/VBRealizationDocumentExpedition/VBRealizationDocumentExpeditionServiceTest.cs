using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRealizationDocumentExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRealizationDocumentExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.VBRealizationDocumentExpedition;
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

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.VBRealizationDocumentExpedition
{
    public class VBRealizationDocumentExpeditionServiceTest
    {
        private const string ENTITY = "VBRealizationDocumentExpeditionService";
        [MethodImpl(MethodImplOptions.NoInlining)]
        protected string GetCurrentMethod()
        {

            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }

        protected string GetCurrentAsyncMethod([CallerMemberName] string methodName = "")
        {
            var method = new StackTrace()
                .GetFrames()
                .Select(frame => frame.GetMethod())
                .FirstOrDefault(item => item.Name == methodName);

            return method.Name;

        }
        private FinanceDbContext _dbContext(string testName)
        {
            var serviceProvider = new ServiceCollection()
              .AddEntityFrameworkInMemoryDatabase()
              .BuildServiceProvider();

            DbContextOptionsBuilder<FinanceDbContext> optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                 .UseInternalServiceProvider(serviceProvider);

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

        private VBRealizationDocumentExpeditionDataUtil _dataUtil(VBRealizationDocumentExpeditionService service, FinanceDbContext financeDbContext)
        {
            return new VBRealizationDocumentExpeditionDataUtil(service,financeDbContext);
        }

        [Fact]
        public async Task  CashierReceipt_Return_Success()
        {
            FinanceDbContext dbContext = _dbContext(GetCurrentMethod());

            VBRealizationDocumentExpeditionService service = new  VBRealizationDocumentExpeditionService(dbContext, GetServiceProvider().Object);
            VBRealizationDocumentExpeditionModel model = _dataUtil(service, dbContext).GetTestData_VBRealizationDocumentExpedition();
          
            var data = model.Id + 1;
            int result = await service.CashierReceipt(new List<int>() { data });
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task InitializeExpedition_Return_Success()
        {
            FinanceDbContext dbContext = _dbContext(GetCurrentMethod());

            VBRealizationDocumentExpeditionService service = new VBRealizationDocumentExpeditionService(dbContext, GetServiceProvider().Object);
            RealizationVbModel model = _dataUtil(service, dbContext).GetTestData_RealizationVbs();

            int result = await service.InitializeExpedition(model.Id);
            Assert.NotEqual(0, result);
        }


        [Fact]
        public void Read_Return_Success()
        {
            FinanceDbContext dbContext = _dbContext(GetCurrentMethod());

            VBRealizationDocumentExpeditionService service = new VBRealizationDocumentExpeditionService(dbContext, GetServiceProvider().Object);

            var  result =  service.Read(1,1,"{}","",1);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Reject_Return_Success()
        {
            FinanceDbContext dbContext = _dbContext(GetCurrentMethod());

            VBRealizationDocumentExpeditionService service = new VBRealizationDocumentExpeditionService(dbContext, GetServiceProvider().Object);
            VBRealizationDocumentExpeditionModel model = _dataUtil(service, dbContext).GetTestData_VBRealizationDocumentExpedition();
            int result = await service.Reject(model.Id, "reason");
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task SubmitToVerification_Return_Success()
        {
            FinanceDbContext dbContext = _dbContext(GetCurrentMethod());

            VBRealizationDocumentExpeditionService service = new VBRealizationDocumentExpeditionService(dbContext, GetServiceProvider().Object);
            VBRealizationDocumentExpeditionModel model = _dataUtil(service, dbContext).GetTestData_VBRealizationDocumentExpedition();
            var data = model.Id + 1;
            int result = await service.SubmitToVerification(new List<int>() { data });
            Assert.Equal(0, result);
        }


        [Fact]
        public async Task VerifiedToCashier_Return_Success()
        {
            FinanceDbContext dbContext = _dbContext(GetCurrentMethod());

            VBRealizationDocumentExpeditionService service = new VBRealizationDocumentExpeditionService(dbContext, GetServiceProvider().Object);
            VBRealizationDocumentExpeditionModel model = _dataUtil(service, dbContext).GetTestData_VBRealizationDocumentExpedition();
            var data = model.Id + 1;
            int result = await service.VerificationDocumentReceipt(new List<int>() { data });
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task VerificationDocumentReceipt_Return_Success()
        {
            FinanceDbContext dbContext = _dbContext(GetCurrentMethod());

            VBRealizationDocumentExpeditionService service = new VBRealizationDocumentExpeditionService(dbContext, GetServiceProvider().Object);
            VBRealizationDocumentExpeditionModel model = _dataUtil(service, dbContext).GetTestData_VBRealizationDocumentExpedition();
            var data = model.Id + 1;
            int result = await service.VerificationDocumentReceipt(new List<int>() { data });
            Assert.Equal(0, result);
        }
        

        [Fact]
        public void  ReadRelizationToVerification_Return_Success()
        {
            FinanceDbContext dbContext = _dbContext(GetCurrentMethod());

            VBRealizationDocumentExpeditionService service = new VBRealizationDocumentExpeditionService(dbContext, GetServiceProvider().Object);
            RealizationVbModel model = _dataUtil(service, dbContext).GetTestData_RealizationVbs();
            List<RealizationVbModel> result =  service.ReadRelizationToVerification(model.Id);
            Assert.NotNull(result);
        }

    }
}
