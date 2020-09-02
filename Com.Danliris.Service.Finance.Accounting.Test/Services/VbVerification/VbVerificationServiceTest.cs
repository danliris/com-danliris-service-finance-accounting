using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.CashierApproval;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.RealizationVBNonPO;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.VbNonPORequest;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.VBVerification;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRealizationDocumentExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRealizationDocument;
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

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test", TimezoneOffset = 7 });
            Mock<IVBRealizationDocumentExpeditionService> IVBRealizationDocumentExpeditionServiceMock = new Mock<IVBRealizationDocumentExpeditionService>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IVBRealizationDocumentExpeditionService)))
                .Returns(IVBRealizationDocumentExpeditionServiceMock.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientOthersExpenditureServiceHelper());

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

        private RealizationVbWithPODataUtil _realizationVbWithPODataUtil(RealizationVbWithPOService service)
        {
            return new RealizationVbWithPODataUtil(service);
        }

        private RealizationVBNonPODataUtil _realizationVBNonPODataUtil(RealizationVbNonPOService service)
        {
            return new RealizationVBNonPODataUtil(service);
        }

        private CashierApprovalDataUtil _cashierApprovalDataUtil(CashierApprovalService service)
        {
            return new CashierApprovalDataUtil(service);
        }

        private VbVerificationDataUtil _vbVerificationDataUtil(VbVerificationService service)
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
            //Setup
            var dbContext = GetDbContext(GetCurrentAsyncMethod());
            var serviceProviderMock = GetServiceProvider();
          
            var realizationVbWithPOService = new RealizationVbWithPOService(dbContext, serviceProviderMock.Object);
            var vbVerificationService = new VbVerificationService(dbContext, serviceProviderMock.Object);
            var cashierApprovalService = new CashierApprovalService(serviceProviderMock.Object, dbContext);
          

            var vbRequestModel = _realizationVbWithPODataUtil(realizationVbWithPOService).GetVbRequestModel();
            dbContext.VbRequests.Add(vbRequestModel);
            dbContext.SaveChanges();

            var realizationVbModel = _realizationVbWithPODataUtil(realizationVbWithPOService).GetNewData();
            var realizationVbWithPOViewModel = _realizationVbWithPODataUtil(realizationVbWithPOService).GetNewViewModel();

            await realizationVbWithPOService.CreateAsync(realizationVbModel, realizationVbWithPOViewModel);

            var cashierApprovalViewModel = _cashierApprovalDataUtil(cashierApprovalService).GetDataToValidate();
            await cashierApprovalService.CashierAproval(cashierApprovalViewModel);

            //Act
            var response = vbVerificationService.Read(1, 1, "{}", new List<string>(), "", "{}");

            //Assert
            Assert.NotNull(response);

        }

       


        [Fact]
        public async Task ReadVerification_Return_Success()
        {
            //Setup
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProvider();

            RealizationVbWithPOService realizationVbWithPOService = new RealizationVbWithPOService(dbContext, serviceProviderMock.Object);
            VbVerificationService vbVerificationService = new VbVerificationService(dbContext, serviceProviderMock.Object);
            RealizationVbModel realizationVbModel = _realizationVbWithPODataUtil(realizationVbWithPOService).GetNewData();

            var vbRequestModel = _realizationVbWithPODataUtil(realizationVbWithPOService).GetDataRequestVB();
            dbContext.VbRequests.Add(vbRequestModel);
            dbContext.SaveChanges();

            RealizationVbWithPOViewModel realizationVbWithPOViewModel = _realizationVbWithPODataUtil(realizationVbWithPOService).GetNewViewModel();
            VbVerificationViewModel vbVerificationViewModel = _vbVerificationDataUtil(vbVerificationService).GetVbVerificationViewModel();

            await realizationVbWithPOService.CreateAsync(realizationVbModel, realizationVbWithPOViewModel);
            await vbVerificationService.CreateAsync(vbVerificationViewModel);

            //Act
            var response = vbVerificationService.ReadVerification(1, 1, "{}", new List<string>(), "", "{}");

            //Assert
            Assert.NotNull(response);
            Assert.True(response.Data.Count > 0);

        }


        [Fact]
        public async Task Should_Success_Create_Data()
        {
            //Setup
            var dbContext = GetDbContext(GetCurrentAsyncMethod());
            RealizationVbWithPOService service = new RealizationVbWithPOService(dbContext, GetServiceProvider().Object);

            VbVerificationService vbVerificationService = new VbVerificationService(dbContext, GetServiceProvider().Object);
            RealizationVbModel dataRealizationVb = _realizationVbWithPODataUtil(service).GetNewData();

            dbContext.RealizationVbs.Add(dataRealizationVb);
            dbContext.SaveChanges();

            VbVerificationViewModel viewModel = _vbVerificationDataUtil(vbVerificationService).GetVbVerificationViewModel();

            //Act
            var Response = await vbVerificationService.CreateAsync(viewModel);

            //Assert
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Create_Data_with_DataIsVerified()
        {
            //Setup
            var dbContext = GetDbContext(GetCurrentAsyncMethod());
            RealizationVbWithPOService service = new RealizationVbWithPOService(dbContext, GetServiceProvider().Object);

            VbVerificationService vbVerificationService = new VbVerificationService(dbContext, GetServiceProvider().Object);
            RealizationVbModel dataRealizationVb = _realizationVbWithPODataUtil(service).GetNewData();

            dbContext.RealizationVbs.Add(dataRealizationVb);
            dbContext.SaveChanges();

            VbVerificationViewModel viewModel = _vbVerificationDataUtil(vbVerificationService).GetVbVerificationViewModelNotVerified();
            
            //Act
            var Response = await vbVerificationService.CreateAsync(viewModel);

            //Assert
            Assert.NotEqual(0, Response);
        }


        [Fact]
        public async Task Should_Success_Read_ById()
        {
           //Setup
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProvider();

            var realizationVbNonPOService = new RealizationVbNonPOService(dbContext, serviceProviderMock.Object);
            var vbVerificationService = new VbVerificationService(dbContext, serviceProviderMock.Object);

            var dataRequestVb = _realizationVBNonPODataUtil(realizationVbNonPOService).GetDataRequestVB();
            dbContext.VbRequests.Add(dataRequestVb);
            dbContext.SaveChanges();

            var data = await _realizationVBNonPODataUtil(realizationVbNonPOService).GetCreatedData();

            //Act
            var result = await vbVerificationService.ReadById(data.Id);

            //Assert
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

            public Task<VBRealizationDocumentExpeditionReportDto> GetReports(int vbId, int vbRealizationId, string vbRequestName, int unitId, int divisionId, DateTimeOffset dateStart, DateTimeOffset dateEnd, int page = 1, int size = 25)
            {
                throw new NotImplementedException();
            }

            public Task<VBRealizationDocumentExpeditionReportDto> GetReports(int vbId, int vbRealizationId, string vbRequestName, int unitId, int divisionId, DateTimeOffset dateStart, DateTimeOffset dateEnd, string status, int page = 1, int size = 25)
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

            public ReadResponse<VBRealizationDocumentExpeditionModel> Read(int page, int size, string order, string keyword, int position, int vbId, int vbRealizationId, DateTimeOffset? realizationDate, string vbRealizationRequestPerson, int unitId)
            {
                throw new NotImplementedException();
            }

            public ReadResponse<VBRealizationDocumentExpeditionModel> Read(int page, int size, string order, string keyword, VBRealizationPosition position, int vbId, int vbRealizationId, DateTimeOffset? realizationDate, string vbRealizationRequestPerson, int unitId)
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

            public ReadResponse<VBRealizationDocumentExpeditionModel> ReadVerification(int page, int size, string order, string keyword, VBRealizationPosition position, int vbId, int vbRealizationId, DateTimeOffset? realizationDate, string vbRealizationRequestPerson, int unitId)
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

            public Task<int> UpdateExpeditionByRealizationId(int vbRealizationId)
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

            ReadResponse<VBRealizationDocumentModel> IVBRealizationDocumentExpeditionService.ReadRealizationToVerification(int vbId, int vbRealizationId, DateTimeOffset? realizationDate, string vbRealizationRequestPerson, int unitId)
            {
                throw new NotImplementedException();
            }
        }
    }
}
