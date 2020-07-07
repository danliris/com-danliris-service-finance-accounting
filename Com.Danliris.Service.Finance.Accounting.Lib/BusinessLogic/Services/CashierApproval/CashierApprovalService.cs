using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.CashierApproval;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.CashierApproval;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CashierApproval;
using Com.Moonlay.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.CashierApproval
{
    public class CashierApprovalService : ICashierApprovalService
    {
        public Task<int> CashierApproval(CashierApprovalViewModel data)
        {
            throw new NotImplementedException();
        }

        public Task<int> CreateAsync(CashierApprovalModel model)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteCashierApproval(int id)
        {
            throw new NotImplementedException();
        }

        public ReadResponse<CashierApprovalModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            throw new NotImplementedException();
        }

        public Task<CashierApprovalModel> ReadByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(int id, CashierApprovalModel model)
        {
            throw new NotImplementedException();
        }
    }
}
