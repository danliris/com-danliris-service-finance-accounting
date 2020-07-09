using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.CashierApproval;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CashierApproval;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.CashierApproval;
using Com.Danliris.Service.Finance.Accounting.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        private FinanceDbContext _dbContext(string testName)
        {
            DbContextOptionsBuilder<FinanceDbContext> optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            FinanceDbContext dbContext = new FinanceDbContext(optionsBuilder.Options);

            return dbContext;
        }

        private CashierApprovalDataUtil _dataUtil(CashierApprovalService service)
        {
            return new CashierApprovalDataUtil(service);
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

        [Fact]
        public void Validate_Validation_ViewModel()
        {
            List<CashierApprovalViewModel> viewModels = new List<CashierApprovalViewModel>
            {
                new CashierApprovalViewModel{
                    VBRequestCategory = "PO",
                    CashierApproval = new List<CashierApprovalItemViewModel>{
                    }
                }
            };
            foreach (var viewModel in viewModels)
            {
                var defaultValidationResult = viewModel.Validate(null);
                Assert.True(defaultValidationResult.Count() > 0);
            }
        }

        [Fact]
        public async Task Should_Success_Post_Approval_With_PO()
        {
            CashierApprovalService service = new CashierApprovalService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            VbRequestModel model = await _dataUtil(service).GetTestData();

            CashierApprovalViewModel data = new CashierApprovalViewModel()
            {
                VBRequestCategory = "PO",
                CashierApproval = new List<CashierApprovalItemViewModel>()
                {
                    new CashierApprovalItemViewModel()
                    {
                        VBNo  = model.VBNo,
                        Id = model.Id
                    }
                }
            };

            var response = await service.CashierAproval(data);
            Assert.NotEqual(0, response);
        }

        [Fact]
        public async Task Should_Success_Post_Approval_Non_PO()
        {
            CashierApprovalService service = new CashierApprovalService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            VbRequestModel model = await _dataUtil(service).GetTestData();

            CashierApprovalViewModel data = new CashierApprovalViewModel()
            {
                VBRequestCategory = "NONPO",
                CashierApproval = new List<CashierApprovalItemViewModel>()
                {
                    new CashierApprovalItemViewModel()
                    {
                        VBNo  = model.VBNo,
                        Id = model.Id
                    }
                }
            };

            var response = await service.CashierAproval(data);
            Assert.NotEqual(0, response);
        }

        [Fact]
        public async Task Should_Fail_Post_Approval_With_PO()
        {

            CashierApprovalService service = new CashierApprovalService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            VbRequestModel model = await _dataUtil(service).GetTestData();

            CashierApprovalViewModel data = new CashierApprovalViewModel()
            {
                VBRequestCategory = "PO",
                CashierApproval = new List<CashierApprovalItemViewModel>()
                {
                    new CashierApprovalItemViewModel()
                    {
                        VBNo  = model.VBNo,
                        Id = model.Id
                    }
                }
            };

            await Assert.ThrowsAnyAsync<Exception>(() => service.CashierAproval(null));

        }

        [Fact]
        public async Task Should_Fail_Post_Approval_Non_PO()
        {

            CashierApprovalService service = new CashierApprovalService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            VbRequestModel model = await _dataUtil(service).GetTestData();

            CashierApprovalViewModel data = new CashierApprovalViewModel()
            {
                VBRequestCategory = "NONPO",
                CashierApproval = new List<CashierApprovalItemViewModel>()
                {
                    new CashierApprovalItemViewModel()
                    {
                        VBNo  = model.VBNo,
                        Id = model.Id
                    }
                }
            };

            await Assert.ThrowsAnyAsync<Exception>(() => service.CashierAproval(null));

        }

        [Fact]
        public async Task Should_Success_Delete_Approval_With_PO()
        {
            CashierApprovalService service = new CashierApprovalService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            VbRequestModel model = await _dataUtil(service).GetTestData();
            model.VBRequestCategory = "PO";
            CashierApprovalViewModel data = new CashierApprovalViewModel()
            {
                VBRequestCategory = "PO",
                CashierApproval = new List<CashierApprovalItemViewModel>()
                {
                    new CashierApprovalItemViewModel()
                    {
                        VBNo  = model.VBNo,
                        Id = model.Id
                    }
                }
            };
            var acceptedResponse = await service.CashierAproval(data);
            var newModel = await service.ReadByIdAsync(model.Id);
            var deleteResponse = await service.DeleteCashierAproval(newModel.Id);
            Assert.NotEqual(0, deleteResponse);
        }

        [Fact]
        public async Task Should_Success_Delete_Approval_Non_PO()
        {
            CashierApprovalService service = new CashierApprovalService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            VbRequestModel model = await _dataUtil(service).GetTestData();

            CashierApprovalViewModel data = new CashierApprovalViewModel()
            {
                VBRequestCategory = "NONPO",
                CashierApproval = new List<CashierApprovalItemViewModel>()
                {
                    new CashierApprovalItemViewModel()
                    {
                        VBNo  = model.VBNo,
                        Id = model.Id
                    }
                }
            };
            var acceptedResponse = await service.CashierAproval(data);
            var newModel = await service.ReadByIdAsync(model.Id);
            var deleteResponse = await service.DeleteCashierAproval(newModel.Id);
            Assert.NotEqual(0, deleteResponse);
        }

        //[Fact]
        //public async Task Should_Fail_Delete_Approval_Non_PO()
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

        //    var acceptedResponse = await service.CashierAproval(data);
        //    var newModel = await service.ReadByIdAsync(model.Id);
        //service = new CashierApprovalService(GetServiceProviderWrongHttpClient().Object, _dbContext(GetCurrentMethod()));
        //    await Assert.ThrowsAnyAsync<Exception>(() => service.DeleteCashierAproval(0));

        //}

        [Fact]
        public async Task Should_Fail_Delete_Empty_Id()
        {
            CashierApprovalService service = new CashierApprovalService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var deleteResponse = await service.DeleteCashierAproval(-1);
            Assert.Equal(0, deleteResponse);
        }
    }
}
