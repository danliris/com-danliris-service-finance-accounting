using Com.Danliris.Service.Finance.Accounting.Lib.Enums.Master;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Enums.Master
{
    public class COACategoryEnumTest
    {

        [Fact]
        public void EnumTest()
        {
            var aktivaLancar = COACategoryEnum.AKTIVA_LANCAR.ToDescriptionString();
            Assert.Equal("AKTIVA LANCAR", aktivaLancar);
        }
    }
}
