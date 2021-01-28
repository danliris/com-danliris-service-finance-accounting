using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentPurchasingPphBankExpenditureNote
{
    public class GarmentPurchasingPphBankExpenditureNoteInvoiceModel: StandardEntity
    {
        public string InvoicesNo { get; private set; }
        public DateTimeOffset InvoicesDate { get; set; }
        public long InvoicesId { get; set; }
        public string ProductName { get; set; }
        public long ProductId { get; set; }
        public string ProductCategory { get; set; }
        public decimal Total { get; set; }
        public virtual int GarmentPurchasingPphBankExpenditureNoteItemId { get; set; }
        [ForeignKey("GarmentPurchasingPphBankExpenditureNoteItemId")]
        public virtual GarmentPurchasingPphBankExpenditureNoteItemModel GarmentPurchasingPphBankExpenditureNoteItem { get; set; }
    }
}
