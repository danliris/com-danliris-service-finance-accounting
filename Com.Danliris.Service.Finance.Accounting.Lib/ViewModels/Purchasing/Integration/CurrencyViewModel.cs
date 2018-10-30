using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.Purchasing.Integration
{
    public class CurrencyViewModel
    {
        public string _id { get; set; }
        public string code { get; set; }
        public string symbol { get; set; }
        public double rate { get; set; }
        public string description { get; set; }
    }
}
