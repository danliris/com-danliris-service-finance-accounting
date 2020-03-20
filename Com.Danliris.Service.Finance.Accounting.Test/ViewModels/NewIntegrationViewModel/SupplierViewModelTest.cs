using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.ViewModels.NewIntegrationViewModel
{
  public  class SupplierViewModelTest
    {
        [Fact]
        public void Should_Success_Instantiate()
        {
            long id = 1;
            string code = "code test";
            string nama = "name test";
            bool import = true;
            string PIC = "PIC test";
            string contact = "conatct test";

            SupplierViewModel spm = new SupplierViewModel();
            spm.Id = id;
            spm.Code = code;
            spm.Name = nama;
            spm.Import = import;
            spm.PIC = PIC;
            spm.Contact = contact;


            Assert.Equal(id, spm.Id);
            Assert.Equal(nama, spm.Name);
            Assert.Equal(code, spm.Code);
            Assert.Equal(import, spm.Import);
            Assert.Equal(PIC, spm.PIC);
            Assert.Equal(contact, spm.Contact);
        }
    }


    public class NewSupplierViewModelTest
    {
        [Fact]
        public void Should_Succes_Instantiate_NewSupplierViewModel()
        {
            int id = 1;
            string code = "code test";
            string nama = "name test";
            bool import = true;


            NewSupplierViewModel nspm = new NewSupplierViewModel();
            nspm._id = id;
            nspm.code = code;
            nspm.name = nama;
            nspm.import = import;
           
            Assert.Equal(id, nspm._id);
            Assert.Equal(nama, nspm.name);
            Assert.Equal(code, nspm.code);
            Assert.Equal(import, nspm.import);
           
        }
    }
}
