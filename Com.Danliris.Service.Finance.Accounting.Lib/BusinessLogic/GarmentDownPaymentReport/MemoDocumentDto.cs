using System;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentDownPaymentReport
{
    public class MemoDocumentDto
    {
        public MemoDocumentDto(int memoId, string memoNo, DateTimeOffset memoDate)
        {
            MemoId = memoId;
            MemoNo = memoNo;
            MemoDate = memoDate;
        }

        public int MemoId { get; private set; }
        public string MemoNo { get; private set; }
        public DateTimeOffset MemoDate { get; private set; }
        public double RealizationDownPaymentCurrencyTotal { get; set; }
        public double RealizationDownPaymentCurrencyRate { get; set; }
        public double RealizationDownPaymentCurrencyAmount { get; set; }
        public DateTimeOffset InternNoteDate { get; set; }
        public string InternNoteNo { get; set; }
        public DateTimeOffset DeliveryOrderDate { get; set; }
        public string DeliveryOrderNo { get; set; }
        public string PaymentNo { get; set; }
        public string PaymentDescription { get; set; }
        public string PaymentCurrencyCode { get; set; }
        public double PaymentCurrencyRate { get; set; }
        public double PaymentCurrencyAmount { get; set; }
        public double DifferenceCurrencyRate { get; set; }

    }
}