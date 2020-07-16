using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseClass;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.RealizationVBNonPO
{
    public class RealizationVbNonPOViewModel : BaseViewModel, IValidatableObject
    {
        public string VBRealizationNo { get; set; }

        public DateTimeOffset? Date { get; set; }

        public DetailRequestNonPO numberVB { get; set; }

        public ICollection<VbNonPORequestDetailViewModel> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Date == null)
                yield return new ValidationResult("Tanggal harus diisi!", new List<string> { "Date" });

            if (numberVB == null)
                yield return new ValidationResult("No VB harus diisi!", new List<string> { "VBCode" });

            if (Items == null || Items.Count.Equals(0))
            {
                yield return new ValidationResult("Daftar harus diisi", new List<string> { "Item" });
            }
            else if (Items.Count > 0)
            {
                int CountItemsError = 0;
                string ItemsError = "[";

                foreach (var item in Items)
                {
                    ItemsError += "{ ";
                    if (item.DateDetail == null)
                    {
                        CountItemsError++;
                        ItemsError += "'DateDetail': 'Tanggal harus diisi', ";
                    }

                    if (string.IsNullOrWhiteSpace(item.Remark))
                    {
                        CountItemsError++;
                        ItemsError += "'Remark': 'Keterangan harus diisi', ";
                    }

                    if (item.Amount <= 0)
                    {
                        CountItemsError++;
                        ItemsError += "'Amount': 'Jumlah harus lebih besar dari 0', ";
                    }
                }

                ItemsError += "]";

                if (CountItemsError > 0)
                    yield return new ValidationResult(ItemsError, new List<string> { "Items" });
            }
        }
    }
}