using System.Collections.Generic;
using System.Threading.Tasks;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.OthersExpenditureProofDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.OthersExpenditureProofDocumentViewModels;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.DailyBankTransaction
{
    public interface IAutoDailyBankTransactionService
    {
        Task<int> AutoCreate(OthersExpenditureProofDocumentCreateUpdateViewModel viewModel, string documentNo);
        Task<int> AutoDelete(string documentNo);
    }
}