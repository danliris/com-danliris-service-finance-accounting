using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentDebtBalance
{
    public interface IGarmentDebtBalanceService
    {
        List<GarmentDebtBalanceCardDto> GetDebtBalanceCardDto(int supplierId, int month, int year);
        GarmentDebtBalanceIndexDto GetDebtBalanceCardIndex(int supplierId, int month, int year);
    }
}
