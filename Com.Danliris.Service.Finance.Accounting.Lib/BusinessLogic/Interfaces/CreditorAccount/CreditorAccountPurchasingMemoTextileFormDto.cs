namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.CreditorAccount
{
    public class CreditorAccountPurchasingMemoTextileFormDto
    {
        public CreditorAccountPurchasingMemoTextileFormDto(string unitPaymentOrderNo, int purchasingMemoId, string purchasingMemoNo, double purchasingMemoAmount)
        {
            UnitPaymentOrderNo = unitPaymentOrderNo;
            PurchasingMemoId = purchasingMemoId;
            PurchasingMemoNo = purchasingMemoNo;
            PurchasingMemoAmount = purchasingMemoAmount;
        }

        public string UnitPaymentOrderNo { get; private set; }
        public int PurchasingMemoId { get; private set; }
        public string PurchasingMemoNo { get; private set; }
        public double PurchasingMemoAmount { get; private set; }
    }
}