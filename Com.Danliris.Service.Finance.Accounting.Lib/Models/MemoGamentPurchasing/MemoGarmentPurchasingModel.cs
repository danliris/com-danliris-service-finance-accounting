using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.MemoGamentPurchasing
{
    public class MemoGarmentPurchasingModel : StandardEntity
    {
        public string MemoNo { get; set; }
        public DateTimeOffset Memodate { get; set; }
        public int AccountingBookId { get; set; }
        public string AccountingBookType { get; set; }
        public int GarmentCurrenciesId { get; set; }
        public string GarmentCurrenciesCode { get; set; }
        public int GarmentCurrenciesRate { get; set; }
        public string Remarks { get; set; }
        public bool IsPosted { get; set; }
    }
}
