using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentDebtBalance;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentDebtBalance;
using Com.Moonlay.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.DebtBalance
{
    public class DebtBalanceServiceTest
    {
        private const string ENTITY = "DebtBalance";
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
            var optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            var dbContext = new FinanceDbContext(optionsBuilder.Options);

            return dbContext;
        }

        private GarmentDebtBalanceModel GenerateDataUtil(FinanceDbContext dbContext)
        {
            var model = new GarmentDebtBalanceModel(1, "purchasingCategory","billsNo", "paymentBills", 1, "deliveryOrder", 1, DateTimeOffset.Now, "invoice", 1, "supplier", 1, "IDR", 100, 0, 10, 3, true, true);
            EntityExtension.FlagForCreate(model, "unit-test", "data-util");
            dbContext.GarmentDebtBalances.Add(model);
            dbContext.SaveChanges();

            return model;
        }

        private IServiceProvider GetServiceProvider(FinanceDbContext dbContext)
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(FinanceDbContext))).Returns(dbContext);

            return serviceProviderMock.Object;
        }

        [Fact]
        public void Should_Success_Get_Data_With_Match_Params()
        {
            var dbContext = GetDbContext(GetCurrentMethod());
            GenerateDataUtil(dbContext);

            var serviceProvider = GetServiceProvider(dbContext);

            var service = new GarmentDebtBalanceService(serviceProvider);

            var result = service.GetDebtBalanceCardDto(1, DateTimeOffset.Now.Month, DateTimeOffset.Now.Year);

            Assert.NotEmpty(result);
        }
    }
}
