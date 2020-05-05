using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.NewIntegrationViewModel;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.ViewModels.NewIntegrationViewModel
{
    public class SalesInvoiceViewModelTest
    {
        [Fact]
        public void Should_Success_Instantiate()
        {
            int id = 1;
            string salesInvoiceNo = "salesInvoiceNo test";
            string vatType = "vatType test";
            CurrencyViewModel cvm = new CurrencyViewModel();

            SalesInvoiceViewModel sivm = new SalesInvoiceViewModel();
            sivm.Id = id;
            sivm.SalesInvoiceNo = salesInvoiceNo;
            sivm.VatType = vatType;
            sivm.Currency = cvm;

            Assert.Equal(id, sivm.Id);
            Assert.Equal(salesInvoiceNo, sivm.SalesInvoiceNo);
            Assert.Equal(vatType, sivm.VatType);
            Assert.Equal(cvm, sivm.Currency);

        }
    }
}