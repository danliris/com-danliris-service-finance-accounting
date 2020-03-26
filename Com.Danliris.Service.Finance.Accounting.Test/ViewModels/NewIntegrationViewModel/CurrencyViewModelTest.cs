using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.ViewModels.NewIntegrationViewModel
{
   public class CurrencyViewModelTest
    {
        [Fact]
        public void Succes_Instantiate_CurrencyViewModel()
        {
            long id = 1;
            string Code = "code test";
            string Symbol = "symbol test";
            double Rate = 1 ;
            string Description = "Description test";
            
            CurrencyViewModel cvm = new CurrencyViewModel();
            cvm.Id = id;
            cvm.Code = Code;
            cvm.Symbol = Symbol;
            cvm.Rate = Rate;
            cvm.Description = Description;

            Assert.Equal(id, cvm.Id);
            Assert.Equal(Code, cvm.Code);
            Assert.Equal(Rate, cvm.Rate);
            Assert.Equal(Symbol, cvm.Symbol);
            Assert.Equal(Description, cvm.Description);


        }
    }
}
