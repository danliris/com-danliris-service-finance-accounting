using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.Rreports.ExportSalesDebtorReportController
{
    public class ExportSalesDebtorReportViewModel
    {
        public int index { get; set; }
        public string month { get; set; }
        public int year { get; set; }
        public string buyerCode { get; set; }
        public string buyerName { get; set; }
        public decimal beginingBalance { get; set; }
        public double sales { get; set; }
        public double receipt { get; set; }
        public double endBalance { get; set; }
        public double lessThan { get; set; }
        public double between { get; set; }
        public double moreThan { get; set; }
    }
}
