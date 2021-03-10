using Com.Danliris.Service.Finance.Accounting.Lib.Enums.Expedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Helpers;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentDispositionExpedition;
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

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentDispositionExpedition
{
    public class GarmentDispositionExpeditionService : IGarmentDispositionExpeditionService
    {
        private const string UserAgent = "finance-service";
        private readonly FinanceDbContext _dbContext;
        private readonly IIdentityService _identityService;
        private readonly IServiceProvider _serviceProvider;

        public GarmentDispositionExpeditionService(IServiceProvider serviceProvider)
        {
            _dbContext = serviceProvider.GetService<FinanceDbContext>();
            _identityService = serviceProvider.GetService<IIdentityService>();
            _serviceProvider = serviceProvider;
        }

        public ReadResponse<IndexDto> GetSendToVerificationOrAccounting(string keyword, int page, int size, string order)
        {
            var query = _dbContext.GarmentDispositionExpeditions.Where(entity => entity.Position == GarmentPurchasingExpeditionPosition.SendToVerification || entity.Position == GarmentPurchasingExpeditionPosition.SendToAccounting);

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(entity => entity.DispositionNoteNo.Contains(keyword) || entity.SupplierName.Contains(keyword) || entity.CurrencyCode.Contains(keyword));

            var orderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            query = QueryHelper<GarmentDispositionExpeditionModel>.Order(query, orderDictionary);

            var count = query.Count();

            var data = query
                .Skip((page - 1) * size)
                .Take(size)
                .Select(entity => new IndexDto(entity.Id, entity.DispositionNoteNo, entity.DispositionNoteDate, entity.DispositionNoteDueDate, entity.DispositionNoteId, entity.CurrencyTotalPaid, entity.TotalPaid, entity.CurrencyId, entity.CurrencyCode))
                .ToList();

            return new ReadResponse<IndexDto>(data, count, orderDictionary, new List<string>());
        }

        public async Task<int> SendToAccounting(SendToVerificationAccountingFormDto form)
        {
            var models = new List<GarmentDispositionExpeditionModel>();
            foreach (var item in form.Items)
            {
                var model = new GarmentDispositionExpeditionModel(item.DispositionNote.Id, item.DispositionNote.DocumentNo, item.DispositionNote.Date, item.DispositionNote.DueDate, item.DispositionNote.SupplierId, item.DispositionNote.SupplierName, item.DispositionNote.VATAmount, item.DispositionNote.CurrencyVATAmount, item.DispositionNote.IncomeTaxAmount, item.DispositionNote.CurrencyIncomeTaxAmount, item.DispositionNote.TotalPaid, item.DispositionNote.CurrencyTotalPaid, item.DispositionNote.CurrencyId, item.DispositionNote.CurrencyCode, item.Remark, item.DispositionNote.DPPAmount, item.DispositionNote.CurrencyDPPAmount, item.DispositionNote.SupplierCode, item.DispositionNote.CurrencyRate);
                model.SendToAccounting(_identityService.Username);

                EntityExtension.FlagForCreate(model, _identityService.Username, UserAgent);
                models.Add(model);
            }
            _dbContext.GarmentDispositionExpeditions.UpdateRange(models);

            var httpClient = _serviceProvider.GetService<IHttpClientService>();
            var updateDispositionNotePositionData = new
            {
                Ids = models.Select(element => element.DispositionNoteId).ToList(),
                Position = GarmentPurchasingExpeditionPosition.SendToAccounting
            };

            await httpClient.PutAsync($"{APIEndpoint.Purchasing}garment-purchasing-expeditions/disposition/position", new StringContent(JsonConvert.SerializeObject(updateDispositionNotePositionData), Encoding.UTF8, General.JsonMediaType));
            return _dbContext.SaveChanges();
        }

        public async Task<int> SendToPurchasing(int id)
        {
            var model = _dbContext.GarmentDispositionExpeditions.FirstOrDefault(entity => entity.Id == id);
            model.SendToPurchasing(_identityService.Username);
            EntityExtension.FlagForUpdate(model, _identityService.Username, UserAgent);
            _dbContext.GarmentDispositionExpeditions.Update(model);

            var httpClient = _serviceProvider.GetService<IHttpClientService>();
            var updateDispositionNotePositionData = new
            {
                Ids = new List<int>() { model.DispositionNoteId },
                Position = GarmentPurchasingExpeditionPosition.Purchasing
            };

            await httpClient.PutAsync($"{APIEndpoint.Purchasing}garment-purchasing-expeditions/disposition-notes/position", new StringContent(JsonConvert.SerializeObject(updateDispositionNotePositionData), Encoding.UTF8, General.JsonMediaType));

            return _dbContext.SaveChanges();
        }

        private async Task<int> UpdateDispositionNotePosition(List<int> internalNoteIds, GarmentPurchasingExpeditionPosition position)
        {
            var httpClient = _serviceProvider.GetService<IHttpClientService>();
            var updateDispositionNotePositionData = new
            {
                Ids = internalNoteIds,
                Position = position
            };

            await httpClient.PutAsync($"{APIEndpoint.Purchasing}garment-purchasing-expeditions/disposition-notes/position", new StringContent(JsonConvert.SerializeObject(updateDispositionNotePositionData), Encoding.UTF8, General.JsonMediaType));

            return _dbContext.SaveChanges();
        }

        public async Task<int> SendToVerification(SendToVerificationAccountingFormDto form)
        {
            var models = new List<GarmentDispositionExpeditionModel>();
            foreach (var item in form.Items)
            {
                var model = new GarmentDispositionExpeditionModel(item.DispositionNote.Id, item.DispositionNote.DocumentNo, item.DispositionNote.Date, item.DispositionNote.DueDate, item.DispositionNote.SupplierId, item.DispositionNote.SupplierName, item.DispositionNote.VATAmount, item.DispositionNote.CurrencyVATAmount, item.DispositionNote.IncomeTaxAmount, item.DispositionNote.CurrencyIncomeTaxAmount, item.DispositionNote.TotalPaid, item.DispositionNote.CurrencyTotalPaid, item.DispositionNote.CurrencyId, item.DispositionNote.CurrencyCode, item.Remark, item.DispositionNote.DPPAmount, item.DispositionNote.CurrencyDPPAmount, item.DispositionNote.SupplierCode, item.DispositionNote.CurrencyRate);
                model.SendToVerification(_identityService.Username);

                EntityExtension.FlagForCreate(model, _identityService.Username, UserAgent);
                models.Add(model);
            }
            _dbContext.GarmentDispositionExpeditions.UpdateRange(models);

            var httpClient = _serviceProvider.GetService<IHttpClientService>();
            var updateDispositionNotePositionData = new
            {
                Ids = models.Select(element => element.DispositionNoteId).ToList(),
                Position = GarmentPurchasingExpeditionPosition.SendToVerification
            };

            await httpClient.PutAsync($"{APIEndpoint.Purchasing}garment-purchasing-expeditions/disposition/position", new StringContent(JsonConvert.SerializeObject(updateDispositionNotePositionData), Encoding.UTF8, General.JsonMediaType));
            return _dbContext.SaveChanges();
        }

        public async Task<int> VerificationAccepted(List<int> ids)
        {
            var models = _dbContext.GarmentDispositionExpeditions.Where(entity => ids.Contains(entity.Id)).ToList();

            models = models.Select(model =>
            {
                model.VerificationAccepted(_identityService.Username);

                EntityExtension.FlagForUpdate(model, _identityService.Username, UserAgent);
                return model;
            }).ToList();


            _dbContext.GarmentDispositionExpeditions.UpdateRange(models);
            var result = await _dbContext.SaveChangesAsync();

            var dispositionNoteIds = models.Select(model => model.DispositionNoteId).ToList();
            await UpdateDispositionNotePosition(dispositionNoteIds, GarmentPurchasingExpeditionPosition.VerificationAccepted);

            return result;
        }

        public async Task<int> CashierAccepted(List<int> ids)
        {
            var models = _dbContext.GarmentDispositionExpeditions.Where(entity => ids.Contains(entity.Id)).ToList();

            models = models.Select(model =>
            {
                model.CashierAccepted(_identityService.Username);

                EntityExtension.FlagForUpdate(model, _identityService.Username, UserAgent);
                return model;
            }).ToList();


            _dbContext.GarmentDispositionExpeditions.UpdateRange(models);
            var result = await _dbContext.SaveChangesAsync();

            var internalNoteIds = models.Select(model => model.DispositionNoteId).ToList();
            await UpdateDispositionNotePosition(internalNoteIds, GarmentPurchasingExpeditionPosition.CashierAccepted);

            return result;
        }

        public async Task<int> AccountingAccepted(List<int> ids)
        {
            var models = _dbContext.GarmentDispositionExpeditions.Where(entity => ids.Contains(entity.Id)).ToList();

            models = models.Select(model =>
            {
                model.AccountingAccepted(_identityService.Username);

                EntityExtension.FlagForUpdate(model, _identityService.Username, UserAgent);
                return model;
            }).ToList();

            _dbContext.GarmentDispositionExpeditions.UpdateRange(models);
            var result = await _dbContext.SaveChangesAsync();

            var internalNoteIds = models.Select(model => model.DispositionNoteId).ToList();
            await UpdateDispositionNotePosition(internalNoteIds, GarmentPurchasingExpeditionPosition.AccountingAccepted);

            return result;
        }

        public async Task<int> PurchasingAccepted(List<int> ids)
        {
            var models = _dbContext.GarmentDispositionExpeditions.Where(entity => ids.Contains(entity.Id)).ToList();

            models = models.Select(model =>
            {
                model.PurchasingAccepted(_identityService.Username);

                EntityExtension.FlagForUpdate(model, _identityService.Username, UserAgent);
                return model;
            }).ToList();

            _dbContext.GarmentDispositionExpeditions.UpdateRange(models);
            var result = await _dbContext.SaveChangesAsync();

            var internalNoteIds = models.Select(model => model.DispositionNoteId).ToList();
            await UpdateDispositionNotePosition(internalNoteIds, GarmentPurchasingExpeditionPosition.Purchasing);

            return result;
        }

        public async Task<int> VoidVerificationAccepted(int id)
        {
            var model = _dbContext.GarmentDispositionExpeditions.FirstOrDefault(entity => entity.Id == id);
            model.VoidVerification(_identityService.Username);
            EntityExtension.FlagForUpdate(model, _identityService.Username, UserAgent);
            _dbContext.GarmentDispositionExpeditions.Update(model);

            var httpClient = _serviceProvider.GetService<IHttpClientService>();
            var updateDispositionNotePositionData = new
            {
                Ids = new List<int>() { model.DispositionNoteId },
                Position = GarmentPurchasingExpeditionPosition.SendToVerification
            };

            await httpClient.PutAsync($"{APIEndpoint.Purchasing}garment-purchasing-expeditions/disposition-notes/position", new StringContent(JsonConvert.SerializeObject(updateDispositionNotePositionData), Encoding.UTF8, General.JsonMediaType));

            return _dbContext.SaveChanges();
        }

        public async Task<int> VoidCashierAccepted(int id)
        {
            var model = _dbContext.GarmentDispositionExpeditions.FirstOrDefault(entity => entity.Id == id);
            model.VoidCashier(_identityService.Username);
            EntityExtension.FlagForUpdate(model, _identityService.Username, UserAgent);
            _dbContext.GarmentDispositionExpeditions.Update(model);

            var httpClient = _serviceProvider.GetService<IHttpClientService>();
            var updateDispositionNotePositionData = new
            {
                Ids = new List<int>() { model.DispositionNoteId },
                Position = GarmentPurchasingExpeditionPosition.VerificationAccepted
            };

            await httpClient.PutAsync($"{APIEndpoint.Purchasing}garment-purchasing-expeditions/disposition-notes/position", new StringContent(JsonConvert.SerializeObject(updateDispositionNotePositionData), Encoding.UTF8, General.JsonMediaType));

            return _dbContext.SaveChanges();
        }

        public async Task<int> VoidAccountingAccepted(int id)
        {
            var model = _dbContext.GarmentDispositionExpeditions.FirstOrDefault(entity => entity.Id == id);
            model.VoidAccounting(_identityService.Username);
            EntityExtension.FlagForUpdate(model, _identityService.Username, UserAgent);
            _dbContext.GarmentDispositionExpeditions.Update(model);

            var httpClient = _serviceProvider.GetService<IHttpClientService>();
            var updateDispositionNotePositionData = new
            {
                Ids = new List<int>() { model.DispositionNoteId },
                model.Position
            };

            await httpClient.PutAsync($"{APIEndpoint.Purchasing}garment-purchasing-expeditions/disposition-notes/position", new StringContent(JsonConvert.SerializeObject(updateDispositionNotePositionData), Encoding.UTF8, General.JsonMediaType));

            return _dbContext.SaveChanges();
        }

        public async Task<int> SendToAccounting(int id)
        {
            var model = _dbContext.GarmentDispositionExpeditions.FirstOrDefault(entity => entity.Id == id);
            model.SendToAccounting(_identityService.Username);
            EntityExtension.FlagForUpdate(model, _identityService.Username, UserAgent);
            _dbContext.GarmentDispositionExpeditions.Update(model);

            var httpClient = _serviceProvider.GetService<IHttpClientService>();
            var updateDispositionNotePositionData = new
            {
                Ids = new List<int>() { model.DispositionNoteId },
                Position = GarmentPurchasingExpeditionPosition.SendToAccounting
            };

            await httpClient.PutAsync($"{APIEndpoint.Purchasing}garment-purchasing-expeditions/disposition-notes/position", new StringContent(JsonConvert.SerializeObject(updateDispositionNotePositionData), Encoding.UTF8, General.JsonMediaType));

            return _dbContext.SaveChanges();
        }

        public async Task<int> SendToCashier(int id)
        {
            var model = _dbContext.GarmentDispositionExpeditions.FirstOrDefault(entity => entity.Id == id);
            model.SendToCashier(_identityService.Username);
            EntityExtension.FlagForUpdate(model, _identityService.Username, UserAgent);
            _dbContext.GarmentDispositionExpeditions.Update(model);

            var httpClient = _serviceProvider.GetService<IHttpClientService>();
            var updateDispositionNotePositionData = new
            {
                Ids = new List<int>() { model.DispositionNoteId },
                Position = GarmentPurchasingExpeditionPosition.SendToCashier
            };

            await httpClient.PutAsync($"{APIEndpoint.Purchasing}garment-purchasing-expeditions/disposition-notes/position", new StringContent(JsonConvert.SerializeObject(updateDispositionNotePositionData), Encoding.UTF8, General.JsonMediaType));

            return _dbContext.SaveChanges();
        }

        public async Task<int> SendToPurchasingRejected(int id, string remark)
        {
            var model = _dbContext.GarmentDispositionExpeditions.FirstOrDefault(entity => entity.Id == id);
            model.SendToPurchasingRejected(_identityService.Username, remark);
            EntityExtension.FlagForUpdate(model, _identityService.Username, UserAgent);
            _dbContext.GarmentDispositionExpeditions.Update(model);

            var httpClient = _serviceProvider.GetService<IHttpClientService>();
            var updateDispositionNotePositionData = new
            {
                Ids = new List<int>() { model.DispositionNoteId },
                Position = GarmentPurchasingExpeditionPosition.SendToPurchasing
            };

            await httpClient.PutAsync($"{APIEndpoint.Purchasing}garment-purchasing-expeditions/disposition-notes/position", new StringContent(JsonConvert.SerializeObject(updateDispositionNotePositionData), Encoding.UTF8, General.JsonMediaType));

            return _dbContext.SaveChanges();
        }

        public IndexDto GetById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
