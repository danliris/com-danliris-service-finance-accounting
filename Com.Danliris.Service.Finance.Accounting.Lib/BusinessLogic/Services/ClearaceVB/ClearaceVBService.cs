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
using System.Globalization;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRequestDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRealizationDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRealizationDocumentExpedition;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.ClearaceVB
{
    public class ClearaceVBService : IClearaceVBService
    {
        private const string _UserAgent = "finance-service";
        protected DbSet<VBRequestDocumentModel> _RequestDbSet;
        protected DbSet<VBRealizationDocumentModel> _RealizationDbSet;
        private readonly IServiceProvider _serviceProvider;
        protected IIdentityService _IdentityService;
        public FinanceDbContext _DbContext;

        public ClearaceVBService(IServiceProvider serviceProvider, FinanceDbContext dbContext)
        {
            _DbContext = dbContext;
            _RequestDbSet = dbContext.Set<VBRequestDocumentModel>();
            _RealizationDbSet = dbContext.Set<VBRealizationDocumentModel>();
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
                    //if (Attribute.Contains("."))
                    //{
                    //    var Key = Attribute.Split(".");
                    //    SearchQuery = string.Concat(SearchQuery, Key[0], $".Any({Key[1]}.Contains(@0)) OR ");
                    //}
                    //else
                    //{
                    //    SearchQuery = string.Concat(SearchQuery, Attribute, ".Contains(@0) OR ");
                    //}
                    SearchQuery = string.Concat(SearchQuery, Attribute, ".Contains(@0) OR ");
                }

                SearchQuery = SearchQuery.Remove(SearchQuery.Length - 4);

                //if (ToLowerCase)
                //{
                //    SearchQuery = SearchQuery.Replace(".Contains(@0)", ".ToLower().Contains(@0)");
                //    keyword = keyword.ToLower();
                //}

                query = query.Where(SearchQuery, keyword);
            }
            return query;
        }

        public Task<int> CreateAsync(VBRequestDocumentModel model)
        {
            EntityExtension.FlagForCreate(model, _IdentityService.Username, _UserAgent);

            _DbContext.VBRequestDocuments.Add(model);

            return _DbContext.SaveChangesAsync();
        }

        public virtual void UpdateAsync(long id, VBRequestDocumentModel model)
        {
            EntityExtension.FlagForUpdate(model, _IdentityService.Username, _UserAgent);
            _RequestDbSet.Update(model);
        }

        public Task<VBRequestDocumentModel> ReadByIdAsync(long id)
        {
            return _DbContext.VBRequestDocuments.Where(entity => entity.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> ClearanceVBPost(List<long> listId)
        {
            foreach (var id in listId)
            {
                var model = await ReadByIdAsync(id);
                model.SetIsCompleted(true, _IdentityService.Username, _UserAgent);
                model.SetCompletedDate(DateTimeOffset.UtcNow, _IdentityService.Username, _UserAgent);
                model.SetCompletedBy(_IdentityService.Username, _IdentityService.Username, _UserAgent);
                
                UpdateAsync(id, model);
            }
            return await _DbContext.SaveChangesAsync();
        }

        public async Task<int> ClearanceVBUnpost(long id)
        {
            var model = await ReadByIdAsync(id);
            model.SetIsCompleted(false, _IdentityService.Username, _UserAgent);
            UpdateAsync(id, model);
            return await _DbContext.SaveChangesAsync();
        }

        public ReadResponse<ClearaceVBViewModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            var query = _RequestDbSet.AsQueryable();
            var realizationQuery = _RealizationDbSet.AsQueryable().Where(s => s.IsVerified && s.Position == VBRealizationPosition.Cashier);

            List<string> SearchAttributes = new List<string>()
            {
                "RqstNo","Appliciant","RealNo","Status","DiffStatus"
            };

            var data = query
               .Join(realizationQuery,
               (rqst) => rqst.Id,
               (real) => real.VBRequestDocumentId,
               (rqst, real) => new ClearaceVBViewModel()
               {
                   Id = rqst.Id,
                   RqstNo = rqst.DocumentNo,
                   VBCategory = rqst.Type,
                   RqstDate = rqst.Date,
                   //RqstDate = rqst.Date.AddHours(7).ToString("dd MMMM yyyy", new CultureInfo("id-ID")),
                   Unit = new Unit()
                   {
                       Id = rqst.SuppliantUnitId,
                       Name = rqst.SuppliantUnitName,
                   },
                   Appliciant = rqst.CreatedBy,
                   RealNo = real.DocumentNo,
                   RealDate = real.Date,
                   //RealDate = rqst.Realization_Status == true ? real.Date.AddHours(7).ToString("dd MMMM yyyy", new CultureInfo("id-ID")) : "",
                   VerDate = real.VerificationDate,
                   //VerDate = real.isVerified == true ? real.VerifiedDate.AddHours(7).ToString("dd MMMM yyyy", new CultureInfo("id-ID")) : "",
                   //DiffStatus = real.StatusReqReal,
                   DiffAmount = rqst.Amount - real.Amount,
                   ClearanceDate = rqst.CompletedDate,
                   DiffStatus = rqst.Amount - real.Amount < 0 ? "Kurang" : rqst.Amount - real.Amount > 0 ? "Sisa" : "Sesuai",
                   //ClearanceDate = rqst.Complete_Status == true ? rqst.CompleteDate.ToString() : "",
                   IsPosted = rqst.IsCompleted,
                   Status = rqst.IsCompleted ? "Completed" : "Uncompleted",
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
