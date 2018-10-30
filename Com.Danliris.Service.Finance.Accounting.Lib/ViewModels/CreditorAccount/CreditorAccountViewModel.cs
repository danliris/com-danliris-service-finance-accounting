using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseClass;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CreditorAccount
{
    public class CreditorAccountViewModel : BaseViewModel
    {
        public DateTimeOffset Date { get; set; }

        public string UnitReceiptNoteNo { get; set; }
        
        public string BankExpenditureNoteNo { get; set; }

        public string MemoNo { get; set; }

        public string InvoiceNo { get; set; }

        public long DPP { get; set; }

        public long PPN { get; set; }

        public long Total { get; set; }

        public long Mutation { get; set; }
        
    }
}
