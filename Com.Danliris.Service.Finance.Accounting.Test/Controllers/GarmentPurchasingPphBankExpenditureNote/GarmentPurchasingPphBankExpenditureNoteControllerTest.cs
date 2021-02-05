using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentPurchasingPphBankExpenditureNote;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.ValidateService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.GarmentPurchasingPphBankExpenditureNoteViewModels;
using Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.GarmentPurchasingPphBankExpenditureNote;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Controllers.GarmentPurchasingPphBankExpenditureNote
{
    public class GarmentPurchasingPphBankExpenditureNoteControllerTest
    {
        protected (Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IGarmentPurchasingPphBankExpenditureNoteService> Service, Mock<IMapper> Mapper) GetMocks()
        {
            return (IdentityService: new Mock<IIdentityService>(), ValidateService: new Mock<IValidateService>(), Service: new Mock<IGarmentPurchasingPphBankExpenditureNoteService>(), Mapper: new Mock<IMapper>());
        }

        protected GarmentPurchasingPphBankExpenditureNoteController GetController((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IGarmentPurchasingPphBankExpenditureNoteService> Service, Mock<IMapper> Mapper) mocks)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentPurchasingPphBankExpenditureNoteController controller = new GarmentPurchasingPphBankExpenditureNoteController(mocks.IdentityService.Object, mocks.ValidateService.Object, mocks.Service.Object, mocks.Mapper.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "7";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");
            return controller;
        }


        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        protected ServiceValidationException GetServiceValidationExeption(dynamic obj)
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj, serviceProvider.Object, null);
            return new ServiceValidationException(validationContext, validationResults);
        }

        [Fact]
        public void Get_Return_OK()
        {
            var mocks = GetMocks();
            mocks.Service
                .Setup(f => f.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ReadResponse<GarmentPurchasingPphBankExpenditureNoteDataViewModel>(new List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>(), 1, new Dictionary<string, string>(), new List<string>()));

            mocks.Mapper
                .Setup(f => f.Map<List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>>(It.IsAny<List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>>()))
                .Returns(new List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>());

            var controller = GetController(mocks);
            var response = controller.Get();

            int statusCode = GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public void Get_Return_InternalServerError()
        {
            var mocks = GetMocks();
            mocks.Service
                .Setup(f => f.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());

            mocks.Mapper
                .Setup(f => f.Map<List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>>(It.IsAny<List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>>()))
                .Returns(new List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>());

            var controller = GetController(mocks);
            var response = controller.Get();

            int statusCode = GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }


        [Fact]
        public async Task Post_Return_Created()
        {
            var mocks = GetMocks();
            mocks.Service
                .Setup(f => f.CreateAsync(It.IsAny<FormInsert>()))
                .Returns(Task.FromResult(1));


            mocks.Mapper
                .Setup(f => f.Map<List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>>(It.IsAny<List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>>()))
                .Returns(new List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>());

            var controller = GetController(mocks);

            FormInsert viewModel = new FormInsert();
            var response = await controller.Post(viewModel);

            int statusCode = GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.Created, statusCode);
        }

        [Fact]
        public async Task Post_Return_ValidationException()
        {
            var mocks = GetMocks();
            FormInsert viewModel = new FormInsert();
            mocks.Service
                .Setup(f => f.CreateAsync(It.IsAny<FormInsert>()))
                .ThrowsAsync(GetServiceValidationExeption(viewModel));


            mocks.Mapper
                .Setup(f => f.Map<List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>>(It.IsAny<List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>>()))
                .Returns(new List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>());

            var controller = GetController(mocks);


            var response = await controller.Post(viewModel);

            int statusCode = GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public async Task Post_Return_InternalServerError()
        {
            var mocks = GetMocks();
            FormInsert viewModel = new FormInsert();
            mocks.Service
                .Setup(f => f.CreateAsync(It.IsAny<FormInsert>()))
                .ThrowsAsync(new Exception());


            mocks.Mapper
                .Setup(f => f.Map<List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>>(It.IsAny<List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>>()))
                .Returns(new List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>());

            var controller = GetController(mocks);


            var response = await controller.Post(viewModel);

            int statusCode = GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public async Task Put_Return_Created()
        {
            var mocks = GetMocks();
            mocks.Service
                .Setup(f => f.UpdateAsync(It.IsAny<FormInsert>()))
                .Returns(Task.FromResult(1));


            mocks.Mapper
                .Setup(f => f.Map<List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>>(It.IsAny<List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>>()))
                .Returns(new List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>());

            var controller = GetController(mocks);

            FormInsert viewModel = new FormInsert();
            var response = await controller.Put(viewModel);

            int statusCode = GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.Created, statusCode);
        }

        [Fact]
        public async Task Put_Return_ValidationException()
        {
            var mocks = GetMocks();
            FormInsert viewModel = new FormInsert();
            mocks.Service
                .Setup(f => f.UpdateAsync(It.IsAny<FormInsert>()))
                .ThrowsAsync(GetServiceValidationExeption(viewModel));


            mocks.Mapper
                .Setup(f => f.Map<List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>>(It.IsAny<List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>>()))
                .Returns(new List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>());

            var controller = GetController(mocks);


            var response = await controller.Put(viewModel);

            int statusCode = GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public async Task Put_Return_InternalServerError()
        {
            var mocks = GetMocks();
            FormInsert viewModel = new FormInsert();
            mocks.Service
                .Setup(f => f.UpdateAsync(It.IsAny<FormInsert>()))
                .ThrowsAsync(new Exception());


            mocks.Mapper
                .Setup(f => f.Map<List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>>(It.IsAny<List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>>()))
                .Returns(new List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>());

            var controller = GetController(mocks);


            var response = await controller.Put(viewModel);

            int statusCode = GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public async Task DownloadPdfById_Return_PdfFile()
        {
            //Arrange
            var mocks = GetMocks();

            FormInsert formInsert = new FormInsert()
            {

                Bank = new Bank()
                {
                    AccountCOA = "AccountCOA",
                    AccountName = "AccountName",
                    AccountNumber = "AccountNumber",
                    BankAddress = "BankAddress",
                    BankCode = "BankCode",
                    BankName = "BankName",
                    Code = "Code",
                    Currency = new Currency()
                    {
                        Code = "COde",
                        Description = "Description",
                        Rate = 1
                    },
                    SwiftCode = "SwiftCode",


                },

                Date = DateTimeOffset.Now,
                DateFrom = DateTimeOffset.Now.AddDays(-1),
                DateTo = DateTimeOffset.Now.AddDays(1),
                IncomeTax = new IncomeTax()
                {
                    Description = "Description"
                },
                IsPosted = false,
                PPHBankExpenditureNoteItems = new List<FormAdd>() {
                new FormAdd()
                {
                    CurrencyCode="IDR",
                    CurrencyRate=1,

                    GarmentInvoice=new GarmentPurchasingInvoiceInfoDto()
                    {
                        CurrencyCode="IDR",
                        HasInternNote=false,
                        IncomeTaxDate=DateTimeOffset.Now,
                        CurrencyId=1,
                        IncomeTaxId=1,
                        IncomeTaxName="IncomeTaxName",
                        IncomeTaxNo="IncomeTaxNo",
                        IncomeTaxRate=1,
                        InvoiceDate=DateTimeOffset.Now,
                        InvoiceNo="InvoiceNo",
                        IsPayTax=false,
                        IsPayVat=false,
                        NPH="NPH",
                        NPN="NPN",
                        SupplierCode="SupplierCode",
                        SupplierName="SupplierName",
                        TotalAmount=1,
                        SupplierId=1,
                        UseIncomeTax=false,
                        UseVat=false,
                        VatDate=DateTimeOffset.Now,
                        VatNo="VatNo",

                    },
                    TotalIncomeTaxNI=1,
                    CurrencyId=1,
                    INDueDate=DateTimeOffset.Now,
                    Position=1,
                    Remark="Remark",
                    SupplierCode="SupplierCode",
                    IsCreatedVB=false,
                    INId=1,
                    INNo="INNo",
                    SupplierId=1,
                    SupplierName="SupplierName",
                       INDate=DateTimeOffset.Now,
                       Items=new List<Item>()
                       {
                           new Item()
                           {
                               ProductCode="ProductCode",
                               Details=new List<Detail>()
                               {
                                   new Detail()
                                   {
                                       POSerialNumber="POSerialNumber",
                                       PaymentType="PaymentType",
                                       PaymentMethod="PaymentMethod",
                                       PaymentDueDays=1,
                                       PaymentDueDate=DateTimeOffset.Now.AddDays(1),
                                       DODate=DateTimeOffset.Now.AddDays(1),
                                       DOId=1,
                                       DONo="DONo",
                                       EPOId=1,
                                       EPONo="EPONo",
                                       GarmentDeliveryOrder=new DeliveryOrderInfo(),
                                       InvoiceDate=DateTimeOffset.Now,
                                       InvoiceDetailId=1,
                                       InvoiceId=1,
                                       InvoiceNo="InvoiceNo",
                                       InvoiceTotalAmount=1,
                                       PricePerDealUnit=1,
                                       PriceTotal=1,
                                       ProductCategory="ProductCategory",
                                       ProductCode="ProductCode",
                                       ProductId=1,
                                       ProductName="ProductName",
                                       Quantity=1,
                                       RONo="RONo",
                                       UnitCode="UnitCode",
                                       UnitId="UnitId",
                                       UnitName="UnitName",
                                       UOMId=1,
                                       UOMUnit="UOMUnit"
                                   }
                               }
                           }
                       }

,                }
                },
                PphBankInvoiceNo = "PphBankInvoiceNo",

            };
            mocks.Service
                .Setup(f => f.ReadByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(formInsert);


            var controller = GetController(mocks);

            //Act
            var response = await controller.DownloadPdfById(1);

            //Assert
            Assert.NotNull(response);
            Assert.Equal("application/pdf", response.GetType().GetProperty("ContentType").GetValue(response, null));
        }


        [Fact]
        public async Task DownloadPdfById_Return_InternalServerError()
        {
            //Arrange
            var mocks = GetMocks();

            mocks.Service
                .Setup(f => f.ReadByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception());


            var controller = GetController(mocks);

            //Act
            var response = await controller.DownloadPdfById(1);

            //Assert
            int statusCode = GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }


        [Fact]
        public async Task GetReport_Return_OK()
        {
            //Arrange
            var mocks = GetMocks();



            mocks.Service
                .Setup(f => f.GetReportView(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<GarmentPurchasingPphBankExpenditureNoteFilterReportDto>()))
                .Returns(new ReadResponse<GarmentPurchasingPphBankExpenditureNoteReportViewDto>(new List<GarmentPurchasingPphBankExpenditureNoteReportViewDto>(), 1, new Dictionary<string, string>(), new List<string>()));


            var controller = GetController(mocks);

            //Act
            GarmentPurchasingPphBankExpenditureNoteFilterReportDto filter = new GarmentPurchasingPphBankExpenditureNoteFilterReportDto()
            {
                DateEnd = DateTime.Now.AddDays(2),
                DateStart = DateTime.Now.AddDays(-1),
                INNo = "INNo",
                InvoiceNo = "InvoiceNo",
                InvoiceOutNo = "InvoiceOutNo",
                SupplierName = "SupplierName"
            };
            var response = await controller.GetReport(filter, 1, 25, "{}");

            //Assert
            int statusCode = GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public async Task GetReport_Return_InternalServerError()
        {
            //Arrange
            var mocks = GetMocks();

            mocks.Service
                .Setup(f => f.GetReportView(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<GarmentPurchasingPphBankExpenditureNoteFilterReportDto>()))
                .Throws(new Exception());


            var controller = GetController(mocks);

            //Act
            GarmentPurchasingPphBankExpenditureNoteFilterReportDto filter = new GarmentPurchasingPphBankExpenditureNoteFilterReportDto()
            {
                DateEnd = DateTime.Now.AddDays(2),
                DateStart = DateTime.Now.AddDays(-1),
                INNo = "INNo",
                InvoiceNo = "InvoiceNo",
                InvoiceOutNo = "InvoiceOutNo",
                SupplierName = "SupplierName"
            };
            var response = await controller.GetReport(filter, 1, 25, "{}");

            //Assert
            int statusCode = GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }


        [Fact]
        public async Task GetReportGroup_Return_OK()
        {
            //Arrange
            var mocks = GetMocks();

            mocks.Service
                .Setup(f => f.GetReportGroupView(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<GarmentPurchasingPphBankExpenditureNoteFilterReportDto>()))
                .Returns(new ReadResponse<GarmentPurchasingPphBankExpenditureNoteReportGroupView>(new List<GarmentPurchasingPphBankExpenditureNoteReportGroupView>(), 1, new Dictionary<string, string>(), new List<string>()));

            var controller = GetController(mocks);

            //Act
            GarmentPurchasingPphBankExpenditureNoteFilterReportDto filter = new GarmentPurchasingPphBankExpenditureNoteFilterReportDto()
            {
                DateEnd = DateTime.Now.AddDays(2),
                DateStart = DateTime.Now.AddDays(-1),
                INNo = "INNo",
                InvoiceNo = "InvoiceNo",
                InvoiceOutNo = "InvoiceOutNo",
                SupplierName = "SupplierName"
            };
            var response = await controller.GetReportGroup(filter, 1, 25, "{}");

            //Assert
            int statusCode = GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }


        [Fact]
        public async Task GetReportGroup_Return_InternalServerError()
        {
            //Arrange
            var mocks = GetMocks();

            mocks.Service
                .Setup(f => f.GetReportGroupView(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<GarmentPurchasingPphBankExpenditureNoteFilterReportDto>()))
                .Throws(new Exception());

            var controller = GetController(mocks);

            //Act
            GarmentPurchasingPphBankExpenditureNoteFilterReportDto filter = new GarmentPurchasingPphBankExpenditureNoteFilterReportDto()
            {
                DateEnd = DateTime.Now.AddDays(2),
                DateStart = DateTime.Now.AddDays(-1),
                INNo = "INNo",
                InvoiceNo = "InvoiceNo",
                InvoiceOutNo = "InvoiceOutNo",
                SupplierName = "SupplierName"
            };
            var response = await controller.GetReportGroup(filter, 1, 25, "{}");

            //Assert
            int statusCode = GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public async Task DownloadReportXls_Return_XlsFile()
        {
            //Arrange
            var mocks = GetMocks();

            mocks.Service
                .Setup(f => f.DownloadReportXls(It.IsAny<GarmentPurchasingPphBankExpenditureNoteFilterReportDto>()))
                .Returns(new System.IO.MemoryStream());

            var controller = GetController(mocks);

            //Act
            GarmentPurchasingPphBankExpenditureNoteFilterReportDto filter = new GarmentPurchasingPphBankExpenditureNoteFilterReportDto()
            {
                DateEnd = DateTime.Now.AddDays(2),
                DateStart = DateTime.Now.AddDays(-1),
                INNo = "INNo",
                InvoiceNo = "InvoiceNo",
                InvoiceOutNo = "InvoiceOutNo",
                SupplierName = "SupplierName"
            };
            var response = await controller.DownloadReportXls(filter, 1, 25, "{}");

            //Assert
            Assert.NotNull(response);
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));
        }

        [Fact]
        public async Task DownloadReportXls_Return_InternalServerError()
        {
            //Arrange
            var mocks = GetMocks();

            mocks.Service
                .Setup(f => f.DownloadReportXls(It.IsAny<GarmentPurchasingPphBankExpenditureNoteFilterReportDto>()))
                .Throws(new Exception());

            var controller = GetController(mocks);

            //Act
            GarmentPurchasingPphBankExpenditureNoteFilterReportDto filter = new GarmentPurchasingPphBankExpenditureNoteFilterReportDto()
            {
                DateEnd = DateTime.Now.AddDays(2),
                DateStart = DateTime.Now.AddDays(-1),
                INNo = "INNo",
                InvoiceNo = "InvoiceNo",
                InvoiceOutNo = "InvoiceOutNo",
                SupplierName = "SupplierName"
            };
            var response = await controller.DownloadReportXls(filter, 1, 25, "{}");

            //Assert
            int statusCode = GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public async Task GetLoaderInternNote_Return_OK()
        {
            //Arrange
            var mocks = GetMocks();

            GarmentPurchasingPphBankExpenditureNoteLoaderInternNote data = new GarmentPurchasingPphBankExpenditureNoteLoaderInternNote()
            {
                Name = ""
            };
            mocks.Service
                .Setup(f => f.GetLoaderInterNotePPH(It.IsAny<string>()))
                .Returns(new List<GarmentPurchasingPphBankExpenditureNoteLoaderInternNote>() {
                data
                });

            var controller = GetController(mocks);

            //Act
            GarmentPurchasingPphBankExpenditureNoteFilterReportDto filter = new GarmentPurchasingPphBankExpenditureNoteFilterReportDto()
            {
                DateEnd = DateTime.Now.AddDays(2),
                DateStart = DateTime.Now.AddDays(-1),
                INNo = "INNo",
                InvoiceNo = "InvoiceNo",
                InvoiceOutNo = "InvoiceOutNo",
                SupplierName = "SupplierName"
            };
            var response = await controller.GetLoaderInternNote("");

            //Assert
            int statusCode = GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }


        [Fact]
        public async Task GetLoaderInternNote_Return_InternalServerError()
        {
            //Arrange
            var mocks = GetMocks();

            GarmentPurchasingPphBankExpenditureNoteLoaderInternNote data = new GarmentPurchasingPphBankExpenditureNoteLoaderInternNote()
            {
                Name = ""
            };
            mocks.Service
                .Setup(f => f.GetLoaderInterNotePPH(It.IsAny<string>()))
                .Throws(new Exception());

            var controller = GetController(mocks);

            //Act
            GarmentPurchasingPphBankExpenditureNoteFilterReportDto filter = new GarmentPurchasingPphBankExpenditureNoteFilterReportDto()
            {
                DateEnd = DateTime.Now.AddDays(2),
                DateStart = DateTime.Now.AddDays(-1),
                INNo = "INNo",
                InvoiceNo = "InvoiceNo",
                InvoiceOutNo = "InvoiceOutNo",
                SupplierName = "SupplierName"
            };
            var response = await controller.DownloadReportXls(filter, 1, 25, "{}");

            //Assert
            int statusCode = GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }


    }
}
