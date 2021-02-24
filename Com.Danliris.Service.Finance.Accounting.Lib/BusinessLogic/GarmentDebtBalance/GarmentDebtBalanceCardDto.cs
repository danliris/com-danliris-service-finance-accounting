using System;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentDebtBalance
{
    public class GarmentDebtBalanceCardDto
    {
        public int PurchasingCategoryId { get; private set; }
        public string PurchasingCategoryName { get; private set; }
        public string BillsNo { get; private set; }
        public string PaymentBills { get; private set; }
        public int GarmentDeliveryOrderId { get; private set; }
        public string GarmentDeliveryOrderNo { get; private set; }
        public int InvoiceId { get; private set; }
        public DateTimeOffset InvoiceDate { get; private set; }
        public string InvoiceNo { get; private set; }
        public int SupplierId { get; private set; }
        public string SupplierName { get; private set; }
        public int CurrencyId { get; private set; }
        public string CurrencyCode { get; private set; }
        public double DPPAmount { get; private set; }
        public double CurrencyDPPAmount { get; private set; }
        public double VATAmount { get; private set; }
        public double IncomeTaxAmount { get; private set; }
        public bool IsPayVAT { get; private set; }
        public bool IsPayIncomeTax { get; private set; }

        public int BankExpenditureNoteId { get; private set; }
        public string BankExpenditureNoteNo { get; private set; }
        public double BankExpenditureNoteInvoiceAmount { get; private set; }
        public int InternalNoteId { get; private set; }
        public string InternalNoteNo { get; private set; }
        public double TotalInvoice { get; set; }
        public double MutationPurchase { get; set; }
        public double MutationPayment { get; set; }
        public double RemainBalance { get; set; }
    }
}