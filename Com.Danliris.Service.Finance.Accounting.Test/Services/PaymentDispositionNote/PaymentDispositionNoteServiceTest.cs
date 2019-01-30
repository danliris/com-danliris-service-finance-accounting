using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.PaymentDispositionNote;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.PurchasingDispositionExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.PaymentDispositionNote;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.PaymentDispositionNote;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.PurchasingDispositionExpedition;
using Com.Danliris.Service.Finance.Accounting.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.PaymentDispositionNote
{
    public class PaymentDispositionNoteServiceTest
    {
        private const string ENTITY = "PaymentDispositionNotes";

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

        private PaymentDispositionNoteDataUtil _dataUtil(PaymentDispositionNoteService service, string testname)
        {
            var expeditionService = new PurchasingDispositionExpeditionService(GetServiceProvider().Object, _dbContext(testname));
            var expeditionDataUtil = new PurchasingDispositionExpeditionDataUtil(expeditionService);

            return new PaymentDispositionNoteDataUtil(service, expeditionDataUtil);
        }

        [Fact]
        public async void Should_Success_Get_Data()
        {
            PaymentDispositionNoteService service = new PaymentDispositionNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service, GetCurrentMethod()).GetTestData();
            var Response = service.Read(1, 25, "{}", null, null, "{}");
            Assert.NotEmpty(Response.Data);
        }

        [Fact]
        public async void Should_Success_Get_Data_By_Id()
        {
            PaymentDispositionNoteService service = new PaymentDispositionNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            PaymentDispositionNoteModel model = await _dataUtil(service, GetCurrentMethod()).GetTestData();
            var Response = await service.ReadByIdAsync(model.Id);
            Assert.NotNull(Response);
        }

        [Fact]
        public async void Should_Success_Create_Data()
        {
            PaymentDispositionNoteService service = new PaymentDispositionNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            PaymentDispositionNoteModel model = await _dataUtil(service, GetCurrentMethod()).GetTestData();
            var Response = await service.CreateAsync(model);
            Assert.NotEqual(0, Response);
        }

    }
}
