using System.Collections.Generic;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.DailyBankTransaction
{
    public class DailyBankTransactionReportViewModel
    {
        public List<DailyBankTransactionViewModel> Transactions { get; set; }
        public double InitialBalance { get; set; }
        public double RemainingBalance { get; set; }
    }
}
