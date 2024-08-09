using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CashierVBRealizationReport
{
    public class CashierVBRealizationViewModel
    {
        public string DocumentNo { get; set; }
        public DateTimeOffset? RealizationDate { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Amount { get; set; }
        public string ApprovedBy { get; set; }
        public string CompletedBy { get; set; }
        public string BankAccountName { get; set; }
        public string CreateBy { get; set; }
        public string Remark { get; set; }
        public string TakenBy { get; internal set; }
        public string PhoneNumber { get; internal set; }
        public string Email { get; internal set; }
        public string IsInklaring { get;  set; }
        public string DivisioName { get; set; }
        public DateTimeOffset CreatedUTC { get; set; }
    }
}
