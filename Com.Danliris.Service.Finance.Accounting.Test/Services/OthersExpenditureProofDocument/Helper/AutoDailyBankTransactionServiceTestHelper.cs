using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.OthersExpenditureProofDocumentViewModels;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.OthersExpenditureProofDocument.Helper
{
    public class AutoDailyBankTransactionServiceTestHelper : IAutoDailyBankTransactionService
    {
        public Task<int> AutoCreate(OthersExpenditureProofDocumentCreateUpdateViewModel viewModel, string documentNo)
        {
            return Task.FromResult(1);
        }

        public Task<int> AutoDelete(string documentNo)
        {
            return Task.FromResult(1);
        }
    }
}
