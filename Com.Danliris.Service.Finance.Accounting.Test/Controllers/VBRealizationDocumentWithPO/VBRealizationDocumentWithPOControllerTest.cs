using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRealizationDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRequestDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRealizationDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.ValidateService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Controllers.VBRealizationDocumentWithPO
{
    public class VBRealizationDocumentWithPOControllerTest
    {
        protected VBRealizationDocumentWithPOController GetController(Mock<IServiceProvider> serviceProvider)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            VBRealizationDocumentWithPOController controller = new VBRealizationDocumentWithPOController(serviceProvider.Object);
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

        Mock<IServiceProvider> GetServiceProvider()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
              .Setup(s => s.GetService(typeof(IIdentityService)))
              .Returns(new IdentityService() { TimezoneOffset = 1, Token = "token", Username = "username" });

            var validateService = new Mock<IValidateService>();
            serviceProvider
              .Setup(s => s.GetService(typeof(IValidateService)))
              .Returns(validateService.Object);
            return serviceProvider;
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        protected ServiceValidationException GetServiceValidationException()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            var validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(formDto, serviceProvider.Object, null);
            return new ServiceValidationException(validationContext, validationResults);
        }

        FormDto formDto
        {
            get
            {
                return new FormDto()
                {
                    Id = 1,
                    Currency = new CurrencyDto()
                    {
                        Id=1,
                        Code = "IDR",
                        Description = "Description",
                        Rate = 1,
                        Symbol = "Rp"
                    },
                    
                    Date = DateTimeOffset.Now,
                    SuppliantUnit = new UnitDto()
                    {
                        Id=1,
                        Code = "Code",
                        Name = "Name",
                        Division = new DivisionDto()
                        {
                            Id = 1,
                            Code = "Code",
                            Name = "Name"
                        },
                        VBDocumentLayoutOrder = 12
                    },
                    Type = "With PO",
                    VBRequestDocument = new VBRequestDocumentDto()
                    {
                        Id = 1,
                    },
                    Items = new List<FormItemDto>()
                    {
                        new FormItemDto()
                        {
                            UnitPaymentOrder=new UnitPaymentOrderDto()
                            {
                                Id=1,
                                No="1",
                                Items=new List<UnitPaymentOrderItemDto>()
                                {
                                    new UnitPaymentOrderItemDto()
                                    {
                                        Amount=1,
                                        IncomeTaxBy="Supplier",
                                        Remark="Remark",
                                        IncomeTax=new IncomeTaxDto()
                                        {
                                            Id=1,
                                            Name="Name",
                                            Rate=1
                                        },
                                        UseIncomeTax=true,
                                        UseVat=true,
                                        Date=DateTimeOffset.Now,
                                        
                                    }
                                }
                            }
                        }
                    }
                };
            }
        }

        [Fact]
        public void Get_Return_OK()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IVBRealizationWithPOService>();

            Dictionary<string, string> order = new Dictionary<string, string>();
            service
                .Setup(s => s.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ReadResponse<VBRealizationDocumentModel>(new List<VBRealizationDocumentModel>(), 1, order, new List<string>()));

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationWithPOService)))
               .Returns(service.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).Get();

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public void Get_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IVBRealizationWithPOService>();

            service
                .Setup(s => s.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationWithPOService)))
               .Returns(service.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).Get();

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void Post_Return_OK()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IVBRealizationWithPOService>();

            service
                .Setup(s => s.Create(It.IsAny<FormDto>()))
                .Returns(1);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationWithPOService)))
               .Returns(service.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).Post(formDto);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.Created, statusCode);
        }

        [Fact]
        public void Post_Throws_ServiceValidationException()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IVBRealizationWithPOService>();

            service
                .Setup(s => s.Create(It.IsAny<FormDto>()))
                .Throws(GetServiceValidationException());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationWithPOService)))
               .Returns(service.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).Post(formDto);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public void Post_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IVBRealizationWithPOService>();

            service
                .Setup(s => s.Create(It.IsAny<FormDto>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationWithPOService)))
               .Returns(service.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).Post(formDto);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void GetById_Return_OK()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IVBRealizationWithPOService>();

            service
                .Setup(s => s.ReadById(It.IsAny<int>()))
                .Returns(new VBRealizationWithPODto());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationWithPOService)))
               .Returns(service.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).GetById(1);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public void GetById_Return_NotFound()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IVBRealizationWithPOService>();

            service
                .Setup(s => s.ReadById(It.IsAny<int>()))
                .Returns(() => null);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationWithPOService)))
               .Returns(service.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).GetById(1);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.NotFound, statusCode);
        }

        [Fact]
        public void GetById_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IVBRealizationWithPOService>();

            service
                .Setup(s => s.ReadById(It.IsAny<int>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationWithPOService)))
               .Returns(service.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).GetById(1);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void Delete_Return_NoContent()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IVBRealizationWithPOService>();

            service
                .Setup(s => s.Delete(It.IsAny<int>()))
                .Returns(1);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationWithPOService)))
               .Returns(service.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).Delete(1);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCode);
        }

        [Fact]
        public void Delete_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IVBRealizationWithPOService>();

            service
                .Setup(s => s.Delete(It.IsAny<int>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationWithPOService)))
               .Returns(service.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).Delete(1);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void Put_Success_Return_NoContent()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IVBRealizationWithPOService>();

            service
                .Setup(s => s.Update(It.IsAny<int>(), It.IsAny<FormDto>()))
                .Returns(1);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationWithPOService)))
               .Returns(service.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).Put(formDto.Id, formDto);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCode);
        }

        [Fact]
        public void Put_Return_BadRequest()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IVBRealizationWithPOService>();

            service
                .Setup(s => s.Update(It.IsAny<int>(), It.IsAny<FormDto>()))
                .Returns(1);

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationWithPOService)))
               .Returns(service.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).Put(formDto.Id + 1, formDto);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public void Put_Throws_ServiceValidationException_Return_BadRequest()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IVBRealizationWithPOService>();

            service
                .Setup(s => s.Update(It.IsAny<int>(), It.IsAny<FormDto>()))
                .Throws(GetServiceValidationException());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationWithPOService)))
               .Returns(service.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).Put(formDto.Id, formDto);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public void Put_Return_InternalServerError()
        {
            //Setup
            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            var service = new Mock<IVBRealizationWithPOService>();

            service
                .Setup(s => s.Update(It.IsAny<int>(), It.IsAny<FormDto>()))
                .Throws(new Exception());

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(IVBRealizationWithPOService)))
               .Returns(service.Object);

            //Act
            IActionResult response = GetController(serviceProviderMock).Put(formDto.Id, formDto);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }
    }
}
