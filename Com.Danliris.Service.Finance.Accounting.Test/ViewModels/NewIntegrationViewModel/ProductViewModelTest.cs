using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.ViewModels.NewIntegrationViewModel
{
  public  class ProductViewModelTest
    {
        [Fact]
        public void Should_Succes_Instantiate_ProductViewModel()
        {
            string id = "id test";
            string code = "code test";
            string nama = "name test";
            double Price = 1.1;

            UomViewModel uvm = new UomViewModel();
            

            ProductViewModel pvm = new ProductViewModel();
            pvm.Id = id;
            pvm.Name = nama;
            pvm.Code = code;
            pvm.Price = Price;
            pvm.UOM = uvm;

            Assert.Equal(id, pvm.Id);
            Assert.Equal(nama, pvm.Name);
            Assert.Equal(code, pvm.Code);
            Assert.Equal(Price, pvm.Price);
            Assert.NotNull( pvm.UOM);

        }
    }
}
