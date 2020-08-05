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
        public string TypeVBNonPO { get; set; }

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
                yield return new ValidationResult("Daftar harus diisi!", new List<string> { "Item" });
            }
            else if (Date == null)
            {
                yield return new ValidationResult("Tanggal Realisasi harus ada!", new List<string> { "Item" });
            }
            else if (numberVB == null)
            {
                yield return new ValidationResult("No VB harus ada!", new List<string> { "Item" });
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
                        ItemsError += "'DateDetail': 'Tanggal harus diisi!', ";
                    }

                    if (item.DateDetail.HasValue && item.DateDetail.Value >= Date.Value)
                    {
                        CountItemsError++;
                        ItemsError += "'DateDetail': 'Tanggal Nota harus kurang atau sama dengan Tanggal Realisasi!', ";
                    }
                    else if(item.DateDetail.HasValue && item.DateDetail.Value <= numberVB.Date.Value)
                    {
                        CountItemsError++;
                        ItemsError += "'DateDetail': 'Tanggal Nota harus lebih atau sama dengan Tanggal VB!', ";
                    }

                    if (string.IsNullOrWhiteSpace(item.Remark))
                    {
                        CountItemsError++;
                        ItemsError += "'Remark': 'Keterangan harus diisi!', ";
                    }

                    if (item.Amount <= 0)
                    {
                        CountItemsError++;
                        ItemsError += "'Amount': 'Jumlah harus lebih besar dari 0!', ";
                    }

                    if(item.isGetPPh == true)
                    {
                        if (item.incomeTax == null)
                        {
                            CountItemsError++;
                            ItemsError += "'incomeTax': 'Nomor PPh Harus Diisi!', ";
                        }

                        if (string.IsNullOrWhiteSpace(item.IncomeTaxBy))
                        {
                            CountItemsError++;
                            ItemsError += "'IncomeTaxBy': 'Ditanggung Oleh harus Dipilih!', ";
                        }
                    }

                    

                    ItemsError += "}, ";
                }

                ItemsError += "]";

                if (CountItemsError > 0)
                    yield return new ValidationResult(ItemsError, new List<string> { "Items" });
            }
        }
    }
}