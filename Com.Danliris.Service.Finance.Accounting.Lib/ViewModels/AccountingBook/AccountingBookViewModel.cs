using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseClass;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.AccountingBook
{
    public class AccountingBookViewModel : BaseViewModel
    {
        
        public string Code { get; set; }        
        public string Remarks { get; set; }        
        public string AccountingBookType { get; set; }
    }
}
