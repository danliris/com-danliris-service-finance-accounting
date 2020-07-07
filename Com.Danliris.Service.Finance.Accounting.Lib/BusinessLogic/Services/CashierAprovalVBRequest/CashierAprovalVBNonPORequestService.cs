using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.CashierAprovalVBRequest;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.CashierAprovalVBRequest;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Helpers;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CashierApproval;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.CashierAprovalVBRequest
{
    public class CashierAprovalVBNonPORequestService : ICashierAprovalVBNonPORequestService
    {
        private const string UserAgent = "finance-service";
        protected DbSet<CashierAprovalVBNonPORequestModel> DbSet;
        public IIdentityService IdentityService;
        public readonly IServiceProvider ServiceProvider;
        public FinanceDbContext DbContext;

        public CashierAprovalVBNonPORequestService(IServiceProvider serviceProvider, FinanceDbContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            DbSet = dbContext.Set<CashierAprovalVBNonPORequestModel>();
            IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        public void CreateModel(CashierAprovalVBNonPORequestModel m)
        {
            EntityExtension.FlagForCreate(m, IdentityService.Username, UserAgent);
            m.Position = ExpeditionPosition.SEND_TO_VERIFICATION_DIVISION;
            foreach (var item in m.Items)
            {
                EntityExtension.FlagForCreate(item, IdentityService.Username, UserAgent);
            }
            List<string> dispoNo = new List<string>();
            dispoNo.Add(m.DispositionNo);
            UpdateDispositionPosition(dispoNo, ExpeditionPosition.SEND_TO_VERIFICATION_DIVISION);

            DbSet.Add(m);
        }

        public async Task<int> CreateAsync(CashierAprovalVBNonPORequestModel m)
        {
            CreateModel(m);

            return await DbContext.SaveChangesAsync();
        }

        public async Task DeleteModel(int id)
        {
            CashierAprovalVBNonPORequestModel model = await ReadByIdAsync(id);
            foreach (var item in model.Items)
            {
                EntityExtension.FlagForDelete(item, IdentityService.Username, UserAgent, true);
            }

            List<string> dispoNo = new List<string>();
            dispoNo.Add(model.DispositionNo);

            var dispoCount = this.DbSet.Count(x => x.DispositionNo == model.DispositionNo && x.IsDeleted == false && x.Id != model.Id);
            if (dispoCount > 0)
            {
                UpdateDispositionPosition(dispoNo, ExpeditionPosition.SEND_TO_PURCHASING_DIVISION);
            }
            else
            {
                UpdateDispositionPosition(dispoNo, ExpeditionPosition.PURCHASING_DIVISION);
            }

            EntityExtension.FlagForDelete(model, IdentityService.Username, UserAgent, true);
            DbSet.Update(model);
        }

        public async Task<int> DeleteAsync(int id)
        {
            await DeleteModel(id);
            return await DbContext.SaveChangesAsync();
        }

        public ReadResponse<CashierAprovalVBNonPORequestModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {

            IQueryable<CashierAprovalVBNonPORequestModel> Query = this.DbSet.Include(m => m.Items);
            List<string> searchAttributes = new List<string>()
            {
                "DispositionId", "DispositionNo",  "SupplierName", "CurrencyCode"
            };

            Query = QueryHelper<CashierAprovalVBNonPORequestModel>.Search(Query, searchAttributes, keyword);

            if (filter.Contains("verificationFilter"))
            {
                filter = "{}";
                List<ExpeditionPosition> positions = new List<ExpeditionPosition> { ExpeditionPosition.SEND_TO_PURCHASING_DIVISION, ExpeditionPosition.SEND_TO_ACCOUNTING_DIVISION, ExpeditionPosition.SEND_TO_CASHIER_DIVISION };
                Query = Query.Where(p => positions.Contains(p.Position));
            }

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<CashierAprovalVBNonPORequestModel>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<CashierAprovalVBNonPORequestModel>.Order(Query, OrderDictionary);

            Pageable<CashierAprovalVBNonPORequestModel> pageable = new Pageable<CashierAprovalVBNonPORequestModel>(Query, page - 1, size);
            List<CashierAprovalVBNonPORequestModel> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<CashierAprovalVBNonPORequestModel>(Data, TotalData, OrderDictionary, new List<string>());
        }

        public Task<CashierAprovalVBNonPORequestModel> ReadByIdAsync(int id)
        {
            return DbSet.Include(m => m.Items).FirstOrDefaultAsync(d => d.Id.Equals(id) && d.IsDeleted.Equals(false));
        }

        public Task<int> Update(int id, CashierAprovalVBNonPORequestModel m, string user, int clientTimeZoneOffset = 7)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(int id, CashierAprovalVBNonPORequestModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<int> PurchasingDispositionAcceptance(CashierApprovalViewModel data)
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
                            CashierAprovalVBNonPORequestModel model = DbContext.PurchasingDispositionExpeditions.Single(x => x.Id == item.Id);
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
                            CashierAprovalVBNonPORequestModel model = DbContext.PurchasingDispositionExpeditions.Single(x => x.Id == item.Id);
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
                    throw e;
                }
            }

            return updated;
        }

        public async Task<int> DeleteCashierAproval(int id)
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
                    CashierAprovalVBNonPORequestModel purchasingDispositionExpedition = DbContext.PurchasingDispositionExpeditions.Single(x => x.Id == id);

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
    }
}
