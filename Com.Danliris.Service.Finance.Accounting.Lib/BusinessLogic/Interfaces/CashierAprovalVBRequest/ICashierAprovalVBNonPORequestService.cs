using Com.Danliris.Service.Finance.Accounting.Lib.Models.CashierAprovalVBRequest;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseInterface;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CashierApproval;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.CashierAprovalVBRequest
{
    public interface ICashierAprovalVBNonPORequestService : IBaseService<CashierAprovalVBNonPORequestModel>
    {
        Task<int> CashierAproval(CashierApprovalViewModel data);
        Task<int> DeleteCashierAproval(int id);
    }
}
