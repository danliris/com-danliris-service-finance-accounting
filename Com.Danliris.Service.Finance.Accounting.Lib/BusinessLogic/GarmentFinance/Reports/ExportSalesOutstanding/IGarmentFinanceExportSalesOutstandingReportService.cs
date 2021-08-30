using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.GarmentFinance.Report.ExportSalesOutstanding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentFinance.Reports.ExportSalesOutstanding
{
    public interface IGarmentFinanceExportSalesOutstandingReportService
    {
        List<GarmentFinanceExportSalesOutstandingReportViewModel> GetMonitoring(int month, int year, string buyer, int offset);
        Tuple<MemoryStream, string> GenerateExcel(int month, int year, string buyer, int offset);
    }
}
