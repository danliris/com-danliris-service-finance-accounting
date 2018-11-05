using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseClass;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.JournalTransaction
{
    public class JournalTransactionViewModel : BaseViewModel, IValidatableObject
    {
        public string DocumentNo { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Date { get; set; }
        public string ReferenceNo { get; set; }
        public List<JournalTransactionItemViewModel> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
