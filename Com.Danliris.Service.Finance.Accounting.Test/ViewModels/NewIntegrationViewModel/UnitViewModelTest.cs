using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.ViewModels.NewIntegrationViewModel
{
    public class UnitViewModelTest
    {
        [Fact]
        public void Succes_Instantiate_UnitViewModel()
        {
            string id = "id test";
            string code = "code test";
            string name = "name test";

            DivisionViewModel divison = new DivisionViewModel();
            divison.Id = "Id test";
            divison.Code = "Code Test";
            divison.Name = "Name test";

            UnitViewModel uvm = new UnitViewModel();
            uvm.Id = id;
            uvm.Name = name;
            uvm.Code = code;
            uvm.Division = divison;
          

            Assert.Equal(id, uvm.Id);
            Assert.Equal(name, uvm.Name);
            Assert.Equal(code, uvm.Code);
            Assert.NotNull(uvm.Division);
     

        }

    }
}
