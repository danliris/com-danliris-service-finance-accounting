using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.Master;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MasterCOA;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.ValidateService;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.MasterCOA;
using Com.Danliris.Service.Finance.Accounting.Test.Controller.Utils;
using Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.Master;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Controllers.Masters
{
    public class COAControllerTest : BaseControllerTest<COAController, COAModel, COAViewModel, ICOAService>
    {
        [Fact]
        public void UploadFile_WithoutException_ReturnOK()
        {
            var mockFacade = new Mock<ICOAService>();
            mockFacade.Setup(f => f.UploadData(It.IsAny<List<COAModel>>()));

            var mockMapper = new Mock<IMapper>();

            var mockIdentityService = new Mock<IIdentityService>();

            var mockValidateService = new Mock<IValidateService>();

            COAController controller = new COAController(mockIdentityService.Object, mockValidateService.Object, mockFacade.Object, mockMapper.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = $"{It.IsAny<int>()}";

            var response = controller.PostCSVFileAsync(It.IsAny<IFormFile>());
            Assert.NotNull(response.Result);
        }

        [Fact]
        public void UploadFile_WithException_ReturnError()
        {
            var mockFacade = new Mock<ICOAService>();
            mockFacade.Setup(f => f.UploadData(It.IsAny<List<COAModel>>())).Throws(new Exception());

            var mockMapper = new Mock<IMapper>();

            var mockIdentityService = new Mock<IIdentityService>();

            var mockValidateService = new Mock<IValidateService>();

            COAController controller = new COAController(mockIdentityService.Object, mockValidateService.Object, mockFacade.Object, mockMapper.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = $"{It.IsAny<int>()}";

            var response = controller.PostCSVFileAsync(It.IsAny<IFormFile>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response.Result));
        }
    }
}
