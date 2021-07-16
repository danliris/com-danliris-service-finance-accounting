using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.PurchasingMemoDetailTextile
{
    public interface IPurchasingMemoDetailTextileService
    {
        int Create(FormDto form);
        int Read(string keyword, int page = 1, int size = 25);

    }
}
