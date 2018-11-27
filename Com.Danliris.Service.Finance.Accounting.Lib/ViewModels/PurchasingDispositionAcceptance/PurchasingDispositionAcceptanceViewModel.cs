using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.PurchasingDispositionAcceptance
{
    public class PurchasingDispositionAcceptanceViewModel : IValidatableObject
    {
        public string Role { get; set; }

        public List<PurchasingDispositionAcceptanceItemViewModel> PurchasingDispositionAcceptances { get; set; } 

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.PurchasingDispositionAcceptances.Count.Equals(0))
            {
                yield return new ValidationResult("Purchasing Disposition Expeditions is required", new List<string> { "PurchaseDispositionExpeditionCollection" });
            }
        }
    }
}
