using Com.Danliris.Service.Finance.Accounting.Lib.Enums.Expedition;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentPurchasingExpedition
{
    public class GarmentPurchasingExpeditionModel : StandardEntity
    {
        public GarmentPurchasingExpeditionModel(int internalNoteId, string internalNoteNo, DateTimeOffset internalNoteDate, DateTimeOffset internalNoteDueDate, int supplierId, string supplierName, double vat, double incomeTax, double totalPaid, int currencyId, string currencyCode, string remark)
        {
            InternalNoteId = internalNoteId;
            InternalNoteNo = internalNoteNo;
            InternalNoteDate = internalNoteDate;
            InternalNoteDueDate = internalNoteDueDate;
            SupplierId = supplierId;
            SupplierName = supplierName;
            VAT = vat;
            IncomeTax = incomeTax;
            TotalPaid = totalPaid;
            CurrencyId = currencyId;
            CurrencyCode = currencyCode;
            Remark = remark;
        }

        public int InternalNoteId { get; private set; }
        [MaxLength(64)]
        public string InternalNoteNo { get; private set; }
        public DateTimeOffset InternalNoteDate { get; private set; }
        public DateTimeOffset InternalNoteDueDate { get; private set; }
        public int SupplierId { get; private set; }
        [MaxLength(512)]
        public string SupplierName { get; private set; }
        public double VAT { get; private set; }
        public double IncomeTax { get; private set; }
        public double TotalPaid { get; private set; }
        public int CurrencyId { get; private set; }
        [MaxLength(16)]
        public string CurrencyCode { get; private set; }
        public string Remark { get; private set; }
        public PurchasingGarmentExpeditionPosition Position { get; private set; }

        public DateTimeOffset? SendToVerificationDate { get; private set; }
        [MaxLength(64)]
        public string SendToVerificationBy { get; private set; }

        public DateTimeOffset? VerificationAcceptedDate { get; private set; }
        [MaxLength(64)]
        public string VerificationAcceptedBy { get; private set; }

        public DateTimeOffset? SendToCashierDate { get; private set; }
        [MaxLength(64)]
        public string SendToCashierBy { get; private set; }

        public DateTimeOffset? CashierAcceptedDate { get; private set; }
        [MaxLength(64)]
        public string CashierAcceptedBy { get; private set; }

        public DateTimeOffset? SendToPurchasingDate { get; private set; }
        [MaxLength(64)]
        public string SendToPurchasingBy { get; private set; }

        public DateTimeOffset? SendToAccountingDate { get; private set; }
        [MaxLength(64)]
        public string SendToAccountingBy { get; private set; }

        public DateTimeOffset? AccountingAcceptedDate { get; private set; }
        [MaxLength(64)]
        public string AccountingAcceptedBy { get; private set; }

        public void SendToVerification(string username)
        {
            SendToVerificationBy = username;
            SendToVerificationDate = DateTimeOffset.Now;
            Position = PurchasingGarmentExpeditionPosition.SendToVerification;
        }

        public void SendToPurchasing(string username)
        {
            SendToPurchasingBy = username;
            SendToPurchasingDate = DateTimeOffset.Now;
            Position = PurchasingGarmentExpeditionPosition.SendToPurchasing;
        }
    }
}
