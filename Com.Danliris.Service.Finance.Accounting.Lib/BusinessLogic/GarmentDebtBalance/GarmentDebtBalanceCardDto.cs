﻿using System;

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
        public string ProductNames { get; private set; }
        public double CurrencyRate { get; set; }

        public double TotalInvoice { get; set; }
        public double MutationPurchase { get; set; }
        public double MutationPayment { get; set; }
        public double RemainBalance { get; set; }

        public GarmentDebtBalanceCardDto(Models.GarmentDebtBalance.GarmentDebtBalanceModel model)
        {
            PurchasingCategoryId = model.PurchasingCategoryId;
            PurchasingCategoryName = model.PurchasingCategoryName;
            BillsNo = model.BillsNo;
            PaymentBills = model.PaymentBills;
            GarmentDeliveryOrderId = model.GarmentDeliveryOrderId;
            GarmentDeliveryOrderNo = model.GarmentDeliveryOrderNo;
            InvoiceId = model.InvoiceId;
            InvoiceDate = model.InvoiceDate;
            InvoiceNo = model.InvoiceNo;
            SupplierId = model.SupplierId;
            SupplierName = model.SupplierName;
            CurrencyId = model.CurrencyId;
            CurrencyCode = model.CurrencyCode;
            DPPAmount = model.DPPAmount;
            CurrencyDPPAmount = model.CurrencyDPPAmount;
            VATAmount = model.VATAmount;
            IncomeTaxAmount = model.IncomeTaxAmount;
            IsPayVAT = model.IsPayVAT;
            IsPayIncomeTax = model.IsPayIncomeTax;
            BankExpenditureNoteId = model.BankExpenditureNoteId;
            BankExpenditureNoteNo = model.BankExpenditureNoteNo;
            BankExpenditureNoteInvoiceAmount = model.BankExpenditureNoteInvoiceAmount;
            InternalNoteId = model.InternalNoteId;
            InternalNoteNo = model.InternalNoteNo;
            ProductNames = model.ProductNames;
            CurrencyRate = model.CurrencyRate;
            TotalInvoice = model.CurrencyDPPAmount + model.VATAmount - model.IncomeTaxAmount;
            MutationPurchase = (model.CurrencyDPPAmount + model.VATAmount - model.IncomeTaxAmount) * model.CurrencyRate;
            MutationPayment = model.BankExpenditureNoteInvoiceAmount;
            RemainBalance = ((model.CurrencyDPPAmount + model.VATAmount - model.IncomeTaxAmount) * model.CurrencyRate) - model.BankExpenditureNoteInvoiceAmount;
        }
        /// <summary>
        /// ovveride for saldo awal (pdf)
        /// </summary>
        /// <param name="productName"></param>
        /// <param name="remainBalance"></param>
        public GarmentDebtBalanceCardDto(string productName, double remainBalance)
        {
            ProductNames = productName;
            RemainBalance = remainBalance;
            InvoiceDate = DateTimeOffset.MaxValue;
        }
        /// <summary>
        /// override for total (pdf)
        /// </summary>
        /// <param name="productName"></param>
        /// <param name="remainBalance"></param>
        /// <param name="mutationPurchase"></param>
        /// <param name="mutationPayment"></param>
        public GarmentDebtBalanceCardDto(string productName, double remainBalance,double mutationPurchase, double mutationPayment)
        {
            ProductNames = productName;
            RemainBalance = remainBalance;
            InvoiceDate = DateTimeOffset.MinValue;

        }
    }
}