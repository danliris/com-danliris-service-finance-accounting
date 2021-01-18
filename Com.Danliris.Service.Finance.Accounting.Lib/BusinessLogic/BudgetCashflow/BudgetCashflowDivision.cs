using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.BudgetCashflow
{
    public class BudgetCashflowDivision
    {
        public BudgetCashflowDivision(List<string> headers, List<BudgetCashflowItemDivisionDto> items)
        {
            Headers = headers;
            Items = items;
        }

        public List<string> Headers { get; private set; }
        public List<BudgetCashflowItemDivisionDto> Items { get; private set; }
    }
}
