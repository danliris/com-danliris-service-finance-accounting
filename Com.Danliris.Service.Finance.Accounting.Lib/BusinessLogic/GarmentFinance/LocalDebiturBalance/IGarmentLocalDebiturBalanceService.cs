using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentFinance.LocalDebiturBalance;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentFinance.LocalDebiturBalance
{
    public interface IGarmentLocalDebiturBalanceService
    {
        ReadResponse<GarmentLocalDebiturBalanceModel> Read(int page, int size, string order, List<string> select, string keyword, string filter = "{}");
        
        Task<GarmentLocalDebiturBalanceModel> ReadByIdAsync(int id);
    }
}
