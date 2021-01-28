using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.GarmentPurchasingPphBankExpenditureNoteViewModels
{
    public class GarmentPurchasingPphBankExpenditureNoteDataViewModel
    {
        public int Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public string No { get; set; }
        public DateTimeOffset CreatedUtc { get; set; }
        public string BankAccountName { get; set; }
        public string IncomeTaxName { get; set; }
        public double IncomeTaxRate { get; set; }
        public Int64 TotalDpp { get; set; }
        public int TotalIncomeTax { get; set; }
        public string Currency { get; set; }
        public DateTimeOffset LastModifiedUtc { get; set; }
        public bool IsPosted { get; set; }
    }
}
