using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.Memo;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.Memo;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.Memo;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.Memo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.Memo
{
    public class MemoServiceTest
    {
        private const string ENTITY = "Memo";
        //private PurchasingDocumentAcceptanceDataUtil pdaDataUtil;
        //private readonly IIdentityService identityService;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }

        private FinanceDbContext GetDbContext(string testName)
        {
            DbContextOptionsBuilder<FinanceDbContext> optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            FinanceDbContext dbContext = new FinanceDbContext(optionsBuilder.Options);

            return dbContext;
        }

        private Mock<IServiceProvider> GetServiceProviderMock()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test", TimezoneOffset = 7 });


            return serviceProvider;
        }

        [Fact]
        public void Should_Success_Validate_All_Null_Data()
        {
            var viewModel = new MemoViewModel();

            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_All_Null_ObjectProperty()
        {
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();
            var service = new MemoService(dbContext, serviceProviderMock.Object);
            var dataUtil = new MemoDataUtil(service);
            var viewModel = dataUtil.GetViewModelToValidate();

            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public async Task Should_Success_Create_Model()
        {
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();
            var service = new MemoService(dbContext, serviceProviderMock.Object);
            var dataUtil = new MemoDataUtil(service);
            var modelToCreate = dataUtil.GetMemoModelToCreate();
            var result = await service.CreateAsync(modelToCreate);

            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Should_Success_Create_Same_Buyer()
        {
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();
            var service = new MemoService(dbContext, serviceProviderMock.Object);
            var dataUtil = new MemoDataUtil(service);
            await dataUtil.GetCreatedData();
            var modelToCreate = dataUtil.GetMemoModelToCreate();
            var result = await service.CreateAsync(modelToCreate);

            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Should_Success_Update_Model()
        {
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();
            var service = new MemoService(dbContext, serviceProviderMock.Object);
            var dataUtil = new MemoDataUtil(service);
            var modelToUpdate = await dataUtil.GetCreatedData();
            modelToUpdate.Items.Add(new MemoItemModel()
            {
                CurrencyCode = "CurrencyCode",
                CurrencyId = 1,
                CurrencyRate = 1,
                Interest = 1,
                PaymentAmount = 1
            });

            var result = await service.UpdateAsync(modelToUpdate.Id, modelToUpdate);

            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Should_Success_Read_Data()
        {
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();
            var service = new MemoService(dbContext, serviceProviderMock.Object);
            var dataUtil = new MemoDataUtil(service);
            await dataUtil.GetCreatedData();

            var result = service.Read(1, 10, "{}", new List<string>(), "", "{}");

            Assert.NotEmpty(result.Data);
        }

        [Fact]
        public async Task Should_Success_Read_ById()
        {
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();
            var service = new MemoService(dbContext, serviceProviderMock.Object);
            var dataUtil = new MemoDataUtil(service);
            var data = await dataUtil.GetCreatedData();

            var result = await service.ReadByIdAsync(data.Id);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task Should_Success_Delete_ById()
        {
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();
            var service = new MemoService(dbContext, serviceProviderMock.Object);
            var dataUtil = new MemoDataUtil(service);
            var data = await dataUtil.GetCreatedData();

            var result = await service.DeleteAsync(data.Id);

            Assert.NotEqual(0, result);
        }
    }
}
