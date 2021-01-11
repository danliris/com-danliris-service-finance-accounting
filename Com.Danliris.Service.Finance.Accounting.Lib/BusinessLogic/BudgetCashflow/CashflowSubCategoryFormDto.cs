using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.BudgetCashflow
{
    public class CashflowSubCategoryFormDto : IValidatableObject
    {
        public string Name { get; set; }
        public List<int> PurchasingCategoryIds { get; set; }
        public int CashflowCategoryId { get; set; }
        public int LayoutOrder { get; set; }
        public bool IsReadOnly { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                yield return new ValidationResult("Nama tidak boleh kosong", new List<string> { "Name" });
            }

            if (CashflowCategoryId <= 0)
            {
                yield return new ValidationResult("Kategori Cashflow harus diisi", new List<string> { "CashflowCategoryId" });
            }

            if (IsReadOnly)
            {
                if (PurchasingCategoryIds == null || PurchasingCategoryIds.Count == 0)
                    yield return new ValidationResult("Kategori Pembelian harus diisi", new List<string> { "PurchasingCategoryId" });

            }
        }
    }
}