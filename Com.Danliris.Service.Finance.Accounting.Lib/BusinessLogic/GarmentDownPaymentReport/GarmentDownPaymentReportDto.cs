using System;
using System.Collections.Generic;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentDownPaymentReport
{
    public class GarmentDownPaymentReportDto
    {
        public GarmentDownPaymentReportDto(int dispositionId, string dispositionNo, DateTimeOffset dispositionDueDate, List<int> expenditureIds)
        {
            DispositionId = dispositionId;
            DispositionNo = dispositionNo;
            DispositionDueDate = dispositionDueDate;
            DispositionExpenditures = new List<DispositionExpenditureDto>();
            MemoDocuments = new List<MemoDocumentDto>();
            ExpenditureIds = expenditureIds;
        }

        public int DispositionId { get; private set; }
        public string DispositionNo { get; private set; }
        public DateTimeOffset DispositionDueDate { get; private set; }
        public List<DispositionExpenditureDto> DispositionExpenditures { get; private set; }
        public List<MemoDocumentDto> MemoDocuments { get; private set; }
        public List<int> ExpenditureIds { get; private set; }
        public List<int> MemoIds { get; private set; }
    }
}