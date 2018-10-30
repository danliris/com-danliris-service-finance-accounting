using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.IntegrationViewModel;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.DailyBankTransaction
{
    public class DailyBankTransactionServiceTest
    {
        private const string ENTITY = "DailyBankTransactions";
        //private PurchasingDocumentAcceptanceDataUtil pdaDataUtil;
        private readonly IIdentityService identityService;

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

            GetServiceProvider();
            return new DailyBankTransactionDataUtil(service);
        }

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });


            return serviceProvider;
        }

        [Fact]
        public async void Should_Success_Get_Data_In()
        {
            DailyBankTransactionService service = new DailyBankTransactionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            await _dataUtil(service).GetTestDataIn();
            var Response = service.Read(1, 25, "{}", null, null, "{}");
            Assert.NotEmpty(Response.Data);
        }

        [Fact]
        public async void Should_Success_Get_Data_Out()
        {
            DailyBankTransactionService service = new DailyBankTransactionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            await _dataUtil(service).GetTestDataOut();
            var Response = service.Read(1, 25, "{}", null, null, "{}");
            Assert.NotEmpty(Response.Data);
        }

        [Fact]
        public async void Should_Success_Get_Data_By_Id()
        {
            DailyBankTransactionService service = new DailyBankTransactionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            DailyBankTransactionModel model = await _dataUtil(service).GetTestDataIn();
            var Response = await service.ReadByIdAsync(model.Id);
            Assert.NotNull(Response);
        }

        [Fact]
        public async void Should_Success_Create_Data()
        {
            DailyBankTransactionService service = new DailyBankTransactionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            DailyBankTransactionModel model = _dataUtil(service).GetNewData();
            var Response = await service.CreateAsync(model);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public void Should_No_Error_Validate_Data()
        {
            DailyBankTransactionService service = new DailyBankTransactionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            DailyBankTransactionViewModel vm = _dataUtil(service).GetDataToValidate();

            Assert.True(vm.Validate(null).Count() == 0);
        }

        [Fact]
        public void Should_Success_Validate_All_Null_Data()
        {
            DailyBankTransactionViewModel vm = new DailyBankTransactionViewModel();

            Assert.True(vm.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_With_Invalid_Input_Data_In_Buyer_Null_Operasional()
        {
            DailyBankTransactionViewModel vm = new DailyBankTransactionViewModel
            {
                Date = DateTime.Now.AddYears(1),
                Status = "IN",
                SourceType = "Operasional"
            };


            Assert.True(vm.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_With_Invalid_Input_Data_In_Buyer_NotNull_NonOperasional()
        {
            DailyBankTransactionViewModel vm = new DailyBankTransactionViewModel
            {
                Bank = new AccountBankViewModel()
                {
                    _id = ""
                },
                Buyer = new BuyerViewModel()
                {
                    _id = ""
                },
                Date = DateTime.Now.AddYears(1),
                Nominal = 0,
                Status = "IN",
                SourceType = "Investasi"
            };


            Assert.True(vm.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_With_Invalid_Input_Data_Out_Supplier_Null_Operasional()
        {
            DailyBankTransactionViewModel vm = new DailyBankTransactionViewModel
            {
                Date = DateTime.Now.AddYears(1),
                Status = "OUT",
                SourceType = "Operasional"
            };


            Assert.True(vm.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_With_Invalid_Input_Data_Out_Supplier_NotNull_NonOperasional()
        {
            DailyBankTransactionViewModel vm = new DailyBankTransactionViewModel
            {
                Bank = new AccountBankViewModel()
                {
                    _id = ""
                },
                Date = DateTime.Now.AddYears(1),
                Status = "OUT",
                SourceType = "Investasi",
                Supplier = new SupplierViewModel()
                {
                    _id = ""
                }
            };


            Assert.True(vm.Validate(null).Count() > 0);
        }

        [Fact]
        public async void Should_Success_Update_Data()
        {
            DailyBankTransactionService service = new DailyBankTransactionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            DailyBankTransactionModel model = await _dataUtil(service).GetTestDataOut();
            var newModel = await service.ReadByIdAsync(model.Id);
            var Response = await service.UpdateAsync(newModel.Id, newModel);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async void Should_Success_Delete_Data()
        {
            DailyBankTransactionService service = new DailyBankTransactionService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            DailyBankTransactionModel model = await _dataUtil(service).GetTestDataIn();
            var newModel = await service.ReadByIdAsync(model.Id);

            var Response = await service.DeleteAsync(newModel.Id);
            Assert.NotEqual(0, Response);
        }
    }
}
