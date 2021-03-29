using Com.Danliris.Service.Finance.Accounting.Lib.Enums.Expedition;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentDispositionPaymentReport
{
    public interface IGarmentDispositionPaymentReportService
    {
        List<GarmentDispositionPaymentReportDto> GetReport(int dispositionId, int supplierId, GarmentPurchasingExpeditionPosition position, string purchasingStaff, DateTimeOffset startDate, DateTimeOffset endDate);
        List<PositionOption> GetPositionOptions();
    }
}
