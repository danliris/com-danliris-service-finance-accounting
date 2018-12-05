using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.PurchasingDispositionExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.PurchasingDispositionExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.BasicUploadCsvService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.PurchasingDispositionExpedition;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.PurchasingDispositionExpedition
{
    public class PurchasingDispositionExpeditionService : IPurchasingDispositionExpeditionService
    {
        private const string UserAgent = "finance-service";
        protected DbSet<PurchasingDispositionExpeditionModel> DbSet;
        protected IIdentityService IdentityService;
        public FinanceDbContext DbContext;

        public PurchasingDispositionExpeditionService(IServiceProvider serviceProvider, FinanceDbContext dbContext)
        {
            DbContext = dbContext;
            DbSet = dbContext.Set<PurchasingDispositionExpeditionModel>();
            IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        public void CreateModel(PurchasingDispositionExpeditionModel m)
        {
            EntityExtension.FlagForCreate(m, IdentityService.Username, UserAgent);
            m.Position = 2;
            foreach (var item in m.Items){
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
                "DispositionId", "DispositionNo", "DispositionDate", "PaymentDueDate", "SupplierName", "IncomeTax", "Vat", "TotalPaid", "CurrencyCode"
            };

            Query = QueryHelper<PurchasingDispositionExpeditionModel>.Search(Query, searchAttributes, keyword);

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
    }
}
