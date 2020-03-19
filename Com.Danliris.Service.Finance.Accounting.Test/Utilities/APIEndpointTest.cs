using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Utilities
{
   public class APIEndpointTest
    {


        [Fact]
        public void Should_succes_intantiate_APIEndpoint()
        {
            var result = new APIEndpoint();
            Assert.NotNull(result);
        }
    }
}
