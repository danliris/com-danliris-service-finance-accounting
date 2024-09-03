using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CashierReport;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.CashierReport
{
    public interface ICashierReportService
    {
        List<CashierReportViewModel> GetReport(string divisionName, string isInklaring, string account, DateTimeOffset? approvalDateFrom, DateTimeOffset? approvalDateTo, int offSet);
        MemoryStream GenerateExcel(string divisionName, string isInklaring, string account, DateTimeOffset? approvalDateFrom, DateTimeOffset? approvalDateTo, int offSet);   
    }
}
