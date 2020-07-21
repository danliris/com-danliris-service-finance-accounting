using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Com.Moonlay.Models;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Moonlay.NetCore.Lib;
using Newtonsoft.Json;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.ClearaceVB;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.ClearaceVB;
using System.Linq.Dynamic.Core;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.ClearaceVB
{
    public class ClearaceVBService : IClearaceVBService
    {
        private const string _UserAgent = "finance-service";
        protected DbSet<VbRequestModel> _RequestDbSet;
        protected DbSet<RealizationVbModel> _RealizationDbSet;
        private readonly IServiceProvider _serviceProvider;
        protected IIdentityService _IdentityService;
        public FinanceDbContext _DbContext;

        public ClearaceVBService(IServiceProvider serviceProvider, FinanceDbContext dbContext)
        {
            _DbContext = dbContext;
            _RequestDbSet = dbContext.Set<VbRequestModel>();
            _RealizationDbSet = dbContext.Set<RealizationVbModel>();
            _serviceProvider = serviceProvider;
            _IdentityService = serviceProvider.GetService<IIdentityService>();
        }
        public static IQueryable<ClearaceVBViewModel> Filter(IQueryable<ClearaceVBViewModel> query, Dictionary<string, object> filterDictionary)
        {
            if (filterDictionary != null && !filterDictionary.Count.Equals(0))
            {
                foreach (var f in filterDictionary)
                {
                    string key = f.Key;
                    object Value = f.Value;
                    string filterQuery = string.Concat(string.Empty, key, " == @0");

                    query = query.Where(filterQuery, Value);
                }
            }
            return query;
        }

        public static IQueryable<ClearaceVBViewModel> Order(IQueryable<ClearaceVBViewModel> query, Dictionary<string, string> orderDictionary)
        {
            /* Default Order */
            if (orderDictionary.Count.Equals(0))
            {
                orderDictionary.Add("LastModifiedUtc", "desc");

                query = query.OrderByDescending(b => b.LastModifiedUtc);
            }
            /* Custom Order */
            else
            {
                string Key = orderDictionary.Keys.First();
                string OrderType = orderDictionary[Key];

                query = query.OrderBy(string.Concat(Key.Replace(".", ""), " ", OrderType));
            }
            return query;
        }

        public static IQueryable<ClearaceVBViewModel> Search(IQueryable<ClearaceVBViewModel> query, List<string> searchAttributes, string keyword, bool ToLowerCase = false)
        {
            /* Search with Keyword */
            if (keyword != null)
            {
                string SearchQuery = String.Empty;
                foreach (string Attribute in searchAttributes)
                {
                    if (Attribute.Contains("."))
                    {
                        var Key = Attribute.Split(".");
                        SearchQuery = string.Concat(SearchQuery, Key[0], $".Any({Key[1]}.Contains(@0)) OR ");
                    }
                    else
                    {
                        SearchQuery = string.Concat(SearchQuery, Attribute, ".Contains(@0) OR ");
                    }
                }

                SearchQuery = SearchQuery.Remove(SearchQuery.Length - 4);

                if (ToLowerCase)
                {
                    SearchQuery = SearchQuery.Replace(".Contains(@0)", ".ToLower().Contains(@0)");
                    keyword = keyword.ToLower();
                }

                query = query.Where(SearchQuery, keyword);
            }
            return query;
        }

        public Task<int> CreateAsync(VbRequestModel model)
        {
            EntityExtension.FlagForCreate(model, _IdentityService.Username, _UserAgent);

            _DbContext.VbRequests.Add(model);

            return _DbContext.SaveChangesAsync();
        }

        public virtual void UpdateAsync(long id, VbRequestModel model)
        {
            EntityExtension.FlagForUpdate(model, _IdentityService.Username, "sales-service");
            _RequestDbSet.Update(model);
        }

        public Task<VbRequestModel> ReadByIdAsync(long id)
        {
            return _DbContext.VbRequests.Where(entity => entity.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> ClearanceVBPost(List<long> listId)
        {
            foreach (var id in listId)
            {
                var model = await ReadByIdAsync(id);
                model.Complete_Status = true;
                model.CompleteDate = DateTimeOffset.Now;
                UpdateAsync(id, model);
            }
            return await _DbContext.SaveChangesAsync();
        }

        public async Task<int> ClearanceVBUnpost(long id)
        {
            var model = await ReadByIdAsync(id);
            model.Complete_Status = false;
            model.CompleteDate = DateTimeOffset.Now;
            UpdateAsync(id, model);
            return await _DbContext.SaveChangesAsync();
        }

        public ReadResponse<ClearaceVBViewModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            var query = _RequestDbSet.AsQueryable();
            var realizationQuery = _RealizationDbSet.AsQueryable();

            List<string> SearchAttributes = new List<string>()
            {
                "RqstNo","VBCategory","Appliciant","RealNo","Status","DiffStatus"
            };


            var diffStatus = "";

            var data = query
               .Join(realizationQuery,
               (rqst) => rqst.VBNo,
               (real) => real.VBNo,
               (rqst, real) => new ClearaceVBViewModel()
               {
                   Id = rqst.Id,
                   RqstNo = rqst.VBNo,
                   VBCategory = rqst.VBRequestCategory,
                   RqstDate = rqst.Date,
                   Unit = new Unit()
                   {
                       Id = rqst.Id,
                       Name = rqst.UnitName,
                   },
                   Appliciant = rqst.CreatedBy,
                   RealNo = real.VBNoRealize,
                   RealDate = real.Date,
                   VerDate = null,

                   //DiffStatus = real.StatusReqReal,
                   DiffStatus = diffStatus,

                   DiffAmount = real.DifferenceReqReal,
                   ClearanceDate = rqst.CompleteDate,
                   IsPosted = rqst.Complete_Status,
                   Status = rqst.Complete_Status ? "Completed" : "Uncompleted",
                   LastModifiedUtc = real.LastModifiedUtc,
               })
               .OrderByDescending(s => s.LastModifiedUtc).AsQueryable();

            data = Search(data, SearchAttributes, keyword);

            var filterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            data = Filter(data, filterDictionary);

            var orderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            data = Order(data, orderDictionary);

            var pageable = new Pageable<ClearaceVBViewModel>(data, page - 1, size);

            int totalData = pageable.TotalCount;
            return new ReadResponse<ClearaceVBViewModel>(data.ToList(), totalData, orderDictionary, new List<string>());
        }
    }
}
