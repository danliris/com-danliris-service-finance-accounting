using Com.Moonlay.Models;
using System.ComponentModel.DataAnnotations;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.BudgetCashflow
{
    public class BudgetCashflowTypeModel : StandardEntity
    {
        public BudgetCashflowTypeModel()
        {

        }

        public BudgetCashflowTypeModel(string name, int layoutOrder)
        {
            Name = name;
            LayoutOrder = layoutOrder;
        }

        [MaxLength(512)]
        public string Name { get; private set; }
        public int LayoutOrder { get; private set; }
    }
}
