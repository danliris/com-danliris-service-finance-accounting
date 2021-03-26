using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentInvoicePurchasingDisposition;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.GarmentInvoicePurchasingDisposition;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentInvoicePurchasingDisposition
{
    public interface IGarmentInvoicePurchasingDispositionService
    {
        ReadResponse<GarmentInvoicePurchasingDispositionModel> Read(int page, int size, string order, List<string> select, string keyword, string filter ="{}");
        Task<int> CreateAsync(GarmentInvoicePurchasingDispositionModel model);
        Task<GarmentInvoicePurchasingDispositionModel> ReadByIdAsync(int id);
        Task<int> DeleteAsync(int id);
        Task<int> UpdateAsync(int id, GarmentInvoicePurchasingDispositionModel model);
        Task<int> Post(GarmentInvoicePurchasingDispositionPostingViewModel form);
        ReadResponse<GarmentInvoicePurchasingDispositionItemModel> ReadDetailsByEPOId(string epoId);
    }
}
