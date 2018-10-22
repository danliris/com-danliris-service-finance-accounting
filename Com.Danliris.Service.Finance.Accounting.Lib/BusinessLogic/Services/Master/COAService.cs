using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.Master;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MasterCOA;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.MasterCOA;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.Master
{
    public class COAService : ICOAService
    {
        private const string UserAgent = "finance-service";
        protected DbSet<COAModel> DbSet;
        protected IIdentityService IdentityService;
        protected FinanceDbContext DbContext;

        public COAService(IServiceProvider serviceProvider, FinanceDbContext dbContext)
        {
            DbContext = dbContext;
            this.DbSet = dbContext.Set<COAModel>();
            this.IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        public async Task<int> CreateAsync(COAModel model)
        {
            CreateModel(model);
            return await DbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            await DeleteModel(id);
            return await DbContext.SaveChangesAsync();
        }

        public ReadResponse<COAModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<COAModel> query = DbSet;

            List<string> searchAttributes = new List<string>()
            {
                "Code", "Name"
            };

            query = QueryHelper<COAModel>.Search(query, searchAttributes, keyword);

            Dictionary<string, object> filterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            query = QueryHelper<COAModel>.Filter(query, filterDictionary);

            List<string> selectedFields = new List<string>()
                {
                    "Id", "Name", "Code", "Path", "Nature", "CashAccount", "ReportType", "LastModifiedUtc"
                };

            Dictionary<string, string> orderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            query = QueryHelper<COAModel>.Order(query, orderDictionary);

            query = query.Select(x => new COAModel()
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Path = x.Path,
                CashAccount = x.CashAccount,
                Nature = x.Nature,
                ReportType = x.ReportType,
                LastModifiedUtc = x.LastModifiedUtc
            });

            Pageable<COAModel> pageable = new Pageable<COAModel>(query, page - 1, size);
            List<COAModel> data = pageable.Data.ToList();
            int totalData = pageable.TotalCount;

            return new ReadResponse<COAModel>(data, totalData, orderDictionary, selectedFields);
        }

        public async Task<COAModel> ReadByIdAsync(int id)
        {
            return await ReadModelById(id);
        }

        public async Task<int> UpdateAsync(int id, COAModel model)
        {
            UpdateModelAsync(id, model);
            return await DbContext.SaveChangesAsync();
        }

        public async Task UploadData(List<COAModel> data)
        {

            await BulkInsert(data);
        }

        public void CreateModel(COAModel model)
        {
            EntityExtension.FlagForCreate(model, IdentityService.Username, UserAgent);
            DbSet.Add(model);
        }

        public Task<COAModel> ReadModelById(int id)
        {
            return DbSet.FirstOrDefaultAsync(d => d.Id.Equals(id) && d.IsDeleted.Equals(false));
        }

        public void UpdateModelAsync(int id, COAModel model)
        {
            EntityExtension.FlagForUpdate(model, IdentityService.Username, UserAgent);
            DbSet.Update(model);
        }

        public async Task DeleteModel(int id)
        {
            COAModel model = await ReadModelById(id);
            EntityExtension.FlagForDelete(model, IdentityService.Username, UserAgent, true);
            DbSet.Update(model);
        }

        public Task<int> BulkInsert(IEnumerable<COAModel> entities)
        {
            return Task.Factory.StartNew(async () =>
            {
                const int pageSize = 1000;
                int offset = 0;
                int processed = 0;

                var batch = entities.Where((data, index) => offset <= index && index < offset + pageSize);
                while (batch.Count() > 0)
                {
                    foreach (var item in batch)
                    {
                        EntityExtension.FlagForCreate(item, IdentityService.Username, UserAgent);
                    }
                    DbSet.AddRange(batch);
                    var result = await DbContext.SaveChangesAsync();
                    processed += batch.Count();
                    offset = pageSize;
                };
                return processed;
            }).Unwrap();
        }
    }
}
