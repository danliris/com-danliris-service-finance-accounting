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
           
            APIEndpoint.Core = "core test";
            APIEndpoint.Inventory = "Inventory test";
            APIEndpoint.Production = "Production test";
            APIEndpoint.Purchasing = "Purchasing test";
            APIEndpoint.Finishing = "Finishing test";
            APIEndpoint.Finance = "Finance test";

            Assert.Equal("core test", APIEndpoint.Core);
            Assert.Equal("Inventory test", APIEndpoint.Inventory);
            Assert.Equal("Production test", APIEndpoint.Production);
            Assert.Equal("Purchasing test", APIEndpoint.Purchasing);
            Assert.Equal("Finishing test", APIEndpoint.Finishing);
            Assert.Equal("Finance test", APIEndpoint.Finance);
            
        }
        
    }
}
