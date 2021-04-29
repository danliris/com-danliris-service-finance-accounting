using Com.Danliris.Service.Finance.Accounting.Lib.Enums.Expedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentDownPaymentReport
{
    public class GarmentDownPaymentReportService : IGarmentDownPaymentReportService
    {
        private readonly FinanceDbContext _dbContext;
        private readonly object _identityService;

        public GarmentDownPaymentReportService(IServiceProvider serviceProvider)
        {
            _dbContext = serviceProvider.GetService<FinanceDbContext>();
            _identityService = serviceProvider.GetService<IIdentityService>();
        }
        public List<GarmentDownPaymentReportDto> GetReport(SupplierType supplierType, DateTimeOffset date)
        {
            if (supplierType == SupplierType.All)
            {
                var dispositions = _dbContext.GarmentInvoicePurchasingDispositionItems
                        .Where(entity => entity.DispositionDate <= date)
                        .GroupBy(entity => entity.DispositionId)
                        .Select(group => new GarmentDownPaymentReportDto(group.Key, group.FirstOrDefault().DispositionNo, group.FirstOrDefault().DipositionDueDate, group.Select(entity => entity.GarmentInvoicePurchasingDispositionId).ToList()))
                        .ToList();

                var dispositionIds = dispositions.Select(element => element.DispositionId).ToList();

                var expenditureIds = dispositions.SelectMany(element => element.ExpenditureIds).Distinct().ToList();
                var memoDocumentDispositionItems = _dbContext.MemoDetailGarmentPurchasingDispositions.Where(entity => dispositionIds.Contains(entity.DispositionId)).Select(entity => new { entity.DispositionId, entity.MemoDetailGarmentPurchasingId }).ToList();
                var memoDocumentIds = memoDocumentDispositionItems.Select(entity => entity.MemoDetailGarmentPurchasingId).ToList();

                return dispositions;
            } 
            else if (supplierType == SupplierType.Local)
            {
                var dispositions = _dbContext.GarmentInvoicePurchasingDispositionItems
                        .Where(entity => entity.DispositionDate <= date)
                        .GroupBy(entity => entity.DispositionId)
                        .Select(group => new GarmentDownPaymentReportDto(group.Key, group.FirstOrDefault().DispositionNo, group.FirstOrDefault().DipositionDueDate, group.Select(entity => entity.GarmentInvoicePurchasingDispositionId).ToList()))
                        .ToList();

                var dispositionIds = dispositions.Select(element => element.DispositionId).ToList();

                var expenditureIds = dispositions.SelectMany(element => element.ExpenditureIds).Distinct().ToList();
                var memoDocumentDispositionItems = _dbContext.MemoDetailGarmentPurchasingDispositions.Where(entity => dispositionIds.Contains(entity.DispositionId)).Select(entity => new { entity.DispositionId, entity.MemoDetailGarmentPurchasingId }).ToList();
                var memoDocumentIds = memoDocumentDispositionItems.Select(entity => entity.MemoDetailGarmentPurchasingId).ToList();

                return dispositions;
            }
            else
            {
                var dispositions = _dbContext.GarmentInvoicePurchasingDispositionItems
                        .Where(entity => entity.DispositionDate <= date)
                        .GroupBy(entity => entity.DispositionId)
                        .Select(group => new GarmentDownPaymentReportDto(group.Key, group.FirstOrDefault().DispositionNo, group.FirstOrDefault().DipositionDueDate, group.Select(entity => entity.GarmentInvoicePurchasingDispositionId).ToList()))
                        .ToList();

                var dispositionIds = dispositions.Select(element => element.DispositionId).ToList();

                var expenditureIds = dispositions.SelectMany(element => element.ExpenditureIds).Distinct().ToList();
                var memoDocumentDispositionItems = _dbContext.MemoDetailGarmentPurchasingDispositions.Where(entity => dispositionIds.Contains(entity.DispositionId)).Select(entity => new { entity.DispositionId, entity.MemoDetailGarmentPurchasingId }).ToList();
                var memoDocumentIds = memoDocumentDispositionItems.Select(entity => entity.MemoDetailGarmentPurchasingId).ToList();

                return dispositions;
            }
        }
    }
}
