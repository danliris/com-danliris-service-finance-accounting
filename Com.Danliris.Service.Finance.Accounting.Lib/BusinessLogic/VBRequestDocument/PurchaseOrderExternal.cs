using iTextSharp.text;
using System.Collections.Generic;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRequestDocument
{
    public class PurchaseOrderExternal
    {
        public int? _id { get; set; }
        public string no { get; set; }
        public IncomeTaxDto incomeTax { get; set; }
        public string incomeTaxBy { get; set; }
        public List<PurchaseOrderExternalItem> Items { get; set; }
        public OldUnitDto unit { get; set; }
        public bool useIncomeTax { get; set; }
        public bool useVat { get; set; }

    }
}