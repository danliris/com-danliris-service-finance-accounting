using System;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.DPPVATBankExpenditureNote
{
    public class DPPVATBankExpenditureNoteIndexDto
    {
        public DPPVATBankExpenditureNoteIndexDto(int id, string documentNo, string bankCashNo, DateTimeOffset date, string bankName, string bankAccountNumber, double totalAmount, string currencyCode, string documentNoInternalNotes, string supplierName, bool isPosted)
        {
            Id = id;
            DocumentNo = documentNo;
            BankCashNo = bankCashNo;
            Date = date;
            BankName = bankName;
            BankAccountNumber = bankAccountNumber;
            TotalAmount = totalAmount;
            CurrencyCode = currencyCode;
            DocumentNoInternalNotes = documentNoInternalNotes;
            SupplierName = supplierName;
            IsPosted = isPosted;
        }

        public int Id { get; }
        public string DocumentNo { get; }
        public string BankCashNo { get; }
        public DateTimeOffset Date { get; }
        public string BankName { get; }
        public string BankAccountNumber { get; }
        public double TotalAmount { get; }
        public string CurrencyCode { get; }
        public string DocumentNoInternalNotes { get; }
        public string SupplierName { get; }
        public bool IsPosted { get; private set; }
    }
}