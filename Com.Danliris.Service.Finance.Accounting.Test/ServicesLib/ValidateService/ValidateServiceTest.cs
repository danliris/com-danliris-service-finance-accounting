using System;
using System.Collections.Generic;
using System.Text;
using Com.Danliris.Service.Finance.Accounting.Lib.Services;
using Moq;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.ServicesLib.ValidateService
{
   public class ValidateServiceTest
    {

        [Fact]
        public void Should_Succes_Instantiate_validateService()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            var sut = new Lib.Services.ValidateService.ValidateService(serviceProviderMock.Object);

            Assert.NotNull(sut);
        }

        [Fact]
        public void Validate_Succes()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            var sut = new Lib.Services.ValidateService.ValidateService(serviceProviderMock.Object);

            var model = new Mock<dynamic>();
           

            try
            {
                sut.Validate(model);
                return; // indicates success
            }
            catch (Exception ex)
            {
                Assert.True(false, "gagal");
            }


        }




    }
}
