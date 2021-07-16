using System.Collections.Generic;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.PurchasingMemoDetailTextile
{
    public class FormDetailDto
    {
        public ExpenditureDto Expenditure { get; set; }
        public SupplierDto Supplier { get; set; }
        public string Remark { get; set; }
        public UnitPaymentOrderDto UnitPaymentOder { get; set; }
        public List<UnitReceiptNoteDto> UnitReceiptNotes { get; set; }
        public double PurchaseAmountCurrency { get; set; }
        public double PurchaseAmount { get; set; }
        public double PaymentAmountCurrency { get; set; }
        public double PaymentAmount { get; set; }
    }
}