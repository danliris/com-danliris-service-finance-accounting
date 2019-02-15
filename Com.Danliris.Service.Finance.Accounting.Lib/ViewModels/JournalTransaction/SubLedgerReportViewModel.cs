using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.JournalTransaction
{
    public class SubLedgerReportViewModel
    {
        public SubLedgerReportViewModel()
        {
            Info = new List<SubLedgerReport>();
        }
        public decimal InitialBalance { get; set; }
        public decimal ClosingBalance { get; set; }
        public List<SubLedgerReport> Info { get; set; }
    }

    public class SubLedgerReport
    {
        public string Date { get; set; }
        public string No { get; set; }
        public string BankName { get; set; }
        public string BGCheck { get; set; }
        public string Remark { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }

    public class SubLedgerXlsFormat
    {
        public string Filename { get; set; }
        public MemoryStream Result { get; set; }
    }

}
