using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MemoGarmentPurchasing;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseInterface;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.MemoDetailGarmentPurchasing;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

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
            var query = _dbContext.MemoGarmentPurchasingDetails.AsQueryable();

            var searchAttributes = new List<string>
            {
               "Code",
               "AccountingBookType"
            };

            query = QueryHelper<MemoDetailGarmentPurchasingModel>.Search(query, searchAttributes, keyword);

            var filterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            query = QueryHelper<MemoDetailGarmentPurchasingModel>.Filter(query, filterDictionary);

            var orderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            query = QueryHelper<MemoDetailGarmentPurchasingModel>.Order(query, orderDictionary);

            var pageable = new Pageable<MemoDetailGarmentPurchasingModel>(query, page - 1, size);

            var data = pageable.Data.Select(entity => new MemoDetailGarmentPurchasingModel
            {
                Id = entity.Id,
                MemoId = entity.MemoId,
                Remarks = entity.Remarks,
                IsPosted = entity.IsPosted,
                LastModifiedUtc = entity.LastModifiedUtc
            }).ToList();

            int totalData = pageable.TotalCount;
            return new ReadResponse<MemoDetailGarmentPurchasingModel>(data, totalData, orderDictionary, new List<string>());
        }

        public DetailRincian GetDetailById(int memoId)
        {
            var memoGarments = _dbContext.MemoGarmentPurchasings.AsQueryable();
            var memoDetailsGarments = _dbContext.MemoGarmentPurchasingDetails.AsQueryable();
            var memoDetailsGarmentsDetails = _dbContext.MemoDetailGarmentPurchasingDetails.AsQueryable();
            var garmentDebts = _dbContext.GarmentDebtBalances.AsQueryable();

            var listDataDetails = from memoDetailsGarmentDetail in memoDetailsGarmentsDetails
                           join garmentDebt in garmentDebts on memoDetailsGarmentDetail.MemoDetailId equals memoId
                           select new {
                               memoDetailsGarmentDetail.MemoDetailId,
                               memoDetailsGarmentDetail.GarmentDeliveryOrderNo,
                               memoDetailsGarmentDetail.GarmentDeliveryOrderId,
                               memoDetailsGarmentDetail.RemarksDetail,
                               memoDetailsGarmentDetail.PaymentRate,
                               memoDetailsGarmentDetail.PurchasingRate,
                               memoDetailsGarmentDetail.MemoAmount,
                               memoDetailsGarmentDetail.MemoIdrAmount
                           };

            var listData = from listDataDetail in listDataDetails
                           join garmentDebt in garmentDebts on listDataDetail.GarmentDeliveryOrderId equals garmentDebt.GarmentDeliveryOrderId
                           select new DetailRincianItems {
                               GarmentDeliveryOrderNo =  listDataDetail.GarmentDeliveryOrderNo,
                               InternalNoteNo = garmentDebt.InternalNoteNo,
                               BillsNo = garmentDebt.BillsNo,
                               PaymentBills = garmentDebt.PaymentBills,
                               SupplierCode = garmentDebt.SupplierCode,
                               RemarksDetail = listDataDetail.RemarksDetail,
                               CurrencyCode = garmentDebt.CurrencyCode,
                               PaymentRate = listDataDetail.PaymentRate,
                               PurchasingRate = listDataDetail.PurchasingRate,
                               SaldoAkhir = garmentDebt.DPPAmount + garmentDebt.VATAmount - garmentDebt.IncomeTaxAmount,
                               MemoAmount = listDataDetail.MemoAmount,
                               MemoIdrAmount = listDataDetail.MemoIdrAmount,
                           };

            var queryJoin = from memoGarment in memoGarments
                            join memoDetailsGarment in memoDetailsGarments on memoGarment.Id equals memoId

                            select new DetailRincian
                            {
                                MemoId = memoGarment.Id,
                                MemoDate = memoGarment.MemoDate,
                                MemoNo = memoGarment.MemoNo,
                                AccountingBookType = memoGarment.AccountingBookType,
                                GarmentCurrenciesCode = memoGarment.GarmentCurrenciesCode,
                                GarmentCurrenciesRate = memoGarment.GarmentCurrenciesRate,
                                Remarks = memoDetailsGarment.Remarks,
                                Items = listData.ToList()
                            };

            return queryJoin.FirstOrDefault(s => s.MemoId == memoId);
        }

        public Task<int> UpdateAsync(int id, MemoDetailGarmentPurchasingModel model)
        {
            throw new NotImplementedException();
        }

        public Task<MemoDetailGarmentPurchasingModel> ReadByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
