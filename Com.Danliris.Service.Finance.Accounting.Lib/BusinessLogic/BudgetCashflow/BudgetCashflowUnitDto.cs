using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRequestDocument;
using System;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.BudgetCashflow
{
    public class BudgetCashflowUnitDto
    {
        public BudgetCashflowUnitDto(int cashflowTypeId, string cashflowTypeName, int cashflowCategoryId, string cashflowCategoryName, int cashflowSubCategoryId, string cashflowSubCategoryName, bool cashflowSubCategoryReadOnly, CashType cashflowCashType)
        {
            CashflowTypeId = cashflowTypeId;
            CashflowTypeName = cashflowTypeName;
            CashflowCategoryId = cashflowCategoryId;
            CashflowCategoryName = cashflowCategoryName;
            CashflowSubCategoryId = cashflowSubCategoryId;
            CashflowSubCategoryName = cashflowSubCategoryName;
            IsReadOnly = cashflowSubCategoryReadOnly;
            TypeName = cashflowCashType.ToDescriptionString();
        }

        public BudgetCashflowUnitDto(BudgetCashflowUnitDto cashflowItem)
        {
            CashflowTypeId = cashflowItem.CashflowTypeId;
            CashflowTypeName = cashflowItem.CashflowTypeName;
            CashflowCategoryId = cashflowItem.CashflowCategoryId;
            CashflowCategoryName = cashflowItem.CashflowCategoryName;
            CashflowSubCategoryId = cashflowItem.CashflowSubCategoryId;
            CashflowSubCategoryName = cashflowItem.CashflowSubCategoryName;
            IsReadOnly = cashflowItem.IsReadOnly;
        }

        public int CashflowTypeId { get; private set; }
        public string CashflowTypeName { get; private set; }
        public int CashflowCategoryId { get; private set; }
        public string CashflowCategoryName { get; private set; }
        public int CashflowSubCategoryId { get; private set; }
        public string CashflowSubCategoryName { get; private set; }
        public bool IsReadOnly { get; private set; }
        public string TypeName { get; private set; }
        public bool IsUseSection { get; private set; }
        public int GroupRowspan { get; private set; }
        public bool IsUseGroup { get; private set; }
        public bool IsLabelOnly { get; private set; }
        public CurrencyDto Currency { get; private set; }
        public double CurrencyNominal { get; private set; }
        public double Nominal { get; private set; }
        public double Total { get; private set; }
        public int CashflowTypeRowspan { get; private set; }

        public void UseSection()
        {
            IsUseSection = true;
        }

        public void UseGroup()
        {
            IsUseGroup = true;
        }

        public void LabelOnly()
        {
            IsLabelOnly = true;
        }

        public void NotLabelOnly()
        {
            IsLabelOnly = false;
        }

        internal void SetNominal(CurrencyDto currency, double currencyNominal, double nominal, double total)
        {
            Currency = currency;
            CurrencyNominal = currencyNominal;
            Nominal = nominal;
            Total = total;
        }

        public void SetCashflowTypeRowspan(int cashflowTypeRowspan)
        {
            CashflowTypeRowspan = cashflowTypeRowspan;
        }

        public void SetGroupRowspan(int rowspan)
        {
            GroupRowspan = rowspan;
            IsUseGroup = true;
        }
    }
}