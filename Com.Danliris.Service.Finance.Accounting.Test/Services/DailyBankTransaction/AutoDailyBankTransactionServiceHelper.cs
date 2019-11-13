using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.OthersExpenditureProofDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.PaymentDispositionNote;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.OthersExpenditureProofDocumentViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.DailyBankTransaction
{
    public class AutoDailyBankTransactionServiceHelper : IAutoDailyBankTransactionService
    {
        public Task<int> AutoCreateFromOthersExpenditureProofDocument(OthersExpenditureProofDocumentModel model, List<OthersExpenditureProofDocumentItemModel> itemModels)
        {
            return Task.FromResult(1);
        }

        public Task<int> AutoCreateFromPaymentDisposition(PaymentDispositionNoteModel model)
        {
            return Task.FromResult(1);
        }

        public Task<int> AutoRevertFromOthersExpenditureProofDocument(OthersExpenditureProofDocumentModel model, List<OthersExpenditureProofDocumentItemModel> itemModels)
        {
            return Task.FromResult(1);
        }

        public Task<int> AutoRevertFromPaymentDisposition(PaymentDispositionNoteModel model)
        {
            return Task.FromResult(1);
        }
    }
}
