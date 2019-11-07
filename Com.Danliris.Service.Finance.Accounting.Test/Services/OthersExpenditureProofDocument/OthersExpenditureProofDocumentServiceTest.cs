using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.OthersExpenditureProofDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.OthersExpenditureProofDocumentViewModels;
using Com.Danliris.Service.Finance.Accounting.Test.Services.OthersExpenditureProofDocument.Helper;
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

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.OthersExpenditureProofDocument
{
    public class OthersExpenditureProofDocumentServiceTest
    {
        private const string ENTITY = "OthersExpenditureProofDocument";
        //private PurchasingDocumentAcceptanceDataUtil pdaDataUtil;
        //private readonly IIdentityService identityService;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

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

        private OthersExpenditureProofDocumentCreateUpdateViewModel GetCreateDataUtil()
        {
            return new OthersExpenditureProofDocumentCreateUpdateViewModel()
            {
                AccountBankCode = "BankCode",
                AccountBankId = 1,
                Date = DateTimeOffset.Now,
                Items = new List<OthersExpenditureProofDocumentCreateUpdateItemViewModel>()
                {
                    new OthersExpenditureProofDocumentCreateUpdateItemViewModel()
                    {
                        COAId = 1,
                        Debit = 1,
                        Remark = "Remark"
                    }
                },
                Remark = "Remark",
                Type = "Operasional"
            };
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var dbContext = GetDbContext(GetCurrentMethod());

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IAutoJournalService))).Returns(new AutoJournalServiceTestHelper());
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(new IdentityService() { Username = "Username", Token = "token", TimezoneOffset = 1 });

            var service = new OthersExpenditureProofDocumentService(dbContext, serviceProviderMock.Object);
            var model = GetCreateDataUtil();

            var response = await service.CreateAsync(model);

            Assert.NotEqual(0, response);
        }

        [Fact]
        public async Task Should_Success_Delete_Data()
        {
            var dbContext = GetDbContext(GetCurrentMethod());

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IAutoJournalService))).Returns(new AutoJournalServiceTestHelper());
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(new IdentityService() { Username = "Username", Token = "token", TimezoneOffset = 1 });

            var service = new OthersExpenditureProofDocumentService(dbContext, serviceProviderMock.Object);

            var model = GetCreateDataUtil();
            await service.CreateAsync(model);

            var createdModel = dbContext.OthersExpenditureProofDocuments.FirstOrDefault();
            var response = await service.DeleteAsync(createdModel.Id);

            Assert.NotEqual(0, response);
        }

        [Fact]
        public async Task Should_Success_Get_Single_Data_By_Id()
        {
            var dbContext = GetDbContext(GetCurrentMethod());

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IAutoJournalService))).Returns(new AutoJournalServiceTestHelper());
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService))).Returns(new IdentityService() { Username = "Username", Token = "token", TimezoneOffset = 1 });

            var service = new OthersExpenditureProofDocumentService(dbContext, serviceProviderMock.Object);

            var model = GetCreateDataUtil();
            await service.CreateAsync(model);

            var createdModel = dbContext.OthersExpenditureProofDocuments.FirstOrDefault();
            var response = await service.GetSingleByIdAsync(createdModel.Id);

            Assert.NotNull(response);
        }
    }
}
