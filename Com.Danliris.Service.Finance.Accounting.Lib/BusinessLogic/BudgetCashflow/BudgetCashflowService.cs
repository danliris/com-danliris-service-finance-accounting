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

            return new ReadResponse<BudgetCashflowMasterDto>(query.OrderBy(entity => entity.CashflowTypeLayoutOrder).ThenBy(entity => entity.CashType).ThenBy(entity => entity.CashflowCategoryLayoutOrder).ThenBy(entity => entity.CashflowSubCategoryLayoutOrder).ToList(), query.Count(), new Dictionary<string, string>(), new List<string>());
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

        //public List<BudgetCashflowUnitDto> GetBudgetCashflowUnit(int unitId, DateTimeOffset date)
        //{
        //    var query = from cashflowType in _dbContext.BudgetCashflowTypes

        //                join cashflowCategory in _dbContext.BudgetCashflowCategories on cashflowType.Id equals cashflowCategory.CashflowTypeId into cashflowTypeCategories
        //                from cashflowTypeCategory in cashflowTypeCategories.DefaultIfEmpty()

        //                join cashflowSubCategory in _dbContext.BudgetCashflowSubCategories on cashflowTypeCategory.Id equals cashflowSubCategory.CashflowCategoryId into cashflowCategorySubCategories
        //                from cashflowCategorySubCategory in cashflowCategorySubCategories.DefaultIfEmpty()

        //                select new
        //                {
        //                    CashflowTypeId = cashflowType.Id,
        //                    CashflowTypeName = cashflowType.Name,
        //                    CashflowTypeLayoutOrder = cashflowType.LayoutOrder,
        //                    CashflowCashType = cashflowTypeCategory != null ? cashflowTypeCategory.Type : 0,
        //                    CashflowCategoryId = cashflowTypeCategory != null ? cashflowTypeCategory.Id : 0,
        //                    CashflowCategoryName = cashflowTypeCategory != null ? cashflowTypeCategory.Name : "",
        //                    CashflowCategoryLayoutOrder = cashflowTypeCategory != null ? cashflowTypeCategory.LayoutOrder : 0,
        //                    CashflowSubCategoryId = cashflowCategorySubCategory != null ? cashflowCategorySubCategory.Id : 0,
        //                    CashflowSubCategoryName = cashflowCategorySubCategory != null ? cashflowCategorySubCategory.Name : "",
        //                    CashflowSubCategoryLayoutOrder = cashflowCategorySubCategory != null ? cashflowCategorySubCategory.LayoutOrder : 0,
        //                    CashflowSubCategoryReadOnly = cashflowCategorySubCategory != null && cashflowCategorySubCategory.IsReadOnly
        //                };

        //    query = query.OrderBy(entity => entity.CashflowTypeLayoutOrder).ThenBy(entity => entity.CashflowCashType).ThenBy(entity => entity.CashflowCategoryLayoutOrder).ThenBy(entity => entity.CashflowSubCategoryLayoutOrder);

        //    var result = new List<BudgetCashflowUnitDto>();

        //    var cashflowSubCategoryIds = query.Select(entity => entity.CashflowSubCategoryId).ToList();
        //    var cashflowUnits = _dbContext.BudgetCashflowUnits
        //        .Where(entity => entity.UnitId == unitId && cashflowSubCategoryIds.Contains(entity.BudgetCashflowSubCategoryId) && entity.Month == date.AddHours(_identityService.TimezoneOffset).AddMonths(1).Month && entity.Year == date.AddHours(_identityService.TimezoneOffset).AddMonths(1).Year)
        //        .ToList();

        //    var previousCashflowTypeId = 0;
        //    var previousCashflowCategoryId = 0;
        //    var previousCashInCashOut = (CashType)0;
        //    foreach (var item in query.ToList())
        //    {
        //        var selectedCashflowUnits = cashflowUnits.Where(element => element.BudgetCashflowSubCategoryId == item.CashflowSubCategoryId).ToList();
        //        var cashflowItem = new BudgetCashflowUnitDto(item.CashflowTypeId, item.CashflowTypeName, item.CashflowCategoryId, item.CashflowCategoryName, item.CashflowSubCategoryId, item.CashflowSubCategoryName, item.CashflowSubCategoryReadOnly, item.CashflowCashType);

        //        if (item.CashflowTypeId != previousCashflowTypeId)
        //        {
        //            previousCashflowTypeId = item.CashflowTypeId;
        //            cashflowItem.UseSection();
        //        }

        //        if (item.CashflowCashType != previousCashInCashOut)
        //        {
        //            previousCashInCashOut = item.CashflowCashType;
        //            cashflowItem.UseGroup();
        //        }

        //        if (item.CashflowCategoryId != previousCashflowCategoryId)
        //        {
        //            previousCashflowCategoryId = item.CashflowCategoryId;
        //            cashflowItem.LabelOnly();
        //            result.Add(cashflowItem);
        //            cashflowItem = new BudgetCashflowUnitDto(cashflowItem);
        //        }

        //        var isFirst = true;
        //        if (selectedCashflowUnits.Count > 0)
        //            foreach (var cashflowUnit in selectedCashflowUnits)
        //            {
        //                cashflowItem = new BudgetCashflowUnitDto(cashflowItem, isFirst);
        //                var currency = _currencies.FirstOrDefault(element => element.Id.GetValueOrDefault() == cashflowUnit.CurrencyId);
        //                cashflowItem.SetNominal(currency, cashflowUnit.CurrencyNominal, cashflowUnit.Nominal, cashflowUnit.Total);
        //                result.Add(cashflowItem);
        //                isFirst = false;
        //            }
        //        else if (item.CashflowSubCategoryId > 0)
        //        {
        //            cashflowItem.ShowLabel();
        //            result.Add(cashflowItem);
        //        }
        //    }

        //    var cashflowTypeIds = query.Select(element => element.CashflowTypeId).Distinct().ToList();
        //    var counter = 0;

        //    //foreach (var cashflowTypeId in cashflowTypeIds)
        //    //{
        //    //    var cashInSummary = result.Where(element => element.CashflowTypeId == cashflowTypeId && element.TypeName == CashType.In.ToDescriptionString());
        //    //    var cashInCurrencies = cashInSummary.Where(element => element.Currency != null && element.Currency.Id > 0).Select(element => element.Currency.Id).Distinct().ToList();

        //    //    foreach (var currency in cashInCurrencies)
        //    //    {

        //    //    }
        //    //}

        //    foreach (var cashflowTypeId in cashflowTypeIds)
        //    {
        //        var item = result.FirstOrDefault(element => element.CashflowTypeId == cashflowTypeId);
        //        item.SetCashflowTypeRowspan(result.Count(element => element.CashflowTypeId == cashflowTypeId));

        //        var cashInSummary = result.Where(element => element.CashflowTypeId == cashflowTypeId && element.TypeName == CashType.In.ToDescriptionString());
        //        var cashOutSummary = result.Where(element => element.CashflowTypeId == cashflowTypeId && element.TypeName == CashType.Out.ToDescriptionString());

        //        var cashIn = result.FirstOrDefault(element => element.CashflowTypeId == cashflowTypeId && element.TypeName == CashType.In.ToDescriptionString());
        //        if (cashIn != null)
        //            cashIn.SetGroupRowspan(result.Count(element => element.CashflowTypeId == cashflowTypeId && element.TypeName == CashType.In.ToDescriptionString()));

        //        var cashOut = result.FirstOrDefault(element => element.CashflowTypeId == cashflowTypeId && element.TypeName == CashType.Out.ToDescriptionString());
        //        if (cashOut != null)
        //            cashOut.SetGroupRowspan(result.Count(element => element.CashflowTypeId == cashflowTypeId && element.TypeName == CashType.Out.ToDescriptionString()));


        //    }

        //    return result;
        //}

        public List<BudgetCashflowItemDto> GetBudgetCashflowUnit(int unitId, DateTimeOffset date)
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
                            CashflowSubCategoryLayoutOrder = cashflowCategorySubCategory != null ? cashflowCategorySubCategory.LayoutOrder : 0
                        };

            query = query.OrderBy(entity => entity.CashflowTypeLayoutOrder).ThenBy(entity => entity.CashflowCashType).ThenBy(entity => entity.CashflowCategoryLayoutOrder).ThenBy(entity => entity.CashflowSubCategoryLayoutOrder);

            var cashflowTypeIds = query.Select(entity => entity.CashflowTypeId).Distinct().ToList();

            var cashflowSubCategoryIds = query.Select(entity => entity.CashflowSubCategoryId).ToList();
            var cashflowUnits = _dbContext.BudgetCashflowUnits
                .Where(entity => entity.UnitId == unitId && cashflowSubCategoryIds.Contains(entity.BudgetCashflowSubCategoryId) && entity.Month == date.AddHours(_identityService.TimezoneOffset).AddMonths(1).Month && entity.Year == date.AddHours(_identityService.TimezoneOffset).AddMonths(1).Year)
                .ToList();

            var summaries = new List<SummaryPerType>();
            foreach (var cashflowTypeId in cashflowTypeIds)
            {
                var cashflowType = _dbContext.BudgetCashflowTypes.FirstOrDefault(entity => entity.Id == cashflowTypeId);
                var summary = new SummaryPerType();
                summary.SetCashflowType(cashflowType);
                //var sectionRowSpan = 0;
                var cashTypes = query.Select(entity => entity.CashflowCashType).Distinct().ToList();
                foreach (var cashType in cashTypes)
                {
                    //var groupRowSpan = 0;
                    var cashflowCategoryIds = query.Where(entity => entity.CashflowTypeId == cashflowTypeId && entity.CashflowCashType == cashType).Select(entity => entity.CashflowCategoryId).Distinct().ToList();

                    foreach (var cashflowCategoryId in cashflowCategoryIds)
                    {
                        var cashflowCategory = _dbContext.BudgetCashflowCategories.FirstOrDefault(entity => entity.Id == cashflowCategoryId);
                        cashflowSubCategoryIds = query.Where(entity => entity.CashflowCategoryId == cashflowCategoryId).Select(entity => entity.CashflowSubCategoryId).Distinct().ToList();

                        summary.CashflowCategories.Add(cashflowCategory);

                        foreach (var cashflowSubCategoryId in cashflowSubCategoryIds)
                        {
                            var cashflowSubCategory = _dbContext.BudgetCashflowSubCategories.FirstOrDefault(entity => entity.Id == cashflowSubCategoryId);

                            var selectedCashflowUnits = cashflowUnits.Where(element => element.BudgetCashflowSubCategoryId == cashflowSubCategoryId).ToList();

                            if (selectedCashflowUnits.Count > 0)
                                foreach (var cashflowUnit in selectedCashflowUnits)
                                {
                                    summary.Items.Add(new BudgetCashflowUnitDto(cashflowType, cashflowCategory, cashflowSubCategory, cashflowUnit));
                                }
                            else
                                summary.Items.Add(new BudgetCashflowUnitDto(cashflowType, cashflowCategory, cashflowSubCategory, new BudgetCashflowUnitModel()));
                        }
                    }

                    var totalCashTypes = summary
                        .Items
                        .Where(element => element.CashflowType.Id == cashflowTypeId && element.CashflowCategory.Type == cashType && element.CashflowUnit.CurrencyId > 0)
                        .GroupBy(element => element.CashflowUnit.CurrencyId)
                        .Select(element =>
                        {
                            var nominal = element.Sum(sum => sum.CashflowUnit.Nominal);
                            var currencyNominal = element.Sum(sum => sum.CashflowUnit.CurrencyNominal);
                            var total = element.Sum(sum => sum.CashflowUnit.Total);

                            return new TotalCashType(cashflowTypeId, cashType, element.Key, nominal, currencyNominal, total);
                        })
                        .ToList();

                    summary.AddTotalCashTypes(totalCashTypes);
                }

                summaries.Add(summary);


            }

            var result = new List<BudgetCashflowItemDto>();

            foreach (var summary in summaries)
            {
                var summaryItem = new BudgetCashflowItemDto(cashflowTypeId: summary.CashflowType.Id, cashflowTypeName: summary.CashflowType.Name, isUseSection: true);

                var cashCategoryRow = summary.CashflowCategories.Where(element => element.CashflowTypeId == summary.CashflowType.Id).Count();
                var itemRow = summary.Items.Where(element => element.CashflowType.Id == summary.CashflowType.Id).Count();
                var totalInRow = summary.TotalCashTypes.Where(element => element.CashflowTypeId == summary.CashflowType.Id && element.CashType == CashType.In).Count() == 0 ? 1 : summary.TotalCashTypes.Where(element => element.CashflowTypeId == summary.CashflowType.Id).Count();
                var totalOutRow = summary.TotalCashTypes.Where(element => element.CashflowTypeId == summary.CashflowType.Id && element.CashType == CashType.Out).Count() == 0 ? 1 : summary.TotalCashTypes.Where(element => element.CashflowTypeId == summary.CashflowType.Id).Count();
                var differenceRow = summary.GetDifference(summary.CashflowType.Id).Count == 0 ? 1 : summary.GetDifference(summary.CashflowType.Id).Count;
                summaryItem.SetSectionRowSpan(sectionRowSpan: cashCategoryRow + itemRow + totalInRow + totalOutRow + differenceRow);

                var cashTypes = summary.CashflowCategories.Where(element => element.CashflowTypeId == summary.CashflowType.Id).Select(element => element.Type).Distinct().ToList();

                foreach (var cashType in cashTypes)
                {
                    var selectedCasCategoryRow = summary.CashflowCategories.Where(element => element.CashflowTypeId == summary.CashflowType.Id && element.Type == cashType).Count();
                    var selectedItemRow = summary.Items.Where(element => element.CashflowType.Id == summary.CashflowType.Id && element.CashflowCategory.Type == cashType).Count();
                    var selectedTotalRow = summary.TotalCashTypes.Where(element => element.CashflowTypeId == summary.CashflowType.Id && element.CashType == cashType).Count() == 0 ? 1 : summary.TotalCashTypes.Where(element => element.CashflowTypeId == summary.CashflowType.Id && element.CashType == cashType).Count(); 
                    summaryItem.SetGroup(isUseGroup: true, groupRowSpan: selectedCasCategoryRow + selectedItemRow + selectedTotalRow, type: cashType);

                    var cashflowCategories = summary.CashflowCategories.Where(element => element.CashflowTypeId == summary.CashflowType.Id && element.Type == cashType).ToList();

                    foreach (var cashflowCategory in cashflowCategories)
                    {
                        summaryItem.SetLabelOnly(cashflowCategory);
                        result.Add(summaryItem);
                        summaryItem = new BudgetCashflowItemDto(cashflowCategory);

                        var items = summary.Items.Where(element => element.CashflowCategory.Id == cashflowCategory.Id).ToList();

                        var isShowSubCategoryLabel = false;
                        var previousSubCategoryId = 0;
                        foreach (var item in items)
                        {
                            if (item.CashflowSubCategory.Id != previousSubCategoryId)
                            {
                                isShowSubCategoryLabel = true;
                                previousSubCategoryId = item.CashflowSubCategory.Id;
                            }

                            result.Add(new BudgetCashflowItemDto(isShowSubCategoryLabel, item, _currencies));
                            isShowSubCategoryLabel = false;
                        }
                    }

                    var totalCashTypes = summary.TotalCashTypes.Where(element => element.CashflowTypeId == summary.CashflowType.Id && element.CashType == cashType).ToList();
                    var isShowTotalLabel = true;
                    if (totalCashTypes.Count > 0)
                        foreach (var totalCashType in totalCashTypes)
                        {
                            result.Add(new BudgetCashflowItemDto(isShowTotalLabel, totalLabel: cashType == CashType.In ? $"Total Penerimaan {summary.CashflowType.Name}" : $"Total Pengeluaran {summary.CashflowType.Name}", totalCashType, _currencies));
                            isShowTotalLabel = false;
                        }
                    else
                        result.Add(new BudgetCashflowItemDto(isShowTotalLabel, totalLabel: cashType == CashType.In ? $"Total Penerimaan {summary.CashflowType.Name}" : $"Total Pengeluaran {summary.CashflowType.Name}", new TotalCashType(), _currencies));
                }

                var differenceCashTypes = summary.GetDifference(summary.CashflowType.Id);
                var isShowDifferenceLabel = true;
                if (differenceCashTypes.Count > 0)
                    foreach (var differenceCashType in differenceCashTypes)
                    {
                        result.Add(new BudgetCashflowItemDto(isShowDifferenceLabel, differenceLabel: $"Surplus/Deficit-Kas dari {summary.CashflowType.Name}", differenceCashType, _currencies, "difference type"));
                        isShowDifferenceLabel = false;
                    }
                else
                    result.Add(new BudgetCashflowItemDto(isShowDifferenceLabel, differenceLabel: $"Surplus/Deficit-Kas dari {summary.CashflowType.Name}", new TotalCashType(), _currencies, "difference type"));
            }

            return result;
        }

        public List<BudgetCashflowUnitItemDto> GetBudgetCashflowUnit(int unitId, int subCategoryId, DateTimeOffset date)
        {
            return _dbContext.BudgetCashflowUnits
                .Where(entity => entity.UnitId == unitId && subCategoryId == entity.BudgetCashflowSubCategoryId && entity.Month == date.AddHours(_identityService.TimezoneOffset).AddMonths(1).Month && entity.Year == date.AddHours(_identityService.TimezoneOffset).AddMonths(1).Year)
                .ToList()
                .Select(entity =>
                {
                    return new BudgetCashflowUnitItemDto(entity, _currencies);
                })
                .ToList();
        }

        public int EditBudgetCashflowUnit(CashflowUnitFormDto form)
        {
            var existingModels = _dbContext.BudgetCashflowUnits.Where(entity => entity.BudgetCashflowSubCategoryId == form.CashflowSubCategoryId && entity.UnitId == form.UnitId && entity.DivisionId == form.DivisionId && entity.Month == form.Date.AddHours(_identityService.TimezoneOffset).AddMonths(1).Month && entity.Year == form.Date.AddHours(_identityService.TimezoneOffset).AddMonths(1).Year).ToList();
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

    public class TotalCashType
    {
        public TotalCashType()
        {

        }

        public TotalCashType(int cashflowTypeId, CashType cashType, int currencyId, double nominal, double currencyNominal, double total)
        {
            CashflowTypeId = cashflowTypeId;
            CashType = cashType;
            CurrencyId = currencyId;
            Nominal = nominal;
            CurrencyNominal = currencyNominal;
            Total = total;
        }

        public int CashflowTypeId { get; }
        public CashType CashType { get; }
        public int CurrencyId { get; }
        public double Nominal { get; }
        public double CurrencyNominal { get; }
        public double Total { get; }
    }

    public class BudgetCashflowItemDto
    {
        public BudgetCashflowItemDto()
        {

        }

        public BudgetCashflowItemDto(int cashflowTypeId, string cashflowTypeName, bool isUseSection)
        {
            CashflowTypeId = cashflowTypeId;
            CashflowTypeName = cashflowTypeName;
            IsUseSection = isUseSection;
        }

        public BudgetCashflowItemDto(BudgetCashflowCategoryModel cashflowCategory)
        {
            IsLabelOnly = true;
            CashflowCategoryId = cashflowCategory.Id;
            CashflowCategoryName = cashflowCategory.Name;
        }

        public BudgetCashflowItemDto(bool isShowSubCategoryLabel, BudgetCashflowUnitDto item, List<CurrencyDto> currencies)
        {
            var currency = currencies.FirstOrDefault(element => element.Id == item.CashflowUnit.CurrencyId);
            IsShowSubCategoryLabel = isShowSubCategoryLabel;
            SubCategoryId = item.CashflowSubCategory.Id;
            SubCategoryName = item.CashflowSubCategory.Name;
            Currency = currency;
            Nominal = item.CashflowUnit.Nominal;
            CurrencyNominal = item.CashflowUnit.CurrencyNominal;
            Total = item.CashflowUnit.Total;
        }

        public BudgetCashflowItemDto(bool isShowTotalLabel, string totalLabel, TotalCashType totalCashType, List<CurrencyDto> currencies)
        {
            var currency = currencies.FirstOrDefault(element => element.Id == totalCashType.CurrencyId);
            IsShowTotalLabel = isShowTotalLabel;
            TotalLabel = totalLabel;
            Currency = currency;
            Nominal = totalCashType.Nominal;
            CurrencyNominal = totalCashType.CurrencyNominal;
            Total = totalCashType.Total;
        }

        public BudgetCashflowItemDto(bool isShowDifferenceLabel, string differenceLabel, TotalCashType differenceCashType, List<CurrencyDto> currencies, /*Intentionally unused for flag*/ string type = "Difference Constructor")
        {
            var currency = currencies.FirstOrDefault(element => element.Id == differenceCashType.CurrencyId);
            IsShowDifferenceLabel = isShowDifferenceLabel;
            DifferenceLabel = differenceLabel;
            Currency = currency;
            Nominal = differenceCashType.Nominal;
            CurrencyNominal = differenceCashType.CurrencyNominal;
            Total = differenceCashType.Total;
        }

        public int CashflowTypeId { get; private set; }
        public string CashflowTypeName { get; private set; }
        public bool IsUseSection { get; private set; }
        public int SectionRowSpan { get; private set; }
        public bool IsUseGroup { get; private set; }
        public int GroupRowSpan { get; private set; }
        public CashType Type { get; private set; }
        public string TypeName { get; private set; }
        public bool IsLabelOnly { get; private set; }
        public int CashflowCategoryId { get; private set; }
        public string CashflowCategoryName { get; private set; }
        public bool IsShowSubCategoryLabel { get; private set; }
        public int SubCategoryId { get; private set; }
        public string SubCategoryName { get; private set; }
        public CurrencyDto Currency { get; private set; }
        public double Nominal { get; private set; }
        public double CurrencyNominal { get; private set; }
        public double Total { get; private set; }
        public bool IsShowTotalLabel { get; private set; }
        public string TotalLabel { get; private set; }
        public bool IsShowDifferenceLabel { get; private set; }
        public string DifferenceLabel { get; private set; }

        public void SetSectionRowSpan(int sectionRowSpan)
        {
            SectionRowSpan = sectionRowSpan;
        }

        public void SetGroup(bool isUseGroup, int groupRowSpan, CashType type)
        {
            IsUseGroup = isUseGroup;
            GroupRowSpan = groupRowSpan;
            Type = type;
            TypeName = type.ToDescriptionString();
        }

        public void SetLabelOnly(BudgetCashflowCategoryModel cashflowCategory)
        {
            IsLabelOnly = true;
            CashflowCategoryId = cashflowCategory.Id;
            CashflowCategoryName = cashflowCategory.Name;
        }
    }

    public class SummaryPerType
    {
        public SummaryPerType()
        {
            CashflowCategories = new List<BudgetCashflowCategoryModel>();
            Items = new List<BudgetCashflowUnitDto>();
            TotalCashTypes = new List<TotalCashType>();
        }

        public SummaryPerType(int cashflowTypeId, List<BudgetCashflowUnitDto> cashIn, List<BudgetCashflowUnitDto> cashOut, List<BudgetCashflowUnitDto> totalCashIn, List<BudgetCashflowUnitDto> totalCashOut)
        {

        }

        public BudgetCashflowTypeModel CashflowType { get; private set; }
        public List<BudgetCashflowCategoryModel> CashflowCategories { get; }
        public List<BudgetCashflowUnitDto> Items { get; }
        public List<TotalCashType> TotalCashTypes { get; }

        public void SetCashflowType(BudgetCashflowTypeModel cashflowType)
        {
            CashflowType = cashflowType;
        }

        public void AddTotalCashTypes(List<TotalCashType> totalCashTypes)
        {
            TotalCashTypes.AddRange(totalCashTypes);
        }

        public List<TotalCashType> GetDifference(int cashflowTypeId)
        {
            return TotalCashTypes
                .Where(element => element.CashflowTypeId == cashflowTypeId)
                .GroupBy(element => new { element.CashflowTypeId, element.CurrencyId })
                .Select(element =>
                {
                    var nominalIn = element.Where(w => w.CashType == CashType.In).Sum(sum => sum.Nominal);
                    var nominalOut = element.Where(w => w.CashType == CashType.Out).Sum(sum => sum.Nominal);

                    var currencyNominalIn = element.Where(w => w.CashType == CashType.In).Sum(sum => sum.CurrencyNominal);
                    var currencyNominalOut = element.Where(w => w.CashType == CashType.Out).Sum(sum => sum.CurrencyNominal);

                    var totalIn = element.Where(w => w.CashType == CashType.In).Sum(sum => sum.Total);
                    var totalOut = element.Where(w => w.CashType == CashType.Out).Sum(sum => sum.Total);

                    return new TotalCashType(cashflowTypeId, 0, element.Key.CurrencyId, nominalIn - nominalOut, currencyNominalIn - currencyNominalOut, totalIn - totalOut);
                })
                .ToList();
        }
    }
}
