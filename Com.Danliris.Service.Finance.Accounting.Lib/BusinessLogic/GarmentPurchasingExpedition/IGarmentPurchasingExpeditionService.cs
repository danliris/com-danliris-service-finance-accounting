using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentPurchasingExpedition
{
    public interface IGarmentPurchasingExpeditionService
    {
        Task<int> SendToVerification(SendToVerificationForm form);
        ReadResponse<IndexDto> GetSendToVerification(string keyword, int page, int size, string order);
        Task<int> SendToPurchasing(int id);
    }
}
