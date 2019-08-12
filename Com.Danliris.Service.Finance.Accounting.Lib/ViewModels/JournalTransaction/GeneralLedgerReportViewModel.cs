using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.JournalTransaction
{
    public class GeneralLedgerReportViewModel
    {
        public DateTimeOffset Date { get; set; }
        public string Description { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
        public string COACode { get; set; }
        public double RemainingBalance { get; set; }
    }

    public class GeneralLedgerWrapperReportViewModel
    {
        public string COACode { get; set; }
        public string COAName { get; set; }
        public double Summary { get; set; }
        public double InitialBalance { get; set; }
        public List<GeneralLedgerReportViewModel> Items { get; set; }
    }
}
