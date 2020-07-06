using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseClass;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Danliris.Service.Finance.Accounting.Lib
{
    public class VbWithPORequestViewModel : BaseViewModel, IValidatableObject
    {

        public string VBNo { get; set; }
        public DateTimeOffset? Date { get; set; }
        public Unit Unit { get; set; }

        public ICollection<VbWithPORequestDetailViewModel> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Date == null)
                yield return new ValidationResult("Tanggal harus diisi!", new List<string> { "Date" });

            if (Unit == null || Unit.Id <= 0)
                yield return new ValidationResult("Kode VB harus diisi!", new List<string> { "VBCode" });

            if (Items == null)
                yield return new ValidationResult("Nomor PO harus diisi!", new List<string> { "purchaseOrder" });
        }
    }
}