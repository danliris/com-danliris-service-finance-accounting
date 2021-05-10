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

                var dispositionPaymentIds = dispositions.SelectMany(element => element.DispositionPaymentIds).Distinct().ToList();

                var dispositionPayments = _dbContext.GarmentInvoicePurchasingDispositions.Where(entity => dispositionPaymentIds.Contains(entity.Id)).Select(entity => new { entity.Id, entity.InvoiceNo, entity.InvoiceDate }).ToList();

                var memoDetailDocumentDispositionItems = _dbContext.MemoDetailGarmentPurchasingDispositions.Where(entity => dispositionIds.Contains(entity.DispositionId)).Select(entity => new { entity.DispositionId, entity.MemoDetailGarmentPurchasingId }).ToList();
                var memoDetailDocumentIds = memoDetailDocumentDispositionItems.Select(entity => entity.MemoDetailGarmentPurchasingId).ToList();
                var memoDetailDocumentDetails = _dbContext.MemoDetailGarmentPurchasingDetails.Where(entity => memoDetailDocumentIds.Contains(entity.MemoDetailId)).Select(entity => new { entity.Id, entity.MemoDetailId, entity.MemoDispositionId, entity.GarmentDeliveryOrderNo }).ToList();
                var memoDetailDocuments = _dbContext.MemoDetailGarmentPurchasings.Where(entity => memoDetailDocumentIds.Contains(entity.Id)).Select(entity => new { entity.Id, entity.MemoId }).ToList();
                var memoDocumentIds = memoDetailDocuments.Select(entity => entity.MemoId).ToList();
                var memoDocuments = _dbContext.MemoGarmentPurchasings.Where(entity => memoDocumentIds.Contains(entity.Id)).ToList();


                foreach (var disposition in dispositions)
                {
                    var selectedDispositionPayments = dispositionPayments.Where(entity => disposition.DispositionPaymentIds.Contains(entity.Id)).ToList();
                    //var selectedMemoDocuments = memoDetailDocumentDetails
                    disposition.DispositionPayments.AddRange(selectedDispositionPayments.Select(element => new DispositionPaymentDto(element.Id, element.InvoiceNo, element.InvoiceDate)).ToList());
                }

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

                var dispositionPaymentIds = dispositions.SelectMany(element => element.DispositionPaymentIds).Distinct().ToList();

                var dispositionPayments = _dbContext.GarmentInvoicePurchasingDispositions.Where(entity => dispositionPaymentIds.Contains(entity.Id)).Select(entity => new { entity.Id, entity.InvoiceNo, entity.InvoiceDate }).ToList();

                var memoDetailDocumentDispositionItems = _dbContext.MemoDetailGarmentPurchasingDispositions.Where(entity => dispositionIds.Contains(entity.DispositionId)).Select(entity => new { entity.DispositionId, entity.MemoDetailGarmentPurchasingId }).ToList();
                var memoDetailDocumentIds = memoDetailDocumentDispositionItems.Select(entity => entity.MemoDetailGarmentPurchasingId).ToList();
                var memoDetailDocumentDetails = _dbContext.MemoDetailGarmentPurchasingDetails.Where(entity => memoDetailDocumentIds.Contains(entity.MemoDetailId)).Select(entity => new { entity.Id, entity.MemoDetailId, entity.MemoDispositionId, entity.GarmentDeliveryOrderNo }).ToList();
                var memoDetailDocuments = _dbContext.MemoDetailGarmentPurchasings.Where(entity => memoDetailDocumentIds.Contains(entity.Id)).Select(entity => new { entity.Id, entity.MemoId }).ToList();
                var memoDocumentIds = memoDetailDocuments.Select(entity => entity.MemoId).ToList();
                var memoDocuments = _dbContext.MemoGarmentPurchasings.Where(entity => memoDocumentIds.Contains(entity.Id)).ToList();


                foreach (var disposition in dispositions)
                {
                    var selectedDispositionPayments = dispositionPayments.Where(entity => disposition.DispositionPaymentIds.Contains(entity.Id)).ToList();
                    //var selectedMemoDocuments = memoDetailDocumentDetails
                    disposition.DispositionPayments.AddRange(selectedDispositionPayments.Select(element => new DispositionPaymentDto(element.Id, element.InvoiceNo, element.InvoiceDate)).ToList());
                }

                return dispositions;
            }
            else
            {
                var isImportIds = _dbContext.GarmentInvoicePurchasingDispositions.Where(entity => entity.IsImportSupplier).Select(entity => entity.Id).ToList();
                var dispositions = _dbContext.GarmentInvoicePurchasingDispositionItems
                        .Where(entity => entity.DispositionDate <= date && isImportIds.Contains(entity.Id))
                        .GroupBy(entity => entity.DispositionId)
                        .Select(group => new GarmentDownPaymentReportDto(group.Key, group.FirstOrDefault().DispositionNo, group.FirstOrDefault().DipositionDueDate, group.Select(entity => entity.GarmentInvoicePurchasingDispositionId).ToList()))
                        .ToList();

                var dispositionIds = dispositions.Select(element => element.DispositionId).ToList();

                var dispositionPaymentIds = dispositions.SelectMany(element => element.DispositionPaymentIds).Distinct().ToList();

                var dispositionPayments = _dbContext.GarmentInvoicePurchasingDispositions.Where(entity => dispositionPaymentIds.Contains(entity.Id)).Select(entity => new { entity.Id, entity.InvoiceNo, entity.InvoiceDate, entity.SupplierName, entity.IsImportSupplier, entity.SupplierCode, entity.SupplierId }).ToList();

                var memoDetailDocumentDispositionItems = _dbContext.MemoDetailGarmentPurchasingDispositions.Where(entity => dispositionIds.Contains(entity.DispositionId)).Select(entity => new { entity.DispositionId, entity.MemoDetailGarmentPurchasingId }).ToList();
                var memoDetailDocumentIds = memoDetailDocumentDispositionItems.Select(entity => entity.MemoDetailGarmentPurchasingId).ToList();
                var memoDetailDocumentDetails = _dbContext.MemoDetailGarmentPurchasingDetails.Where(entity => memoDetailDocumentIds.Contains(entity.MemoDetailId)).Select(entity => new { entity.Id, entity.MemoDetailId, entity.MemoDispositionId, entity.GarmentDeliveryOrderNo }).ToList();
                var memoDetailDocuments = _dbContext.MemoDetailGarmentPurchasings.Where(entity => memoDetailDocumentIds.Contains(entity.Id)).Select(entity => new { entity.Id, entity.MemoId }).ToList();
                var memoDocumentIds = memoDetailDocuments.Select(entity => entity.MemoId).ToList();
                var memoDocuments = _dbContext.MemoGarmentPurchasings.Where(entity => memoDocumentIds.Contains(entity.Id)).ToList();


                foreach (var disposition in dispositions)
                {
                    var selectedDispositionPayments = dispositionPayments.Where(entity => disposition.DispositionPaymentIds.Contains(entity.Id)).ToList();
                    //var selectedMemoDocuments = memoDetailDocumentDetails
                    disposition.DispositionPayments.AddRange(selectedDispositionPayments.Select(element => new DispositionPaymentDto(element.Id, element.InvoiceNo, element.InvoiceDate)).ToList());
                    var selectedMemoDispositionItems = memoDetailDocumentDispositionItems.Where(element => element.DispositionId == disposition.DispositionId).ToList();
                    var memoDocumentDetailIds = selectedMemoDispositionItems.Select(element => element.MemoDetailGarmentPurchasingId).ToList();
                    var memoIds = memoDetailDocuments.Where(element => memoDocumentDetailIds.Contains(element.Id)).Select(element => element.MemoId).ToList();
                    //disposition.MemoDocuments
                }

                return dispositions;
            }
        }
    }
}
