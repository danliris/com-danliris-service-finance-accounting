using System;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentPurchasingExpedition
{
    public class InternalNoteDto
    {
        public int Id { get;  set; }
        public string DocumentNo { get;  set; }
        public DateTimeOffset Date { get;  set; }
        public DateTimeOffset DueDate { get;  set; }
        public int SupplierId { get;  set; }
        public string SupplierName { get;  set; }
        public double AmountDPP { get;  set; }
        public double VAT { get;  set; }
        public double IncomeTax { get;  set; }
        public double TotalPaid { get;  set; }
        public int CurrencyId { get;  set; }
        public string CurrencyCode { get;  set; }
    }
}