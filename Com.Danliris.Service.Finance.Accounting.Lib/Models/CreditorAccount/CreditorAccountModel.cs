using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.CreditorAccount
{
    public class CreditorAccountModel : StandardEntity, IValidatableObject
    {
        #region Supplier
        public string SupplierName { get; set; }

        public string SupplierCode { get; set; }
        #endregion


        #region UnitReceiptNote
        public string UnitReceiptNoteNo { get; set; }

        public DateTimeOffset? UnitReceiptNoteDate { get; set; }

        public long UnitReceiptNoteDPP { get; set; }

        public long UnitReceiptNotePPN { get; set; }

        public long UnitReceiptMutation { get; set; }
        #endregion

        #region BankExpenditureNote
        public int BankExpenditureNoteId { get; set; }

        public string BankExpenditureNoteNo { get; set; }

        public DateTimeOffset? BankExpenditureNoteDate { get; set; }

        public long BankExpenditureNoteDPP { get; set; }

        public long BankExpenditureNotePPN { get; set; }

        public long BankExpenditureNoteMutation { get; set; }
        #endregion

        #region Memo
        public string MemoNo { get; set; }

        public DateTimeOffset? MemoDate { get; set; }

        public long MemoDPP { get; set; }

        public long MemoPPN { get; set; }

        public long MemoMutation { get; set; }
        #endregion
        
        public string InvoiceNo { get; set; }

        public long FinalBalance { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
