using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Enums.Expedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentDispositionPaymentReport
{
    public class GarmentDispositionPaymentReportService : IGarmentDispositionPaymentReportService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly FinanceDbContext _dbContext;

        public GarmentDispositionPaymentReportService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _dbContext = serviceProvider.GetService<FinanceDbContext>();
        }

        public List<PositionOption> GetPositionOptions()
        {
            var result = new List<PositionOption>();

            var positions = Enum.GetValues(typeof(GarmentPurchasingExpeditionPosition)).Cast<GarmentPurchasingExpeditionPosition>();

            foreach (var position in positions)
            {
                result.Add(new PositionOption(position));
            }

            return result;
        }

        public async Task<List<GarmentDispositionPaymentReportDto>> GetReport(int dispositionId, int supplierId, GarmentPurchasingExpeditionPosition position, string purchasingStaff, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var result = new List<GarmentDispositionPaymentReportDto>();
            if (position <= GarmentPurchasingExpeditionPosition.Purchasing)
            {
                var dispositions = await GetDispositions(startDate, endDate, new List<int>());

                if (dispositionId > 0)
                    dispositions = dispositions.Where(element => element.DispositionId == dispositionId).ToList();
                var dispositionIds = dispositions.Select(element => element.DispositionId).ToList();


                if (supplierId > 0)
                    dispositions = dispositions.Where(entity => entity.SupplierId == supplierId).ToList();

                if (!string.IsNullOrWhiteSpace(purchasingStaff))
                    dispositions = dispositions.Where(entity => entity.DispositionCreatedBy == purchasingStaff).ToList();

                foreach (var disposition in dispositions)
                {
                    result.Add(new GarmentDispositionPaymentReportDto(dispositionId, disposition.DispositionNoteNo, disposition.DispositionNoteDate, disposition.DispositionNoteDueDate, disposition.ProformaNo, disposition.SupplierId, disposition.SupplierCode, disposition.SupplierName, disposition.CurrencyId, disposition.CurrencyCode, disposition.CurrencyRate, disposition.DPPAmount, 0, disposition.VATAmount, 0, disposition.IncomeTaxAmount, 0, disposition.OthersExpenditureAmount, disposition.TotalAmount, disposition.CategoryId, disposition.CategoryCode, disposition.CategoryName, position, disposition.ExternalPurchaseOrderId, disposition.ExternalPurchaseOrderNo, disposition.DispositionQuantity, disposition.DeliveryOrderId, disposition.DeliveryOrderNo, disposition.DeliveryOrderQuantity, disposition.PaymentBillsNo, disposition.BillsNo, disposition.CustomsNoteId, disposition.CustomsNoteNo, disposition.CustomsNoteDate, disposition.UnitReceiptNoteId, disposition.UnitReceiptNoteNo, disposition.InternalNoteId, disposition.InternalNoteNo, disposition.InternalNoteDate));
                }
            }
            else
            {
                

                var expeditionQuery = _dbContext.GarmentDispositionExpeditions.AsQueryable();
                var expenditureQuery = _dbContext.PaymentDispositionNotes.AsQueryable();

                switch (position)
                {
                    case GarmentPurchasingExpeditionPosition.SendToVerification:
                        expeditionQuery = expeditionQuery.Where(entity => entity.SendToVerificationDate.HasValue && entity.SendToVerificationDate.GetValueOrDefault() >= startDate && entity.SendToVerificationDate.GetValueOrDefault() <= endDate);
                        break;
                    case GarmentPurchasingExpeditionPosition.VerificationAccepted:
                        expeditionQuery = expeditionQuery.Where(entity => entity.VerificationAcceptedDate.HasValue && entity.VerificationAcceptedDate.GetValueOrDefault() >= startDate && entity.VerificationAcceptedDate.GetValueOrDefault() <= endDate);
                        break;
                    case GarmentPurchasingExpeditionPosition.SendToCashier:
                        expeditionQuery = expeditionQuery.Where(entity => entity.SendToCashierDate.HasValue && entity.SendToCashierDate.GetValueOrDefault() >= startDate && entity.SendToCashierDate.GetValueOrDefault() <= endDate);
                        break;
                    case GarmentPurchasingExpeditionPosition.CashierAccepted:
                        expeditionQuery = expeditionQuery.Where(entity => entity.CashierAcceptedDate.HasValue && entity.CashierAcceptedDate.GetValueOrDefault() >= startDate && entity.CashierAcceptedDate.GetValueOrDefault() <= endDate);
                        break;
                    case GarmentPurchasingExpeditionPosition.SendToPurchasing:
                        expeditionQuery = expeditionQuery.Where(entity => entity.SendToPurchasingDate.HasValue && entity.SendToPurchasingDate.GetValueOrDefault() >= startDate && entity.SendToPurchasingDate.GetValueOrDefault() <= endDate);
                        break;
                    case GarmentPurchasingExpeditionPosition.DispositionPayment:
                        expenditureQuery = expenditureQuery.Where(entity =>  entity.PaymentDate >= startDate && entity.PaymentDate <= endDate);
                        break;
                }

                var dispositions = await GetDispositions(startDate, endDate, new List<int>());

                if (dispositionId > 0)
                    dispositions = dispositions.Where(element => element.DispositionId == dispositionId).ToList();
                var dispositionIds = dispositions.Select(element => element.DispositionId).ToList();

                if (supplierId > 0)
                    expeditionQuery = expeditionQuery.Where(entity => entity.SupplierId == supplierId);

                if (!string.IsNullOrWhiteSpace(purchasingStaff))
                    expeditionQuery = expeditionQuery.Where(entity => entity.SendToVerificationBy == purchasingStaff);

                var expeditionQueryResult = expeditionQuery.ToList();

                var expenditureItemQuery = _dbContext.PaymentDispositionNoteItems.Where(entity => dispositionIds.Contains(entity.DispositionId));
                var expenditureQueryIds = expenditureItemQuery.Select(entity => entity.PaymentDispositionNoteId).ToList();
                var expenditureItemQueryResult = expenditureItemQuery.ToList();
                var expenditureQueryResult = expenditureQuery.ToList();

                foreach (var disposition in dispositions)
                {
                    var expeditions = expeditionQueryResult.Where(element => disposition.DispositionId == element.DispositionNoteId).ToList();
                    var expenditureItems = expenditureItemQueryResult.Where(element => element.DispositionId == disposition.DispositionId).ToList();
                    var expenditureIds = expenditureItems.Select(element => element.DispositionId).ToList();
                    var expenditures = expenditureQueryResult.Where(element => expenditureIds.Contains(element.Id)).ToList();

                    if (expeditions.Count > 0)
                    {
                        foreach (var expedition in expeditions)
                        {
                            if (expenditures.Count > 0)
                            {
                                foreach (var expenditure in expenditures)
                                {
                                    result.Add(new GarmentDispositionPaymentReportDto(dispositionId, disposition.DispositionNoteNo, disposition.DispositionNoteDate, disposition.DispositionNoteDueDate, disposition.ProformaNo, disposition.SupplierId, disposition.SupplierCode, disposition.SupplierName, disposition.CurrencyId, disposition.CurrencyCode, disposition.CurrencyRate, disposition.DPPAmount, 0, disposition.VATAmount, 0, disposition.IncomeTaxAmount, 0, disposition.OthersExpenditureAmount, disposition.TotalAmount, disposition.CategoryId, disposition.CategoryCode, disposition.CategoryName, expedition.Position, expedition.SendToPurchasingRemark, expedition.SendToVerificationDate, expedition.VerificationAcceptedDate, expedition.VerifiedBy, expedition.CashierAcceptedDate, expenditure.PaymentDate, expenditure.PaymentDispositionNo, expenditure.Amount, disposition.ExternalPurchaseOrderId, disposition.ExternalPurchaseOrderNo, disposition.DispositionQuantity, disposition.DeliveryOrderId, disposition.DeliveryOrderNo, disposition.DeliveryOrderQuantity, disposition.PaymentBillsNo, disposition.BillsNo, disposition.CustomsNoteId, disposition.CustomsNoteNo, disposition.CustomsNoteDate, disposition.UnitReceiptNoteId, disposition.UnitReceiptNoteNo, disposition.InternalNoteId, disposition.InternalNoteNo, disposition.InternalNoteDate, expedition.SendToVerificationBy));
                                }
                            }
                            else
                            {
                                result.Add(new GarmentDispositionPaymentReportDto(dispositionId, disposition.DispositionNoteNo, disposition.DispositionNoteDate, disposition.DispositionNoteDueDate, disposition.ProformaNo, disposition.SupplierId, disposition.SupplierCode, disposition.SupplierName, disposition.CurrencyId, disposition.CurrencyCode, disposition.CurrencyRate, disposition.DPPAmount, 0, disposition.VATAmount, 0, disposition.IncomeTaxAmount, 0, disposition.OthersExpenditureAmount, disposition.TotalAmount, disposition.CategoryId, disposition.CategoryCode, disposition.CategoryName, expedition.Position, expedition.SendToPurchasingRemark, expedition.SendToVerificationDate, expedition.VerificationAcceptedDate, expedition.VerifiedBy, expedition.CashierAcceptedDate, null, null, 0, disposition.ExternalPurchaseOrderId, disposition.ExternalPurchaseOrderNo, disposition.DispositionQuantity, disposition.DeliveryOrderId, disposition.DeliveryOrderNo, disposition.DeliveryOrderQuantity, disposition.PaymentBillsNo, disposition.BillsNo, disposition.CustomsNoteId, disposition.CustomsNoteNo, disposition.CustomsNoteDate, disposition.UnitReceiptNoteId, disposition.UnitReceiptNoteNo, disposition.InternalNoteId, disposition.InternalNoteNo, disposition.InternalNoteDate, expedition.SendToVerificationBy));
                            }
                        }
                    }
                    else
                    {
                        result.Add(new GarmentDispositionPaymentReportDto(dispositionId, disposition.DispositionNoteNo, disposition.DispositionNoteDate, disposition.DispositionNoteDueDate, disposition.ProformaNo, disposition.SupplierId, disposition.SupplierCode, disposition.SupplierName, disposition.CurrencyId, disposition.CurrencyCode, disposition.CurrencyRate, disposition.DPPAmount, 0, disposition.VATAmount, 0, disposition.IncomeTaxAmount, 0, disposition.OthersExpenditureAmount, disposition.TotalAmount, disposition.CategoryId, disposition.CategoryCode, disposition.CategoryName, position, disposition.ExternalPurchaseOrderId, disposition.ExternalPurchaseOrderNo, disposition.DispositionQuantity, disposition.DeliveryOrderId, disposition.DeliveryOrderNo, disposition.DeliveryOrderQuantity, disposition.PaymentBillsNo, disposition.BillsNo, disposition.CustomsNoteId, disposition.CustomsNoteNo, disposition.CustomsNoteDate, disposition.UnitReceiptNoteId, disposition.UnitReceiptNoteNo, disposition.InternalNoteId, disposition.InternalNoteNo, disposition.InternalNoteDate));
                    }
                }
            }

            return result;
        }

        private async Task<List<GarmentDispositionDto>> GetDispositions(DateTimeOffset startDate, DateTimeOffset endDate, List<int> dispositionIds)
        {
            if (dispositionIds == null)
                dispositionIds = new List<int>();


            var jsonSerializerSettings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var http = _serviceProvider.GetService<IHttpClientService>();
            var uri = APIEndpoint.Purchasing + $"garment-purchasing-expeditions/report/disposition-payment?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}dispositionIds={JsonConvert.SerializeObject(dispositionIds)}";
            var response = await http.GetAsync(uri);

            var result = new BaseResponse<List<GarmentDispositionDto>>();

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<BaseResponse<string>>(responseContent, jsonSerializerSettings);
            }

            return result.data;
        }
    }
}
