using System.Collections.Generic;
using System.Threading.Tasks;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.OthersExpenditureProofDocumentViewModels;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Services.OthersExpenditureProofDocument
{
    public interface IOthersExpenditureProofDocumentService
    {
        Task<int> Create(OthersExpenditureProofDocumentViewModel viewModel);
        Task<int> Update(int id, OthersExpenditureProofDocumentViewModel viewModel);
        Task<int> Delete(int id);
        Task<OthersExpenditureProofPagedListViewModel> GetPagedList(int page, int size, string order, string keyword, string filter);
    }
}