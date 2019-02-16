using Com.Danliris.Service.Finance.Accounting.Lib.Enums.Expedition;
using System;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.PurchasingDispositionReport
{
    public class PurchasingDispositionReportViewModel
    {
        public string DispositionNo { get; set; }

        public DateTimeOffset? CreatedUtc { get; set; }

        public DateTimeOffset? PaymentDueDate { get; set; }

        public string InvoiceNo { get; set; }

        public string SupplierName { get; set; }

        public int Position { get; set; }

        public DateTimeOffset? SentToVerificationDivisionDate { get; set; }

        public DateTimeOffset? VerificationDivisionDate { get; set; }

        public DateTimeOffset? VerifyDate { get; set; }

        public DateTimeOffset? SendDate { get; set; }

        public DateTimeOffset? CashierDivisionDate { get; set; }

        public DateTimeOffset? BankExpenditureNoteDate { get; set; }

        public string BankExpenditureNoteNo { get; set; }

        public string BankExpenditureNotePPHNo { get; set; }

        public DateTimeOffset? BankExpenditureNotePPHDate { get; set; }

        public string Staff { get; set; }

    }
}
