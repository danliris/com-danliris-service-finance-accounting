using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRequestDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRequestDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.VBRequestDocument;
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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.VBRequestDocument
{
  public  class VBRequestDocumentServiceTest
    {
        private const string ENTITY = "VBRequestDocuments";
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

        private VBRequestDocumentDataUtil _dataUtil(VBRequestDocumentService service, FinanceDbContext financeDbContext)
        {
            return new VBRequestDocumentDataUtil(service, financeDbContext);
        }

        [Fact]
        public async Task CreateNonPO_Return_Success()
        {
            //Setup
            FinanceDbContext dbContext = _dbContext(GetCurrentAsyncMethod());

            VBRequestDocumentService service = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            VBRequestDocumentNonPOFormDto newForm = _dataUtil(service, dbContext).GetNewData_VBRequestDocumentNonPOFormDto();
            VBRequestDocumentModel testData =  _dataUtil(service, dbContext).GetTestData_VBRequestDocumentNonPO();

            //Act
            int result = await service.CreateNonPO(newForm);

            //Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task CreateNonPO_Throw_Exception()
        {
            //Setup
            FinanceDbContext dbContext = _dbContext(GetCurrentAsyncMethod());

            VBRequestDocumentService service = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            VBRequestDocumentNonPOFormDto newForm = _dataUtil(service, dbContext).GetNewData_VBRequestDocumentNonPOFormDto();
            newForm.Items = null;

            //Assert
            await Assert.ThrowsAsync<System.ArgumentNullException>(()=>service.CreateNonPO(newForm));
        }


        [Fact]
        public void CreateWithPO_Return_Success()
        {
            //Setup
            FinanceDbContext dbContext = _dbContext(GetCurrentAsyncMethod());

            VBRequestDocumentService service = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            VBRequestDocumentWithPOFormDto newForm = _dataUtil(service, dbContext).GetNewData_VBRequestDocumentWithPOFormDto();
            VBRequestDocumentModel testData = _dataUtil(service, dbContext).GetTestData_VBRequestDocumentNonPO();

            //Act
            int result =  service.CreateWithPO(newForm);

            //Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task DeleteNonPO_Return_Success()
        {
            //Setup
            FinanceDbContext dbContext = _dbContext(GetCurrentAsyncMethod());

            VBRequestDocumentService service = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            VBRequestDocumentItemModel testData = _dataUtil(service, dbContext).GetTestData_VBRequestDocumentItem();

            //Act
            int result = await service.DeleteNonPO(testData.Id);

            //Assert
            Assert.NotEqual(0, result);
        }


        [Fact]
        public void DeleteWithPO_Return_Success()
        {
            //Setup
            FinanceDbContext dbContext = _dbContext(GetCurrentAsyncMethod());

            VBRequestDocumentService service = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            VBRequestDocumentItemModel testData = _dataUtil(service, dbContext).GetTestData_VBRequestDocumentItem();

            //Act
            int result =  service.DeleteWithPO(testData.Id);

            //Assert
            Assert.NotEqual(0, result);
        }


        [Fact]
        public void Get_Return_Success()
        {
            //Setup
            FinanceDbContext dbContext = _dbContext(GetCurrentAsyncMethod());

            VBRequestDocumentService service = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            VBRequestDocumentItemModel testData = _dataUtil(service, dbContext).GetTestData_VBRequestDocumentItem();

            var orderData = new
            {
                DocumentNo = "desc"
            };
            string order =JsonConvert.SerializeObject(orderData);

            //Act
            var result = service.Get(1,1, order,new List<string>(),"","{}");

            //Assert
            Assert.NotNull( result);
            Assert.True(0 < result.Data.Count());
        }

        [Fact]
        public void GetNonPOById_Return_Success()
        {
            //Setup
            FinanceDbContext dbContext = _dbContext(GetCurrentAsyncMethod());

            VBRequestDocumentService service = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            VBRequestDocumentItemModel testData = _dataUtil(service, dbContext).GetTestData_VBRequestDocumentItem();

            //Act
            var result = service.GetNonPOById(testData.Id);

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetNonPOById_Return_Null()
        {
            //Setup
            FinanceDbContext dbContext = _dbContext(GetCurrentAsyncMethod());

            VBRequestDocumentService service = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            VBRequestDocumentItemModel testData = _dataUtil(service, dbContext).GetTestData_VBRequestDocumentItem();

            //Act
            var result =await service.GetNonPOById(testData.Id+1);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetWithPOById_Return_Success()
        {
            //Setup
            FinanceDbContext dbContext = _dbContext(GetCurrentAsyncMethod());

            VBRequestDocumentService service = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            VBRequestDocumentItemModel testData = _dataUtil(service, dbContext).GetTestData_VBRequestDocumentItem();

            //Act
            var result =  service.GetWithPOById(testData.Id );

            //Assert
            Assert.NotNull(result);
            Assert.NotEqual(0,result.Id);
        }

        [Fact]
        public async Task UpdateNonPO_Return_Null()
        {
            //Setup
            FinanceDbContext dbContext = _dbContext(GetCurrentAsyncMethod());

            VBRequestDocumentService service = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            VBRequestDocumentNonPOFormDto newForm = _dataUtil(service, dbContext).GetNewData_VBRequestDocumentNonPOFormDto();
            VBRequestDocumentItemModel testData = _dataUtil(service, dbContext).GetTestData_VBRequestDocumentItem();

            //Act
            int result = await service.UpdateNonPO(testData.Id, newForm);

            //Assert
            Assert.NotEqual(0,result);
        }

        [Fact]
        public void UpdateWithPO_Return_Succes()
        {
            //Setup
            FinanceDbContext dbContext = _dbContext(GetCurrentMethod());
            
            VBRequestDocumentService service = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            VBRequestDocumentEPODetailModel testData = _dataUtil(service, dbContext).GetTestData_VBRequestDocumentEPODetail();
            VBRequestDocumentWithPOFormDto newForm = _dataUtil(service, dbContext).GetNewData_VBRequestDocumentWithPOFormDto();
            
            //Act
            int result =  service.UpdateWithPO(testData.Id, newForm);

            //Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public void GetNotApprovedData_Return_Succes()
        {
            //Setup
            FinanceDbContext dbContext = _dbContext(GetCurrentMethod());

            VBRequestDocumentService service = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            VBRequestDocumentModel testData = _dataUtil(service, dbContext).GetTestData_VBRequestDocumentWithPO();

            //Act
            var result = service.GetNotApprovedData((int)VBType.WithPO, testData.Id,testData.SuppliantUnitId,DateTime.Now,"{}");

            //Assert
            Assert.NotNull(result);
            Assert.True(0 <result.Count());
        }

        [Fact]
        public async Task ApproveData_Return_Succes()
        {
            //Setup
            FinanceDbContext dbContext = _dbContext(GetCurrentAsyncMethod());

            VBRequestDocumentService service = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            VBRequestDocumentModel testData = _dataUtil(service, dbContext).GetTestData_VBRequestDocumentWithPO();

            ApprovalVBFormDto approvalVBFormDto = new ApprovalVBFormDto()
            {
                IsApproved=true,
                Ids = new List<int>() { testData.Id }
            };
            //Act
            int result = await service.ApprovalData(approvalVBFormDto);

            //Assert
            Assert.True(0 < result);
        }
    }
}
