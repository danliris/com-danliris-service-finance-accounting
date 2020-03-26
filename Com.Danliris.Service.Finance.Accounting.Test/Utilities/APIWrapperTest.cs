using System;
using System.Collections.Generic;
using System.Text;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Moq;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Utilities
{
   public class APIWrapperTest
    {
        [Fact]
        public void Should_succes_intantiate_APIWrapper()
        {
            var result =new APIWrapper()
            {
                apiVersion ="1",
                data ="data test",
                message="message test",
                statusCode ="status code",
                info="info test"
            };
            Assert.Equal("1",result.apiVersion);
            Assert.Equal("data test", result.data);
            Assert.Equal("message test", result.message);
            Assert.Equal("status code", result.statusCode);
            Assert.Equal("info test",result.info);
        }
        
    }
}
