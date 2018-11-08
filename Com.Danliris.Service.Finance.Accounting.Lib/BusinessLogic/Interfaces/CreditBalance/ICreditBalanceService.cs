using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CreditBalance;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.CreditBalance
{
    public interface ICreditBalanceService
    {
        ReadResponse<CreditBalanceViewModel> GetReport(int page, int size, string suplierName, int month, int year, int offSet);
        MemoryStream GenerateExcel(string suplierName, int month, int year, int offSet);
    }
}
