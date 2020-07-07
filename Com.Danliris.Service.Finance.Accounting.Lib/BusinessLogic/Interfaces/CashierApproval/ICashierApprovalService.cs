using Com.Danliris.Service.Finance.Accounting.Lib.Models.CashierApproval;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseInterface;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CashierApproval;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.CashierApproval
{
    public interface ICashierApprovalService : IBaseService<CashierApprovalModel>
    {
        Task<int> CashierApproval(CashierApprovalViewModel data);
        Task<int> DeleteCashierApproval(int id);
    }
}
