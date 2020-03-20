
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.ViewModels.NewIntegrationViewModel
{
    public class IncomeTaxViewModelTest
    {
        [Fact]
        public void Should_Succes_Intantiate_IncomeTaxViewModel()
        {
            int id = 1;
            string nama = "name test";
            double rate = 1.1;

            IncomeTaxViewModel ivm = new IncomeTaxViewModel();
            ivm.Id = id;
            ivm.Name = nama;
            ivm.Rate = rate;

            Assert.Equal(id, ivm.Id);
            Assert.Equal(nama, ivm.Name);
            Assert.Equal(rate, ivm.Rate);
        }
    }
}
