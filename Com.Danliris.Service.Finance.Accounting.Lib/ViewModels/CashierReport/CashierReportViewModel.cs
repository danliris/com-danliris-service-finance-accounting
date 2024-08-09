using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CashierReport
{
    public class CashierReportViewModel
    {
        public int Aging { get; set; }
        public string DocumentNo { get; set; }
        public string ApprovalDate { get; set; }
        public string ApprovedBy { get; set; }
        public string CreateBy { get; set; }
        public string Purpose { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Amount { get; set; }
        public string TakenBy { get; internal set; }
        public string PhoneNumber { get; internal set; }
        public string Email { get; internal set; }
        public string IsInklaring { get;  set; }
        public string DivisioName { get; set; }
        public string BankAccountName { get; set; }
        public DateTimeOffset CreatedUTC { get; set; }
    }
}
