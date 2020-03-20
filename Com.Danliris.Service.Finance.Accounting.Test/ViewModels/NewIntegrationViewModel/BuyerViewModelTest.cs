
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.ViewModels.NewIntegrationViewModel
{
   
    public class BuyerViewModelTest
    {
        [Fact]
        public void Should_Succes_Instantiate_BuyerViewModel()
        {
            string id= "id test";
            string nama =  "name test" ;
            string code = "name test";

            BuyerViewModel bvm = new BuyerViewModel();
            bvm.Id = id;
            bvm.Name = nama;
            bvm.Code = code;

            Assert.Equal(id, bvm.Id);
            Assert.Equal(nama, bvm.Name);
            Assert.Equal(code, bvm.Code);

        }

        [Fact]
        public void Should_Succes_Instantiate_NewBuyerViewModel()
        {
            int id = 1;
            string nama = "name test";
            string code = "code test";

            NewBuyerViewModel nbvm = new NewBuyerViewModel();

            nbvm.Id = 1;
            nbvm.Name = nama;
            nbvm.Code = code;

            Assert.Equal(id, nbvm.Id);
            Assert.Equal(nama, nbvm.Name);
            Assert.Equal(code, nbvm.Code);

        }
    }
}
