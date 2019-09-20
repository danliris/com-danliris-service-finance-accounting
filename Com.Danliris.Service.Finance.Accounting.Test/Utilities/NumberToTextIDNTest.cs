using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Utilities
{
    public class NumberToTextIDNTest
    {
        [Fact]
        public void Should_Success_TerbilangKoma_MoreThan_4_DecimalPlace()
        {
            var terbilangKoma = NumberToTextIDN.terbilangKoma(10.00123);
            Assert.NotNull(terbilangKoma);
        }

        [Fact]
        public void Should_Success_TerbilangKoma_LessThanEqual_4_DecimalPlace()
        {
            var terbilangKoma = NumberToTextIDN.terbilangKoma(1004);
            Assert.NotNull(terbilangKoma);
        }
    }
}
