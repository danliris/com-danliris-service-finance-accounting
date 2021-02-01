using System;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.DPPVATBankExpenditureNote
{
    public class DPPVATBankExpenditureNoteIndexDto
    {
        public DPPVATBankExpenditureNoteIndexDto(int id, string documentNo, DateTimeOffset date, string bankName, string bankAccountNumber, double totalAmount, string currencyCode, string documentNoInternalNotes)
        {
            Id = id;
            DocumentNo = documentNo;
            Date = date;
            BankName = bankName;
            BankAccountNumber = bankAccountNumber;
            TotalAmount = totalAmount;
            CurrencyCode = currencyCode;
            DocumentNoInternalNotes = documentNoInternalNotes;
        }

        public int Id { get; }
        public string DocumentNo { get; }
        public DateTimeOffset Date { get; }
        public string BankName { get; }
        public string BankAccountNumber { get; }
        public double TotalAmount { get; }
        public string CurrencyCode { get; }
        public string DocumentNoInternalNotes { get; }
    }
}