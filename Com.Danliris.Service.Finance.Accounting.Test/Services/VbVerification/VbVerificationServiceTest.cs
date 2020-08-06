using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.CashierApproval;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.RealizationVBNonPO;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.VbNonPORequest;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.VBVerification;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRealizationDocumentExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRealizationDocumentExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CashierApproval;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.CashierApproval;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.RealizationVBNonPO;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.RealizationVBWIthPO;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.VbNonPORequest;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.VbVerification;
using Com.Danliris.Service.Finance.Accounting.Test.Helpers;
using Com.Danliris.Service.Finance.Accounting.Test.Services.OthersExpenditureProofDocument.Helper;
using Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.RealizationVBWIthPO;
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


namespace Com.Danliris.Service.Finance.Accounting.Test.Services.VbVerification
{
    public class VbVerificationServiceTest
    {
        private const string ENTITY = "RealizationVbs";

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

        private Mock<IServiceProvider> GetServiceProviderMock()
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

        protected FinanceDbContext GetDbContext(string testName)
        {
            string databaseName = testName;
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(databaseName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                 .UseInternalServiceProvider(serviceProvider);

            FinanceDbContext DbContex = new FinanceDbContext(optionsBuilder.Options);
            return DbContex;
        }

        private RealizationVbWithPODataUtil _dataUtil(RealizationVbWithPOService service)
        {
            return new RealizationVbWithPODataUtil(service);
        }

        private CashierApprovalDataUtil _dataUtilCashier(CashierApprovalService service)
        {
            return new CashierApprovalDataUtil(service);
        }

        private VbVerificationDataUtil _dataUtil2(VbVerificationService service)
        {
            return new VbVerificationDataUtil(service);
        }

        private VbNonPORequestDataUtil _dataUtil3(VbNonPORequestService service)
        {
            return new VbNonPORequestDataUtil(service);
        }

        [Fact]
        public async Task Read_Return_Success()
        {
            var dbContext = GetDbContext(GetCurrentMethod());

            var serviceProviderMock = GetServiceProvider();
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService))).Returns(new RealizationVbServiceHelper());
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService))).Returns(new HttpClientOthersExpenditureServiceHelper());
            RealizationVbWithPOService service = new RealizationVbWithPOService(dbContext, serviceProviderMock.Object);
            VbVerificationService service2 = new VbVerificationService(dbContext, serviceProviderMock.Object);
            CashierApprovalService service3 = new CashierApprovalService(serviceProviderMock.Object, dbContext);
            VbNonPORequestService service4 = new VbNonPORequestService(dbContext, serviceProviderMock.Object);

            RealizationVbModel model = _dataUtil(service).GetNewData();

            var dataRequestVb = _dataUtil(service).GetDataRequestVB2();
            dbContext.VbRequests.Add(dataRequestVb);
            dbContext.SaveChanges();

            RealizationVbWithPOViewModel viewModel = _dataUtil(service).GetNewViewModel();
            await service.CreateAsync(model, viewModel);

            CashierApprovalViewModel viewModelCashier = _dataUtilCashier(service3).GetDataToValidate();
            await service3.CashierAproval(viewModelCashier);            

            var response = service2.Read(1, 1, "{}", new List<string>(), "", "{}");
            Assert.NotNull(response);

        }

        [Fact]
        public async Task Read_Return_Success2()
        {
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProvider();
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService))).Returns(new RealizationVbServiceHelper());
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService))).Returns(new HttpClientOthersExpenditureServiceHelper());
            RealizationVbWithPOService service = new RealizationVbWithPOService(dbContext, serviceProviderMock.Object);
            VbVerificationService service2 = new VbVerificationService(dbContext, serviceProviderMock.Object);
            RealizationVbModel model = _dataUtil(service).GetNewData();

            var dataRequestVb = _dataUtil(service).GetDataRequestVB();
            dbContext.VbRequests.Add(dataRequestVb);
            dbContext.SaveChanges();

            RealizationVbWithPOViewModel viewModel = _dataUtil(service).GetNewViewModel();

            VbVerificationViewModel viewmodel2 = _dataUtil2(service2).GetViewModelToValidate();

            await service.CreateAsync(model, viewModel);

            await service2.CreateAsync(viewmodel2);
            //
            var response = service2.ReadVerification(1, 1, "{}", new List<string>(), "", "{}");
            Assert.NotNull(response);

        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var dbContext = GetDbContext(GetCurrentMethod());
            RealizationVbWithPOService service = new RealizationVbWithPOService(dbContext, GetServiceProvider().Object);
            VbVerificationService service2 = new VbVerificationService(dbContext, GetServiceProvider().Object);
            RealizationVbModel model = _dataUtil(service).GetNewData();

            var dataRequestVb = _dataUtil(service).GetNewData();
            dbContext.RealizationVbs.Add(dataRequestVb);
            dbContext.SaveChanges();

            VbVerificationViewModel viewModel = _dataUtil2(service2).GetViewModelToValidate();
            var Response = await service2.CreateAsync(viewModel);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Create_Data2()
        {
            var dbContext = GetDbContext(GetCurrentMethod());
            RealizationVbWithPOService service = new RealizationVbWithPOService(dbContext, GetServiceProvider().Object);
            VbVerificationService service2 = new VbVerificationService(dbContext, GetServiceProvider().Object);
            RealizationVbModel model = _dataUtil(service).GetNewData();

            var dataRequestVb = _dataUtil(service).GetNewData();
            dbContext.RealizationVbs.Add(dataRequestVb);
            dbContext.SaveChanges();

            VbVerificationViewModel viewModel = _dataUtil2(service2).GetViewModelToValidate2();
            var Response = await service2.CreateAsync(viewModel);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Read_ById()
        {
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService))).Returns(new RealizationVbServiceHelper());
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService))).Returns(new HttpClientOthersExpenditureServiceHelper());
            var service = new RealizationVbNonPOService(dbContext, serviceProviderMock.Object);
            var service2 = new VbVerificationService(dbContext, serviceProviderMock.Object);
            var dataUtil = new RealizationVBNonPODataUtil(service);
            var dataRequestVb = dataUtil.GetDataRequestVB();
            dbContext.VbRequests.Add(dataRequestVb);
            dbContext.SaveChanges();
            var data = await dataUtil.GetCreatedData();
            var result = await service2.ReadById(data.Id);

            Assert.NotNull(result);
        }

        internal class RealizationVbServiceHelper : IVBRealizationDocumentExpeditionService
        {
            public RealizationVbServiceHelper()
            {
            }

            public Task<int> CashierReceipt(List<int> vbRealizationIds)
            {
                throw new NotImplementedException();
            }

            public Task<VBRealizationDocumentExpeditionReportDto> GetReports(int vbId, int vbRealizationId, string vbRequestName, int unitId, DateTimeOffset dateStart, DateTimeOffset dateEnd, int page = 1, int size = 25)
            {
                throw new NotImplementedException();
            }

            public Task<int> InitializeExpedition(int vbRealizationId)
            {
                //throw new NotImplementedException();

                return Task.FromResult(1);
            }

            public ReadResponse<VBRealizationDocumentExpeditionModel> Read(int page, int size, string order, string keyword, int position)
            {
                throw new NotImplementedException();
            }

            public ReadResponse<RealizationVbModel> ReadRealizationToVerification()
            {
                throw new NotImplementedException();
            }

            public ReadResponse<VBRealizationDocumentExpeditionModel> ReadRealizationToVerification(int vbId, int vbRealizationId, DateTimeOffset? realizationDate, string vbRealizationRequestPerson, int unitId)
            {
                throw new NotImplementedException();
            }

            public List<RealizationVbModel> ReadRelizationToVerification(int position)
            {
                throw new NotImplementedException();
            }

            public Task<int> Reject(int vbRealizationId, string reason)
            {
                throw new NotImplementedException();
            }

            public Task<int> SubmitToVerification(List<int> vbRealizationIds)
            {
                throw new NotImplementedException();
            }

            public Task<int> VerificationDocumentReceipt(List<int> vbRealizationIds)
            {
                throw new NotImplementedException();
            }

            public Task<int> VerifiedToCashier(List<int> vbRealizationIds)
            {
                throw new NotImplementedException();
            }

            public Task<int> VerifiedToCashier(int vbRealizationId)
            {
                throw new NotImplementedException();
            }
        }
    }
}
