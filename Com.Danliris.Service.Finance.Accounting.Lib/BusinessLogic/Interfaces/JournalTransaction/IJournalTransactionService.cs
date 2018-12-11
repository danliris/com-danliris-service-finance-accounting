using Com.Danliris.Service.Finance.Accounting.Lib.Models.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseInterface;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.JournalTransaction;
using System.IO;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.JournalTransaction
{
    public interface IJournalTransactionService : IBaseService<JournalTransactionModel>
    {
        (ReadResponse<JournalTransactionReportViewModel>, double, double) GetReport(int page, int size, int month, int year, int offSet);
        MemoryStream GenerateExcel(int month, int year, int offSet);
    }
}
