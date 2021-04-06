using Com.Danliris.Service.Finance.Accounting.Lib.Models.MemoGarmentPurchasing;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.MemoGarmentPurchasingReport
{
    public interface IMemoGarmentPurchasingReportService
    {
        ReadResponse<MemoGarmentPurchasingDetailModel> ReadReportDetailBased(int page, int size, string filter);
        MemoryStream GenerateExcel(int year, int month, int accountingBookId);
        ReadResponse<MemoGarmentPurchasingModel> GetReportPdfData(int year, int month, int accountingBookId, int page = 1, int size = 25);
    }
}
