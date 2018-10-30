using Com.Danliris.Service.Finance.Accounting.Lib.Models.CreditorAccount;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseInterface;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CreditorAccount;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.CreditorAccount
{
    public interface ICreditorAccountService : IBaseService<CreditorAccountModel>
    {
        ReadResponse<CreditorAccountViewModel> GetReport(int page, int size, string suplierName, int month, int year, int offSet);
        MemoryStream GenerateExcel(string suplierName, int month, int year, int offSet);
        Task<int> UpdateFromUnitReceiptNoteAsync(string supplierCode, string unitReceiptNote, string invoiceNo, CreditorAccountModel model);
    }
}
