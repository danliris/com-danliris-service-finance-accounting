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
        protected (Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IGarmentPurchasingPphBankExpenditureNoteService> Service, Mock<IMapper> Mapper ) GetMocks()
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
            GarmentPurchasingPphBankExpenditureNoteController controller =new  GarmentPurchasingPphBankExpenditureNoteController( mocks.IdentityService.Object, mocks.ValidateService.Object, mocks.Service.Object, mocks.Mapper.Object);
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
        public void  Get_Return_OK()
        {
            var mocks = GetMocks();
            mocks.Service
                .Setup(f => f.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ReadResponse<GarmentPurchasingPphBankExpenditureNoteDataViewModel>(new List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>(), 1, new Dictionary<string, string>(), new List<string>()));

            mocks.Mapper
                .Setup(f => f.Map<List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>>(It.IsAny<List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>>()))
                .Returns(new List<GarmentPurchasingPphBankExpenditureNoteDataViewModel>());

            var controller = GetController(mocks);
            var response =  controller.Get();

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
            var response =await controller.Post(viewModel);

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
    }
}
