using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseClass;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.PaymentDispositionNoteViewModel
{
    public class PaymentDispositionNoteViewModel : BaseViewModel, IValidatableObject
    {
        public string BGCheckNumber { get; set; }
        public AccountBankViewModel AccountBank { get; set; }
        public string PaymentDispositionNo { get; set; }
        public double Amount { get; set; }
        public SupplierViewModel Supplier { get; set; }
        public DateTimeOffset PaymentDate { get; set; }
        public string BankAccountCOA { get; set; }
        public List<PaymentDispositionNoteItemViewModel> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
