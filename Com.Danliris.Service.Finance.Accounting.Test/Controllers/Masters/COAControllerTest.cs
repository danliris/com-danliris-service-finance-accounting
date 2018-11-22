using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.AutoMapperProfiles.Master;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.Master;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MasterCOA;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.ValidateService;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.MasterCOA;
using Com.Danliris.Service.Finance.Accounting.Test.Controller.Utils;
using Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.Master;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Controllers.Masters
{
    public class COAControllerTest : BaseControllerTest<COAController, COAModel, COAViewModel, ICOAService>
    {
        [Fact]
        public void UploadFile_WithoutException_ReturnOK()
        {
            string header = "Kode; Nama;Path;Report Type;Nature;Cash Account";
            var mockFacade = new Mock<ICOAService>();
            mockFacade.Setup(f => f.UploadData(It.IsAny<List<COAModel>>())).Verifiable();
            mockFacade.Setup(f => f.CsvHeader).Returns(header.Split(';').ToList());
            
            mockFacade.Setup(f => f.UploadValidate(ref It.Ref<List<COAViewModel>>.IsAny, It.IsAny<List<KeyValuePair<string, StringValues>>>())).Returns(new Tuple<bool, List<object>>(true, new List<object>()));
            COAProfile profile = new COAProfile();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.ConfigurationProvider).Returns(new MapperConfiguration(cfg => cfg.AddProfile(profile)));

            mockMapper.Setup(x => x.Map<List<COAModel>>(It.IsAny<List<COAViewModel>>())).Returns(new List<COAModel>() { Model });
            var mockIdentityService = new Mock<IIdentityService>();
            var mockValidateService = new Mock<IValidateService>();

            var controller = GetController((mockIdentityService, mockValidateService, mockFacade, mockMapper));
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = $"{It.IsAny<int>()}";
            controller.ControllerContext.HttpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(header + "\n" + header)), 0, Encoding.UTF8.GetBytes(header + "\n" + header).LongLength, "Data", "test.csv");
            controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection { file });

            var response = controller.PostCSVFileAsync();
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

            var controller = GetController((mockIdentityService, mockValidateService, mockFacade, mockMapper));
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = $"{It.IsAny<int>()}";

            var response = controller.PostCSVFileAsync();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response.Result));
        }

        [Fact]
        public void UploadFile_WithException_FileNotFound()
        {
            string header = "Kode; Nama;Path;Report Type;Nature;Cash Account";
            var mockFacade = new Mock<ICOAService>();
            mockFacade.Setup(f => f.UploadData(It.IsAny<List<COAModel>>())).Verifiable();
            mockFacade.Setup(f => f.CsvHeader).Returns(header.Split(';').ToList());
            mockFacade.Setup(f => f.UploadValidate(ref It.Ref<List<COAViewModel>>.IsAny, It.IsAny<List<KeyValuePair<string, StringValues>>>())).Returns(new Tuple<bool, List<object>>(false, new List<object>()));
            COAProfile profile = new COAProfile();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.ConfigurationProvider).Returns(new MapperConfiguration(cfg => cfg.AddProfile(profile)));

            mockMapper.Setup(x => x.Map<List<COAModel>>(It.IsAny<List<COAViewModel>>())).Returns(new List<COAModel>() { Model });
            var mockIdentityService = new Mock<IIdentityService>();
            var mockValidateService = new Mock<IValidateService>();

            var controller = GetController((mockIdentityService, mockValidateService, mockFacade, mockMapper));
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = $"{It.IsAny<int>()}";
            controller.ControllerContext.HttpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(header + "\n" + header)), 0, Encoding.UTF8.GetBytes(header + "\n" + header).LongLength, "Data", "test.csv");
            controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection { });

            var response = controller.PostCSVFileAsync();
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response.Result));
        }

        [Fact]
        public void UploadFile_WithException_CSVError()
        {
            string header = "Kode; Nama;Path;Report Type;Nature;Cash Account";
            var mockFacade = new Mock<ICOAService>();
            mockFacade.Setup(f => f.UploadData(It.IsAny<List<COAModel>>())).Verifiable();
            mockFacade.Setup(f => f.CsvHeader).Returns(header.Split(',').ToList());
            var data = It.IsAny<List<COAViewModel>>();
            mockFacade.Setup(f => f.UploadValidate(ref It.Ref<List<COAViewModel>>.IsAny, It.IsAny<List<KeyValuePair<string, StringValues>>>())).Returns(new Tuple<bool, List<object>>(false, new List<object>()));
            COAProfile profile = new COAProfile();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.ConfigurationProvider).Returns(new MapperConfiguration(cfg => cfg.AddProfile(profile)));

            mockMapper.Setup(x => x.Map<List<COAModel>>(It.IsAny<List<COAViewModel>>())).Returns(new List<COAModel>() { Model });
            var mockIdentityService = new Mock<IIdentityService>();
            var mockValidateService = new Mock<IValidateService>();

            var controller = GetController((mockIdentityService, mockValidateService, mockFacade, mockMapper));
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = $"{It.IsAny<int>()}";
            controller.ControllerContext.HttpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(header + "\n" + header)), 0, Encoding.UTF8.GetBytes(header + "\n" + header).LongLength, "Data", "test.csv");
            controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection { file });

            var response = controller.PostCSVFileAsync();
            Assert.Equal((int)HttpStatusCode.NotFound, GetStatusCode(response.Result));
        }

        [Fact]
        public void UploadFile_WithException_ErrorInFile()
        {
            string header = "Kode; Nama;Path;Report Type;Nature;Cash Account";
            var mockFacade = new Mock<ICOAService>();
            mockFacade.Setup(f => f.UploadData(It.IsAny<List<COAModel>>())).Verifiable();
            mockFacade.Setup(f => f.CsvHeader).Returns(header.Split(';').ToList());
            var data = It.IsAny<List<COAViewModel>>();
            mockFacade.Setup(f => f.UploadValidate(ref It.Ref<List<COAViewModel>>.IsAny, It.IsAny<List<KeyValuePair<string, StringValues>>>())).Returns(new Tuple<bool, List<object>>(false, new List<object>()));
            COAProfile profile = new COAProfile();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.ConfigurationProvider).Returns(new MapperConfiguration(cfg => cfg.AddProfile(profile)));

            mockMapper.Setup(x => x.Map<List<COAModel>>(It.IsAny<List<COAViewModel>>())).Returns(new List<COAModel>() { Model });
            var mockIdentityService = new Mock<IIdentityService>();
            var mockValidateService = new Mock<IValidateService>();

            var controller = GetController((mockIdentityService, mockValidateService, mockFacade, mockMapper));
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = $"{It.IsAny<int>()}";
            controller.ControllerContext.HttpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(header + "\n" + header)), 0, Encoding.UTF8.GetBytes(header + "\n" + header).LongLength, "Data", "test.csv");
            controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection { file });

            var response = controller.PostCSVFileAsync();
            Assert.NotNull(response.Result);
        }
    }
}
