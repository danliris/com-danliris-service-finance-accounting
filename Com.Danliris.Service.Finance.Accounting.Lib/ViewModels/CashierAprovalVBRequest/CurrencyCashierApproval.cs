using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CashierAprovalVBRequest
{
    public class CurrencyCashierApproval
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal Rate { get; set; }
        public string Symbol { get; set; }
    }
}
