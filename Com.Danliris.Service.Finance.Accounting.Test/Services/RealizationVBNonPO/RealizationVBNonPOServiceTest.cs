using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.RealizationVBNonPO;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRealizationDocumentExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.VbNonPORequest;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRealizationDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRealizationDocumentExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRequestDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.RealizationVBNonPO;
using Com.Danliris.Service.Finance.Accounting.Test.Helpers;
using Com.Danliris.Service.Finance.Accounting.Test.Services.OthersExpenditureProofDocument.Helper;
using Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.RealizationVBNonPO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.RealizationVBNonPO
{
    public class RealizationVBNonPOServiceTest
    {
        private const string ENTITY = "RealizationVbs";

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1);

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

        private Mock<IServiceProvider> GetServiceProviderMock()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test", TimezoneOffset = 7 });


            return serviceProvider;
        }

        [Fact]
        public void Should_Success_Validate_All_Null_Data()
        {
            var viewModel = new RealizationVbNonPOViewModel();

            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_All_Null_ObjectProperty()
        {
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();
            var service = new RealizationVbNonPOService(dbContext, serviceProviderMock.Object);
            var dataUtil = new RealizationVBNonPODataUtil(service);
            var viewModel = dataUtil.GetNewViewModelFalse();

            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_All_Null_ObjectProperty2()
        {
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();
            var service = new RealizationVbNonPOService(dbContext, serviceProviderMock.Object);
            var dataUtil = new RealizationVBNonPODataUtil(service);
            var viewModel = dataUtil.GetNewViewModelFalse2();

            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_All_Null_ObjectProperty3()
        {
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();
            var service = new RealizationVbNonPOService(dbContext, serviceProviderMock.Object);
            var dataUtil = new RealizationVBNonPODataUtil(service);
            var viewModel = dataUtil.GetNewViewModelFalse3();

            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_All_Null_ObjectProperty3a()
        {
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();
            var service = new RealizationVbNonPOService(dbContext, serviceProviderMock.Object);
            var dataUtil = new RealizationVBNonPODataUtil(service);
            var viewModel = dataUtil.GetNewViewModelFalse3a();

            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_All_Null_ObjectProperty3b()
        {
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();
            var service = new RealizationVbNonPOService(dbContext, serviceProviderMock.Object);
            var dataUtil = new RealizationVBNonPODataUtil(service);
            var viewModel = dataUtil.GetNewViewModelFalse3b();

            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_All_Null_ObjectProperty4()
        {
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();
            var service = new RealizationVbNonPOService(dbContext, serviceProviderMock.Object);
            var dataUtil = new RealizationVBNonPODataUtil(service);
            var viewModel = dataUtil.GetNewViewModelFalse4();

            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_All_Null_ObjectProperty4a()
        {
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();
            var service = new RealizationVbNonPOService(dbContext, serviceProviderMock.Object);
            var dataUtil = new RealizationVBNonPODataUtil(service);
            var viewModel = dataUtil.GetNewViewModelFalse4a();

            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_All_Null_ObjectProperty4b()
        {
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();
            var service = new RealizationVbNonPOService(dbContext, serviceProviderMock.Object);
            var dataUtil = new RealizationVBNonPODataUtil(service);
            var viewModel = dataUtil.GetNewViewModelFalse4b();

            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        //[Fact]
        //public void Should_Success_Validate_Date_Failed_ObjectProperty()
        //{
        //    var dbContext = GetDbContext(GetCurrentMethod());
        //    var serviceProviderMock = GetServiceProviderMock();
        //    var service = new RealizationVbNonPOService(dbContext, serviceProviderMock.Object);
        //    var dataUtil = new RealizationVBNonPODataUtil(service);
        //    var viewModel = dataUtil.GetNewViewModelDateFalse();

        //    Assert.True(viewModel.Validate(null).Count() > 0);
        //}

        //[Fact]
        //public void Should_Success_Validate_Date_Success_ObjectProperty()
        //{
        //    var dbContext = GetDbContext(GetCurrentMethod());
        //    var serviceProviderMock = GetServiceProviderMock();
        //    var service = new RealizationVbNonPOService(dbContext, serviceProviderMock.Object);
        //    var dataUtil = new RealizationVBNonPODataUtil(service);
        //    var viewModel = dataUtil.GetNewViewModelDateTrue();

        //    Assert.True(viewModel.Validate(null).Count() > 0);
        //}



        [Fact]
        public async Task Should_Success_Create_Model_New()
        {
            //Arrange
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();

            var IVBRealizationDocumentExpeditionServiceMock = new Mock<IVBRealizationDocumentExpeditionService>();
            IVBRealizationDocumentExpeditionServiceMock.Setup(s => s.InitializeExpedition(It.IsAny<int>())).ReturnsAsync(1);

            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService))).Returns(IVBRealizationDocumentExpeditionServiceMock.Object);
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService))).Returns(new HttpClientOthersExpenditureServiceHelper());

            var service = new RealizationVbNonPOService(dbContext, serviceProviderMock.Object);

            var dataUtil = new RealizationVBNonPODataUtil(service);
            var model = dataUtil.GetNewData();
            var viewmodel = dataUtil.GetNewViewModel();

            //Act
            var result = await service.CreateAsync(model, viewmodel);

            //Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Should_Success_Create_Model_New_2()
        {
            //Arrange
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();

            var IVBRealizationDocumentExpeditionServiceMock = new Mock<IVBRealizationDocumentExpeditionService>();
            IVBRealizationDocumentExpeditionServiceMock
                .Setup(s => s.InitializeExpedition(It.IsAny<int>()))
                .ReturnsAsync(1);

            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService))).Returns(IVBRealizationDocumentExpeditionServiceMock.Object);
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService))).Returns(new HttpClientOthersExpenditureServiceHelper());

            var service = new RealizationVbNonPOService(dbContext, serviceProviderMock.Object);
            var dataUtil = new RealizationVBNonPODataUtil(service);

            var model = dataUtil.GetNewData();
            var viewmodel = dataUtil.GetNewViewModel2();

            var dataRequestVb = dataUtil.GetNewData_RequestVB();
            dbContext.VbRequests.Add(dataRequestVb);
            dbContext.SaveChanges();

            //Act
            var result = await service.CreateAsync(model, viewmodel);

            //Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Should_Success_Create_Model_New_3()
        {
            //Arrange
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();

            var IVBRealizationDocumentExpeditionServiceMock = new Mock<IVBRealizationDocumentExpeditionService>();
            IVBRealizationDocumentExpeditionServiceMock
               .Setup(s => s.InitializeExpedition(It.IsAny<int>()))
               .ReturnsAsync(1);

            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService))).Returns(IVBRealizationDocumentExpeditionServiceMock.Object);
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService))).Returns(new HttpClientOthersExpenditureServiceHelper());

            var service = new RealizationVbNonPOService(dbContext, serviceProviderMock.Object);

            var dataUtil = new RealizationVBNonPODataUtil(service);

            var model = dataUtil.GetNewData();
            var dataRequestVb = dataUtil.GetNewData_RequestVB();
            dbContext.VbRequests.Add(dataRequestVb);
            dbContext.SaveChanges();

            var viewmodel = dataUtil.GetNewViewModel3();

            //Act
            var result = await service.CreateAsync(model, viewmodel);

            //Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Should_Success_Create_Model_New_4()
        {
            //Arrange
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();

            var IVBRealizationDocumentExpeditionServiceMock = new Mock<IVBRealizationDocumentExpeditionService>();
            IVBRealizationDocumentExpeditionServiceMock
               .Setup(s => s.InitializeExpedition(It.IsAny<int>()))
               .ReturnsAsync(1);

            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService))).Returns(IVBRealizationDocumentExpeditionServiceMock.Object);
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService))).Returns(new HttpClientOthersExpenditureServiceHelper());

            var service = new RealizationVbNonPOService(dbContext, serviceProviderMock.Object);

            var dataUtil = new RealizationVBNonPODataUtil(service);

            var model = dataUtil.GetNewData();
            var dataRequestVb = dataUtil.GetNewData_RequestVB();
            dbContext.VbRequests.Add(dataRequestVb);
            dbContext.SaveChanges();

            var viewmodel = dataUtil.GetNewViewModel4();

            //Act
            var result = await service.CreateAsync(model, viewmodel);

            //Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Should_Success_Create_Model_New_5()
        {
            //Arrange
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();

            var IVBRealizationDocumentExpeditionServiceMock = new Mock<IVBRealizationDocumentExpeditionService>();
            IVBRealizationDocumentExpeditionServiceMock
               .Setup(s => s.InitializeExpedition(It.IsAny<int>()))
               .ReturnsAsync(1);

            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService))).Returns(IVBRealizationDocumentExpeditionServiceMock.Object);
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService))).Returns(new HttpClientOthersExpenditureServiceHelper());

            var service = new RealizationVbNonPOService(dbContext, serviceProviderMock.Object);

            var dataUtil = new RealizationVBNonPODataUtil(service);

            var model = dataUtil.GetNewData();
            var dataRequestVb = dataUtil.GetNewData_RequestVB();
            dbContext.VbRequests.Add(dataRequestVb);
            dbContext.SaveChanges();

            var viewmodel = dataUtil.GetNewViewModel5();

            //Act
            var result = await service.CreateAsync(model, viewmodel);

            //Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Should_Success_Create_Model_New_6()
        {
            //Arrange
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();

            var IVBRealizationDocumentExpeditionServiceMock = new Mock<IVBRealizationDocumentExpeditionService>();
            IVBRealizationDocumentExpeditionServiceMock
               .Setup(s => s.InitializeExpedition(It.IsAny<int>()))
               .ReturnsAsync(1);

            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService))).Returns(IVBRealizationDocumentExpeditionServiceMock.Object);
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService))).Returns(new HttpClientOthersExpenditureServiceHelper());

            var service = new RealizationVbNonPOService(dbContext, serviceProviderMock.Object);

            var dataUtil = new RealizationVBNonPODataUtil(service);

            var model = dataUtil.GetNewData();
            var dataRequestVb = dataUtil.GetNewData_RequestVB();
            dbContext.VbRequests.Add(dataRequestVb);
            dbContext.SaveChanges();

            var viewmodel = dataUtil.GetNewViewModel6();

            //Act
            var result = await service.CreateAsync(model, viewmodel);

            //Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Should_Success_Update_Model()
        {
            //Arrange
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();

            var IVBRealizationDocumentExpeditionServiceMock = new Mock<IVBRealizationDocumentExpeditionService>();
            IVBRealizationDocumentExpeditionServiceMock
               .Setup(s => s.UpdateExpeditionByRealizationId(It.IsAny<int>()))
               .ReturnsAsync(1);

            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService))).Returns(IVBRealizationDocumentExpeditionServiceMock.Object);
            var service = new RealizationVbNonPOService(dbContext, serviceProviderMock.Object);

            var dataUtil = new RealizationVBNonPODataUtil(service);
            //var modelToUpdate = await dataUtil.GetCreatedData();
            var viewmodel = dataUtil.GetNewViewModel();
            viewmodel.Items.Add(new VbNonPORequestDetailViewModel()
            {
                DateDetail = DateTimeOffset.Now,
                Amount = 123,
                Remark = "Remark",
                isGetPPn = true,
                IncomeTax = new IncomeTaxNew()
                {
                    Id = 1,
                    name = "name",
                    rate = 1
                },
                IncomeTaxBy = "income"
            });

            var dataRequestVb = dataUtil.GetNewData_RequestVB();
            

            //Act
            var result = await service.UpdateAsync(viewmodel.Id, viewmodel);

            //Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Should_Success_Update_Model2()
        {
            //Arrange
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();

            var IVBRealizationDocumentExpeditionServiceMock = new Mock<IVBRealizationDocumentExpeditionService>();
            IVBRealizationDocumentExpeditionServiceMock
               .Setup(s => s.UpdateExpeditionByRealizationId(It.IsAny<int>()))
               .ReturnsAsync(1);

            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService))).Returns(IVBRealizationDocumentExpeditionServiceMock.Object);
            var service = new RealizationVbNonPOService(dbContext, serviceProviderMock.Object);
            var dataUtil = new RealizationVBNonPODataUtil(service);
            var viewmodel = dataUtil.GetNewViewModel();
            viewmodel.Items.Add(new VbNonPORequestDetailViewModel()
            {
                DateDetail = DateTimeOffset.Now,
                Amount = 123,
                Remark = "Remark",
                isGetPPn = false,
                IncomeTax = new IncomeTaxNew()
                {
                    Id = 1,
                    name = "name",
                    rate = 1
                },
                IncomeTaxBy = "income"
            });


            //Act
            var result = await service.UpdateAsync(viewmodel.Id, viewmodel);

            //Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Should_Success_Update_Model3()
        {
            //Arrange
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();

            var IVBRealizationDocumentExpeditionServiceMock = new Mock<IVBRealizationDocumentExpeditionService>();
            IVBRealizationDocumentExpeditionServiceMock
               .Setup(s => s.UpdateExpeditionByRealizationId(It.IsAny<int>()))
               .ReturnsAsync(1);

            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService))).Returns(IVBRealizationDocumentExpeditionServiceMock.Object);
            var service = new RealizationVbNonPOService(dbContext, serviceProviderMock.Object);
            var dataUtil = new RealizationVBNonPODataUtil(service);
            var viewmodel = dataUtil.GetNewViewModelNew();
            viewmodel.Items.Add(new VbNonPORequestDetailViewModel()
            {
                DateDetail = DateTimeOffset.Now,
                Amount = 0,
                Remark = "Remark",
                isGetPPn = false,
                IncomeTax = new IncomeTaxNew()
                {
                    Id = 1,
                    name = "name",
                    rate = 1
                },
                IncomeTaxBy = "income"
            });

            var dataRequestVb = dataUtil.GetNewData_RequestVB();
            dbContext.VbRequests.Add(dataRequestVb);
            dbContext.SaveChanges();

            //Act
            var result = await service.UpdateAsync(viewmodel.Id, viewmodel);

            //Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Should_Success_Update_Model4()
        {
            //Arrange
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();

            var IVBRealizationDocumentExpeditionServiceMock = new Mock<IVBRealizationDocumentExpeditionService>();
            IVBRealizationDocumentExpeditionServiceMock
              .Setup(s => s.UpdateExpeditionByRealizationId(It.IsAny<int>()))
              .ReturnsAsync(1);

            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService))).Returns(IVBRealizationDocumentExpeditionServiceMock.Object);
            var service = new RealizationVbNonPOService(dbContext, serviceProviderMock.Object);
            var dataUtil = new RealizationVBNonPODataUtil(service);
            var viewmodel = dataUtil.GetNewViewModelNew();
        
            viewmodel.Items.Add(new VbNonPORequestDetailViewModel()
            {
                DateDetail = DateTimeOffset.Now,
                Amount = -1000,
                Remark = "Remark",
                isGetPPn = false,
                IncomeTax = new IncomeTaxNew()
                {
                    Id = 1,
                    name = "name",
                    rate = 1
                },
                IncomeTaxBy = "income"
            });

            var dataRequestVb = dataUtil.GetNewData_RequestVB();
            dbContext.VbRequests.Add(dataRequestVb);
            dbContext.SaveChanges();

            //Act
            var result = await service.UpdateAsync(viewmodel.Id, viewmodel);

            //Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Should_Success_Update_Model5()
        {
            //Arrange
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();

            var IVBRealizationDocumentExpeditionServiceMock = new Mock<IVBRealizationDocumentExpeditionService>();
            IVBRealizationDocumentExpeditionServiceMock
             .Setup(s => s.UpdateExpeditionByRealizationId(It.IsAny<int>()))
             .ReturnsAsync(1);

            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService))).Returns(IVBRealizationDocumentExpeditionServiceMock.Object);
            var service = new RealizationVbNonPOService(dbContext, serviceProviderMock.Object);
            var dataUtil = new RealizationVBNonPODataUtil(service);
            var viewmodel = dataUtil.GetNewViewModelNew();
            viewmodel.Items.Add(new VbNonPORequestDetailViewModel()
            {
                DateDetail = DateTimeOffset.Now,
                Amount = -1000,
                Remark = "Remark",
                isGetPPn = false,
                isGetPPh = true,
                IncomeTax = new IncomeTaxNew()
                {
                    Id = 1,
                    name = "name",
                    rate = 1
                },
                IncomeTaxBy = "income"
            });

            var dataRequestVb = dataUtil.GetNewData_RequestVB();
            dbContext.VbRequests.Add(dataRequestVb);
            dbContext.SaveChanges();

            var viewmodel1 = dataUtil.GetNewViewModel6();

            await service.MappingData(viewmodel1);

            //Act
            var result = await service.UpdateAsync(viewmodel.Id, viewmodel);

            //Assert
            Assert.NotEqual(0, result);
        }

       

        [Fact]
        public async Task Should_Success_UpdateAsync_When_Data_Exist()
        {
            //Arrange
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();

            var IVBRealizationDocumentExpeditionServiceMock = new Mock<IVBRealizationDocumentExpeditionService>();
            IVBRealizationDocumentExpeditionServiceMock
             .Setup(s => s.UpdateExpeditionByRealizationId(It.IsAny<int>()))
             .ReturnsAsync(1);

            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService))).Returns(IVBRealizationDocumentExpeditionServiceMock.Object);
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService))).Returns(new HttpClientOthersExpenditureServiceHelper());
            var service = new RealizationVbNonPOService(dbContext, serviceProviderMock.Object);
            var dataUtil = new RealizationVBNonPODataUtil(service);
            var modelToCreate = dataUtil.GetNewData();
            var dataRequestVb = dataUtil.GetNewData_RequestVB();
            dbContext.VbRequests.Add(dataRequestVb);
            dbContext.SaveChanges();

            var viewmodel = dataUtil.GetNewViewModel5();

            var viewmodelnew = dataUtil.GetNewViewModelNew1();
            viewmodelnew.Items.Add(new VbNonPORequestDetailViewModel()
            {
                DateDetail = DateTimeOffset.Now,
                Amount = 1000,
                Remark = "Remark",
                isGetPPn = false,
                isGetPPh = true,
                IncomeTax = new IncomeTaxNew()
                {
                    Id = 1,
                    name = "name",
                    rate = 1
                },
                IncomeTaxBy = "income"
            });

            var viewmodel1 = dataUtil.GetNewViewModel6();
            await service.CreateAsync(modelToCreate, viewmodel);

            await service.MappingData(viewmodel1);

            //Act
            var result = await service.UpdateAsync(modelToCreate.Id, viewmodelnew);

            //Assert
            Assert.NotEqual(0, result);
        }

       

        [Fact]
        public async Task Should_Success_Read_Data()
        {
            //Arrange
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();

            var IVBRealizationDocumentExpeditionServiceMock = new Mock<IVBRealizationDocumentExpeditionService>();

            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService))).Returns(IVBRealizationDocumentExpeditionServiceMock.Object);
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService))).Returns(new HttpClientOthersExpenditureServiceHelper());
            
            var service = new RealizationVbNonPOService(dbContext, serviceProviderMock.Object);
            var dataUtil = new RealizationVBNonPODataUtil(service);
            await dataUtil.GetTestData();

            //Act
            var result = service.Read(1, 10, "{}", new List<string>(), "", "{}");

            //Assert
            Assert.NotEmpty(result.Data);
        }

        [Fact]
        public async Task Should_Success_Read_ById()
        {
            //Arrange
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();
            var IVBRealizationDocumentExpeditionServiceMock = new Mock<IVBRealizationDocumentExpeditionService>();

            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService))).Returns(IVBRealizationDocumentExpeditionServiceMock.Object);
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService))).Returns(new HttpClientOthersExpenditureServiceHelper());
            
            var service = new RealizationVbNonPOService(dbContext, serviceProviderMock.Object);
            var dataUtil = new RealizationVBNonPODataUtil(service);
           
            var data = await dataUtil.GetTestData();

            //Act
            var result = await service.ReadByIdAsync2(data.Id);

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Should_Success_Delete_ById()
        {
            //Arrange
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();
            var IVBRealizationDocumentExpeditionServiceMock = new Mock<IVBRealizationDocumentExpeditionService>();
            
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService))).Returns(IVBRealizationDocumentExpeditionServiceMock.Object);
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService))).Returns(new HttpClientOthersExpenditureServiceHelper());
            
            var service = new RealizationVbNonPOService(dbContext, serviceProviderMock.Object);
            var dataUtil = new RealizationVBNonPODataUtil(service);
            var data = await dataUtil.GetTestData();

            //Act
            var result = await service.DeleteAsync(data.Id);

            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Should_Success_Delete_ById2()
        {
            //Arrange
            var dbContext = GetDbContext(GetCurrentMethod());
            var serviceProviderMock = GetServiceProviderMock();
            
            var IVBRealizationDocumentExpeditionServiceMock = new Mock<IVBRealizationDocumentExpeditionService>();
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationDocumentExpeditionService))).Returns(IVBRealizationDocumentExpeditionServiceMock.Object);
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService))).Returns(new HttpClientOthersExpenditureServiceHelper());
           
            var service = new RealizationVbNonPOService(dbContext, serviceProviderMock.Object);
            var dataUtil = new RealizationVBNonPODataUtil(service);
            var data = await dataUtil.GetTestData2();

            //Act
            var result = await service.DeleteAsync(data.Id);

            //Assert
            Assert.NotEqual(0, result);
        }

        
    }
}
