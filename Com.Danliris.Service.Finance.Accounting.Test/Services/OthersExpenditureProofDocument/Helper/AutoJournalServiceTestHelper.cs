using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.OthersExpenditureProofDocumentViewModels;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.OthersExpenditureProofDocument.Helper
{
    public class AutoJournalServiceTestHelper : IAutoJournalService
    {
        public Task<int> AutoJournalFromOthersExpenditureProof(OthersExpenditureProofDocumentCreateUpdateViewModel viewModel, string documentNo)
        {
            return Task.FromResult(1);
        }

        public Task<int> AutoJournalReverseFromOthersExpenditureProof(string documentNo)
        {
            return Task.FromResult(1);
        }
    }
}
