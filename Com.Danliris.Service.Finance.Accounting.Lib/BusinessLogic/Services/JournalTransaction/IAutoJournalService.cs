using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.OthersExpenditureProofDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.NewIntegrationViewModel;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.OthersExpenditureProofDocumentViewModels;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.JournalTransaction
{
    public interface IAutoJournalService
    {
        Task<int> AutoJournalFromOthersExpenditureProof(OthersExpenditureProofDocumentCreateUpdateViewModel viewModel, string documentNo);
        Task<int> AutoJournalFromOthersExpenditureProof(OthersExpenditureProofDocumentModel model, List<OthersExpenditureProofDocumentItemModel> items);
        Task<int> AutoJournalReverseFromOthersExpenditureProof(string documentNo);
        Task<int> AutoJournalVBNonPOClearence(List<int> vbRealizationIds);
        Task<int> AutoJournalVBNonPOClearence(List<int> vbRealizationIds, AccountBankViewModel bank);
        //Task<int> AutoJournalInklaring(List<int> vbRequestIds);
        Task<int> AutoJournalInklaring(List<int> vbRequestIds, AccountBankViewModel bank);
        string DocumentNoGenerator(AccountBankViewModel bank);
    }
}