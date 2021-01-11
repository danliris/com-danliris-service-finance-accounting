using Com.Danliris.Service.Finance.Accounting.Lib.Models.BudgetCashflow;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.BudgetCashflow
{
    public interface IBudgetCashflowService
    {
        ReadResponse<BudgetCashflowTypeModel> GetBudgetCashflowTypes(string keyword, int page, int size);
        ReadResponse<BudgetCashflowCategoryModel> GetBudgetCashflowCategories(string keyword, int categoryTypeId, int page, int size);
        ReadResponse<BudgetCashflowSubCategoryModel> GetBudgetCashflowSubCategories(string keyword, int subCategoryId, int page, int size);
        ReadResponse<BudgetCashflowMasterDto> GetBudgetCashflowMasterLayout(string keyword, int page, int size);
        int DeleteBudgetCashflowType(int id);
        int DeleteBudgetCashflowCategories(int id);
        int DeleteBudgetCashflowSubCategories(int id);
        int CreateBudgetCashflowType(CashflowTypeFormDto form);
        int CreateBudgetCashflowCategory(CashflowCategoryFormDto form);
        int CreateBudgetCashflowSubCategory(CashflowSubCategoryFormDto form);
        int CreateBudgetCashflowUnit(CashflowUnitFormDto form);
        int EditBudgetCashflowUnit(CashflowUnitFormDto form);
        List<BudgetCashflowUnitDto> GetBudgetCashflowUnit(int unitId, DateTimeOffset date);
    }
}
