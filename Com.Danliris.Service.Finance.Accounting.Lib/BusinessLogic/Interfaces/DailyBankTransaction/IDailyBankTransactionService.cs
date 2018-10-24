using Com.Danliris.Service.Finance.Accounting.Lib.Models.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.DailyBankTransaction
{
    public interface IDailyBankTransactionService
    {
        ReadResponse<DailyBankTransactionModel> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        Task<int> Create(DailyBankTransactionModel model, string username);
        Task<DailyBankTransactionModel> ReadById(int Id);
        ReadResponse<DailyBankTransactionModel> GetReport(string bankId, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int clientTimeZoneOffset);
        MemoryStream GenerateExcel(string bankId, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int clientTimeZoneOffset);
    }
}
