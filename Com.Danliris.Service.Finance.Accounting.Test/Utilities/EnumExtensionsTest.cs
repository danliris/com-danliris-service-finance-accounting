using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace Com.Danliris.Service.Finance.Accounting.Test.Utilities
{
   public class EnumExtensionsTest
    {
        public enum enumValue
        {
            [Display(Name = "test")]
            test
          
        }

        [Fact]
        public void Should_Succes_Instantiate_EnumExtentensions()
        {

            var test = EnumExtensions.GetDisplayName(enumValue.test);
            Assert.NotNull(test);

        }
    }
}
