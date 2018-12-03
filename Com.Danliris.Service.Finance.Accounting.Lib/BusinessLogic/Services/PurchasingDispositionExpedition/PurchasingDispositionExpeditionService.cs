using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.PurchasingDispositionExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Enums.Expedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Helpers;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.PurchasingDispositionExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.PurchasingDispositionAcceptance;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.PurchasingDispositionExpedition
{
    public class PurchasingDispositionExpeditionService : IPurchasingDispositionExpeditionService
    {
        private const string UserAgent = "finance-service";
        protected DbSet<PurchasingDispositionExpeditionModel> DbSet;
        protected IIdentityService IdentityService;
        public readonly IServiceProvider ServiceProvider;
        public FinanceDbContext DbContext;

        public PurchasingDispositionExpeditionService(IServiceProvider serviceProvider, FinanceDbContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            DbSet = dbContext.Set<PurchasingDispositionExpeditionModel>();
            IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        public void CreateModel(PurchasingDispositionExpeditionModel m)
        {
            EntityExtension.FlagForCreate(m, IdentityService.Username, UserAgent);
            m.Position = ExpeditionPosition.SEND_TO_VERIFICATION_DIVISION;
            foreach (var item in m.Items)
            {
                EntityExtension.FlagForCreate(item, IdentityService.Username, UserAgent);
            }
            DbSet.Add(m);
        }

        public async Task<int> CreateAsync(PurchasingDispositionExpeditionModel m)
        {
            CreateModel(m);
            return await DbContext.SaveChangesAsync();
        }

        public async Task DeleteModel(int id)
        {
            PurchasingDispositionExpeditionModel model = await ReadByIdAsync(id);
            EntityExtension.FlagForDelete(model, IdentityService.Username, UserAgent, true);
            foreach (var item in model.Items)
            {
                EntityExtension.FlagForDelete(item, IdentityService.Username, UserAgent, true);
            }
            DbSet.Update(model);
        }

        public async Task<int> DeleteAsync(int id)
        {
            await DeleteModel(id);
            return await DbContext.SaveChangesAsync();
        }

        public ReadResponse<PurchasingDispositionExpeditionModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {

            IQueryable<PurchasingDispositionExpeditionModel> Query = this.DbSet.Include(m => m.Items);
            List<string> searchAttributes = new List<string>()
            {
                "DispositionId", "DispositionNo",  "SupplierName", "CurrencyCode"
            };

            Query = QueryHelper<PurchasingDispositionExpeditionModel>.Search(Query, searchAttributes, keyword);

            if (filter.Contains("verificationFilter"))
            {
                filter = "{}";
                List<ExpeditionPosition> positions = new List<ExpeditionPosition> { ExpeditionPosition.SEND_TO_PURCHASING_DIVISION, ExpeditionPosition.SEND_TO_ACCOUNTING_DIVISION, ExpeditionPosition.SEND_TO_CASHIER_DIVISION };
                Query = Query.Where(p => positions.Contains(p.Position));
            }

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<PurchasingDispositionExpeditionModel>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<PurchasingDispositionExpeditionModel>.Order(Query, OrderDictionary);

            Pageable<PurchasingDispositionExpeditionModel> pageable = new Pageable<PurchasingDispositionExpeditionModel>(Query, page - 1, size);
            List<PurchasingDispositionExpeditionModel> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<PurchasingDispositionExpeditionModel>(Data, TotalData, OrderDictionary, new List<string>());
        }

        public Task<PurchasingDispositionExpeditionModel> ReadByIdAsync(int id)
        {
            return DbSet.Include(m => m.Items).FirstOrDefaultAsync(d => d.Id.Equals(id) && d.IsDeleted.Equals(false));
        }

        public Task<int> Update(int id, PurchasingDispositionExpeditionModel m, string user, int clientTimeZoneOffset = 7)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(int id, PurchasingDispositionExpeditionModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<int> PurchasingDispositionAcceptance(PurchasingDispositionAcceptanceViewModel data)
        {

            int updated = 0;

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    List<string> dispositions = new List<string>();

                    if (data.Role.Equals("VERIFICATION"))
                    {
                        foreach (var item in data.PurchasingDispositionExpedition)
                        {
                            dispositions.Add(item.DispositionNo);
                            PurchasingDispositionExpeditionModel model = DbContext.PurchasingDispositionExpeditions.FirstOrDefault(x => x.Id == item.Id);
                            model.VerificationDivisionBy = IdentityService.Username;
                            model.VerificationDivisionDate = DateTimeOffset.UtcNow;
                            model.Position = ExpeditionPosition.VERIFICATION_DIVISION;
                            
                            EntityExtension.FlagForUpdate(model, IdentityService.Username, UserAgent);
                        }

                        updated = await DbContext.SaveChangesAsync();
                        UpdateDispositionPosition(dispositions, ExpeditionPosition.VERIFICATION_DIVISION);
                    }
                    else if (data.Role.Equals("CASHIER"))
                    {
                        foreach (var item in data.PurchasingDispositionExpedition)
                        {
                            dispositions.Add(item.DispositionNo);
                            PurchasingDispositionExpeditionModel model = DbContext.PurchasingDispositionExpeditions.FirstOrDefault(x => x.Id == item.Id);
                            model.CashierDivisionBy = IdentityService.Username;
                            model.CashierDivisionDate = DateTimeOffset.UtcNow;
                            model.Position = ExpeditionPosition.CASHIER_DIVISION;

                            EntityExtension.FlagForUpdate(model, IdentityService.Username, UserAgent);
                        }

                        updated = await DbContext.SaveChangesAsync();
                        UpdateDispositionPosition(dispositions, ExpeditionPosition.CASHIER_DIVISION);
                    }
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw  e;
                }
            }

            return updated;
        }

        public async Task<int> DeletePurchasingDispositionAcceptance(int id)
        {
            int count = 0;

            if (DbContext.PurchasingDispositionExpeditions.Count(x => x.Id == id && !x.IsDeleted).Equals(0))
            {
                return 0;
            }

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    PurchasingDispositionExpeditionModel purchasingDispositionExpedition = DbContext.PurchasingDispositionExpeditions.Single(x => x.Id == id);

                    if (purchasingDispositionExpedition.Position == ExpeditionPosition.VERIFICATION_DIVISION)
                    {
                        purchasingDispositionExpedition.VerificationDivisionBy = null;
                        purchasingDispositionExpedition.VerificationDivisionDate = null;
                        purchasingDispositionExpedition.Position = ExpeditionPosition.SEND_TO_VERIFICATION_DIVISION;

                        EntityExtension.FlagForUpdate(purchasingDispositionExpedition, IdentityService.Username, UserAgent);
                        
                        count = await DbContext.SaveChangesAsync();
                        UpdateDispositionPosition(new List<string>() { purchasingDispositionExpedition.DispositionNo }, ExpeditionPosition.SEND_TO_VERIFICATION_DIVISION);
                    }
                    else if (purchasingDispositionExpedition.Position == ExpeditionPosition.CASHIER_DIVISION)
                    {
                        purchasingDispositionExpedition.CashierDivisionBy = null;
                        purchasingDispositionExpedition.CashierDivisionDate = null;
                        purchasingDispositionExpedition.Position = ExpeditionPosition.SEND_TO_CASHIER_DIVISION;
                        
                        EntityExtension.FlagForUpdate(purchasingDispositionExpedition, IdentityService.Username, UserAgent);
                        
                        count = await DbContext.SaveChangesAsync();
                        UpdateDispositionPosition(new List<string>() { purchasingDispositionExpedition.DispositionNo }, ExpeditionPosition.SEND_TO_CASHIER_DIVISION);
                    }

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }

            return count;
        }

        private void UpdateDispositionPosition(List<string> dispositions, ExpeditionPosition position)
        {
            string dispositionUri = "purchasing-dispositions/update/position";

            var data = new
            {
                Position = position,
                PurchasingDispositionNoes = dispositions
            };

            IHttpClientService httpClient = (IHttpClientService)this.ServiceProvider.GetService(typeof(IHttpClientService));
            var response = httpClient.PutAsync($"{APIEndpoint.Purchasing}{dispositionUri}", new StringContent(JsonConvert.SerializeObject(data).ToString(), Encoding.UTF8, General.JsonMediaType)).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(string.Format("{0}, {1}", response.StatusCode, response.Content));
            }
        }
    }
}
