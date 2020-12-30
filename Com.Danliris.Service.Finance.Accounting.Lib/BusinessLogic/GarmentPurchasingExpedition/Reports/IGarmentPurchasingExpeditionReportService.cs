using Com.Danliris.Service.Finance.Accounting.Lib.Enums.Expedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentPurchasingExpedition;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentPurchasingExpedition.Reports
{
    public interface IGarmentPurchasingExpeditionReportService
    {
        List<GarmentPurchasingExpeditionModel> GetReport(int internalNoteId, int supplierId, GarmentPurchasingExpeditionPosition position, DateTimeOffset startDate, DateTimeOffset endDate);
        MemoryStream GenerateExcel(int internalNoteId, int supplierId, GarmentPurchasingExpeditionPosition position, DateTimeOffset startDate, DateTimeOffset endDate);
    }
}
