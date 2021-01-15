using Com.Danliris.Service.Finance.Accounting.Lib.Models.BudgetCashflow;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.BudgetCashflow
{
    public interface IBudgetCashflowService
    {
        int CreateBudgetCashflowType(CashflowTypeFormDto form);
        ReadResponse<BudgetCashflowTypeModel> GetBudgetCashflowTypes(string keyword, int page, int size);
        BudgetCashflowTypeModel GetBudgetCashflowTypeById(int id);
        int EditBudgetCashflowType(int id, CashflowTypeFormDto form);
        int DeleteBudgetCashflowType(int id);

        int CreateBudgetCashflowCategory(CashflowCategoryFormDto form);
        ReadResponse<BudgetCashflowCategoryModel> GetBudgetCashflowCategories(string keyword, int categoryTypeId, int page, int size);
        BudgetCashflowCategoryModel GetBudgetCashflowCategoryById(int id);
        int EditBudgetCashflowCategory(int id, CashflowCategoryFormDto form);
        int DeleteBudgetCashflowCategories(int id);

        int CreateBudgetCashflowSubCategory(CashflowSubCategoryFormDto form);
        ReadResponse<BudgetCashflowSubCategoryModel> GetBudgetCashflowSubCategories(string keyword, int subCategoryId, int page, int size);
        BudgetCashflowSubCategoryTypeDto GetBudgetCashflowSubCategoryById(int id);
        int EditBudgetCashflowSubCategory(int id, CashflowSubCategoryFormDto form);
        int DeleteBudgetCashflowSubCategories(int id);

        ReadResponse<BudgetCashflowMasterDto> GetBudgetCashflowMasterLayout(string keyword, int page, int size);
        int CreateBudgetCashflowUnit(CashflowUnitFormDto form);
        int EditBudgetCashflowUnit(CashflowUnitFormDto form);
        List<BudgetCashflowItemDto> GetBudgetCashflowUnit(int unitId, DateTimeOffset date);
        List<BudgetCashflowUnitItemDto> GetBudgetCashflowUnit(int unitId, int subCategoryId, DateTimeOffset date);
    }
}
