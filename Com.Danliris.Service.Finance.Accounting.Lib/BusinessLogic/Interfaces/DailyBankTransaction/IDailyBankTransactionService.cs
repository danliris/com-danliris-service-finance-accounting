using Com.Danliris.Service.Finance.Accounting.Lib.Models.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseInterface;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.DailyBankTransaction
{
    public interface IDailyBankTransactionService : IBaseService<DailyBankTransactionModel>
    {
        ReadResponse<DailyBankTransactionModel> GetReport(int bankId, int month, int year, int clientTimeZoneOffset);
        MemoryStream GenerateExcel(int bankId, int month, int year, int clientTimeZoneOffset);
        Task<int> DeleteByReferenceNoAsync(string referenceNo);
        Task<int> CreateInOutTransactionAsync(DailyBankTransactionModel model);
    }
}
