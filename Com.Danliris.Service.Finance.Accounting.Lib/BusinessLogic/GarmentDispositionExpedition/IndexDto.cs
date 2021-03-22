using Com.Danliris.Service.Finance.Accounting.Lib.Enums.Expedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentDispositionExpedition;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentDispositionExpedition
{
    public class IndexDto
    {
        public IndexDto(int id, string dispositionNoteNo, DateTimeOffset dispositionNoteDate, int dispositionNoteId, string supplierName, GarmentPurchasingExpeditionPosition position, double totalPaid, string currencyCode, string remark,DateTimeOffset createdDate)
        {
            Id = id;
            DispositionNoteNo = dispositionNoteNo;
            DispositionNoteDate = dispositionNoteDate;
            DispositionNoteId = dispositionNoteId;
            SupplierName = supplierName;
            TotalPaid = totalPaid;
            CurrencyCode = currencyCode;
            Remark = remark;
            Status = position.ToDescriptionString();
            CreatedDate = createdDate;
        }
        public IndexDto(int id, string dispositionNoteNo, DateTimeOffset dispositionNoteDate, int dispositionNoteId, string supplierName, GarmentPurchasingExpeditionPosition position, double totalPaid, string currencyCode, string remark, DateTimeOffset verifiedDateSend, DateTimeOffset verifiedDateReceived,string sendToPurchasingRemark, DateTimeOffset createdDate)
        {
            Id = id;
            DispositionNoteNo = dispositionNoteNo;
            DispositionNoteDate = dispositionNoteDate;
            DispositionNoteId = dispositionNoteId;
            SupplierName = supplierName;
            TotalPaid = totalPaid;
            CurrencyCode = currencyCode;
            Remark = remark;
            Status = position.ToDescriptionString();
            VerifiedDateReceived = verifiedDateReceived;
            VerifiedDateSend = verifiedDateSend;
            SendToPurchasingRemark = sendToPurchasingRemark;
            CreatedDate = createdDate;
        }

        public IndexDto(int id, string dispositionNoteNo,
            DateTimeOffset dispositionNoteDate,
            DateTimeOffset dispositionNoteDueDate,
            int dispositionNoteId,
            double currencyTotalPaid,
            double totalPaid,
            int currencyId,
            string currencyCode,
            string suppliername,
            string remark,
            string proformaNo,
            string createdBy,
            double currencyRate,
            int supplierId,
            string supplierCode,
            double vatAmount,
            double currencyVatAmount,
            double incomeTaxAmount,
            double currencyIncomeTaxAmount,
            double dppAmount,
            double currencyDppAmount,
            DateTimeOffset verifiedDateSend,
            DateTimeOffset verifiedDateReceived,
            string sendToPurchasingRemark,
            DateTimeOffset createdDate
            )
        {
            Id = id;
            DispositionNoteNo = dispositionNoteNo;
            DispositionNoteDate = dispositionNoteDate;
            DispositionNoteDueDate = dispositionNoteDueDate;
            DispositionNoteId = dispositionNoteId;
            CurrencyTotalPaid = currencyTotalPaid;
            TotalPaid = totalPaid;
            CurrencyId = currencyId;
            CurrencyCode = currencyCode;
            SupplierName = suppliername;
            Remark = remark;
            CreatedBy = createdBy;
            ProformaNo = proformaNo;
            CurrencyRate = currencyRate;
            SupplierId = supplierId;
            SupplierCode = supplierCode;
            VATAmount = vatAmount;
            CurrencyVATAmount = currencyVatAmount;
            IncomeTaxAmount = incomeTaxAmount;
            CurrencyIncomeTaxAmount = currencyIncomeTaxAmount;
            DPPAmount = dppAmount;
            CurrencyDPPAmount = currencyDppAmount;
            VerifiedDateSend = verifiedDateSend;
            VerifiedDateReceived = verifiedDateReceived;
            SendToPurchasingRemark = sendToPurchasingRemark;
            CreatedDate = createdDate;
        }

        public int Id { get; private set; }
        public string DispositionNoteNo { get; private set; }
        public DateTimeOffset DispositionNoteDate { get; private set; }
        public DateTimeOffset DispositionNoteDueDate { get; private set; }
        public int DispositionNoteId { get; private set; }
        public double CurrencyTotalPaid { get; private set; }
        public double TotalPaid { get; private set; }
        public int CurrencyId { get; private set; }
        public string CurrencyCode { get; private set; }
        public double CurrencyRate { get; private set; }
        public string SupplierName { get; private set; }
        public int SupplierId { get; private set; }
        public string SupplierCode { get; private set; }
        public double VATAmount { get; private set; }
        public double CurrencyVATAmount { get; private set; }
        public double IncomeTaxAmount { get; private set; }
        public double CurrencyIncomeTaxAmount { get; private set; }
        public double DPPAmount { get; private set; }
        public double CurrencyDPPAmount { get; set; }
        public string Remark { get; private set; }
        public string Status { get; private set; }
        public string ProformaNo { get; private set; }
        public string CreatedBy { get; private set; }
        public DateTimeOffset VerifiedDateSend { get; set; }
        public DateTimeOffset VerifiedDateReceived { get; set; }
        public string SendToPurchasingRemark { get; private set; }
        public DateTimeOffset CreatedDate { get; private set; }
    }
}
