using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.BudgetCashflow
{
    public class BudgetCashflowDivision
    {
        public BudgetCashflowDivision(List<string> headers, List<BudgetCashflowItemDto> items)
        {
            Headers = headers;
            Items = items;
        }

        public List<string> Headers { get; private set; }
        public List<BudgetCashflowItemDto> Items { get; private set; }
    }
}
