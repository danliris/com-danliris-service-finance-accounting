namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.DPPVATBankExpenditureNote
{
    public class AccountBankDto
    {
        public AccountBankDto(int bankAccountId, string bankAccountingCode, string bankAccountNumber, string bankName)
        {
            BankName = bankName;
            Id = bankAccountId;
            BankCode = bankAccountingCode;
            AccountNumber = bankAccountNumber;
        }

        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; }
    }
}