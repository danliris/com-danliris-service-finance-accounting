using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRequestDocument
{
    public class VBRequestDocumentNonPOFormDto : IValidatableObject
    {
        public int Id { get; set; }
        public DateTimeOffset? Date { get; set; }
        public DateTimeOffset? RealizationEstimationDate { get; set; }
        public UnitDto SuppliantUnit { get; set; }
        public CurrencyDto Currency { get; set; }
        public decimal? Amount { get; set; }
        public string Purpose { get; set; }

        public List<VBRequestDocumentNonPOItemFormDto> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Date == null)
                yield return new ValidationResult("Tanggal harus diisi", new List<string> { "Date" });

            if (RealizationEstimationDate == null)
                yield return new ValidationResult("Estimasi Tanggal Realisasi harus diisi", new List<string> { "RealizationEstimationDate" });

            if (SuppliantUnit == null || SuppliantUnit.Id.GetValueOrDefault() <= 0)
                yield return new ValidationResult("Unit Pemohon harus diisi", new List<string> { "SuppliantUnit" });

            if (Currency == null || Currency.Id.GetValueOrDefault() <= 0)
                yield return new ValidationResult("Mata Uang harus diisi", new List<string> { "Currency" });

            if (Items == null || Items.Count.Equals(0))
            {
                yield return new ValidationResult("Beban unit harus diisi", new List<string> { "Item" });
            }
            else
            {
                if (!Items.Any(element => element.IsSelected && element.Unit != null && element.Unit.Id.GetValueOrDefault() != 0))
                    yield return new ValidationResult("Beban unit harus dipilih minimal 1", new List<string> { "Item" });
            }
        }
    }
}