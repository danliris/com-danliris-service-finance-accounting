using Com.Danliris.Service.Finance.Accounting.Lib.Enums;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.MemoDetailGarmentPurchasing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.PurchasingMemoDetailTextile
{
    public class FormDto : IValidatableObject
    {
        public DateTimeOffset Date { get; set; }
        public DivisionDto Division { get; set; }
        public CurrencyDto Currency { get; set; }
        public bool SupplierIsImport { get; set; }
        public PurchasingMemoType Type { get; set; }
        public List<FormItemDto> Items { get; set; }
        public List<FormDetailDto> Details { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
