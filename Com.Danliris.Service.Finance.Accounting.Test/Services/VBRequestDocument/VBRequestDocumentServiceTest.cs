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
            FinanceDbContext dbContext = _dbContext(GetCurrentAsyncMethod());

            VBRequestDocumentService service = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            VBRequestDocumentNonPOFormDto form = _dataUtil(service, dbContext).GetNewData_VBRequestDocumentNonPOFormDto();
            VBRequestDocumentModel data = _dataUtil(service, dbContext).GetTestData_VBRequestDocument();

            int result = await service.CreateNonPO(form);
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task CreateNonPO_Throw_Exception()
        {
            FinanceDbContext dbContext = _dbContext(GetCurrentAsyncMethod());

            VBRequestDocumentService service = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            VBRequestDocumentNonPOFormDto form = _dataUtil(service, dbContext).GetNewData_VBRequestDocumentNonPOFormDto();
            form.Items = null;

            await Assert.ThrowsAsync<System.ArgumentNullException>(()=>service.CreateNonPO(form));
        }


        [Fact]
        public void CreateWithPO_Return_Success()
        {
            FinanceDbContext dbContext = _dbContext(GetCurrentAsyncMethod());

            VBRequestDocumentService service = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            VBRequestDocumentWithPOFormDto form = _dataUtil(service, dbContext).GetNewData_VBRequestDocumentWithPOFormDto();
            VBRequestDocumentModel data = _dataUtil(service, dbContext).GetTestData_VBRequestDocument();

            int result =  service.CreateWithPO(form);
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task DeleteNonPO_Return_Success()
        {
            FinanceDbContext dbContext = _dbContext(GetCurrentAsyncMethod());

            VBRequestDocumentService service = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            VBRequestDocumentItemModel data = _dataUtil(service, dbContext).GetTestData_VBRequestDocumentItem();

            int result = await service.DeleteNonPO(data.Id);
            Assert.NotEqual(0, result);
        }


        [Fact]
        public void DeleteWithPO_Return_Success()
        {
            FinanceDbContext dbContext = _dbContext(GetCurrentAsyncMethod());

            VBRequestDocumentService service = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            VBRequestDocumentItemModel data = _dataUtil(service, dbContext).GetTestData_VBRequestDocumentItem();

            int result =  service.DeleteWithPO(data.Id);
            Assert.NotEqual(0, result);
        }


        [Fact]
        public void Get_Return_Success()
        {
            FinanceDbContext dbContext = _dbContext(GetCurrentAsyncMethod());

            VBRequestDocumentService service = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            VBRequestDocumentItemModel data = _dataUtil(service, dbContext).GetTestData_VBRequestDocumentItem();

            var orderData = new
            {
                DocumentNo = "desc"
            };
            string order =JsonConvert.SerializeObject(orderData);
          
            var result = service.Get(1,1, order,new List<string>(),"","{}");
            Assert.NotNull( result);
            Assert.True(0 < result.Data.Count());
        }

        [Fact]
        public void GetNonPOById_Return_Success()
        {
            FinanceDbContext dbContext = _dbContext(GetCurrentAsyncMethod());

            VBRequestDocumentService service = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            VBRequestDocumentItemModel data = _dataUtil(service, dbContext).GetTestData_VBRequestDocumentItem();

            var result = service.GetNonPOById(data.Id);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetNonPOById_Return_Null()
        {
            FinanceDbContext dbContext = _dbContext(GetCurrentAsyncMethod());

            VBRequestDocumentService service = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            VBRequestDocumentItemModel data = _dataUtil(service, dbContext).GetTestData_VBRequestDocumentItem();

            var result =await service.GetNonPOById(data.Id+1);
            Assert.Null(result);
        }

        [Fact]
        public void GetWithPOById_Return_Success()
        {
            FinanceDbContext dbContext = _dbContext(GetCurrentAsyncMethod());

            VBRequestDocumentService service = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            VBRequestDocumentItemModel data = _dataUtil(service, dbContext).GetTestData_VBRequestDocumentItem();

            var result =  service.GetWithPOById(data.Id );
            Assert.NotNull(result);
            Assert.NotEqual(0,result.Id);
        }

        [Fact]
        public async Task UpdateNonPO_Return_Null()
        {
            FinanceDbContext dbContext = _dbContext(GetCurrentAsyncMethod());

            VBRequestDocumentService service = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            VBRequestDocumentNonPOFormDto form = _dataUtil(service, dbContext).GetNewData_VBRequestDocumentNonPOFormDto();
            VBRequestDocumentItemModel data = _dataUtil(service, dbContext).GetTestData_VBRequestDocumentItem();

            int result = await service.UpdateNonPO(data.Id, form);
            Assert.NotEqual(0,result);
        }

    }
}
