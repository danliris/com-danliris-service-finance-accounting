using System;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentDownPaymentReport
{
    public class MemoDocumentDto
    {
        public int MemoId { get; private set; }
        public string MemoNo { get; private set; }
        public DateTimeOffset MemoDate { get; private set; }
    }
}