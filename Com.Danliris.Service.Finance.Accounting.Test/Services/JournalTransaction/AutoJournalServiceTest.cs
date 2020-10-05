using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRealizationDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRequestDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.OthersExpenditureProofDocumentViewModels;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.VBRealizationDocument;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.VBRequestDocument;
using Com.Danliris.Service.Finance.Accounting.Test.Services.OthersExpenditureProofDocument.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.NewIntegrationViewModel;
using Moq;
using Xunit;
using Com.Danliris.Service.Finance.Accounting.Test.Helpers;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.VBRealizationDocumentExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRealizationDocumentExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRealizationDocumentExpedition;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.JournalTransaction
{
    public class AutoJournalServiceTest
    {
        private const string ENTITY = "AutoJournal";
        [MethodImpl(MethodImplOptions.NoInlining)]
        private string GetCurrentMethod()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }

        private FinanceDbContext GetDbContext(string testName)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .UseInternalServiceProvider(serviceProvider);

            var dbContext = new FinanceDbContext(optionsBuilder.Options);

            return dbContext;
        }

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test", TimezoneOffset = 7 });

           

            
            return serviceProvider;
        }

        private VBRequestDocumentDataUtil GetdataUtil(VBRequestDocumentService service)
        {
            return new VBRequestDocumentDataUtil(service);
        }

        private VBRealizationDocumentDataUtil GetDataUtil(VBRealizationWithPOService service)
        {
            return new VBRealizationDocumentDataUtil(service);
        }

        private VBRealizationDocumentExpeditionDataUtil _dataUtil(VBRealizationDocumentExpeditionService service, FinanceDbContext financeDbContext)
        {
            return new VBRealizationDocumentExpeditionDataUtil(service, financeDbContext);
        }

        [Fact]
        public async Task Should_Success_Auto_Journal_From_Others_Bank_Expenditure()
        {
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProvider();

            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IJournalTransactionService))).Returns(new JournalTransactionServiceTestHelper());
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService))).Returns(new HttpClientOthersExpenditureServiceHelper());

            var service = new AutoJournalService(dbContext, serviceProviderMock.Object);

            var viewModel = new OthersExpenditureProofDocumentCreateUpdateViewModel()
            {
                Items = new List<OthersExpenditureProofDocumentCreateUpdateItemViewModel>()
                {
                    new OthersExpenditureProofDocumentCreateUpdateItemViewModel()
                }
            };

            var result = await service.AutoJournalFromOthersExpenditureProof(viewModel, "any");
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Should_Success_Auto_Journal_From_Others_Bank_Expenditure_Reverse()
        {
            var dbContext = GetDbContext(GetCurrentMethod());

            var serviceProviderMock = GetServiceProvider();

            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IJournalTransactionService))).Returns(new JournalTransactionServiceTestHelper());

            var service = new AutoJournalService(dbContext, serviceProviderMock.Object);

            var result = await service.AutoJournalReverseFromOthersExpenditureProof("any");
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Should_Success_AutoJournalVBNonPOClearence_With_ViewModel_When_NotKlaring()
        {
            var dbContext = GetDbContext(GetCurrentMethod());

            var serviceProviderMock = GetServiceProvider();

            serviceProviderMock.Setup(s => s.GetService(typeof(FinanceDbContext))).Returns(dbContext);
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new JournalHttpClientTestService());
            //IHttpClientService
            Mock<IJournalTransactionService> journalTransactionServiceMock = new Mock<IJournalTransactionService>();

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IJournalTransactionService)))
                .Returns(journalTransactionServiceMock.Object);

            //  Mock<IMasterCOAService> masterCOAServiceMock = new Mock<IMasterCOAService>();
            var masterCOAServiceMock = new MasterCOAService(serviceProviderMock.Object);
            serviceProviderMock
               .Setup(x => x.GetService(typeof(IMasterCOAService)))
               .Returns(masterCOAServiceMock);


            VBRequestDocumentService vBRequestDocumentService = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            VBRequestDocumentNonPODto data = await GetdataUtil(vBRequestDocumentService).GetTestData_VBRequestDocumentNonPO_NotKlaring();

            VBRealizationWithPOService vBRealizationWithPOService = new VBRealizationWithPOService(dbContext, serviceProviderMock.Object);
            var vBRealizationDocumenData = GetDataUtil(vBRealizationWithPOService).GetTestData_DenganNomorVB();

            var service = new AutoJournalService(dbContext, serviceProviderMock.Object);

            AccountBankViewModel viewModel = new AccountBankViewModel()
            {
                AccountCOA = "AccountCOA",
                AccountName = "AccountName",
                AccountNumber = "AccountNumber",
                BankCode = "BankCode",
                BankName = "BankName",
                Code = "Code",
                Currency = new CurrencyViewModel()
                {
                    Code = "Rp",
                    Description = "Description",
                    Rate = 1,
                    Symbol = "IDR"
                }
            };

            List<int> vbRealizationIds = new List<int>()
            {
                1
            };
            var result = await service.AutoJournalVBNonPOClearence(vbRealizationIds, viewModel);
            Assert.NotEqual(0, result);


        }

        [Fact]
        public async Task Should_Success_AutoJournalVBNonPOClearence_With_ViewModel()
        {
            //Setup
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProvider();

            serviceProviderMock.Setup(s => s.GetService(typeof(FinanceDbContext))).Returns(dbContext);
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new JournalHttpClientTestService());
         
            Mock<IJournalTransactionService> journalTransactionServiceMock = new Mock<IJournalTransactionService>();

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IJournalTransactionService)))
                .Returns(journalTransactionServiceMock.Object);

            var masterCOAServiceMock = new MasterCOAService(serviceProviderMock.Object);
            serviceProviderMock
               .Setup(x => x.GetService(typeof(IMasterCOAService)))
               .Returns(masterCOAServiceMock);


            var vBRequestDocumentService = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            var data = await GetdataUtil(vBRequestDocumentService).GetTestData_VBRequestDocumentNonPO();

            var vBRealizationWithPOService = new VBRealizationWithPOService(dbContext, serviceProviderMock.Object);
            var vBRealizationDocumenData = GetDataUtil(vBRealizationWithPOService).GetTestData_DenganNomorVB();

            var expeditionService = new VBRealizationDocumentExpeditionService(dbContext, GetServiceProvider().Object);
            var model = _dataUtil(expeditionService, dbContext).GetTestData_VBRealizationDocumentExpedition();

            var service = new AutoJournalService(dbContext, serviceProviderMock.Object);

            AccountBankViewModel viewModel = new AccountBankViewModel()
            {
                AccountCOA = "AccountCOA",
                AccountName = "AccountName",
                AccountNumber = "AccountNumber",
                BankCode = "BankCode",
                BankName = "BankName",
                Code = "Code",
                Currency = new CurrencyViewModel()
                {
                    Code = "Rp",
                    Description = "Description",
                    Rate = 1,
                    Symbol = "IDR"
                },
                
            };

            List<int> vbRealizationIds = new List<int>()
            {
                1
            };

            //Act
            var result = await service.AutoJournalVBNonPOClearence(vbRealizationIds, viewModel);
            
            //Assert
            Assert.NotEqual(0, result);
        }


        [Fact]
        public async Task Should_Success_AutoJournalInklaring_With_ViewModel()
        {
            //Setup
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProvider();

            serviceProviderMock.Setup(s => s.GetService(typeof(FinanceDbContext))).Returns(dbContext);
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new JournalHttpClientTestService());

            Mock<IJournalTransactionService> journalTransactionServiceMock = new Mock<IJournalTransactionService>();

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IJournalTransactionService)))
                .Returns(journalTransactionServiceMock.Object);

            var masterCOAServiceMock = new MasterCOAService(serviceProviderMock.Object);
            serviceProviderMock
               .Setup(x => x.GetService(typeof(IMasterCOAService)))
               .Returns(masterCOAServiceMock);


            var vBRequestDocumentService = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            var data = await GetdataUtil(vBRequestDocumentService).GetTestData_VBRequestDocumentNonPO();

            var vBRealizationWithPOService = new VBRealizationWithPOService(dbContext, serviceProviderMock.Object);
            var vBRealizationDocumenData = GetDataUtil(vBRealizationWithPOService).GetTestData_DenganNomorVB();

            var service = new AutoJournalService(dbContext, serviceProviderMock.Object);

            AccountBankViewModel viewModel = new AccountBankViewModel()
            {
                AccountCOA = "AccountCOA",
                AccountName = "AccountName",
                AccountNumber = "AccountNumber",
                BankCode = "BankCode",
                BankName = "BankName",
                Code = "Code",
                Currency = new CurrencyViewModel()
                {
                    Code = "Rp",
                    Description = "Description",
                    Rate = 1,
                    Symbol = "IDR"
                },

            };

            List<int> vbRealizationIds = new List<int>()
            {
                1
            };

            //Act
            var result = await service.AutoJournalInklaring(vbRealizationIds, viewModel);

            //Assert
            Assert.NotEqual(0, result);
        }

       

        [Fact]
        public async Task Should_Success_AutoJournalVBNonPOClearence()
        {
            //Setup
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProvider();

            serviceProviderMock.Setup(s => s.GetService(typeof(FinanceDbContext))).Returns(dbContext);
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new JournalHttpClientTestService());

            Mock<IJournalTransactionService> journalTransactionServiceMock = new Mock<IJournalTransactionService>();

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IJournalTransactionService)))
                .Returns(journalTransactionServiceMock.Object);

            var masterCOAServiceMock = new MasterCOAService(serviceProviderMock.Object);
            serviceProviderMock
               .Setup(x => x.GetService(typeof(IMasterCOAService)))
               .Returns(masterCOAServiceMock);

            var vBRequestDocumentService = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            var data = await GetdataUtil(vBRequestDocumentService).GetTestData_VBRequestDocumentNonPO();

            var vBRealizationWithPOService = new VBRealizationWithPOService(dbContext, serviceProviderMock.Object);
            var vBRealizationDocumenData = GetDataUtil(vBRealizationWithPOService).GetTestData_DenganNomorVB();

            var service = new AutoJournalService(dbContext, serviceProviderMock.Object);

            List<int> vbRealizationIds = new List<int>()
            {
                1
            };

            //Act
            var result = await service.AutoJournalVBNonPOClearence(vbRealizationIds);

            //Assert
            Assert.NotEqual(0, result);
        }


       

        [Fact]
        public async Task Should_Success_AutoJournalVBNonPOClearence_When_NotKlaring()
        {
            //Setup
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProvider();

            serviceProviderMock.Setup(s => s.GetService(typeof(FinanceDbContext))).Returns(dbContext);
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new JournalHttpClientTestService());

            Mock<IJournalTransactionService> journalTransactionServiceMock = new Mock<IJournalTransactionService>();

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IJournalTransactionService)))
                .Returns(journalTransactionServiceMock.Object);

            var masterCOAServiceMock = new MasterCOAService(serviceProviderMock.Object);
            serviceProviderMock
               .Setup(x => x.GetService(typeof(IMasterCOAService)))
               .Returns(masterCOAServiceMock);

            var vBRequestDocumentService = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            var data = await GetdataUtil(vBRequestDocumentService).GetTestData_VBRequestDocumentNonPO_NotKlaring();

            var vBRealizationWithPOService = new VBRealizationWithPOService(dbContext, serviceProviderMock.Object);
            var vBRealizationDocumenData = GetDataUtil(vBRealizationWithPOService).GetTestData_DenganNomorVB();

            var service = new AutoJournalService(dbContext, serviceProviderMock.Object);

            List<int> vbRealizationIds = new List<int>()
            {
                1
            };

            //Act
            var result = await service.AutoJournalVBNonPOClearence(vbRealizationIds);

            //Assert
            Assert.NotEqual(0, result);
        }


        [Fact]
        public async Task Should_Success_AutoJournalInklaring()
        {
            //Setup
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProvider();

            serviceProviderMock.Setup(s => s.GetService(typeof(FinanceDbContext))).Returns(dbContext);
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new JournalHttpClientTestService());

            Mock<IJournalTransactionService> journalTransactionServiceMock = new Mock<IJournalTransactionService>();

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IJournalTransactionService)))
                .Returns(journalTransactionServiceMock.Object);

            var masterCOAServiceMock = new MasterCOAService(serviceProviderMock.Object);
            serviceProviderMock
               .Setup(x => x.GetService(typeof(IMasterCOAService)))
               .Returns(masterCOAServiceMock);


            var vBRequestDocumentService = new VBRequestDocumentService(dbContext, GetServiceProvider().Object);
            var data = await GetdataUtil(vBRequestDocumentService).GetTestData_VBRequestDocumentNonPO();

            var vBRealizationWithPOService = new VBRealizationWithPOService(dbContext, serviceProviderMock.Object);
            var vBRealizationDocumenData = GetDataUtil(vBRealizationWithPOService).GetTestData_DenganNomorVB();

            var service = new AutoJournalService(dbContext, serviceProviderMock.Object);

            List<int> vbRealizationIds = new List<int>()
            {
                1
            };

            //Act
            var result = await service.AutoJournalInklaring(vbRealizationIds);

            //Assert
            Assert.NotEqual(0, result);
        }
    }

    

    internal class JournalTransactionServiceTestHelper : IJournalTransactionService
    {
        public JournalTransactionServiceTestHelper()
        {
        }

        public Task<int> CreateAsync(JournalTransactionModel model)
        {
            return Task.FromResult(1);
        }

        public Task<int> CreateManyAsync(List<JournalTransactionModel> models)
        {
            return Task.FromResult(1);
        }

        public Task<int> DeleteAsync(int id)
        {
            return Task.FromResult(1);
        }

        public MemoryStream GenerateExcel(DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int offSet)
        {
            throw new NotImplementedException();
        }

        public Task<List<GeneralLedgerWrapperReportViewModel>> GetGeneralLedgerReport(DateTimeOffset startDate, DateTimeOffset endDate, int timezoneoffset)
        {
            throw new NotImplementedException();
        }

        public Task<MemoryStream> GetGeneralLedgerReportXls(DateTimeOffset startDate, DateTimeOffset endDate, int timezoneoffset)
        {
            throw new NotImplementedException();
        }

        public (ReadResponse<JournalTransactionReportHeaderViewModel>, decimal, decimal) GetReport(int page, int size, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int offSet)
        {
            throw new NotImplementedException();
        }

        public Task<SubLedgerReportViewModel> GetSubLedgerReport(int? coaId, int month, int year, int timeoffset)
        {
            throw new NotImplementedException();
        }

        public Task<SubLedgerXlsFormat> GetSubLedgerReportXls(int? coaId, int? month, int? year, int timeoffset)
        {
            throw new NotImplementedException();
        }

        public Task<int> PostTransactionAsync(int id)
        {
            return Task.FromResult(1);
        }

        public Task<int> PostTransactionAsync(int id, JournalTransactionModel model)
        {
            return Task.FromResult(1);
        }

        public ReadResponse<JournalTransactionModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            throw new NotImplementedException();
        }

        public Task<JournalTransactionModel> ReadByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public List<JournalTransactionModel> ReadUnPostedTransactionsByPeriod(int month, int year)
        {
            throw new NotImplementedException();
        }

        public Task<int> ReverseJournalTransactionByReferenceNo(string referenceNo)
        {
            return Task.FromResult(1);
        }

        public Task<int> UpdateAsync(int id, JournalTransactionModel model)
        {
            return Task.FromResult(1);
        }
    }
}