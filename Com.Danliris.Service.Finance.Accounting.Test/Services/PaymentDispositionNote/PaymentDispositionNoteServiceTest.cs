using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.PaymentDispositionNote;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.PurchasingDispositionExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.PaymentDispositionNote;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.PaymentDispositionNoteViewModel;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.PaymentDispositionNote;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.PurchasingDispositionExpedition;
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
        public async void Should_Success_Create_Data()
        {
            PaymentDispositionNoteService service = new PaymentDispositionNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            PaymentDispositionNoteModel model =  _dataUtil(service, GetCurrentMethod()).GetNewData();
            var Response = await service.CreateAsync(model);
            Assert.NotEqual(0, Response);
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
        public async void Should_Success_Delete_Data()
        {
            PaymentDispositionNoteService service = new PaymentDispositionNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));


            PaymentDispositionNoteModel model = await _dataUtil(service, GetCurrentMethod()).GetTestData();
            var newModel = await service.ReadByIdAsync(model.Id);

            var Response = await service.DeleteAsync(newModel.Id);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async void Should_Success_Update_Data()
        {
            PaymentDispositionNoteService service = new PaymentDispositionNoteService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));


            PaymentDispositionNoteModel model = await _dataUtil(service, GetCurrentMethod()).GetTestData();
            var newModel = await service.ReadByIdAsync(model.Id);
            newModel.BGCheckNumber = "newBG";
            var Response1 = await service.UpdateAsync(newModel.Id, newModel);
            Assert.NotEqual(0, Response1);

            PaymentDispositionNoteModel model2 = await _dataUtil(service, GetCurrentMethod()).GetTestData();
            //var newModel2 = await service.ReadByIdAsync(model.Id);
            PaymentDispositionNoteModel newModel2 = new PaymentDispositionNoteModel();
            newModel2.Id = model2.Id;
            
            newModel2.Items =new List<PaymentDispositionNoteItemModel> { model2.Items.First() };
            var Response = await service.UpdateAsync(model2.Id, newModel2);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public void Should_Success_Validate_All_Null_Data()
        {
            PaymentDispositionNoteViewModel vm = new PaymentDispositionNoteViewModel();

            Assert.True(vm.Validate(null).Count() > 0);
        }

    }
}
