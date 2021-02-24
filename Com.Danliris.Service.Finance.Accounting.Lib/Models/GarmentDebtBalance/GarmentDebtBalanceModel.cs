using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentDebtBalance
{
    public class GarmentDebtBalanceModel : StandardEntity
    {
        public GarmentDebtBalanceModel()
        {

        }

        public GarmentDebtBalanceModel(int purchasingCategoryId, string purchasingCategoryName, string billsNo, string paymentBills, int garmentDeliveryOrderId, string garmentDeliveryOrderNo, int invoiceId, DateTimeOffset invoiceDate, string invoiceNo, int supplierId, string supplierName, int currencyId, string currencyCode, double dppAmount, double currencyDPPAmount, double vatAmount, double incomeTaxAmount, bool isPayVAT, bool isPayIncomeTax)
        {
            PurchasingCategoryId = purchasingCategoryId;
            PurchasingCategoryName = purchasingCategoryName;
            BillsNo = billsNo;
            PaymentBills = paymentBills;
            GarmentDeliveryOrderId = garmentDeliveryOrderId;
            GarmentDeliveryOrderNo = garmentDeliveryOrderNo;
            InvoiceId = invoiceId;
            InvoiceDate = invoiceDate;
            InvoiceNo = invoiceNo;
            SupplierId = supplierId;
            SupplierName = supplierName;
            CurrencyId = currencyId;
            CurrencyCode = currencyCode;
            DPPAmount = dppAmount;
            CurrencyDPPAmount = currencyDPPAmount;
            VATAmount = vatAmount;
            IncomeTaxAmount = incomeTaxAmount;
            IsPayVAT = isPayVAT;
            IsPayIncomeTax = isPayIncomeTax;
        }

        public int PurchasingCategoryId { get; private set; }
        [MaxLength(64)]
        public string PurchasingCategoryName { get; private set; }
        [MaxLength(256)]
        public string BillsNo { get; private set; }
        [MaxLength(256)]
        public string PaymentBills { get; private set; }
        public int GarmentDeliveryOrderId { get; private set; }
        [MaxLength(64)]
        public string GarmentDeliveryOrderNo { get; private set; }
        public int InvoiceId { get; private set; }
        public DateTimeOffset InvoiceDate { get; private set; }
        [MaxLength(64)]
        public string InvoiceNo { get; private set; }
        public int SupplierId { get; private set; }
        [MaxLength(512)]
        public string SupplierName { get; private set; }
        public int CurrencyId { get; private set; }
        [MaxLength(32)]
        public string CurrencyCode { get; private set; }
        public double DPPAmount { get; private set; }
        public double CurrencyDPPAmount { get; private set; }
        public double VATAmount { get; private set; }
        public double IncomeTaxAmount { get; private set; }
        public bool IsPayVAT { get; private set; }
        public bool IsPayIncomeTax { get; private set; }

        public int BankExpenditureNoteId { get; private set; }
        [MaxLength(64)]
        public string BankExpenditureNoteNo { get; private set; }
        public double BankExpenditureNoteInvoiceAmount { get; private set; }
        public int InternalNoteId { get; private set; }
        [MaxLength(64)]
        public string InternalNoteNo { get; private set; }

        public void SetInternalNote(int internalNoteId, string internalNoteNo)
        {
            InternalNoteId = internalNoteId;
            InternalNoteNo = internalNoteNo;
        }

        public void SetBankExpenditureNote(int bankExpenditureNoteId, string bankExpenditureNoteNo, double bankExpenditureNoteInvoiceAmount)
        {
            BankExpenditureNoteId = bankExpenditureNoteId;
            BankExpenditureNoteNo = bankExpenditureNoteNo;
            BankExpenditureNoteInvoiceAmount = bankExpenditureNoteInvoiceAmount;
        }
    }
}
