using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.CreditorAccount;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.CreditorAccount;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CreditorAccount;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.CreditorAccount
{
    public class CreditorAccountDataUtil
    {
        private readonly CreditorAccountService Service;

        public CreditorAccountDataUtil(CreditorAccountService service)
        {
            Service = service;
        }

        public CreditorAccountUnitReceiptNotePostedViewModel GetUnitReceiptNotePostedViewModel()
        {
            return new CreditorAccountUnitReceiptNotePostedViewModel()
            {
                Code = "Code",
                CreditorAccountId = 1,
                Date = DateTimeOffset.UtcNow,
                DPP = 1000,
                PPN = 100,
                InvoiceNo = "InvoiceNo",
                SupplierCode = "SupplierCode",
                SupplierName = "SupplierName"
            };
        }
        
    }
}
