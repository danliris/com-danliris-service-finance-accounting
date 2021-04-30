using System;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentDownPaymentReport
{
    public class DispositionPaymentDto
    {
        public DispositionPaymentDto(int dispositionPaymentId, string dispositionPaymentNo, DateTimeOffset dispositionPaymentDate)
        {
            DispositionPaymentId = dispositionPaymentId;
            DispositionPaymentNo = dispositionPaymentNo;
            DispositionPaymentDate = dispositionPaymentDate;
        }

        public int DispositionPaymentId { get; private set; }
        public string DispositionPaymentNo { get; private set; }
        public DateTimeOffset DispositionPaymentDate { get; private set; }
    }
}