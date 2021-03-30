using Com.Danliris.Service.Finance.Accounting.Lib.Models.AccountingBook;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.AccountingBook
{
    public class AccountingBookService : IAccountingBookService
    {
        private const string UserAgent = "finance-service";

        private readonly FinanceDbContext _dbContext;
        private readonly IIdentityService _identityService;
        private readonly IDistributedCache _cache;
        private readonly IServiceProvider _serviceProvider;

        public AccountingBookService(IServiceProvider serviceProvider)
        {
            _dbContext = serviceProvider.GetService<FinanceDbContext>();
            _identityService = serviceProvider.GetService<IIdentityService>();
            _cache = serviceProvider.GetService<IDistributedCache>();

            _serviceProvider = serviceProvider;

        }

        private void SetCache()
        {
            var data = _dbContext.AccountingBooks.ToList();
            _cache.SetString("AccountingBook", JsonConvert.SerializeObject(data));
        }

        public async Task<int> CreateAsync(AccountingBookModel model)
        {
            EntityExtension.FlagForCreate(model, _identityService.Username, UserAgent);
            _dbContext.AccountingBooks.Add(model);

            await _dbContext.SaveChangesAsync();
            SetCache();
            return model.Id;
        }

        public async Task<int> DeleteAsync(int id)
        {
            var model = _dbContext.AccountingBooks.FirstOrDefault(entity => entity.Id == id);
            EntityExtension.FlagForDelete(model, _identityService.Username, UserAgent);
            _dbContext.AccountingBooks.Update(model);

            await _dbContext.SaveChangesAsync();
            SetCache();
            return model.Id;
        }

        public ReadResponse<AccountingBookModel> Read(int page = 1, int size = 25, string order = "{}", List<string> select = null, string keyword = "{}", string filter = "{}")
        {
            var query = _dbContext.AccountingBooks.AsQueryable();

            var searchAttributes = new List<string>
            {
               "Code",
               "AccountingBookType"
            };

            query = QueryHelper<AccountingBookModel>.Search(query, searchAttributes, keyword);
            
            var filterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            query = QueryHelper<AccountingBookModel>.Filter(query, filterDictionary);

            var orderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            query = QueryHelper<AccountingBookModel>.Order(query, orderDictionary);

            var pageable = new Pageable<AccountingBookModel>(query, page - 1, size);

            var data = pageable.Data.Select(entity => new AccountingBookModel
            {
                Id = entity.Id,
                AccountingBookType = entity.AccountingBookType,                
                Code = entity.Code,
                Remarks = entity.Remarks,
                LastModifiedUtc = entity.LastModifiedUtc               
            }).ToList();

            int totalData = pageable.TotalCount;
            return new ReadResponse<AccountingBookModel>(data, totalData, orderDictionary, new List<string>());
        }

        public Task<AccountingBookModel> ReadByIdAsync(int id)
        {
            return _dbContext.AccountingBooks.Where(entity => entity.Id == id).FirstOrDefaultAsync();
        }

        public Task<int> UpdateAsync(int id, AccountingBookModel model)
        {
            EntityExtension.FlagForUpdate(model, _identityService.Username, UserAgent);
            _dbContext.AccountingBooks.Update(model);

            return _dbContext.SaveChangesAsync();
        }
    }
}
