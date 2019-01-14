using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.JournalTransaction
{
    public class JournalTransactionReportViewModel
    {
        public DateTimeOffset Date { get; set; }

        public string COAName { get; set; }

        public string COACode { get; set; }

        public string Remark { get; set; }

        public double? Debit { get; set; }

        public double? Credit { get; set; }
    }
}
