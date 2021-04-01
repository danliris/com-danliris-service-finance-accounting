using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.AccountingBook;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.AccountingBook;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.AccountingBook;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.AccountingBook
{
    public class AccountingBookDataUtil
    {
        private readonly AccountingBookService _service;

        public AccountingBookDataUtil(AccountingBookService service)
        {
            _service = service;
        }

        public AccountingBookModel GetNewdata_AccountingBookModel_ModelToCreate()
        {
            return new AccountingBookModel()
            {
                AccountingBookType = "TestName",
                Code="TestCode",
                Remarks = "TestRemarks"
            };
        }

        public AccountingBookViewModel GetNewdata_AccountingBookViewModel()
        {
            return new AccountingBookViewModel()
            {
                AccountingBookType = "TestName",
                Code = "TestCode",
                Remarks = "TestRemarks"
            };
        }

        public AccountingBookViewModel GetDataToValidate()
        {
            return new AccountingBookViewModel()
            {
               Id = 0
            };
        }

        public async Task<AccountingBookModel> GetCreatedData()
        {
            var model = GetNewdata_AccountingBookModel_ModelToCreate();
            await _service.CreateAsync(model);
            return await _service.ReadByIdAsync(model.Id);
        }
    }
}
