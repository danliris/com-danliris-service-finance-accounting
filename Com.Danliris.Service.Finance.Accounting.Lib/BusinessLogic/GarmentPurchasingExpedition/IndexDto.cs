using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentPurchasingExpedition
{
    public class IndexDto
    {
        public IndexDto(int id, string internalNoteNo, DateTimeOffset internalNoteDate, DateTimeOffset internalNoteDueDate, string supplierName, double amount, string currencyCode, string remark)
        {
            Id = id;
            InternalNoteNo = internalNoteNo;
            InternalNoteDate = internalNoteDate;
            InternalNoteDueDate = internalNoteDueDate;
            SupplierName = supplierName;
            Amount = amount;
            CurrencyCode = currencyCode;
            Remark = remark;
        }

        public int Id { get; private set; }
        public string InternalNoteNo { get; private set; }
        public DateTimeOffset InternalNoteDate { get; private set; }
        public DateTimeOffset InternalNoteDueDate { get; private set; }
        public string SupplierName { get; private set; }
        public double Amount { get; private set; }
        public string CurrencyCode { get; private set; }
        public string Remark { get; private set; }
    }
}
