using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Danliris.Service.Finance.Accounting.Lib
{
    public class RealizationVbModel: StandardEntity
    {
        [MaxLength(64)]
        public string VBNo { get; set; }
        [MaxLength(64)]
        public string VBNoRealize { get; set; }
        public DateTimeOffset Date { get; set; }
        public DateTimeOffset DateEstimate { get; set; }
        public DateTimeOffset DateVB { get; set; }
        [MaxLength(64)]
        public string CurrencyCodeNonPO { get; set; }
        public decimal CurrencyRateNonPO { get; set; }
        [MaxLength(64)]
        public string UnitLoad { get; set; }
        [MaxLength(64)]
        public string UnitCode { get; set; }
        [MaxLength(64)]
        public string UnitName { get; set; }
        [MaxLength(64)]
        public string RequestVbName { get; set; }
        public decimal Amount_VB { get; set; }
        public decimal AmountNonPO { get; set; }
        public decimal DifferenceReqReal { get; set; }
        [MaxLength(64)]
        public string StatusReqReal { get; set; }
        public bool isVerified { get; set; }
        public DateTimeOffset VerifiedDate { get; set; }
        public bool isClosed { get; set; }
        public DateTimeOffset CloseDate { get; set; }
        public bool isNotVeridied { get; set; }
        public decimal Amount { get; set; }
        [MaxLength(255)]
        public string Reason_NotVerified { get; set; }
        [MaxLength(64)]
        public string VBRealizeCategory { get; set; }

        public virtual ICollection<RealizationVbDetailModel> RealizationVbDetail { get; set; }
    }
}