using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.DPPVATBankExpenditureNote
{
    public class DPPVATBankExpenditureNoteModel : StandardEntity
    {
        public DPPVATBankExpenditureNoteModel()
        {

        }

        public DPPVATBankExpenditureNoteModel(string documentNo, int bankAccountId, string bankAccountNumber, string bankName, string bankAccountingCode, int currencyId, string currencyCode, double currencyRate, int supplierId, string supplierName, bool isImportSupplier, string bgCheckNo, double amount, DateTimeOffset date)
        {
            DocumentNo = documentNo;
            BankAccountId = bankAccountId;
            BankAccountNumber = bankAccountNumber;
            BankName = bankName;
            BankAccountingCode = bankAccountingCode;
            CurrencyId = currencyId;
            CurrencyCode = currencyCode;
            CurrencyRate = currencyRate;
            SupplierId = supplierId;
            SupplierName = supplierName;
            IsImportSupplier = isImportSupplier;
            BGCheckNo = bgCheckNo;
            Amount = amount;
            Date = date;
        }

        public string DocumentNo { get; private set; }
        public int BankAccountId { get; private set; }
        public string BankAccountNumber { get; private set; }
        public string BankName { get; private set; }
        public string BankAccountingCode { get; private set; }
        public int CurrencyId { get; private set; }
        public string CurrencyCode { get; private set; }
        public double CurrencyRate { get; private set; }
        public int SupplierId { get; private set; }
        public string SupplierName { get; private set; }
        public bool IsImportSupplier { get; private set; }
        public string BGCheckNo { get; private set; }
        public double Amount { get; private set; }
        public DateTimeOffset Date { get; private set; }

        public void UpdateData(double amount, int supplierId, bool isImportSupplier, string supplierName, string bgCheckNo, DateTimeOffset date)
        {
            Amount = amount;
            SupplierId = supplierId;
            SupplierName = supplierName;
            IsImportSupplier = isImportSupplier;
            BGCheckNo = bgCheckNo;
            Date = date;
        }
    }
}
