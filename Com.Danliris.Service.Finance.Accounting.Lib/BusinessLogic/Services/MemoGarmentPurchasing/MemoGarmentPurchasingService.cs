using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.MemoGarmentPurchasing;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MemoGarmentPurchasing;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Com.Moonlay.NetCore.Lib;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.MemoGarmentPurchasing
{
    public class MemoGarmentPurchasingService : IMemoGarmentPurchasingService
    {
        private readonly FinanceDbContext _context;
        private readonly IIdentityService _identityService;
        private const string UserAgent = "finance-service";

        public MemoGarmentPurchasingService(FinanceDbContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _identityService = serviceProvider.GetService<IIdentityService>();
        }

        public async Task<int> CreateAsync(MemoGarmentPurchasingModel model)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                model.MemoNo = GetMemoNo(model);
                EntityExtension.FlagForCreate(model, _identityService.Username, UserAgent);

                foreach (var detail in model.MemoGarmentPurchasingDetails)
                    EntityExtension.FlagForCreate(detail, _identityService.Username, UserAgent);

                _context.Add(model);
                var result = await _context.SaveChangesAsync();

                transaction.Commit();
                return result;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw e;
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var model = await ReadByIdAsync(id);
                EntityExtension.FlagForDelete(model, _identityService.Username, UserAgent);

                foreach (var detail in model.MemoGarmentPurchasingDetails)
                    EntityExtension.FlagForDelete(detail, _identityService.Username, UserAgent);

                _context.Update(model);
                var result = await _context.SaveChangesAsync();

                transaction.Commit();
                return result;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw e;
            }
        }

        public ReadResponse<MemoGarmentPurchasingModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            try
            {
                var query = _context.MemoGarmentPurchasings.Include(x => x.MemoGarmentPurchasingDetails).AsQueryable();

                var searchAttributes = new List<string>()
                {
                    "MemoNo",
                    "AccountingBookType",
                    "GarmentCurrenciesCode"
                };

                query = QueryHelper<MemoGarmentPurchasingModel>.Search(query, searchAttributes, keyword);

                var filterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
                query = QueryHelper<MemoGarmentPurchasingModel>.Filter(query, filterDictionary);

                var orderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
                query = QueryHelper<MemoGarmentPurchasingModel>.Order(query, orderDictionary);

                var pageable = new Pageable<MemoGarmentPurchasingModel>(query, page - 1, size);
                var data = pageable.Data.ToList();

                int totalData = pageable.TotalCount;

                return new ReadResponse<MemoGarmentPurchasingModel>(data, totalData, orderDictionary, new List<string>());
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public async Task<MemoGarmentPurchasingModel> ReadByIdAsync(int id)
        {
            try
            {
                var result = await _context.MemoGarmentPurchasings.AsNoTracking()
                    .Include(x => x.MemoGarmentPurchasingDetails)
                    .Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<int> UpdateAsync(int id, MemoGarmentPurchasingModel model)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var modelToUpdate = await ReadByIdAsync(id);
                modelToUpdate.MemoNo = model.MemoNo;
                modelToUpdate.MemoDate = model.MemoDate;
                modelToUpdate.AccountingBookId = model.AccountingBookId;
                modelToUpdate.AccountingBookType = model.AccountingBookType;
                modelToUpdate.GarmentCurrenciesId = model.GarmentCurrenciesId;
                modelToUpdate.GarmentCurrenciesCode = model.GarmentCurrenciesCode;
                modelToUpdate.GarmentCurrenciesRate = model.GarmentCurrenciesRate;
                modelToUpdate.Remarks = model.Remarks;
                modelToUpdate.MemoGarmentPurchasingDetails = model.MemoGarmentPurchasingDetails;

                EntityExtension.FlagForUpdate(model, _identityService.Username, UserAgent);

                foreach (var detail in modelToUpdate.MemoGarmentPurchasingDetails)
                    EntityExtension.FlagForUpdate(detail, _identityService.Username, UserAgent);

                _context.Update(modelToUpdate);
                var result = await _context.SaveChangesAsync();

                transaction.Commit();
                return result;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw e;
            }
        }

        private string GetMemoNo(MemoGarmentPurchasingModel model)
        {
            var date = DateTime.Now;
            var count = 1 + _context.MemoGarmentPurchasings.Count(x => x.CreatedUtc.Year.Equals(date.Year) && x.CreatedUtc.Month.Equals(date.Month));

            var generatedNo = $"{date.ToString("yy")}{date.ToString("MM")}.MG.{count.ToString("0000")}";

            return generatedNo;
        }
    }
}
