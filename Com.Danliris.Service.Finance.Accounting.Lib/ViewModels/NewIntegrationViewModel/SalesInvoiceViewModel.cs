
namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.NewIntegrationViewModel
{
    public class SalesInvoiceViewModel
    {
        public int Id { get; set; }
        public string SalesInvoiceNo { get; set; }
        public string vatType { get; set; }
        public CurrencyViewModel Currency { get; set; }
    }
}
