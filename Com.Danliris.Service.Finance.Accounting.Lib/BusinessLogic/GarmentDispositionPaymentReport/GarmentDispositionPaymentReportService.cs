using Com.Danliris.Service.Finance.Accounting.Lib.Enums.Expedition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentDispositionPaymentReport
{
    public class GarmentDispositionPaymentReportService : IGarmentDispositionPaymentReportService
    {
        public List<PositionOption> GetPositionOptions()
        {
            var result = new List<PositionOption>();

            var positions = Enum.GetValues(typeof(GarmentPurchasingExpeditionPosition)).Cast<GarmentPurchasingExpeditionPosition>();

            foreach (var position in positions)
            {
                result.Add(new PositionOption(position));
            }

            return result;
        }

        public List<GarmentDispositionPaymentReportDto> GetReport(int dispositionId, int supplierId, GarmentPurchasingExpeditionPosition position, string purchasingStaff, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            throw new NotImplementedException();
        }
    }
}
