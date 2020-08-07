using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseClass;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.RealizationVBWIthPO
{
    public class RealizationVbWithPOViewModel : BaseViewModel, IValidatableObject
    {
        public string VBRealizationNo { get; set; }
        public DateTimeOffset? VBRealizationDate { get; set; }
        //public string TypeWithOrWithoutVB { get; set; }
        public string TypeVBNonPO { get; set; }
        public ICollection<DetailSPB> Items { get; set; }
        public DetailVB numberVB { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (VBRealizationDate == null)
                yield return new ValidationResult("Tanggal harus diisi!", new List<string> { "Date" });

            if (numberVB == null)
                yield return new ValidationResult("Data harus diisi!", new List<string> { "VBCode" });

            int cnt = 0;

            if (Items == null)
            {
                yield return new ValidationResult("Data harus diisi!", new List<string> { "VBCode" });
            }
            else
            {
                foreach (var itm in Items)
                {
                    if (itm.IsSave == false)
                    {
                        cnt += 1;
                    }
                }

                if (Items.Count == cnt)
                {
                    yield return new ValidationResult("Data harus dipilih!", new List<string> { "Item" });
                }
            }

        }
    }
}