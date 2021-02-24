using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentDebtBalance
{
    public interface IGarmentDebtBalanceService
    {
        List<GarmentDebtBalanceCardDto> GetDebtBalanceCardDto(int supplierId, int month, int year);
        int CreateFromCustoms(CustomsFormDto form);
        int UpdateFromInternalNote(InternalNoteFormDto form);
        int UpdateFromInvoice(InvoiceFormDto form);
        int UpdateFromBankExpenditureNote(BankExpenditureNoteFormDto form);
    }
}
