using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CreditorAccount
{
    public class CreditorAccountPostedViewModel : IValidatableObject
    {
        public string SupplierCode { get; set; }

        public string SupplierName { get; set; }

        public string Code { get; set; }

        public long DPP { get; set; }

        public long PPN { get; set; }

        public string InvoiceNo { get; set; }

        public DateTimeOffset Date { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(string.IsNullOrEmpty(Code))
                yield return new ValidationResult("Code harus diisi", new List<string> { "Code" });
        }
    }
}
