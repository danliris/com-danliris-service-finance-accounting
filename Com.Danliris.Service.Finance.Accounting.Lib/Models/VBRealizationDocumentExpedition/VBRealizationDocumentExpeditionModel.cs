using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRealizationDocumentExpedition;
using Com.Moonlay.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRealizationDocumentExpedition
{
    public class VBRealizationDocumentExpeditionModel : StandardEntity
    {
        public VBRealizationDocumentExpeditionModel()
        {

        }

        public VBRealizationDocumentExpeditionModel(
            int vbRealizationId,
            int vbId,
            string vbNo,
            string vbRealizationNo,
            DateTimeOffset vbRealizationDate,
            string vbRequestName,
            int unitId,
            string unitName,
            int divisionId,
            string divisionName,
            decimal vbAmount,
            decimal vbRealizationAmount,
            string currencyCode,
            double currencyRate,
            string vbType
            )
        {
            VBRealizationId = vbRealizationId;
            VBId = vbId;
            VBNo = vbNo;
            VBRealizationNo = vbRealizationNo;
            VBRealizationDate = vbRealizationDate;
            VBRequestName = vbRequestName;
            UnitId = unitId;
            UnitName = unitName;
            DivisionId = divisionId;
            DivisionName = divisionName;
            VBAmount = vbAmount;
            VBRealizationAmount = vbRealizationAmount;
            CurrencyCode = currencyCode;
            CurrencyRate = currencyRate;
            VBType = vbType;

            Position = (int)VBRealizationPosition.Purchasing;
        }

        public int VBRealizationId { get; private set; }
        public int VBId { get; private set; }
        [MaxLength(64)]
        public string VBNo { get; private set; }
        [MaxLength(64)]
        public string VBRealizationNo { get; private set; }
        public DateTimeOffset VBRealizationDate { get; private set; }
        [MaxLength(256)]
        public string VBRequestName { get; private set; }
        public int UnitId { get; private set; }
        [MaxLength(64)]
        public string UnitName { get; private set; }
        public int DivisionId { get; private set; }
        [MaxLength(64)]
        public string DivisionName { get; private set; }
        public decimal VBAmount { get; private set; }
        public decimal VBRealizationAmount { get; private set; }
        public string CurrencyCode { get; private set; }
        public double CurrencyRate { get; private set; }
        [MaxLength(256)]
        public string SendToVerificationBy { get; private set; }
        public DateTimeOffset? SendToVerificationDate { get; private set; }
        [MaxLength(256)]
        public string VerificationReceiptBy { get; private set; }
        public DateTimeOffset? VerificationReceiptDate { get; private set; }
        [MaxLength(256)]
        public string VerifiedToCashierBy { get; private set; }
        public DateTimeOffset? VerifiedToCashierDate { get; private set; }
        [MaxLength(256)]
        public string CashierReceiptBy  { get; private set; }
        public DateTimeOffset? CashierReceiptDate { get; private set; }
        public string NotVerifiedReason { get; private set; }
        [MaxLength(256)]
        public string NotVerifiedBy { get; private set; }
        public DateTimeOffset? NotVerifiedDate { get; private set; }
        public int Position { get; private set; }
        [MaxLength(64)]
        public string VBType { get; set; }

        public void SubmitToVerification(string name)
        {
            SendToVerificationBy = name;
            SendToVerificationDate = DateTimeOffset.Now;
            Position = (int)VBRealizationPosition.PurchasingToVerification;
        }

        public void VerificationReceipt(string name)
        {
            VerificationReceiptBy = name;
            VerificationReceiptDate = DateTimeOffset.Now;
            Position = (int)VBRealizationPosition.Verification;
        }

        public void SendToCashier(string name)
        {
            VerifiedToCashierBy = name;
            VerifiedToCashierDate = DateTimeOffset.Now;
            Position = (int)VBRealizationPosition.VerifiedToCashier;
        }

        public void CashierVerification(string name)
        {
            CashierReceiptBy = name;
            CashierReceiptDate = DateTimeOffset.Now;
            Position = (int)VBRealizationPosition.Cashier;
        }

        public void VerificationRejected(string name, string reason)
        {
            NotVerifiedBy = name;
            NotVerifiedDate = DateTimeOffset.Now;
            NotVerifiedReason = reason;
            Position = (int)VBRealizationPosition.NotVerified   ;
        }
    }
}
