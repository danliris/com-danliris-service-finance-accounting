using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MemoGarmentPurchasing;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Moonlay.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.MemoDetailGarmentPurchasing
{
    public class MemoDetailGarmentPurchasingService : IMemoDetailGarmentPurchasingService
    {
        private const string UserAgent = "finance-service";
        public FinanceDbContext _dbContext;
        private readonly IIdentityService _identityService;
        private readonly IServiceProvider _serviceProvider;
        protected DbSet<MemoDetailGarmentPurchasingModel> DbSet;
        
        public MemoDetailGarmentPurchasingService(IServiceProvider serviceProvider, FinanceDbContext dbContext)
        {
            _dbContext = dbContext;
            _identityService = serviceProvider.GetService<IIdentityService>();
            _serviceProvider = serviceProvider;

            DbSet = _dbContext.Set<MemoDetailGarmentPurchasingModel>();
        }

        public void CreateModel(MemoDetailGarmentPurchasingModel model)
        {
            EntityExtension.FlagForCreate(model, _identityService.Username, UserAgent);
           

            foreach (var item in model.MemoDetailGarmentPurchasingDetail)
            {
                EntityExtension.FlagForCreate(item, _identityService.Username, UserAgent);
            }
            DbSet.Add(model);
        }

        public async Task<int> CreateAsync(MemoDetailGarmentPurchasingModel model)
        {
            System.Diagnostics.Debug.WriteLine(model.MemoId + "");
            System.Diagnostics.Debug.WriteLine(model.IsPosted + "");
            System.Diagnostics.Debug.WriteLine(model.Remarks + "");
            foreach (var item in model.MemoDetailGarmentPurchasingDetail)
            {
                System.Diagnostics.Debug.WriteLine(item.GarmentDeliveryOrderId + "");
                System.Diagnostics.Debug.WriteLine(item.GarmentDeliveryOrderNo + "");
                System.Diagnostics.Debug.WriteLine(item.RemarksDetail + "");
                System.Diagnostics.Debug.WriteLine(item.PaymentRate + "");
                System.Diagnostics.Debug.WriteLine(item.PurchasingRate + "");
                System.Diagnostics.Debug.WriteLine(item.MemoAmount + "");
                System.Diagnostics.Debug.WriteLine(item.MemoIdrAmount + "");
            }

            try
            {
                CreateModel(model);
                return await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public ReadResponse<MemoDetailGarmentPurchasingModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            throw new NotImplementedException();
        }

        public Task<MemoDetailGarmentPurchasingModel> ReadByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(int id, MemoDetailGarmentPurchasingModel model)
        {
            throw new NotImplementedException();
        }
    }
}
