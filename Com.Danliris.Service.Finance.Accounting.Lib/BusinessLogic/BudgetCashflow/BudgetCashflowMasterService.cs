using Com.Danliris.Service.Finance.Accounting.Lib.Models.BudgetCashflow;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Moonlay.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.BudgetCashflow
{
    public class BudgetCashflowMasterService : IBudgetCashflowMasterService
    {
        private const string UserAgent = "finance-service";
        private readonly FinanceDbContext _dbContext;
        private readonly IIdentityService _identityService;

        public BudgetCashflowMasterService(IServiceProvider serviceProvider)
        {
            _dbContext = serviceProvider.GetService<FinanceDbContext>();
            _identityService = serviceProvider.GetService<IIdentityService>();
        }

        public int CreateBudgetCashflowCategory(CashflowCategoryFormDto form)
        {
            var model = new BudgetCashflowCategoryModel(form.Name, form.Type, form.CashflowTypeId, form.LayoutOrder, form.IsLabelOnly);
            EntityExtension.FlagForCreate(model, _identityService.Username, UserAgent);
            _dbContext.BudgetCashflowCategories.Add(model);
            return _dbContext.SaveChanges();
        }

        public int CreateBudgetCashflowSubCategory(CashflowSubCategoryFormDto form)
        {
            var model = new BudgetCashflowSubCategoryModel(form.Name, form.CashflowCategoryId, form.LayoutOrder, form.PurchasingCategoryIds, form.IsReadOnly);
            EntityExtension.FlagForCreate(model, _identityService.Username, UserAgent);
            _dbContext.BudgetCashflowSubCategories.Add(model);
            return _dbContext.SaveChanges();
        }

        public int CreateBudgetCashflowType(CashflowTypeFormDto form)
        {
            var model = new BudgetCashflowTypeModel(form.Name, form.LayoutOrder);
            EntityExtension.FlagForCreate(model, _identityService.Username, UserAgent);
            _dbContext.BudgetCashflowTypes.Add(model);
            return _dbContext.SaveChanges();
        }

        public int DeleteBudgetCashflowCategories(int id)
        {
            var model = _dbContext.BudgetCashflowCategories.FirstOrDefault(entity => entity.Id == id);
            EntityExtension.FlagForDelete(model, _identityService.Username, UserAgent);
            _dbContext.BudgetCashflowCategories.Update(model);
            return _dbContext.SaveChanges();
        }

        public int DeleteBudgetCashflowSubCategories(int id)
        {
            var model = _dbContext.BudgetCashflowSubCategories.FirstOrDefault(entity => entity.Id == id);
            EntityExtension.FlagForDelete(model, _identityService.Username, UserAgent);
            _dbContext.BudgetCashflowSubCategories.Update(model);
            return _dbContext.SaveChanges();
        }

        public int DeleteBudgetCashflowType(int id)
        {
            var model = _dbContext.BudgetCashflowTypes.FirstOrDefault(entity => entity.Id == id);
            EntityExtension.FlagForDelete(model, _identityService.Username, UserAgent);
            _dbContext.BudgetCashflowTypes.Update(model);
            return _dbContext.SaveChanges();
        }

        public ReadResponse<BudgetCashflowCategoryModel> GetBudgetCashflowCategories(string keyword, int cashflowTypeId, int page, int size)
        {
            var query = _dbContext.BudgetCashflowCategories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(entity => entity.Name.Contains(keyword));

            if (cashflowTypeId > 0)
                query = query.Where(entity => entity.CashflowTypeId == cashflowTypeId);

            var data =  query.OrderBy(entity => entity.LayoutOrder).OrderByDescending(entity => entity.LastModifiedUtc).Skip((page - 1) * size).ToList();
            return new ReadResponse<BudgetCashflowCategoryModel>(data, query.Count(), new Dictionary<string, string>(), new List<string>());
        }

        public ReadResponse<BudgetCashflowMasterDto> GetBudgetCashflowMasterLayout(string keyword, int page, int size)
        {
            var budgetCashflowTypeQuery = _dbContext.BudgetCashflowTypes.OrderBy(entity => entity.LayoutOrder).AsQueryable();
            var budgetCashflowCategoryQuery = _dbContext.BudgetCashflowCategories.OrderBy(entity => entity.LayoutOrder).AsQueryable();
            var budgetCashflowSubCategoryQuery = _dbContext.BudgetCashflowSubCategories.OrderBy(entity => entity.LayoutOrder).AsQueryable();

            var query = from cashflowType in budgetCashflowTypeQuery

                        join cashflowCategory in budgetCashflowCategoryQuery on cashflowType.Id equals cashflowCategory.CashflowTypeId into cashflowTypeWithCategories
                        from cashflowTypeWithCategory in cashflowTypeWithCategories.DefaultIfEmpty()

                        join cashflowSubCategory in budgetCashflowSubCategoryQuery on cashflowTypeWithCategory.Id equals cashflowSubCategory.CashflowCategoryId into cashflowCategoryWithSubCategories
                        from cashflowCategoryWithSubCategory in cashflowCategoryWithSubCategories.DefaultIfEmpty()

                        select new BudgetCashflowMasterDto(cashflowType, cashflowTypeWithCategory, cashflowCategoryWithSubCategory);

            return new ReadResponse<BudgetCashflowMasterDto>(query.ToList(), query.Count(), new Dictionary<string, string>(), new List<string>());
        }

        public ReadResponse<BudgetCashflowTypeModel> GetBudgetCashflowTypes(string keyword, int page, int size)
        {
            var query = _dbContext.BudgetCashflowTypes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(entity => entity.Name.Contains(keyword));

            var data = query.OrderBy(entity => entity.LayoutOrder).OrderByDescending(entity => entity.LastModifiedUtc).Skip((page - 1) * size).ToList();
            return new ReadResponse<BudgetCashflowTypeModel>(data, query.Count(), new Dictionary<string, string>(), new List<string>());
        }

        public ReadResponse<BudgetCashflowSubCategoryModel> GetBudgetCashflowSubCategories(string keyword, int subCategoryId, int page, int size)
        {
            var query = _dbContext.BudgetCashflowSubCategories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(entity => entity.Name.Contains(keyword));

            if (subCategoryId > 0)
                query = query.Where(entity => entity.CashflowCategoryId == subCategoryId);

            var data = query.OrderBy(entity => entity.LayoutOrder).OrderByDescending(entity => entity.LastModifiedUtc).Skip((page - 1) * size).ToList();
            return new ReadResponse<BudgetCashflowSubCategoryModel>(data, query.Count(), new Dictionary<string, string>(), new List<string>());
        }
    }
}
