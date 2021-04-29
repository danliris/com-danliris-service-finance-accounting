using System;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentDownPaymentReport
{
    public class DispositionExpenditureDto
    {
        public DispositionExpenditureDto(int dispositionExpenditureId, string dispositionExpenditureNo, DateTimeOffset dispositionExpenditureDate)
        {
            DispositionExpenditureId = dispositionExpenditureId;
            DispositionExpenditureNo = dispositionExpenditureNo;
            DispositionExpenditureDate = dispositionExpenditureDate;
        }

        public int DispositionExpenditureId { get; private set; }
        public string DispositionExpenditureNo { get; private set; }
        public DateTimeOffset DispositionExpenditureDate { get; private set; }
    }
}