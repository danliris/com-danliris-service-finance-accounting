using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.DPPVATBankExpenditureNote
{
    public interface IDPPVATBankExpenditureNoteService
    {
        Task<int> Create(FormDto form);
        DPPVATBankExpenditureNoteDto Read(int id);
        ReadResponse<DPPVATBankExpenditureNoteIndexDto> Read(string keyword, int page = 1, int size = 25, string order = "{}");
        int Update(int id, FormDto form);
        int Delete(int id);
        List<ReportDto> ExpenditureReport(int expenditureId, int internalNoteId, int invoiceId, int supplierId, DateTimeOffset startDate, DateTimeOffset endDate);
    }
}
