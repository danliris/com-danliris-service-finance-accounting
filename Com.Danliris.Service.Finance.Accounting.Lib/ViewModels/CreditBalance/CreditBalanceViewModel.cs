namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CreditBalance
{
    public class CreditBalanceViewModel
    {
        public string Currency { get; set; }

        public string Products { get; set; }

        public string SupplierName { get; set; }

        public double StartBalance { get; set; }

        public double Purchase { get; set; }

        public double Payment { get; set; }

        public double FinalBalance { get; set; }
    }
}
