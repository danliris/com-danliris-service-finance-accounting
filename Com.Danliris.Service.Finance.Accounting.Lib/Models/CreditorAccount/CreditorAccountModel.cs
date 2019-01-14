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

        public string Products { get; set; }

        public DateTimeOffset? UnitReceiptNoteDate { get; set; }

        public double UnitReceiptNoteDPP { get; set; }

        public double UnitReceiptNotePPN { get; set; }

        public double UnitReceiptMutation { get; set; }
        #endregion

        #region BankExpenditureNote
        public int BankExpenditureNoteId { get; set; }

        public string BankExpenditureNoteNo { get; set; }

        public DateTimeOffset? BankExpenditureNoteDate { get; set; }

        public double BankExpenditureNoteDPP { get; set; }

        public double BankExpenditureNotePPN { get; set; }

        public double BankExpenditureNoteMutation { get; set; }
        #endregion

        #region Memo
        public string MemoNo { get; set; }

        public DateTimeOffset? MemoDate { get; set; }

        public double MemoDPP { get; set; }

        public double MemoPPN { get; set; }

        public double MemoMutation { get; set; }
        #endregion
        
        public string InvoiceNo { get; set; }

        public double FinalBalance { get; set; }

        public string CurrencyCode { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
