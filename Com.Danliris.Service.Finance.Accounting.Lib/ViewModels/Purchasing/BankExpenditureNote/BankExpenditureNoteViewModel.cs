using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseClass;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.Purchasing.Integration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.Purchasing.BankExpenditureNote
{
    public class BankExpenditureNoteViewModel : BaseViewModel
    {
        public AccountBankViewModel Bank { get; set; }
        public string BGCheckNumber { get; set; }
        public DateTimeOffset? Date { get; set; }
        public List<BankExpenditureNoteDetailViewModel> Details { get; set; }
        public string DocumentNo { get; set; }
        public SupplierViewModel Supplier { get; set; }
        public double GrandTotal { get; set; }
    }
}
