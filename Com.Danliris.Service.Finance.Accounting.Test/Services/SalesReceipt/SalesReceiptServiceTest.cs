using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.AutoMapperProfiles.SalesReceipt;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.SalesReceipt;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.SalesReceipt;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.NewIntegrationViewModel;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.SalesReceipt;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.SalesReceipt;
using Com.Danliris.Service.Finance.Accounting.Test.Helpers;
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

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.SalesReceipt
{
    public class SalesReceiptServiceTest
    {
        private const string ENTITY = "SalesReceipt";

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

        private SalesReceiptDataUtil _dataUtil(SalesReceiptService service)
        {
            return new SalesReceiptDataUtil(service);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            SalesReceiptService service = new SalesReceiptService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            SalesReceiptModel model = await _dataUtil(service).GetTestDataById();
            var Response = await service.ReadByIdAsync(model.Id);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            SalesReceiptService service = new SalesReceiptService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            SalesReceiptModel model = _dataUtil(service).GetNewData();
            var Response = await service.CreateAsync(model);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public void Should_No_Error_Validate_Data()
        {
            SalesReceiptService service = new SalesReceiptService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            SalesReceiptViewModel vm = _dataUtil(service).GetDataToValidate();

            Assert.True(vm.Validate(null).Count() == 0);
        }

        [Fact]
        public void Should_Success_Validate_All_Null_Data()
        {
            SalesReceiptViewModel vm = new SalesReceiptViewModel();

            Assert.True(vm.Validate(null).Count() > 0);
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
        public void Mapping_With_AutoMapper_Profiles()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SalesReceiptProfile>();
            });
            var mapper = configuration.CreateMapper();

            SalesReceiptViewModel salesReceiptViewModel = new SalesReceiptViewModel { Id = 1 };
            SalesReceiptModel salesReceiptModel = mapper.Map<SalesReceiptModel>(salesReceiptViewModel);

            Assert.Equal(salesReceiptViewModel.Id, salesReceiptModel.Id);

            SalesReceiptDetailViewModel salesReceiptDetailViewModel = new SalesReceiptDetailViewModel { Id = 1 };
            SalesReceiptDetailModel salesReceiptDetailModel = mapper.Map<SalesReceiptDetailModel>(salesReceiptDetailViewModel);

            Assert.Equal(salesReceiptDetailViewModel.Id, salesReceiptDetailModel.Id);
        }

        [Fact]
        public void Validate_Validation_ViewModel()
        {
            List<SalesReceiptViewModel> viewModels = new List<SalesReceiptViewModel>
            {
                new SalesReceiptViewModel{
                    SalesReceiptDate = DateTimeOffset.UtcNow.AddDays(-1),
                    Unit = new NewUnitViewModel()
                    {
                        Id = 0,
                        Name = "",
                    },
                    Buyer = new NewBuyerViewModel()
                    {
                        Id = 0,
                        Name = "",
                        Address = "",
                    },
                    OriginAccountNumber = "",
                    Currency = new CurrencyViewModel()
                    {
                        Id = 0,
                        Code = "",
                        Symbol = "",
                        Rate = 0,
                    },
                    Bank = new AccountBankViewModel()
                    {
                        Id = 0,
                        AccountName = "",
                        AccountNumber = "",
                        BankName = "",
                        Code = "",
                    },
                    AdministrationFee = -1,
                    TotalPaid = -1,
                    SalesReceiptDetails = new List<SalesReceiptDetailViewModel>{
                        new SalesReceiptDetailViewModel{
                            Id = 0,
                            SalesInvoice = new SalesInvoiceViewModel()
                            {
                                Id = 0,
                                SalesInvoiceNo = "",
                                Currency = new CurrencyViewModel()
                                {
                                    Id = 0,
                                    Code = "",
                                    Symbol = "",
                                    Rate = 0,
                                },
                            },
                            DueDate = DateTimeOffset.UtcNow.AddDays(-1),
                            VatType = "",
                            Tempo = -1,
                            TotalPayment = -1,
                            TotalPaid = -1,
                            Paid = -1,
                            Nominal = -1,
                            Unpaid = -1,
                            IsPaidOff = false
                        }
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
        public void Validate_Duplicate_DetailViewModel()
        {
            List<SalesReceiptViewModel> viewModels = new List<SalesReceiptViewModel>
            {
                new SalesReceiptViewModel{
                    Unit = new NewUnitViewModel()
                    {
                        Id = 14,
                    },
                    Buyer = new NewBuyerViewModel()
                    {
                        Id = 28,
                    },
                    Currency = new CurrencyViewModel()
                    {
                        Id = 8,
                    },
                    Bank = new AccountBankViewModel()
                    {
                        Id = 98,
                    },
                    SalesReceiptDetails = new List<SalesReceiptDetailViewModel>{
                        new SalesReceiptDetailViewModel{
                            Id = 10,
                            SalesInvoice = new SalesInvoiceViewModel()
                            {
                                Id = 10,
                                SalesInvoiceNo = "no",
                                Currency = new CurrencyViewModel()
                            {
                                Id = 10,
                                Code = "USD",
                                Symbol = "$",
                                Rate = 10,
                            },
                            },
                            DueDate = DateTimeOffset.UtcNow,
                            VatType = "PPN Kawasan Berikat",
                            Tempo = 10,
                            TotalPayment = 10,
                            TotalPaid = 10,
                            Paid = 10,
                            Nominal = 10,
                            Unpaid = 10,
                            OverPaid = 10,
                            IsPaidOff = true
                        },
                        new SalesReceiptDetailViewModel{
                            Id = 10,
                            SalesInvoice = new SalesInvoiceViewModel()
                            {
                                Id = 10,
                                SalesInvoiceNo = "no",
                                Currency = new CurrencyViewModel()
                                {
                                    Id = 10,
                                    Code = "USD",
                                    Symbol = "$",
                                    Rate = 10,
                                },
                            },
                            DueDate = DateTimeOffset.UtcNow,
                            VatType = "PPN Kawasan Berikat",
                            Tempo = 10,
                            TotalPayment = 10,
                            TotalPaid = 10,
                            Paid = 10,
                            Nominal = 10,
                            Unpaid = 10,
                            OverPaid = 10,
                            IsPaidOff = true
                        }
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
        public void Validate_CurrencySymbol_And_VatType_For_PDF()
        {
            List<SalesReceiptViewModel> viewModels = new List<SalesReceiptViewModel>
            {
                new SalesReceiptViewModel{
                    Unit = new NewUnitViewModel()
                    {
                        Id = 8,
                    },
                    Buyer = new NewBuyerViewModel()
                    {
                        Id = 14,
                    },
                    Currency = new CurrencyViewModel()
                    {
                        Id = 2,
                    },
                    Bank = new AccountBankViewModel()
                    {
                        Id = 18,
                    },
                    SalesReceiptDetails = new List<SalesReceiptDetailViewModel>{
                        new SalesReceiptDetailViewModel{
                            VatType = "PPN Umum",
                            SalesInvoice = new SalesInvoiceViewModel()
                            {
                                Id = 1,
                                Currency = new CurrencyViewModel()
                                {
                                    Id = 20,
                                    Symbol = "$",
                                    Rate = 1000,
                                },
                            },
                        }
                    }
                }
            };
            foreach (var viewModel in viewModels)
            {
                var defaultValidationResult = viewModel.Validate(null);
                Assert.True(defaultValidationResult.Count() > 0);
            }
        }
    }
}
