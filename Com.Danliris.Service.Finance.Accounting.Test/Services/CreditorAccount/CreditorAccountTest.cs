using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.CreditorAccount;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.CreditorAccount;
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

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.CreditorAccount
{
    public class CreditorAccountTest
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

        private CreditorAccountDataUtil _dataUtil(CreditorAccountService service)
        {

            GetServiceProvider();
            return new CreditorAccountDataUtil(service);
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
        public async void Should_Null_Get_UnitReceiptNote()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetUnitReceiptNotePostedViewModel();
            data.SupplierCode += "new";
            var newData = await service.GetByUnitReceiptNote(data.SupplierCode, data.Code, data.InvoiceNo);
            Assert.Null(newData);
        }

        [Fact]
        public async void Should_Success_Get_UnitReceiptNote_WithoutInvoiceNo()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetUnitReceiptNotePostedViewModel();
            data.InvoiceNo = null;
            var Response = await service.CreateFromUnitReceiptNoteAsync(data);
            var newData = await service.GetByUnitReceiptNote(data.SupplierCode, data.Code, data.InvoiceNo);
            Assert.NotNull(newData);
        }

        [Fact]
        public async void Should_Success_Post_UnitReceiptNote()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetUnitReceiptNotePostedViewModel();
            var Response = await service.CreateFromUnitReceiptNoteAsync(data);
            var newData = await service.GetByUnitReceiptNote(data.SupplierCode, data.Code, data.InvoiceNo);
            Assert.NotNull(newData);
        }

        public async void Should_Success_Put_UnitReceiptNote()
        {

        }
    }
}
