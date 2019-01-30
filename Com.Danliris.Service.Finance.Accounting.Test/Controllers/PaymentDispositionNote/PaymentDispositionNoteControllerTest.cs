using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.PaymentDispositionNote;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.PaymentDispositionNote;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.ValidateService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.NewIntegrationViewModel;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.PaymentDispositionNoteViewModel;
using Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.PaymentDispositionNote;
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

namespace Com.Danliris.Service.Finance.Accounting.Test.Controllers.PaymentDispositionNote
{
    public class PaymentDispositionNoteControllerTest
    {
        private PaymentDispositionNoteViewModel ViewModel
        {
            get
            {
                return new PaymentDispositionNoteViewModel
                {
                    Supplier = new SupplierViewModel(),
                    AccountBank=new AccountBankViewModel(),
                    
                    Items = new List<PaymentDispositionNoteItemViewModel>
                    {
                        new PaymentDispositionNoteItemViewModel()
                        {
                            category=new Lib.ViewModels.IntegrationViewModel.CategoryViewModel(),
                            Details=new List<PaymentDispositionNoteDetailViewModel>()
                            {
                                new PaymentDispositionNoteDetailViewModel()
                                {
                                    product = new Lib.ViewModels.IntegrationViewModel.ProductViewModel(),
                                    unit = new Lib.ViewModels.IntegrationViewModel.UnitViewModel(),
                                }
                            }
                        }
                    }
                };
            }
        }
        public (Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IPaymentDispositionNoteService> Service, Mock<IMapper> Mapper) GetMocks()
        {
            return (IdentityService: new Mock<IIdentityService>(), ValidateService: new Mock<IValidateService>(), Service: new Mock<IPaymentDispositionNoteService>(), Mapper: new Mock<IMapper>());
        }

        protected PaymentDispositionNoteController GetController((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IPaymentDispositionNoteService> Service, Mock<IMapper> Mapper) mocks)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                    new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            PaymentDispositionNoteController controller = new PaymentDispositionNoteController(mocks.IdentityService.Object, mocks.ValidateService.Object, mocks.Mapper.Object, mocks.Service.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");
            return controller;
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        private ServiceValidationExeption GetServiceValidationExeption()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(this.ViewModel, serviceProvider.Object, null);
            return new ServiceValidationExeption(validationContext, validationResults);
        }

        private int GetStatusCodeGet((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IPaymentDispositionNoteService> Service, Mock<IMapper> Mapper) mocks)
        {
            PaymentDispositionNoteController controller = GetController(mocks);
            IActionResult response = controller.Get();

            return GetStatusCode(response);
        }

        //[Fact]
        //public void Get_WithoutException_ReturnOK()
        //{
        //    var mocks = GetMocks();
        //    mocks.Service.Setup(f => f.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new ReadResponse<PaymentDispositionNoteModel>(new List<PaymentDispositionNoteModel>(), 0, new Dictionary<string, string>(), new List<string>()));
        //    mocks.Mapper.Setup(f => f.Map<List<PaymentDispositionNoteViewModel>>(It.IsAny<List<PaymentDispositionNoteViewModel>>())).Returns(new List<PaymentDispositionNoteViewModel>());

        //    var response = GetController(mocks).Get();
        //    Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        //}

        [Fact]
        public void Get_ReadThrowException_ReturnInternalServerError()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());

            int statusCode = GetStatusCodeGet(mocks);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        //private async Task<int> GetStatusCodeGetById((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IPaymentDispositionNoteService> Service, Mock<IMapper> Mapper) mocks)
        //{
        //    PaymentDispositionNoteController controller = GetController(mocks);
        //    IActionResult response = await controller.GetById(1);

        //    return GetStatusCode(response);
        //}

        //[Fact]
        //public async Task GetById_NotNullModel_ReturnOK()
        //{
        //    var mocks = GetMocks();
        //    mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(new PurchasingDispositionExpeditionModel());

        //    int statusCode = await GetStatusCodeGetById(mocks);
        //    Assert.Equal((int)HttpStatusCode.OK, statusCode);
        //}

        //[Fact]
        //public async Task GetById_NullModel_ReturnNotFound()
        //{
        //    var mocks = GetMocks();
        //    mocks.Mapper.Setup(f => f.Map<PurchasingDispositionExpeditionViewModel>(It.IsAny<PurchasingDispositionExpeditionModel>())).Returns(ViewModel);
        //    mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync((PurchasingDispositionExpeditionModel)null);

        //    int statusCode = await GetStatusCodeGetById(mocks);
        //    Assert.Equal((int)HttpStatusCode.NotFound, statusCode);
        //}

        //[Fact]
        //public async Task GetById_ThrowException_ReturnInternalServerError()
        //{
        //    var mocks = GetMocks();
        //    mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

        //    int statusCode = await GetStatusCodeGetById(mocks);
        //    Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        //}
    }
}
