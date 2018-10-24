using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.DailyBankTransactionDataUtils;
using Com.Danliris.Service.Finance.Accounting.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Xunit;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
namespace Com.Danliris.Service.Finance.Accounting.Test.Services.DailyBankTransaction
{
    public class DailyBankTransactionServiceTest
    {
        private const string ENTITY = "DailyBankTransaction";
        //private PurchasingDocumentAcceptanceDataUtil pdaDataUtil;

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

        private DailyBankTransactionDataUtil _dataUtil(DailyBankTransactionService service)
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            return new DailyBankTransactionDataUtil(service);
        }

        [Fact]
        public async void Should_Success_Get_Data()
        {
            DailyBankTransactionService facade = new DailyBankTransactionService(_dbContext(GetCurrentMethod()));
            await _dataUtil(facade).GetTestDataIn();
            ReadResponse<DailyBankTransactionModel> Response = facade.Read();
            Assert.NotEmpty(Response.Data);
        }

        [Fact]
        public async void Should_Success_Get_Data_By_Id()
        {
            //var numberGeneratorMock = new Mock<IBankDocumentNumberGenerator>();
            try
            {
                DailyBankTransactionService service = new DailyBankTransactionService(_dbContext(GetCurrentMethod()));
                DailyBankTransactionModel modelIn = await _dataUtil(service).GetTestDataIn();
                DailyBankTransactionModel modelOut = await _dataUtil(service).GetTestDataOut();
                var Response = service.ReadById(modelIn.Id);
                Assert.NotNull(Response);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [Fact]
        public async void Should_Success_Create_Data()
        {
            DailyBankTransactionService service = new DailyBankTransactionService(_dbContext(GetCurrentMethod()));
            DailyBankTransactionModel model = _dataUtil(service).GetNewData();
            var Response = await service.Create(model, "Unit Test");
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public void Should_Success_Validate_All_Null_Data()
        {
            DailyBankTransactionViewModel vm = new DailyBankTransactionViewModel();

            Assert.True(vm.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_Empty_Supplier_Operasional_Data()
        {
            DailyBankTransactionViewModel vm = new DailyBankTransactionViewModel()
            {
                Date = DateTimeOffset.Now.AddDays(1),
                Status = "OUT",
                SourceType = "Operasional",
            };

            Assert.True(vm.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_Empty_Supplier_Non_Operasional_Data()
        {
            DailyBankTransactionViewModel vm = new DailyBankTransactionViewModel()
            {
                Date = DateTimeOffset.Now.AddDays(1),
                Status = "OUT",
                SourceType = "Investasi",
            };

            Assert.True(vm.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_Empty_Buyer_Operasional_Data()
        {
            DailyBankTransactionViewModel vm = new DailyBankTransactionViewModel()
            {
                Date = DateTimeOffset.Now.AddDays(1),
                Status = "IN",
                SourceType = "Operasional",
            };

            Assert.True(vm.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_Empty_Buyer_Non_Operasional_Data()
        {
            DailyBankTransactionViewModel vm = new DailyBankTransactionViewModel()
            {
                Date = DateTimeOffset.Now.AddDays(1),
                Status = "IN",
                SourceType = "Investasi",
            };

            Assert.True(vm.Validate(null).Count() > 0);
        }

        [Fact]
        public async void Should_Success_Get_Report_All_Null()
        {
            DailyBankTransactionService service = new DailyBankTransactionService(_dbContext(GetCurrentMethod()));
            DailyBankTransactionModel model = _dataUtil(service).GetNewData();
            var Response = await service.Create(model, "Unit Test");

            ReadResponse<DailyBankTransactionModel> Result = service.GetReport(null, null, null, 7);
            Assert.NotEmpty(Result.Data);
        }

        [Fact]
        public async void Should_Success_Get_Report_Null_Date()
        {
            DailyBankTransactionService service = new DailyBankTransactionService(_dbContext(GetCurrentMethod()));
            DailyBankTransactionModel model = _dataUtil(service).GetNewData();
            var Response = await service.Create(model, "Unit Test");

            ReadResponse<DailyBankTransactionModel> Result = service.GetReport(model.AccountBankId, null, null, 7);
            Assert.NotEmpty(Result.Data);
        }

        [Fact]
        public async void Should_Success_Get_Report_Null_DateTo()
        {
            DailyBankTransactionService service = new DailyBankTransactionService(_dbContext(GetCurrentMethod()));
            DailyBankTransactionModel model = _dataUtil(service).GetNewData();
            model.Date = model.Date.AddDays(-3);
            var Response = await service.Create(model, "Unit Test");

            ReadResponse<DailyBankTransactionModel> Result = service.GetReport(model.AccountBankId, model.Date.AddDays(-10), null, 7);
            Assert.NotEmpty(Result.Data);
        }

        [Fact]
        public async void Should_Success_Get_Report_Null_DateFrom()
        {
            DailyBankTransactionService service = new DailyBankTransactionService(_dbContext(GetCurrentMethod()));
            DailyBankTransactionModel model = _dataUtil(service).GetNewData();
            model.Date = model.Date.AddDays(-3);
            var Response = await service.Create(model, "Unit Test");

            ReadResponse<DailyBankTransactionModel> Result = service.GetReport(model.AccountBankId, null, DateTimeOffset.Now, 7);
            Assert.NotEmpty(Result.Data);
        }

        [Fact]
        public async void Should_Success_Get_Report_NotNull_Date_Param()
        {
            DailyBankTransactionService service = new DailyBankTransactionService(_dbContext(GetCurrentMethod()));
            DailyBankTransactionModel model = _dataUtil(service).GetNewData();
            model.Date = model.Date.AddDays(-3);
            var Response = await service.Create(model, "Unit Test");

            ReadResponse<DailyBankTransactionModel> Result = service.GetReport(model.AccountBankId, model.Date.AddDays(-10), DateTimeOffset.Now, 7);
            Assert.NotEmpty(Result.Data);
        }
    }
}
