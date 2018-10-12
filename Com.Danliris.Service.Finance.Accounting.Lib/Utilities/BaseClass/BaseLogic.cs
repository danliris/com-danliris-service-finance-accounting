using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseInterface;
using Com.Moonlay.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseClass
{
    public abstract class BaseLogic<TModel> : IBaseLogic<TModel>
        where TModel : StandardEntity, IValidatableObject
    {
        private const string UserAgent = "finance-service";
        protected DbSet<TModel> DbSet;
        protected IIdentityService IdentityService;
        protected FinanceDbContext DbContext;

        public BaseLogic(IIdentityService identityService, FinanceDbContext dbContext)
        {
            DbContext = dbContext;
            this.DbSet = dbContext.Set<TModel>();
            this.IdentityService = identityService;
        }

        public virtual void CreateModel(TModel model)
        {
            EntityExtension.FlagForCreate(model, IdentityService.Username, UserAgent);
            DbSet.Add(model);
        }

        public virtual Task<TModel> ReadModelById(int id)
        {
            return DbSet.FirstOrDefaultAsync(d => d.Id.Equals(id) && d.IsDeleted.Equals(false));
        }

        public virtual void UpdateModelAsync(int id, TModel model)
        {
            EntityExtension.FlagForUpdate(model, IdentityService.Username, UserAgent);
            DbSet.Update(model);
        }

        public virtual async Task DeleteModel(int id)
        {
            TModel model = await ReadModelById(id);
            EntityExtension.FlagForDelete(model, IdentityService.Username, UserAgent, true);
            DbSet.Update(model);
        }

        public Task<int> BulkInsert(IEnumerable<TModel> entities)
        {
            return Task.Factory.StartNew(async () =>
            {
                const int pageSize = 1000;
                int offset = 0;
                int processed = 0;
                
                var batch = entities.Where((data, index) => offset <= index && index < offset + pageSize);
                while (batch.Count() > 0)
                {
                    foreach(var item in batch)
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
