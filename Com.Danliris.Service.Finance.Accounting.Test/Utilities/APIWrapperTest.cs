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
                apiVersion ="1"
            };
            Assert.NotNull(result.apiVersion);
        }
        
    }
}
