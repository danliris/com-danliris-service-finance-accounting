using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.CashierApproval;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.RealizationVBNonPO;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.VbNonPORequest;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.VBVerification;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRealizationDocumentExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.CashierApproval;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.RealizationVBNonPO;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.RealizationVBWIthPO;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.VbNonPORequest;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.VbVerification;
using Com.Danliris.Service.Finance.Accounting.Test.Helpers;
using Com.Danliris.Service.Finance.Accounting.Test.Services.OthersExpenditureProofDocument.Helper;
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

        

        private VbVerificationDataUtil _vbVerificationDataUtil(VbVerificationService service, FinanceDbContext dbContext)
        {
            return new VbVerificationDataUtil(service,dbContext);
        }

      



        [Fact]
        public async Task Read_Return_Success()
        {
            //Setup
            var dbContext = GetDbContext(GetCurrentAsyncMethod());
            var serviceProviderMock = GetServiceProvider();

            var vbVerificationService = new VbVerificationService(dbContext, serviceProviderMock.Object);
          
            var dataRealizationVbModel = await _vbVerificationDataUtil(vbVerificationService, dbContext).GetTestData_RealizationVbModel();
            var dataVbRequestModel = await _vbVerificationDataUtil(vbVerificationService, dbContext).GetTestData_VbRequestModel();

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

            var vbVerificationService = new VbVerificationService(dbContext, serviceProviderMock.Object);

            RealizationVbModel data = await _vbVerificationDataUtil(vbVerificationService, dbContext).GetTestData_RealizationVbModel();

            //Act
            var response = vbVerificationService.ReadVerification(1, 1, "{}", new List<string>(), "", "{}");

            //Assert
            Assert.NotNull(response);
            Assert.True(response.Data.Count > 0);

        }


        [Fact]
        public async Task Should_Success_CreateAsync()
        {
            //Setup
            var dbContext = GetDbContext(GetCurrentAsyncMethod());
          
            VbVerificationService vbVerificationService = new VbVerificationService(dbContext, GetServiceProvider().Object);
          
            VbVerificationViewModel viewModel = _vbVerificationDataUtil(vbVerificationService,dbContext).Get_Verified_VbVerificationViewModel();
            RealizationVbModel data = await _vbVerificationDataUtil(vbVerificationService, dbContext).GetTestData_RealizationVbModel();
            
            //Act
            var Response = await vbVerificationService.CreateAsync(viewModel);

            //Assert
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_CreateAsync_with_DataNotVerified()
        {
            //Setup
            var dbContext = GetDbContext(GetCurrentAsyncMethod());
            
            VbVerificationService vbVerificationService = new VbVerificationService(dbContext, GetServiceProvider().Object);
           
            VbVerificationViewModel viewModel = _vbVerificationDataUtil(vbVerificationService,dbContext).Get_NotVerified_VbVerificationViewModel();
            RealizationVbModel data = await _vbVerificationDataUtil(vbVerificationService, dbContext).GetTestData_RealizationVbModel();

            //Act
            var Response = await vbVerificationService.CreateAsync(viewModel);

            //Assert
            Assert.NotEqual(0, Response);
        }


       

        [Fact]
        public async Task Should_Success_ReadById()
        {
            //Setup
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProvider();

            var vbVerificationService = new VbVerificationService(dbContext, serviceProviderMock.Object);
            RealizationVbModel data = await _vbVerificationDataUtil(vbVerificationService, dbContext).GetTestData_RealizationVbModel();
           
            //Act
            var result = await vbVerificationService.ReadById(data.Id);

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task ReadToVerified_Success()
        {
            //Setup
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProvider();
            var vbVerificationService = new VbVerificationService(dbContext, serviceProviderMock.Object);

            RealizationVbModel data = await _vbVerificationDataUtil(vbVerificationService, dbContext).GetTestData_RealizationVbModel();

            //Act
            var result =  vbVerificationService.ReadToVerified(1,25,"{}",new List<string>(),"","{}");

            //Assert
            Assert.NotNull(result);
        }

       
    }
}
