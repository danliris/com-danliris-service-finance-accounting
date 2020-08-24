using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRequestDocument;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRequestDocument
{
    public class VBRequestDocumentModel : StandardEntity
    {
        public VBRequestDocumentModel()
        {

        }

        public VBRequestDocumentModel(
            string documentNo,
            DateTimeOffset date,
            DateTimeOffset realizationEstimationDate,
            int suppliantUnitId,
            string suppliantUnitCode,
            string suppliantUnitName,
            int suppliantDivisionId,
            string suppliantDivisionCode,
            string suppliantDivisionName,
            int currencyId,
            string currencyCode,
            string currencySymbol,
            string currencyDescription,
            double currencyRate,
            string purpose,
            decimal amount,
            bool isPosted,
            bool isApproved,
            bool isCompleted,
            VBType type
            )
        {
            DocumentNo = documentNo;
            Date = date;
            RealizationEstimationDate = realizationEstimationDate;
            CurrencyId = currencyId;
            CurrencyCode = currencyCode;
            CurrencySymbol = currencySymbol;
            CurrencyRate = currencyRate;
            CurrencyDescription = currencyDescription;
            Purpose = purpose;
            Amount = amount;
            IsPosted = isPosted;
            IsApproved = isApproved;
            IsCompleted = isCompleted;
            Type = type;

            SuppliantUnitId = suppliantUnitId;
            SuppliantUnitCode = suppliantUnitCode;
            SuppliantUnitName = suppliantUnitName;
            SuppliantDivisionId = suppliantDivisionId;
            SuppliantDivisionCode = suppliantDivisionCode;
            SuppliantDivisionName = suppliantDivisionName;
        }

        [MaxLength(64)]
        public string DocumentNo { get; private set; }
        public DateTimeOffset Date { get; private set; }
        public DateTimeOffset RealizationEstimationDate { get; private set; }
        public int CurrencyId { get; private set; }
        [MaxLength(64)]
        public string CurrencyCode { get; private set; }
        [MaxLength(64)]
        public string CurrencySymbol { get; private set; }
        public double CurrencyRate { get; private set; }
        [MaxLength(256)]
        public string CurrencyDescription { get; private set; }
        public string Purpose { get; private set; }
        public decimal Amount { get; private set; }
        public bool IsPosted { get; private set; }
        public bool IsApproved { get; private set; }
        public bool IsCompleted { get; private set; }
        public VBType Type { get; private set; }
        public int SuppliantUnitId { get; private set; }
        [MaxLength(64)]
        public string SuppliantUnitCode { get; private set; }
        [MaxLength(256)]
        public string SuppliantUnitName { get; private set; }
        public int SuppliantDivisionId { get; private set; }
        [MaxLength(64)]
        public string SuppliantDivisionCode { get; private set; }
        [MaxLength(256)]
        public string SuppliantDivisionName { get; private set; }

        public void SetDate(DateTimeOffset newDate, string user, string userAgent)
        {
            if(newDate != Date)
            {
                Date = newDate;
                this.FlagForUpdate(user, userAgent);
            }
        }

        public void SetRealizationEstimationDate(DateTimeOffset newRealizationEstimationDate, string user, string userAgent)
        {
            if (newRealizationEstimationDate != RealizationEstimationDate)
            {
                RealizationEstimationDate = newRealizationEstimationDate;
                this.FlagForUpdate(user, userAgent);
            }
        }

        public void SetCurrency(int newCurrencyId, string newCurrencyCode, string newCurrencySymbol, double newCurrencyRate, string newCurrencyDescription, string user, string userAgent)
        {
            if(newCurrencyId != CurrencyId)
            {
                CurrencyId = newCurrencyId;
                this.FlagForUpdate(user, userAgent);
            }

            if (newCurrencyCode != CurrencyCode)
            {
                CurrencyCode = newCurrencyCode;
                this.FlagForUpdate(user, userAgent);
            }

            if (newCurrencySymbol != CurrencySymbol)
            {
                CurrencySymbol = newCurrencySymbol;
                this.FlagForUpdate(user, userAgent);
            }

            if (newCurrencyRate != CurrencyRate)
            {
                CurrencyRate = newCurrencyRate;
                this.FlagForUpdate(user, userAgent);
            }

            if (newCurrencyDescription != CurrencyDescription)
            {
                CurrencyDescription = newCurrencyDescription;
                this.FlagForUpdate(user, userAgent);
            }
        }

        public void SetAmount(decimal newAmount, string user, string userAgent)
        {
            if (newAmount != Amount)
            {
                Amount = newAmount;
                this.FlagForUpdate(user, userAgent);
            }
        }

        public void SetPurpose(string newPurpose, string user, string userAgent)
        {
            if (newPurpose != Purpose)
            {
                Purpose = newPurpose;
                this.FlagForUpdate(user, userAgent);
            }
        }
    }
}
