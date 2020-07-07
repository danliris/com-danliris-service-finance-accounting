using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.CashierAprovalVBRequest
{
    public class CashierAprovalVBNonPORequestModel : StandardEntity, IValidatableObject
    {
        [MaxLength(64)]
        public string VBNo { get; set; }
        public DateTimeOffset Date { get; set; }

        public int UnitId { get; set; }
        [MaxLength(64)]
        public string UnitCode { get; set; }
        [MaxLength(64)]
        public string UnitName { get; set; }
        public int CurrencyId { get; set; }
        [MaxLength(64)]
        public string CurrencyCode { get; set; }
        public decimal CurrencyRate { get; set; }
        [MaxLength(64)]
        public string CurrencySymbol { get; set; }
        public decimal Amount { get; set; }
        [MaxLength(255)]
        public string Usage { get; set; }
        [MaxLength(255)]
        public string UnitLoad { get; set; }
        [MaxLength(255)]
        public string Status_Post { get; set; }
        [MaxLength(255)]
        public string Apporve_Status { get; set; }
        [MaxLength(255)]
        public string Complete_Status { get; set; }
        [MaxLength(255)]
        public string VBRequestCategory { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
