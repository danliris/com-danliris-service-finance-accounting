using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentDebtBalance
{
    public interface IGarmentDebtBalanceService
    {
        List<GarmentDebtBalanceCardDto> GetDebtBalanceCardDto(int supplierId, int month, int year);
        int CreateFromCustoms(CustomsFormDto form);
        int UpdateFromInternalNote(int deliveryOrderId, InternalNoteFormDto form);
        int UpdateFromInvoice(int deliveryOrderId, InvoiceFormDto form);
        int UpdateFromBankExpenditureNote(int deliveryOrderId, BankExpenditureNoteFormDto form);
        GarmentDebtBalanceIndexDto GetDebtBalanceCardIndex(int supplierId, int month, int year);
        List<GarmentDebtBalanceSummaryDto> GetDebtBalanceSummary(int supplierId, int month, int year, bool isForeignCurrency, bool supplierIsImport);
    }
}
