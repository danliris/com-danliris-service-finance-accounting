using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.AccountingBook;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.AccountingBook;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.AccountingBook
{
    public class AccountingBookDataUtil
    {
        private readonly AccountingBookService _service;

        public AccountingBookDataUtil(AccountingBookService service)
        {
            _service = service;
        }

        public AccountingBookModel GetNewdata_AccountingBookModel()
        {
            return new AccountingBookModel()
            {
                AccountingBookType = "TestName",
                Code="TestCode",
                Remarks = "TestRemarks"
            };
        }
    }
}
