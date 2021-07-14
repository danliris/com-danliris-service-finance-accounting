﻿using System;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.PaymentDispositionNote
{
    public class ReportDto
    {
        public ReportDto(int expenditureId, string expenditureNo, DateTimeOffset expenditureDate, int dispositionId, string dispositionNo, DateTimeOffset dispositionDate, DateTimeOffset dispositionDueDate, int bankId, string bankName, int currencyId, string currencyCode, int supplierId, string supplierName, bool supplierIsImport, string proformaNo, int categoryId, string categoryName, int divisionId, string divisionName, double vATAmount, double paidAmount, string transactionType)
        {
            ExpenditureId = expenditureId;
            ExpenditureNo = expenditureNo;
            ExpenditureDate = expenditureDate;
            DispositionId = dispositionId;
            DispositionNo = dispositionNo;
            DispositionDate = dispositionDate;
            DispositionDueDate = dispositionDueDate;
            BankId = bankId;
            BankName = bankName;
            CurrencyId = currencyId;
            CurrencyCode = currencyCode;
            SupplierId = supplierId;
            SupplierName = supplierName;
            SupplierIsImport = supplierIsImport;
            ProformaNo = proformaNo;
            CategoryId = categoryId;
            CategoryName = categoryName;
            DivisionId = divisionId;
            DivisionName = divisionName;
            VATAmount = vATAmount;
            PaidAmount = paidAmount;
            TransactionType = transactionType;
        }

        public int ExpenditureId { get; private set; }
        public string ExpenditureNo { get; private set; }
        public DateTimeOffset ExpenditureDate { get; private set; }
        public int DispositionId { get; private set; }
        public string DispositionNo { get; private set; }
        public DateTimeOffset DispositionDate { get; private set; }
        public DateTimeOffset DispositionDueDate { get; private set; }
        public int BankId { get; private set; }
        public string BankName { get; private set; }
        public int CurrencyId { get; private set; }
        public string CurrencyCode { get; private set; }
        public int SupplierId { get; private set; }
        public string SupplierName { get; private set; }
        public bool SupplierIsImport { get; private set; }
        public string ProformaNo { get; private set; }
        public int CategoryId { get; private set; }
        public string CategoryName { get; private set; }
        public int DivisionId { get; private set; }
        public string DivisionName { get; private set; }
        public double VATAmount { get; private set; }
        public double PaidAmount { get; private set; }
        public string TransactionType { get; private set; }
    }
}