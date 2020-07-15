using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.RealizationVBNonPO;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.RealizationVBNonPO;
using Com.Danliris.Service.Finance.Accounting.Test.Helpers;
using Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.RealizationVBNonPO;
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

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.RealizationVBNonPO
{
    public class RealizationVBNonPOServiceTest
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

        private RealizationVBNonPODataUtil _dataUtil(RealizationVbNonPOService service)
        {
            return new RealizationVBNonPODataUtil(service);
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            RealizationVbNonPOService service = new RealizationVbNonPOService(GetDbContext(GetCurrentMethod()), GetServiceProvider().Object);
            RealizationVbModel model = _dataUtil(service).GetNewData();
            RealizationVbNonPOViewModel viewModel = _dataUtil(service).GetNewViewModel();
            var Response = await service.CreateAsync(model, viewModel);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task DeleteAsync_Return_Success()
        {
            RealizationVbNonPOService service = new RealizationVbNonPOService(GetDbContext(GetCurrentMethod()), GetServiceProvider().Object);
            RealizationVbModel model = _dataUtil(service).GetNewData();
            RealizationVbNonPOViewModel viewModel = _dataUtil(service).GetNewViewModel();
            await service.CreateAsync(model, viewModel);
            var response = await service.DeleteAsync(model.Id);
            Assert.NotEqual(0, response);
        }

        [Fact]
        public async Task ReadByIdAsync2_Return_Success()
        {
            RealizationVbNonPOService service = new RealizationVbNonPOService(GetDbContext(GetCurrentMethod()), GetServiceProvider().Object);
            RealizationVbModel model = _dataUtil(service).GetNewData();
            RealizationVbNonPOViewModel viewModel = _dataUtil(service).GetNewViewModel();
            await service.CreateAsync(model, viewModel);
            var response = await service.ReadByIdAsync2(model.Id);
            Assert.NotNull(response);
        }

        [Fact]
        public async Task Read_Return_Success()
        {
            RealizationVbNonPOService service = new RealizationVbNonPOService(GetDbContext(GetCurrentMethod()), GetServiceProvider().Object);
            RealizationVbModel model = _dataUtil(service).GetNewData();
            RealizationVbNonPOViewModel viewModel = _dataUtil(service).GetNewViewModel();
            await service.CreateAsync(model, viewModel);


            var response = service.Read(1, 1, "{}", new List<string>(), "", "{}");
            Assert.NotNull(response);

        }

        [Fact]
        public void Should_Success_Validate_All_Null_Data()
        {
            var viewModel = new RealizationVbNonPOViewModel();

            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_No_Error_Validate_Data()
        {
            RealizationVbNonPOService service = new RealizationVbNonPOService(GetDbContext(GetCurrentMethod()), GetServiceProvider().Object);
            RealizationVbNonPOViewModel vm = _dataUtil(service).GetNewViewModel();

            Assert.True(vm.Validate(null).Count() == 0);
        }

        [Fact]
        public void Should_No_Error_Validate_Data_False()
        {
            RealizationVbNonPOService service = new RealizationVbNonPOService(GetDbContext(GetCurrentMethod()), GetServiceProvider().Object);
            RealizationVbNonPOViewModel vm = _dataUtil(service).GetNewViewModelFalse();

            Assert.True(vm.Validate(null).Count() > 0);
        }
    }
}
