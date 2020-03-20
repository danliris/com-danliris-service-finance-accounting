using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.ViewModels.NewIntegrationViewModel
{
   public class AccountBankViewModelTest
    {
        [Fact]
        public void Should_Success_Instantiate()
        {
            int id = 1;
            string code = "code test";
            string BankCode = "BankCode test";
            string AccountName = "AccountName test";
            string AccountNumber = "AccountNumber test";
            string BankName = "BankName test";
            CurrencyViewModel cvm = new CurrencyViewModel();

            AccountBankViewModel abvm = new AccountBankViewModel();
            abvm.Id = id;
            abvm.Code = code;
            abvm.BankCode = BankCode;
            abvm.AccountName = AccountName;
            abvm.AccountNumber = AccountNumber;
            abvm.BankName = BankName;
            abvm.Currency = cvm;

            Assert.Equal(id, abvm.Id);
            Assert.Equal(code, abvm.Code);
            Assert.Equal(BankCode, abvm.BankCode);
            Assert.Equal(AccountName, abvm.AccountName);
            Assert.Equal(AccountNumber, abvm.AccountNumber);
            Assert.Equal(cvm, abvm.Currency);

        }
    }
}
