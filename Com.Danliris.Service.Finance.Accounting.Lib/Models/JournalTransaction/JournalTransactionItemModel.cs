using Com.Danliris.Service.Finance.Accounting.Lib.Models.MasterCOA;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.JournalTransaction
{
    public class JournalTransactionItemModel : StandardEntity, IValidatableObject
    {
        public int COAId { get; set; }
        public COAModel COA { get; set; }
        public string Remark { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
