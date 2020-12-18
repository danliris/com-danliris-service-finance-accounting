using Com.Danliris.Service.Finance.Accounting.Lib.Enums.Expedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Helpers;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentPurchasingExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Moonlay.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentPurchasingExpedition
{
    public class GarmentPurchasingExpeditionService : IGarmentPurchasingExpeditionService
    {
        private const string UserAgent = "finance-service";
        private readonly FinanceDbContext _dbContext;
        private readonly IIdentityService _identityService;
        private readonly IServiceProvider _serviceProvider;

        public GarmentPurchasingExpeditionService(IServiceProvider serviceProvider)
        {
            _dbContext = serviceProvider.GetService<FinanceDbContext>();
            _identityService = serviceProvider.GetService<IIdentityService>();
            _serviceProvider = serviceProvider;
        }

        public ReadResponse<IndexDto> GetSendToVerification(string keyword, int page, int size, string order)
        {
            var query = _dbContext.GarmentPurchasingExpeditions.Where(entity => entity.Position == PurchasingGarmentExpeditionPosition.SendToVerification);

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(entity => entity.InternalNoteNo.Contains(keyword) || entity.SupplierName.Contains(keyword) || entity.CurrencyCode.Contains(keyword));

            var orderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            query = QueryHelper<GarmentPurchasingExpeditionModel>.Order(query, orderDictionary);

            var count = query.Count();

            var data = query
                .Skip((page - 1) * size)
                .Take(size)
                .Select(entity => new IndexDto(entity.Id, entity.InternalNoteNo, entity.InternalNoteDate, entity.InternalNoteDueDate, entity.SupplierName, entity.TotalPaid, entity.CurrencyCode, entity.Remark))
                .ToList();

            return new ReadResponse<IndexDto>(data, count, orderDictionary, new List<string>());
        }

        public async Task<int> SendToPurchasing(int id)
        {
            var model = _dbContext.GarmentPurchasingExpeditions.FirstOrDefault(entity => entity.Id == id);
            model.SendToPurchasing(_identityService.Username);
            EntityExtension.FlagForUpdate(model, _identityService.Username, UserAgent);
            _dbContext.GarmentPurchasingExpeditions.Update(model);

            var httpClient = _serviceProvider.GetService<IHttpClientService>();
            var updateInternalNotePositionData = new
            {
                Ids = new List<int>() { model.InternalNoteId },
                Position = PurchasingGarmentExpeditionPosition.SendToPurchasing
            };

            await httpClient.PutAsync($"{APIEndpoint.Purchasing}garment-purchasing-expeditions/position", new StringContent(JsonConvert.SerializeObject(updateInternalNotePositionData), Encoding.UTF8, General.JsonMediaType));

            return _dbContext.SaveChanges();
        }

        public async Task<int> SendToVerification(SendToVerificationForm form)
        {
            var models = new List<GarmentPurchasingExpeditionModel>();
            foreach (var item in form.Items)
            {
                var model = new GarmentPurchasingExpeditionModel(item.InternalNote.Id, item.InternalNote.DocumentNo, item.InternalNote.Date, item.InternalNote.DueDate, item.InternalNote.SupplierId, item.InternalNote.SupplierName, item.InternalNote.VAT, item.InternalNote.IncomeTax, item.InternalNote.TotalPaid, item.InternalNote.CurrencyId, item.InternalNote.CurrencyCode, item.Remark);
                model.SendToVerification(_identityService.Username);

                EntityExtension.FlagForCreate(model, _identityService.Username, UserAgent);
                models.Add(model);
            }
            _dbContext.GarmentPurchasingExpeditions.UpdateRange(models);

            var httpClient = _serviceProvider.GetService<IHttpClientService>();
            var updateInternalNotePositionData = new
            {
                Ids = models.Select(element => element.InternalNoteId).ToList(),
                Position = PurchasingGarmentExpeditionPosition.SendToVerification
            };

            await httpClient.PutAsync($"{APIEndpoint.Purchasing}garment-purchasing-expeditions/position", new StringContent(JsonConvert.SerializeObject(updateInternalNotePositionData), Encoding.UTF8, General.JsonMediaType));
            return _dbContext.SaveChanges();
        }
    }
}
