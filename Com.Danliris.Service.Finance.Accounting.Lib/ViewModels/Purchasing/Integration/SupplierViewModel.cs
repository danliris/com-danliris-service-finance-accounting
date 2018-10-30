using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.Purchasing.Integration
{
    public class SupplierViewModel
    {
        public string _id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public bool import { get; set; }
        public string npwp { get; set; }
    }
}
