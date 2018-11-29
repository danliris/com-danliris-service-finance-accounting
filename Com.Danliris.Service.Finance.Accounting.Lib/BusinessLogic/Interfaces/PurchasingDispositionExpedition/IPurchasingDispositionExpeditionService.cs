using Com.Danliris.Service.Finance.Accounting.Lib.Models.PurchasingDispositionExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseInterface;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.PurchasingDispositionAcceptance;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.PurchasingDispositionExpedition
{
    public interface IPurchasingDispositionExpeditionService : IBaseService<PurchasingDispositionExpeditionModel>
    {
        Task<int> PurchasingDispositionAcceptance(PurchasingDispositionAcceptanceViewModel data);
        Task<int> DeletePurchasingDispositionAcceptance(int id);
    }
}
