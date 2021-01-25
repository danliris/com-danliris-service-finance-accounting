using Com.Danliris.Service.Finance.Accounting.Lib.Models.BudgetCashflow;
using System;
using System.Collections.Generic;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.BudgetCashflow
{
    public class BudgetCashflowDivisionItemDto
    {
        public BudgetCashflowDivisionItemDto(BudgetCashflowTypeModel cashflowType)
        {
            CashflowType = cashflowType;
        }

        public BudgetCashflowDivisionItemDto()
        {
        }

        public BudgetCashflowTypeModel CashflowType { get; private set; }
        public bool IsUseSection { get; private set; }
        public int SectionRows { get; private set; }
        public string TypeName { get; private set; }
        public CashType Type { get; private set; }
        public bool IsUseGroup { get; private set; }
        public int GroupRows { get; private set; }
        public bool IsLabelOnly { get; private set; }
        public BudgetCashflowCategoryModel CashflowCategory { get; private set; }
        public BudgetCashflowSubCategoryModel CashflowSubCategory { get; private set; }
        public CurrencyDto Currency { get; private set; }
        public bool IsShowSubCategory { get; private set; }
        public List<BudgetCashflowDivisionUnitItemDto> Items { get; private set; }
        public double DivisionCurrencyNominalTotal { get; private set; }
        public double DivisionNominalTotal { get; private set; }
        public double DivisionActualTotal { get; private set; }

        public void SetSection(int sectionRows)
        {
            IsUseSection = true;
            SectionRows = sectionRows;
        }

        public void SetGroup(CashType type, int groupRows)
        {
            TypeName = type.ToDescriptionString();
            Type = type;
            IsUseGroup = true;
            GroupRows = groupRows;
        }

        public void SetLabelOnly(BudgetCashflowCategoryModel cashflowCategory)
        {
            IsLabelOnly = true;
            CashflowCategory = cashflowCategory;
        }

        public void SetSubCategory(BudgetCashflowSubCategoryModel cashflowSubCategory, CurrencyDto currency, bool isShowSubCategory)
        {
            CashflowSubCategory = cashflowSubCategory;
            Currency = currency;
            IsShowSubCategory = isShowSubCategory;
        }

        public void SetRowSummary(double divisionCurrencyNominalTotal, double divisionNominalTotal, double divisionActualTotal)
        {
            DivisionCurrencyNominalTotal = divisionCurrencyNominalTotal;
            DivisionNominalTotal = divisionNominalTotal;
            DivisionActualTotal = divisionActualTotal;
        }

        public void InitializeItems()
        {
            Items = new List<BudgetCashflowDivisionUnitItemDto>();
        }
    }
}