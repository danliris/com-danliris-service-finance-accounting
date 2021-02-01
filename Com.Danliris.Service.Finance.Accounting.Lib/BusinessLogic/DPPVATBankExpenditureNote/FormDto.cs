using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.DPPVATBankExpenditureNote
{
    public class FormDto : IValidatableObject
    {
        public AccountBankDto Bank { get; set; }
        public CurrencyDto Currency { get; set; }
        public SupplierDto Supplier { get; set; }
        public string BGCheckNo { get; set; }
        public double Amount { get; set; }
        public DateTimeOffset Date { get; set; }
        public List<FormItemDto> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}