using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.Purchasing.BankExpenditureNote
{
    public class BankExpenditureNoteItemViewModel : BaseViewModel
    {
        public long UnitPaymentOrderItemId { get; set; }
        public double Price { get; set; }
        public string ProductCode { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public double Quantity { get; set; }
        public string UnitCode { get; set; }
        public string UnitId { get; set; }
        public string UnitName { get; set; }
        public string Uom { get; set; }
    }
}
