using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Test.Controller.Utils;
using Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.DailyBankTransaction;

namespace Com.Danliris.Service.Finance.Accounting.Test.Controllers.DailyBankTransaction
{
    public class DailyBankTransactionControllerTest : BaseControllerTest<DailyBankTransactionController, DailyBankTransactionModel, DailyBankTransactionViewModel, IDailyBankTransactionService>
    {
    }
}
