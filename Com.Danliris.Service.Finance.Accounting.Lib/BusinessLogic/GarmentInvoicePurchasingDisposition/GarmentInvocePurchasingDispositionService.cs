using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentInvoicePurchasingDisposition;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Moonlay.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentDispositionExpedition;
using Newtonsoft.Json;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.JournalTransaction;
using Com.Moonlay.NetCore.Lib;
using System.Net.Http;
using Com.Danliris.Service.Finance.Accounting.Lib.Helpers;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.GarmentInvoicePurchasingDisposition;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentInvoicePurchasingDisposition
{
    public class GarmentInvocePurchasingDispositionService : IGarmentInvoicePurchasingDispositionService
    {
        private const string UserAgent = "finance-service";
        protected DbSet<GarmentInvoicePurchasingDispositionModel> DbSet;
        public IIdentityService IdentityService;
        private readonly IAutoDailyBankTransactionService _autoDailyBankTransactionService;
        public readonly IServiceProvider ServiceProvider;
        public FinanceDbContext DbContext;

        public GarmentInvocePurchasingDispositionService(IServiceProvider serviceProvider, FinanceDbContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            DbSet = dbContext.Set<GarmentInvoicePurchasingDispositionModel>();
            IdentityService = serviceProvider.GetService<IIdentityService>();
            _autoDailyBankTransactionService = serviceProvider.GetService<IAutoDailyBankTransactionService>();
        }

        public void CreateModel(GarmentInvoicePurchasingDispositionModel model)
        {
            EntityExtension.FlagForCreate(model, IdentityService.Username, UserAgent);
            foreach (var item in model.Items)
            {
                GarmentDispositionExpeditionModel expedition = DbContext.GarmentDispositionExpeditions.FirstOrDefault(ex => ex.Id.Equals(item.PurchasingDispositionExpeditionId));
                EntityExtension.FlagForCreate(item, IdentityService.Username, UserAgent);
                expedition.IsPaid = true;
                expedition.BankExpenditureNoteNo = model.InvoiceNo;
                expedition.BankExpenditureNoteDate = model.InvoiceDate;
            }
            DbSet.Add(model);
        }

        public async Task<int> CreateAsync(GarmentInvoicePurchasingDispositionModel model)
        {
            model.InvoiceNo = await GetDocumentNo("K", model.BankCode, IdentityService.Username);

            if (model.CurrencyCode != "IDR")
            {
                var garmentCurrency = await GetGarmentCurrency(model.CurrencyCode);
                model.CurrencyRate = garmentCurrency.Rate.GetValueOrDefault();
            }

            CreateModel(model);
            //await _autoDailyBankTransactionService.AutoCreateFromPaymentDisposition(model);
            return await DbContext.SaveChangesAsync();
        }

        private async Task<string> GetDocumentNo(string type, string bankCode, string username)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var http = ServiceProvider.GetService<IHttpClientService>();
            var uri = APIEndpoint.Purchasing + $"bank-expenditure-notes/bank-document-no?type={type}&bankCode={bankCode}&username={username}";
            var response = await http.GetAsync(uri);

            var result = new BaseResponse<string>();

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<BaseResponse<string>>(responseContent, jsonSerializerSettings);
            }
            return result.data;
        }

        private async Task<GarmentCurrency> GetGarmentCurrency(string codeCurrency)
        {
            var date = DateTimeOffset.UtcNow.ToString("yyyy/MM/dd HH:mm:ss");
            var queryString = $"code={codeCurrency}&stringDate={date}";

            var http = ServiceProvider.GetService<IHttpClientService>();
            var response = await http.GetAsync(APIEndpoint.Core + $"master/garment-currencies/single-by-code-date?{queryString}");

            var responseString = await response.Content.ReadAsStringAsync();
            var jsonSerializationSetting = new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Ignore };

            var result = JsonConvert.DeserializeObject<APIDefaultResponse<GarmentCurrency>>(responseString, jsonSerializationSetting);

            return result.data;
        }

        public async Task DeleteModel(int id)
        {
            GarmentInvoicePurchasingDispositionModel model = await ReadByIdAsync(id);
            foreach (var item in model.Items)
            {
                GarmentDispositionExpeditionModel expedition = DbContext.GarmentDispositionExpeditions.FirstOrDefault(ex => ex.Id.Equals(item.PurchasingDispositionExpeditionId));
                EntityExtension.FlagForDelete(item, IdentityService.Username, UserAgent, true);
                expedition.IsPaid = false;
                expedition.BankExpenditureNoteNo = null;
                expedition.BankExpenditureNoteDate = DateTimeOffset.MinValue;
            }
            EntityExtension.FlagForDelete(model, IdentityService.Username, UserAgent, true);
            DbSet.Update(model);
        }

        public async Task<int> DeleteAsync(int id)
        {
            var existingModel = DbSet
                        .Include(d => d.Items)
                        .Single(dispo => dispo.Id == id && !dispo.IsDeleted);

            //await _autoDailyBankTransactionService.AutoCreateFromPaymentDisposition(existingModel);
            await DeleteModel(id);
            return await DbContext.SaveChangesAsync();
        }

        public async Task<GarmentInvoicePurchasingDispositionModel> ReadByIdAsync(int id)
        {
            return await DbSet.Include(m => m.Items)
                .FirstOrDefaultAsync(d => d.Id.Equals(id) && d.IsDeleted.Equals(false));
        }

        public async Task<int> UpdateAsync(int id, GarmentInvoicePurchasingDispositionModel model)
        {
            var existingModel = DbSet
                        .Include(d => d.Items)
                        .Single(dispo => dispo.Id == id && !dispo.IsDeleted);
            UpdateModel(id, model);
            return await DbContext.SaveChangesAsync();
        }

        public void UpdateModel(int id, GarmentInvoicePurchasingDispositionModel model)
        {
            GarmentInvoicePurchasingDispositionModel exist = DbSet
                        .Include(d => d.Items)
                        .Single(dispo => dispo.Id == id && !dispo.IsDeleted);


            exist.ChequeNo = model.ChequeNo;
            exist.InvoiceDate = model.InvoiceDate;

            foreach (var item in exist.Items)
            {
                GarmentInvoicePurchasingDispositionItemModel itemModel = model.Items.FirstOrDefault(prop => prop.Id.Equals(item.Id));

                if (itemModel == null)
                {
                    GarmentDispositionExpeditionModel expedition = DbContext.GarmentDispositionExpeditions.FirstOrDefault(ex => ex.Id.Equals(item.PurchasingDispositionExpeditionId));
                    expedition.IsPaid = false;
                    expedition.BankExpenditureNoteNo = null;
                    expedition.BankExpenditureNoteDate = DateTimeOffset.MinValue;

                    EntityExtension.FlagForDelete(item, IdentityService.Username, UserAgent, true);
                }
                else
                {
                    EntityExtension.FlagForUpdate(item, IdentityService.Username, UserAgent);

                    foreach (var detail in DbContext.PaymentDispositionNoteDetails.AsNoTracking().Where(p => p.PaymentDispositionNoteItemId == item.Id))
                    {
                        EntityExtension.FlagForUpdate(detail, IdentityService.Username, UserAgent);

                    }
                }
            }

            EntityExtension.FlagForUpdate(exist, IdentityService.Username, UserAgent);
            //DbSet.Update(exist);
        }

        public ReadResponse<GarmentInvoicePurchasingDispositionModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<GarmentInvoicePurchasingDispositionModel> Query = this.DbSet.Include(m => m.Items);
            List<string> searchAttributes = new List<string>()
            {
                "InvoiceNo", "Items.DispositionNo",  "SupplierName", "CurrencyCode","BankName"
            };

            Query = QueryHelper<GarmentInvoicePurchasingDispositionModel>.Search(Query, searchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<GarmentInvoicePurchasingDispositionModel>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<GarmentInvoicePurchasingDispositionModel>.Order(Query, OrderDictionary);

            Pageable<GarmentInvoicePurchasingDispositionModel> pageable = new Pageable<GarmentInvoicePurchasingDispositionModel>(Query, page - 1, size);
            List<GarmentInvoicePurchasingDispositionModel> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<GarmentInvoicePurchasingDispositionModel>(Data, TotalData, OrderDictionary, new List<string>());
        }

        public ReadResponse<GarmentInvoicePurchasingDispositionItemModel> ReadDetailsByEPOId(string epoId)
        {
            List<GarmentInvoicePurchasingDispositionItemModel> paymentDispositionNoteDetails = DbContext.GarmentInvoicePurchasingDispositionItems.Distinct().ToList();

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>("{ }");

            int TotalData = paymentDispositionNoteDetails.Count;

            return new ReadResponse<GarmentInvoicePurchasingDispositionItemModel>(paymentDispositionNoteDetails, TotalData, OrderDictionary, new List<string>());
        }

        private async Task SetTrueDisposition(string dispositionNo)
        {
            var http = ServiceProvider.GetService<IHttpClientService>();
            await http.PutAsync(APIEndpoint.Purchasing + $"purchasing-dispositions/update/is-paid-true/{dispositionNo}", new StringContent("{}", Encoding.UTF8, General.JsonMediaType));
        }

        public async Task<int> Post(GarmentInvoicePurchasingDispositionPostingViewModel form)
        {
            List<int> listIds = form.ListIds.Select(x => x.Id).ToList();

            foreach (int id in listIds)
            {
                var model = await ReadByIdAsync(id);

                if (model != null)
                {
                    model.SetIsPosted(IdentityService.Username, UserAgent);

                    foreach (var item in model.Items)
                    {
                        await SetTrueDisposition(item.DispositionNo);
                    }

                    await _autoDailyBankTransactionService.AutoCreateFromGarmentInvoicePurchasingDisposition(model);
                }
            }

            var result = await DbContext.SaveChangesAsync();

            return result;
        }
    }
}
