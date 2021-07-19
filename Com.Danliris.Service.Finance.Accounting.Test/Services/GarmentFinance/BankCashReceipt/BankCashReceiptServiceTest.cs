using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.GarmentFinance.BankCashReceipt;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentFinance.BankCashReceipt;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.GarmentFinance.BankCashReceipt;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.GarmentFinance.BankCashReceipt;
using Com.Danliris.Service.Finance.Accounting.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.GarmentFinance.BankCashReceipt
{
    public class BankCashReceiptServiceTest
    {
        private const string ENTITY = "BankCashReceipts";

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

        private BankCashReceiptDataUtil _dataUtil(BankCashReceiptService service, string testname)
        {
            return new BankCashReceiptDataUtil(service);
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            BankCashReceiptService service = new BankCashReceiptService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            BankCashReceiptModel model = _dataUtil(service, GetCurrentMethod()).GetNewData();
            var Response = await service.CreateAsync(model);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Get_Data()
        {
            BankCashReceiptService service = new BankCashReceiptService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service, GetCurrentMethod()).GetTestData();
            var Response = service.Read(1, 25, "{}", null, null, "{}");
            Assert.NotEmpty(Response.Data);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            BankCashReceiptService service = new BankCashReceiptService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            BankCashReceiptModel model = await _dataUtil(service, GetCurrentMethod()).GetTestData();
            var Response = await service.ReadByIdAsync(model.Id);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Delete_Data()
        {
            BankCashReceiptService service = new BankCashReceiptService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));


            BankCashReceiptModel model = await _dataUtil(service, GetCurrentMethod()).GetTestData();
            var newModel = await service.ReadByIdAsync(model.Id);

            var Response = await service.DeleteAsync(newModel.Id);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Update_Data()
        {
            BankCashReceiptService service = new BankCashReceiptService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));


            BankCashReceiptModel model = await _dataUtil(service, GetCurrentMethod()).GetTestData();
            var newModel = await service.ReadByIdAsync(model.Id);
            var Response1 = await service.UpdateAsync(newModel.Id, newModel);
            Assert.NotEqual(0, Response1);

            BankCashReceiptModel model2 = await _dataUtil(service, GetCurrentMethod()).GetTestData();
            BankCashReceiptModel newModel2 = new BankCashReceiptModel();
            newModel2.Id = model2.Id;

            newModel2.Items = new List<BankCashReceiptItemModel> { model2.Items.First() };
            var Response = await service.UpdateAsync(model2.Id, newModel2);
            Assert.NotEqual(0, Response);

            BankCashReceiptItemModel newItem = new BankCashReceiptItemModel
            {
                BankCashReceiptId = 1,
                C1A = 1,
                C1B = 1,
                C2A = 1,
                C2B = 1,
                C2C = 1,
            };

            newModel2.Items.Add(newItem);
            var Response3 = await service.UpdateAsync(model2.Id, newModel2);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public void Should_Success_Validate_All_Null_Data()
        {
            BankCashReceiptViewModel vm = new BankCashReceiptViewModel();

            Assert.True(vm.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_Null_Items_Data()
        {
            BankCashReceiptViewModel vm = new BankCashReceiptViewModel();
            vm.Items = new List<BankCashReceiptItemViewModel>
            {
                new BankCashReceiptItemViewModel()
                {
                    Id=0,
                }
            };

            Assert.True(vm.Validate(null).Count() > 0);
        }
    }
}
