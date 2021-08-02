using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentFinance.BankCashReceiptDetail
{
    public class BankCashReceiptDetailModel : StandardEntity
    {
        public int BankCashReceiptId { get; set; }
        [MaxLength(256)]
        public string BankCashReceiptNo { get; set; }
        public DateTimeOffset BankCashReceiptDate { get; set; }
        public virtual ICollection<BankCashReceiptDetailItemModel> Items { get; set; }
    }
}
