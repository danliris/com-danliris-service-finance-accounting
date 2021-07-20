using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.PurchasingMemoDetailTextile
{
    public interface IPurchasingMemoDetailTextileService
    {
        int Create(FormDto form);
        ReadResponse<IndexDto> Read(string keyword, int page = 1, int size = 25);
        PurchasingMemoDetailTextileDto Read(int id);
        List<FormItemDto> ReadDispositions(string keyword, int divisionId, bool supplierIsImport, string currencyCode);
        int Update(int id, FormDto form);
        int Delete(int id);
    }
}
