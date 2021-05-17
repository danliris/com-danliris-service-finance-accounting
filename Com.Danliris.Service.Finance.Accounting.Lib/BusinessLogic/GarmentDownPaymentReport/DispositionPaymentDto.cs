﻿using System;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentDownPaymentReport
{
    public class DispositionPaymentDto
    {
        public DispositionPaymentDto(int dispositionPaymentId, string dispositionPaymentNo, DateTimeOffset dispositionPaymentDate)
        {
            DispositionPaymentId = dispositionPaymentId;
            DispositionPaymentNo = dispositionPaymentNo;
            DispositionPaymentDate = dispositionPaymentDate;
        }

        public int DispositionPaymentId { get; private set; }
        public string DispositionPaymentNo { get; private set; }
        public DateTimeOffset DispositionPaymentDate { get; private set; }
        public DateTimeOffset BankExpenditureDate { get; set; }
        public string BankExpenditureNo { get; set; }
        public string DispositionNo { get; set; }
        public string SupplierName { get; set; }
        public int DownPaymentDuration { get; set; }
        public double InitialBalanceDispositionAmount { get; set; }
        public double InitialBalancePaymentAmount { get; set; }
        public double InitialBalanceCurrencyRate { get; set; }
        public double InitialBalanceCurrencyAmount { get; set; }
        public double DownPaymentDispositionAmount { get; set; }
        public double DownPaymentDispositionPaymentAmount { get; set; }
        public double DownPaymentCurrencyRate { get; set; }
        public double DownPaymentCurrencyAmount { get; set; }
        public string LastBalanceCurrencyCode { get; set; }
        public double LastBalanceCurrencyRate { get; set; }
        public double LastBalanceCurrencyAmount { get; set; }
    }
}