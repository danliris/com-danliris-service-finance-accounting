namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRequestDocument
{
    public class PurchaseOrderExternalItem
    {
        public Product Product { get; set; }
        public double? DefaultQuantity { get; set; }
        
        public double? DealQuantity { get; set; }
        public UnitOfMeasurement DealUOM { get; set; }
        public double? Conversion { get; set; }
        public double? Price { get; set; }
        public bool UseVat { get; set; }
    }
}