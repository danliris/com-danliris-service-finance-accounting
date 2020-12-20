using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentPurchasingExpedition
{
    public interface IGarmentPurchasingExpeditionService
    {
        Task<int> SendToVerification(SendToVerificationAccountingForm form);
        Task<int> SendToAccounting(SendToVerificationAccountingForm form);
        ReadResponse<IndexDto> GetSendToVerificationOrAccounting(string keyword, int page, int size, string order);
        Task<int> SendToPurchasing(int id);
    }
}
