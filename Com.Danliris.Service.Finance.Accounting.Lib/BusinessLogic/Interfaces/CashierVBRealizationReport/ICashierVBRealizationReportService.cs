using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CashierVBRealizationReport;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.CashierVBRealizationReport
{
    public interface ICashierVBRealizationReportService
    {
        List<CashierVBRealizationViewModel> GetReport(string divisionName, string isInklaring, DateTimeOffset? approvalDateFrom, DateTimeOffset? approvalDateTo, DateTimeOffset? realizeDateFrom, DateTimeOffset? realizeDateTo, int offSet);
        MemoryStream GenerateExcel(string divisionName, string isInklaring, DateTimeOffset? approvalDateFrom, DateTimeOffset? approvalDateTo, DateTimeOffset? realizeDateFrom, DateTimeOffset? realizeDateTo, int offSet);   
    }
}
