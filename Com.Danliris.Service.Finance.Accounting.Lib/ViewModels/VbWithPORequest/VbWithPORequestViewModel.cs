using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseClass;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Com.Danliris.Service.Finance.Accounting.Lib
{
    public class VbWithPORequestViewModel : BaseViewModel, IValidatableObject
    {

        public string VBNo { get; set; }
        public DateTimeOffset? Date { get; set; }
        public DateTimeOffset? DateEstimate { get; set; }
        public Unit Unit { get; set; }
        public decimal VBMoney { get; set; }
        public string Usage { get; set; }

        public ICollection<VbWithPORequestDetailViewModel> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Date == null)
                yield return new ValidationResult("Tanggal harus diisi!", new List<string> { "Date" });

            if (DateEstimate == null)
                yield return new ValidationResult("Estimasi Tanggal Realisasi harus diisi!", new List<string> { "DateEstimate" });

            if(VBMoney <= 0)
                yield return new ValidationResult("VB Uang harus lebih dari nol!", new List<string> { "VBMoney" });

            if(string.IsNullOrWhiteSpace(Usage))
                yield return new ValidationResult("Kegunaan harus diisi!", new List<string> { "Usage" });

            if (Unit == null || Unit.Id <= 0)
                yield return new ValidationResult("Kode VB harus diisi!", new List<string> { "VBCode" });

            if (Items == null || Items.Count == 0)
            {
                yield return new ValidationResult("Nomor PO harus diisi!", new List<string> { "itemscount" });
            }
            else
            {
                foreach (var detail in Items)
                {
                    var duplicate = Items.Where(w => w.no != null && detail.no != null && w.no.Equals(detail.no)).ToList();

                    if (duplicate.Count > 1)
                    {
                        yield return new ValidationResult("Nomor PO duplikat!", new List<string> { "itemscount" });
                        break;
                    }
                }
            }

        }
    }
}