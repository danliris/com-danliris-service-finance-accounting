using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.ViewModels.NewIntegrationViewModel
{

  public  class CategoryViewModelTest
    {
        [Fact]
        public void Should_Success_Instantiate_CategoryViewModel()
        {
            string id = "id test";
            string nama = "name test";
            string code = "code test";

            CategoryViewModel cvm = new CategoryViewModel();
            cvm.Id = id;
            cvm.Name = nama;
            cvm.Code = code;

            Assert.Equal(id, cvm.Id);
            Assert.Equal(nama, cvm.Name);
            Assert.Equal(code, cvm.Code);
        }

        

    }
}
