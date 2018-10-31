using Com.Danliris.Service.Finance.Accounting.Lib.Models.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseInterface;
using System;
using System.IO;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.DailyBankTransaction
{
    public interface IDailyBankTransactionService : IBaseService<DailyBankTransactionModel>
    {
        ReadResponse<DailyBankTransactionModel> GetReport(string bankId, int month, int year, int clientTimeZoneOffset);
        MemoryStream GenerateExcel(string bankId, int month, int year, int clientTimeZoneOffset);
    }
}
