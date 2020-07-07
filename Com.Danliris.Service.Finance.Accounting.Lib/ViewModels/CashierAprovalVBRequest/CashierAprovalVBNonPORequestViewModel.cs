using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseClass;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CashierAprovalVBRequest
{
    public class CashierAprovalVBNonPORequestViewModel : BaseViewModel, IValidatableObject
    {
        [MaxLength(255)]
        public string VBNo { get; set; }
        public DateTimeOffset VBDate { get; set; }
        [MaxLength(255)]
        public string VBCode { get; set; }
        public decimal Amount { get; set; }
        [MaxLength(255)]
        public string CreateBy { get; set; }
        [MaxLength(255)]
        public string StatusPO { get; set; }
        public DateTimeOffset ApprovalDate { get; set; }

        public CurrencyCashierApproval Currency { get; set; }
        public UnitCashierApproval Unit { get; set; }
        public bool IsPosted { get; set; }
        public bool IsAproved { get; set; }
        public bool IsCompleted { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.VBNo == null)
            {
                yield return new ValidationResult("VB No is required", new List<string> { "VBNo" });
            }
        }
    }
}
