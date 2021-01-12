using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRequestDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.BudgetCashflow;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Moonlay.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.BudgetCashflow
{
    public class BudgetCashflowService : IBudgetCashflowService
    {
        private const string UserAgent = "finance-service";
        private readonly FinanceDbContext _dbContext;
        private readonly IIdentityService _identityService;
        private readonly List<CurrencyDto> _currencies;

        public BudgetCashflowService(IServiceProvider serviceProvider)
        {
            _dbContext = serviceProvider.GetService<FinanceDbContext>();
            _identityService = serviceProvider.GetService<IIdentityService>();

            var cache = serviceProvider.GetService<IDistributedCache>();
            var jsonCurrencies = cache.GetString("Currency");
            _currencies = JsonConvert.DeserializeObject<List<CurrencyDto>>(jsonCurrencies, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            });
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

            var data = query.OrderBy(entity => entity.LayoutOrder).Skip((page - 1) * size).ToList();
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

            return new ReadResponse<BudgetCashflowMasterDto>(query.OrderBy(entity => entity.CashflowTypeLayoutOrder).ThenBy(entity => entity.CashflowCategoryLayoutOrder).ThenBy(entity => entity.CashflowSubCategoryLayoutOrder).ToList(), query.Count(), new Dictionary<string, string>(), new List<string>());
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

        public int CreateBudgetCashflowUnit(CashflowUnitFormDto form)
        {
            var models = new List<BudgetCashflowUnitModel>();

            foreach (var item in form.Items)
            {
                var model = new BudgetCashflowUnitModel(form.CashflowSubCategoryId, form.UnitId, form.DivisionId, form.Date.AddHours(_identityService.TimezoneOffset).AddMonths(1), item.CurrencyId, item.CurrencyNominal, item.Nominal, item.Total);
                EntityExtension.FlagForCreate(model, _identityService.Username, UserAgent);
                models.Add(model);
            }

            _dbContext.BudgetCashflowUnits.AddRange(models);
            return _dbContext.SaveChanges();
        }

        //public int 

        public List<BudgetCashflowUnitDto> GetBudgetCashflowUnit(int unitId, DateTimeOffset date)
        {
            var query = from cashflowType in _dbContext.BudgetCashflowTypes

                        join cashflowCategory in _dbContext.BudgetCashflowCategories on cashflowType.Id equals cashflowCategory.CashflowTypeId into cashflowTypeCategories
                        from cashflowTypeCategory in cashflowTypeCategories.DefaultIfEmpty()

                        join cashflowSubCategory in _dbContext.BudgetCashflowSubCategories on cashflowTypeCategory.Id equals cashflowSubCategory.CashflowCategoryId into cashflowCategorySubCategories
                        from cashflowCategorySubCategory in cashflowCategorySubCategories.DefaultIfEmpty()

                        select new
                        {
                            CashflowTypeId = cashflowType.Id,
                            CashflowTypeName = cashflowType.Name,
                            CashflowTypeLayoutOrder = cashflowType.LayoutOrder,
                            CashflowCashType = cashflowTypeCategory != null ? cashflowTypeCategory.Type : 0,
                            CashflowCategoryId = cashflowTypeCategory != null ? cashflowTypeCategory.Id : 0,
                            CashflowCategoryName = cashflowTypeCategory != null ? cashflowTypeCategory.Name : "",
                            CashflowCategoryLayoutOrder = cashflowTypeCategory != null ? cashflowTypeCategory.LayoutOrder : 0,
                            CashflowSubCategoryId = cashflowCategorySubCategory != null ? cashflowCategorySubCategory.Id : 0,
                            CashflowSubCategoryName = cashflowCategorySubCategory != null ? cashflowCategorySubCategory.Name : "",
                            CashflowSubCategoryLayoutOrder = cashflowCategorySubCategory != null ? cashflowCategorySubCategory.LayoutOrder : 0,
                            CashflowSubCategoryReadOnly = cashflowCategorySubCategory != null && cashflowCategorySubCategory.IsReadOnly
                        };

            query = query.OrderBy(entity => entity.CashflowTypeLayoutOrder).ThenBy(entity => entity.CashflowCashType).ThenBy(entity => entity.CashflowCategoryLayoutOrder).ThenBy(entity => entity.CashflowSubCategoryLayoutOrder);

            var result = new List<BudgetCashflowUnitDto>();

            var cashflowSubCategoryIds = query.Select(entity => entity.CashflowSubCategoryId).ToList();
            var cashflowUnits = _dbContext.BudgetCashflowUnits
                .Where(entity => entity.UnitId == unitId && cashflowSubCategoryIds.Contains(entity.BudgetCashflowSubCategoryId) && entity.Month == date.AddHours(_identityService.TimezoneOffset).AddMonths(1).Month && entity.Year == date.AddHours(_identityService.TimezoneOffset).AddMonths(1).Year)
                .ToList();

            var previousCashflowTypeId = 0;
            var previousCashflowCategoryId = 0;
            var previousCashInCashOut = (CashType)0;
            foreach (var item in query.ToList())
            {
                var selectedCashflowUnits = cashflowUnits.Where(element => element.BudgetCashflowSubCategoryId == item.CashflowSubCategoryId).ToList();
                var cashflowItem = new BudgetCashflowUnitDto(item.CashflowTypeId, item.CashflowTypeName, item.CashflowCategoryId, item.CashflowCategoryName, item.CashflowSubCategoryId, item.CashflowSubCategoryName, item.CashflowSubCategoryReadOnly, item.CashflowCashType);

                if (item.CashflowTypeId != previousCashflowTypeId)
                {
                    previousCashflowTypeId = item.CashflowTypeId;
                    cashflowItem.UseSection();
                }

                if (item.CashflowCashType != previousCashInCashOut)
                {
                    previousCashInCashOut = item.CashflowCashType;
                    cashflowItem.UseGroup();
                }

                if (item.CashflowCategoryId != previousCashflowCategoryId)
                {
                    previousCashflowCategoryId = item.CashflowCategoryId;
                    result.Add(cashflowItem);
                    cashflowItem = new BudgetCashflowUnitDto(cashflowItem);
                }

                if (selectedCashflowUnits.Count > 0)
                    foreach (var cashflowUnit in selectedCashflowUnits)
                    {
                        var currency = _currencies.FirstOrDefault(element => element.Id.GetValueOrDefault() == cashflowUnit.CurrencyId);
                        cashflowItem.SetNominal(currency, cashflowUnit.CurrencyNominal, cashflowUnit.Nominal, cashflowUnit.Total);
                        result.Add(cashflowItem);
                    }
                else if (item.CashflowSubCategoryId > 0)
                    result.Add(cashflowItem);
            }

            var cashflowTypeIds = result.Select(element => element.CashflowTypeId).Distinct().ToList();
            foreach (var cashflowTypeId in cashflowTypeIds)
            {
                var item = result.FirstOrDefault(element => element.CashflowTypeId == cashflowTypeId);
                item.SetCashflowTypeRowspan(result.Count(element => element.CashflowTypeId == cashflowTypeId));

                var cashIn = result.FirstOrDefault(element => element.CashflowTypeId == cashflowTypeId && element.TypeName == CashType.In.ToDescriptionString());
                if (cashIn != null)
                    cashIn.SetGroupRowspan(result.Count(element => element.CashflowTypeId == cashflowTypeId && element.TypeName == CashType.In.ToDescriptionString()));

                var cashOut = result.FirstOrDefault(element => element.CashflowTypeId == cashflowTypeId && element.TypeName == CashType.Out.ToDescriptionString());
                if (cashOut != null)
                    cashIn.SetGroupRowspan(result.Count(element => element.CashflowTypeId == cashflowTypeId && element.TypeName == CashType.Out.ToDescriptionString()));
            }

            return result;
        }

        public int EditBudgetCashflowUnit(CashflowUnitFormDto form)
        {
            var existingModels = _dbContext.BudgetCashflowUnits.Where(entity => entity.UnitId == form.UnitId && entity.DivisionId == form.DivisionId && entity.Month == form.Date.AddHours(_identityService.TimezoneOffset).AddMonths(1).Month && entity.Year == form.Date.AddHours(_identityService.TimezoneOffset).AddMonths(1).Year).ToList();
            foreach (var existingModel in existingModels)
            {
                EntityExtension.FlagForDelete(existingModel, _identityService.Username, UserAgent);
            }
            _dbContext.BudgetCashflowUnits.UpdateRange(existingModels);

            var models = new List<BudgetCashflowUnitModel>();
            foreach (var item in form.Items)
            {
                var model = new BudgetCashflowUnitModel(form.CashflowSubCategoryId, form.UnitId, form.DivisionId, form.Date.AddHours(_identityService.TimezoneOffset).AddMonths(1), item.CurrencyId, item.CurrencyNominal, item.Nominal, item.Total);
                EntityExtension.FlagForCreate(model, _identityService.Username, UserAgent);
                models.Add(model);
            }
            _dbContext.BudgetCashflowUnits.AddRange(models);

            return _dbContext.SaveChanges();
        }
    }
}
