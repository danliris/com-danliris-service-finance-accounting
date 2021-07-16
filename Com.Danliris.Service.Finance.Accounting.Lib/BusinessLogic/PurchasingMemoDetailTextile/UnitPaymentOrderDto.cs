using System;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.PurchasingMemoDetailTextile
{
    public class UnitPaymentOrderDto
    {
        public int Id { get; set; }
        public string UnitPaymentOrderNo { get; set; }
        public DateTimeOffset UnitPaymentOrderDate { get; set; }
    }
}