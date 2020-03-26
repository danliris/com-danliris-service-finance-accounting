using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.ViewModels.NewIntegrationViewModel
{
    public class DivisionViewModelTest
    {
        [Fact]
        public void Should_succes_Instantiate_DivisionViewModel()
        {
            string id = "id test";
            string nama = "name test";
            string code = "code test";

            DivisionViewModel dvm = new DivisionViewModel();
            dvm.Id = id;
            dvm.Name = nama;
            dvm.Code = code;

            Assert.Equal(id, dvm.Id);
            Assert.Equal(nama, dvm.Name);
            Assert.Equal(code, dvm.Code);
        }
    }
}
