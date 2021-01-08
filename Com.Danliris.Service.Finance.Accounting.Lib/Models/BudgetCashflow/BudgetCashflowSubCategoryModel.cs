using Com.Moonlay.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.BudgetCashflow
{
    public class BudgetCashflowSubCategoryModel : StandardEntity
    {
        public BudgetCashflowSubCategoryModel()
        {

        }

        public BudgetCashflowSubCategoryModel(string name, int cashflowCategoryId, int layoutOrder, List<int> purchasingCategoryIds, bool isReadOnly)
        {
            Name = name;
            CashflowCategoryId = cashflowCategoryId;
            LayoutOrder = layoutOrder;

            if (purchasingCategoryIds == null)
                purchasingCategoryIds = new List<int>();

            PurchasingCategoryIds = JsonConvert.SerializeObject(purchasingCategoryIds);
            IsReadOnly = isReadOnly;
        }

        [MaxLength(512)]
        public string Name { get; private set; }
        public int CashflowCategoryId { get; private set; }
        public int LayoutOrder { get; private set; }
        public string PurchasingCategoryIds { get; private set; }
        public bool IsReadOnly { get; private set; }
    }
}
