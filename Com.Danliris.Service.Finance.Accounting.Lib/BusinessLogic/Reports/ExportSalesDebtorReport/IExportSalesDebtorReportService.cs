using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.Rreports.ExportSalesDebtorReportController;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Reports
{
    public interface IExportSalesDebtorReportService
    {
        Task<List<ExportSalesDebtorReportViewModel>> GetMonitoring(int month, int year, int offset);

    }
}
