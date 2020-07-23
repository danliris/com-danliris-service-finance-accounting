using System;

namespace Com.Danliris.Service.Finance.Accounting.Lib
{
    public class ModelVbItem
    {
        public DateTimeOffset? Date { get; set; }
        public string Remark { get; set; }
        public decimal Amount { get; set; }
        public bool isGetPPn { get; set; }
        public decimal Total { get; set; }
    }
}