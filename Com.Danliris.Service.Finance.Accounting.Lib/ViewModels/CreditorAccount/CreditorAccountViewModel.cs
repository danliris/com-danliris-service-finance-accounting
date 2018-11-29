using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseClass;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CreditorAccount
{
    public class CreditorAccountViewModel : BaseViewModel
    {
        public DateTimeOffset? Date { get; set; }

        public string UnitReceiptNoteNo { get; set; }
        
        public string BankExpenditureNoteNo { get; set; }

        public string MemoNo { get; set; }

        public string InvoiceNo { get; set; }

        public double? DPP { get; set; }

        public double? PPN { get; set; }

        public double? Total { get; set; }

        public double? Mutation { get; set; }

        public double? FinalBalance { get; set; }

        public string Currency { get; set; }
        
    }
}
