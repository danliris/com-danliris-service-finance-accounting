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
            if (this.AccountBank == null || this.AccountBank.Id == 0)
            {
                yield return new ValidationResult("Bank harus dipilih", new List<string> { "bank" });
            }

            if (this.Supplier == null || this.Supplier.Id == 0)
            {
                yield return new ValidationResult("Supplier harus dipilih", new List<string> { "Supplier" });
            }

            if (this.PaymentDate.Equals(DateTimeOffset.MinValue) || this.PaymentDate == null)
            {
                yield return new ValidationResult("Tanggal Pembayaran harus diisi", new List<string> { "PaymentDate" });
            }
            if (this.PaymentDate.AddHours(7).Date > DateTime.Now.Date)
            {
                yield return new ValidationResult("Tanggal Pembayaran harus kurang dari atau sama dengan hari ini", new List<string> { "PaymentDate" });
            }
            if (this.Items==null || this.Items.Count == 0)
            {
                yield return new ValidationResult("Item tidak boleh kosong", new List<string> { "Items" });
            }
        }
    }
}
