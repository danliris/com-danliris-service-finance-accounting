using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.CashierApproval;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CashierApproval;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;


//CashierAproval
//DeleteCashierAproval

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.Non_POApproval
{
    public class Non_POApprovalServiceTest
    {
        private const string ENTITY = "CashierApprovals";

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }

        //private IServiceProvider PutServiceProvider()
        //{
        //    var httpClientService = new Mock<IHttpClientService>();
        //    httpClientService
        //        .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("purchasing-dispositions/update/position"))))
        //        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new PurchasingDispositionDataUtil().GetResultFormatterOkString()) });

        //    var serviceProviderMock = new Mock<IServiceProvider>();
        //    serviceProviderMock
        //        .Setup(x => x.GetService(typeof(IdentityService)))
        //        .Returns(new IdentityService { Username = "Username", TimezoneOffset = 7 });
        //    serviceProviderMock
        //        .Setup(x => x.GetService(typeof(IHttpClientService)))
        //        .Returns(httpClientService.Object);

        //    return serviceProviderMock.Object;
        //}

        //private FinanceDbContext _dbContext(string testName)
        //{
        //    DbContextOptionsBuilder<FinanceDbContext> optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();
        //    optionsBuilder
        //        .UseInMemoryDatabase(testName)
        //        .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

        //    FinanceDbContext dbContext = new FinanceDbContext(optionsBuilder.Options);

        //    return dbContext;
        //}

        //private PurchasingDispositionExpeditionDataUtil _dataUtil(PurchasingDispositionExpeditionService service)
        //{
        //    return new PurchasingDispositionExpeditionDataUtil(service);
        //}

        //private Mock<IServiceProvider> GetServiceProvider()
        //{
        //    var httpClientService = new Mock<IHttpClientService>();
        //    httpClientService
        //        .Setup(x => x.PutAsync(It.Is<string>(s => s.Contains("purchasing-dispositions/update/position")), It.IsAny<StringContent>()))
        //        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        //    var serviceProvider = new Mock<IServiceProvider>();

        //    serviceProvider
        //        .Setup(x => x.GetService(typeof(IHttpClientService)))
        //        .Returns(httpClientService.Object);

        //    serviceProvider
        //        .Setup(x => x.GetService(typeof(IIdentityService)))
        //        .Returns(new IdentityService() { Token = "Token", Username = "Test", TimezoneOffset = 7 });


        //    return serviceProvider;
        //}

        //private Mock<IServiceProvider> GetServiceProviderWrongHttpClient()
        //{
        //    var httpClientService = new Mock<IHttpClientService>();
        //    httpClientService
        //        .Setup(x => x.PutAsync(It.Is<string>(s => s.Contains("purchasing-dispositions/update/position")), It.IsAny<StringContent>()))
        //        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        //    var serviceProvider = new Mock<IServiceProvider>();
        //    serviceProvider
        //        .Setup(x => x.GetService(typeof(IHttpClientService)))
        //        .Returns(httpClientService);

        //    serviceProvider
        //        .Setup(x => x.GetService(typeof(IIdentityService)))
        //        .Returns(new IdentityService() { Token = "Token", Username = "Test", TimezoneOffset = 7 });


        //    return serviceProvider;
        //}

        //[Fact]
        //public async Task Should_Success_Post_Acceptance_With_PO()
        //{
        //    CashierApprovalService service = new CashierApprovalService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    VbRequestModel model = await _dataUtil(service).GetTestData();

        //    CashierApprovalViewModel data = new CashierApprovalViewModel()
        //    {
        //        VBRequestCategory = "PO",
        //        CashierApproval = new List<CashierApprovalItemViewModel>()
        //        {
        //            new CashierApprovalItemViewModel()
        //            {
        //                VBNo  = model.VBNo,
        //                Id = model.Id
        //            }
        //        }
        //    };

        //    var response = await service.CashierAproval(data);
        //    Assert.NotEqual(0, response);
        //}

        //[Fact]
        //public async Task Should_Success_Post_Acceptance_Non_PO()
        //{
        //    CashierApprovalService service = new CashierApprovalService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    VbRequestModel model = await _dataUtil(service).GetTestData();

        //    CashierApprovalViewModel data = new CashierApprovalViewModel()
        //    {
        //        VBRequestCategory = "NONPO",
        //        CashierApproval = new List<CashierApprovalItemViewModel>()
        //        {
        //            new CashierApprovalItemViewModel()
        //            {
        //                VBNo  = model.VBNo,
        //                Id = model.Id
        //            }
        //        }
        //    };

        //    var response = await service.CashierAproval(data);
        //    Assert.NotEqual(0, response);
        //}

        //[Fact]
        //public async Task Should_Fail_Post_Acceptance_PO()
        //{

        //    CashierApprovalService service = new CashierApprovalService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    VbRequestModel model = await _dataUtil(service).GetTestData();

        //    CashierApprovalViewModel data = new CashierApprovalViewModel()
        //    {
        //        VBRequestCategory = "PO",
        //        CashierApproval = new List<CashierApprovalItemViewModel>()
        //        {
        //            new CashierApprovalItemViewModel()
        //            {
        //                VBNo  = model.VBNo,
        //                Id = model.Id
        //            }
        //        }
        //    };
        //    await Assert.ThrowsAnyAsync<Exception>(() => service.CashierAproval(null));

        //}

        //[Fact]
        //public async Task Should_Fail_Post_Acceptance_Non_PO()
        //{

        //    CashierApprovalService service = new CashierApprovalService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    VbRequestModel model = await _dataUtil(service).GetTestData();

        //    CashierApprovalViewModel data = new CashierApprovalViewModel()
        //    {
        //        VBRequestCategory = "NONPO",
        //        CashierApproval = new List<CashierApprovalItemViewModel>()
        //        {
        //            new CashierApprovalItemViewModel()
        //            {
        //                VBNo  = model.VBNo,
        //                Id = model.Id
        //            }
        //        }
        //    };
        //    await Assert.ThrowsAnyAsync<Exception>(() => service.CashierAproval(null));

        //}

        //[Fact]
        //public async Task Should_Success_Delete_Acceptance_With_PO()
        //{
        //    CashierApprovalService service = new CashierApprovalService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var model = await _dataUtil(service).GetTestData();
        //    CashierApprovalViewModel data = new CashierApprovalViewModel()
        //    {
        //        VBRequestCategory = "PO",
        //        CashierApproval = new List<CashierApprovalItemViewModel>()
        //        {
        //            new CashierApprovalItemViewModel()
        //            {
        //                VBNo  = model.VBNo,
        //                Id = model.Id
        //            }
        //        }
        //    };
        //    var acceptedResponse = await service.CashierAproval(data);
        //    var newModel = await service.ReadByIdAsync(model.Id);
        //    var deleteResponse = await service.DeleteCashierAproval(newModel.Id);
        //    Assert.NotEqual(0, deleteResponse);
        //}

        //[Fact]
        //public async Task Should_Success_Delete_Acceptance_Non_PO()
        //{
        //    CashierApprovalService service = new CashierApprovalService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var model = await _dataUtil(service).GetTestData();
        //    CashierApprovalViewModel data = new CashierApprovalViewModel()
        //    {
        //        VBRequestCategory = "NONPO",
        //        CashierApproval = new List<CashierApprovalItemViewModel>()
        //        {
        //            new CashierApprovalItemViewModel()
        //            {
        //                VBNo  = model.VBNo,
        //                Id = model.Id
        //            }
        //        }
        //    };
        //    var acceptedResponse = await service.CashierAproval(data);
        //    var newModel = await service.ReadByIdAsync(model.Id);
        //    var deleteResponse = await service.DeleteCashierAproval(newModel.Id);
        //    Assert.NotEqual(0, deleteResponse);
        //}

        //[Fact]
        //public async Task Should_Fail_Delete_Acceptance()
        //{
        //    CashierApprovalService service = new CashierApprovalService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var model = await _dataUtil(service).GetTestData();
        //    CashierApprovalViewModel data = new CashierApprovalViewModel()
        //    {
        //        VBRequestCategory = "NONPO",
        //        CashierApproval = new List<CashierApprovalItemViewModel>()
        //        {
        //            new CashierApprovalItemViewModel()
        //            {
        //                VBNo  = model.VBNo,
        //                Id = model.Id
        //            }
        //        }
        //    };
        //    var acceptedResponse = await service.CashierAproval(data);
        //    var newModel = await service.ReadByIdAsync(model.Id);
        //    service = new CashierApprovalService(GetServiceProviderWrongHttpClient().Object, _dbContext(GetCurrentMethod()));
        //    await Assert.ThrowsAnyAsync<Exception>(() => service.DeleteCashierAproval(newModel.Id));

        //}
    }
}
