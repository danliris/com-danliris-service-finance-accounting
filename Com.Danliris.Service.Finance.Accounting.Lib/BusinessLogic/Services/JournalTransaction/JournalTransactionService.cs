using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MasterCOA;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
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

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.JournalTransaction
{
    public class JournalTransactionService : IJournalTransactionService
    {
        private const string _UserAgent = "finance-service";
        protected DbSet<JournalTransactionModel> _DbSet;
        protected DbSet<JournalTransactionItemModel> _ItemDbSet;
        protected DbSet<COAModel> _COADbSet;
        protected IIdentityService _IdentityService;
        public FinanceDbContext _DbContext;

        public JournalTransactionService(IServiceProvider serviceProvider, FinanceDbContext dbContext)
        {
            _DbContext = dbContext;
            _DbSet = dbContext.Set<JournalTransactionModel>();
            _ItemDbSet = dbContext.Set<JournalTransactionItemModel>();
            _COADbSet = dbContext.Set<COAModel>();
            _IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        public async Task<int> CreateAsync(JournalTransactionModel model)
        {
            do
            {
                model.DocumentNo = CodeGenerator.Generate();
            }
            while (_DbSet.Any(d => d.DocumentNo.Equals(model.DocumentNo)));

            EntityExtension.FlagForCreate(model, _IdentityService.Username, _UserAgent);
            foreach (var item in model.Items)
            {
                var coa = _COADbSet.FirstOrDefault();
                item.COAId = coa.Id;
                item.COA = null;

                EntityExtension.FlagForCreate(item, _IdentityService.Username, _UserAgent);
                _ItemDbSet.Add(item);
            }

            _DbSet.Add(model);
            return await _DbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            var model = await ReadByIdAsync(id);
            EntityExtension.FlagForDelete(model, _IdentityService.Username, _UserAgent);
            foreach (var item in model.Items)
            {
                EntityExtension.FlagForDelete(item, _IdentityService.Username, _UserAgent);
                _ItemDbSet.Update(item);
            }

            _DbSet.Update(model);
            return await _DbContext.SaveChangesAsync();
        }

        public ReadResponse<JournalTransactionModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<JournalTransactionModel> Query = _DbSet;

            Query = Query
                .Select(s => new JournalTransactionModel
                {
                    Id = s.Id,
                    CreatedUtc = s.CreatedUtc,
                    CreatedBy = s.CreatedBy,
                    DocumentNo = s.DocumentNo,
                    Date = s.Date,
                    Description = s.Description,
                    ReferenceNo = s.ReferenceNo,
                    LastModifiedUtc = s.LastModifiedUtc,
                });

            List<string> searchAttributes = new List<string>()
            {
                "DocumentNo", "ReferenceNo", "Description"
            };

            Query = QueryHelper<JournalTransactionModel>.Search(Query, searchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<JournalTransactionModel>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<JournalTransactionModel>.Order(Query, OrderDictionary);

            Pageable<JournalTransactionModel> pageable = new Pageable<JournalTransactionModel>(Query, page - 1, size);
            List<JournalTransactionModel> Data = pageable.Data.ToList();

            List<JournalTransactionModel> list = new List<JournalTransactionModel>();
            list.AddRange(
               Data.Select(s => new JournalTransactionModel
               {
                   Id = s.Id,
                   CreatedUtc = s.CreatedUtc,
                   CreatedBy = s.CreatedBy,
                   DocumentNo = s.DocumentNo,
                   Date = s.Date,
                   Description = s.Description,
                   ReferenceNo = s.ReferenceNo,
                   LastModifiedUtc = s.LastModifiedUtc,
               }).ToList()
            );

            int TotalData = pageable.TotalCount;

            return new ReadResponse<JournalTransactionModel>(list, TotalData, OrderDictionary, new List<string>());
        }

        public async Task<JournalTransactionModel> ReadByIdAsync(int id)
        {
            var Result = await _DbSet.FirstOrDefaultAsync(d => d.Id.Equals(id) && !d.IsDeleted);
            Result.Items = await _ItemDbSet.Where(w => w.JournalTransactionId.Equals(id) && !w.IsDeleted).ToListAsync();
            foreach (var item in Result.Items)
            {
                var COA = await _COADbSet.FirstOrDefaultAsync(c => c.Id.Equals(item.COAId) && !c.IsDeleted);
                item.COA = COA;
            }
            return Result;
        }

        public async Task<int> UpdateAsync(int id, JournalTransactionModel model)
        {
            EntityExtension.FlagForUpdate(model, _IdentityService.Username, _UserAgent);
            List<int> journalItemIds = await _ItemDbSet.Where(w => w.JournalTransactionId.Equals(id) && !w.IsDeleted).Select(s => s.Id).ToListAsync();

            foreach (var journalItemId in journalItemIds)
            {
                var item = model.Items.FirstOrDefault(f => f.Id.Equals(journalItemId));
                if (item == null)
                {
                    var itemToDelete = await _ItemDbSet.FirstOrDefaultAsync(f => f.Id.Equals(journalItemId));
                    itemToDelete.COA = null;
                    EntityExtension.FlagForDelete(itemToDelete, _IdentityService.Username, _UserAgent);
                    _ItemDbSet.Update(itemToDelete);
                }
                else
                {
                    var coa = _COADbSet.FirstOrDefault(f => f.Code.Equals(item.COA.Code) || f.Id.Equals(item.COA.Id));
                    item.COAId = coa.Id;
                    item.COA = null;
                    EntityExtension.FlagForUpdate(item, _IdentityService.Username, _UserAgent);
                    _ItemDbSet.Update(item);
                }
            }

            foreach (var item in model.Items)
            {
                if (item.Id <= 0)
                {
                    item.COAId = item.COA.Id;
                    item.COA = null;
                    EntityExtension.FlagForCreate(item, _IdentityService.Username, _UserAgent);
                    _ItemDbSet.Add(item);
                }
            }

            _DbSet.Update(model);
            return await _DbContext.SaveChangesAsync();
        }
    }
}
