using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.Purchasing.Integration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.Purchasing.PurchaseOrder
{
    public class PurchaseOrderViewModel
    {
        public CategoryViewModel category { get; set; }
        public bool useIncomeTax { get; set; }
    }
}
