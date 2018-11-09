using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CreditBalance
{
    public class CreditBalanceViewModel
    {
        public string Currency { get; set; }

        public string SupplierName { get; set; }
        
        public long StartBalance { get; set; }

        public long Purchase { get; set; }

        public long Payment { get; set; }

        public long FinalBalance { get; set; }
    }
}
