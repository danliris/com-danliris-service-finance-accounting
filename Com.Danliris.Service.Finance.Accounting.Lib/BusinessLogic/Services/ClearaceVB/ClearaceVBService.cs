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

        //public ReadResponse<VbRequestModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        //{
        //    //IQueryable<VbRequestModel> Query = _RequestDbSet;

        //    var real = _DbContext.RealizationVbs.Select(x => x.VBNo);
        //    IQueryable<VbRequestModel> Query = _RequestDbSet.Where(x => real.Any(y => y == x.VBNo));
        //    //.Select(x => x.Id);

        //    List<string> SearchAttributes = new List<string>()
        //    {
        //        "VBNo"
        //    };

        //    Query = QueryHelper<VbRequestModel>.Search(Query, SearchAttributes, keyword);

        //    Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
        //    Query = QueryHelper<VbRequestModel>.Filter(Query, FilterDictionary);

        //    List<string> SelectedFields = new List<string>()
        //    {
        //        "Id","VBNo"
        //    };

        //    Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
        //    Query = QueryHelper<VbRequestModel>.Order(Query, OrderDictionary);

        //    Pageable<VbRequestModel> pageable = new Pageable<VbRequestModel>(Query, page - 1, size);
        //    List<VbRequestModel> data = pageable.Data.ToList<VbRequestModel>();
        //    int totalData = pageable.TotalCount;

        //    return new ReadResponse<VbRequestModel>(data, totalData, OrderDictionary, SelectedFields);

        //}

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
            var query2 = _RealizationDbSet.AsQueryable();

            List<string> SearchAttributes = new List<string>()
            {
                "VBNo"
            };

            query = QueryHelper<VbRequestModel>.Search(query, SearchAttributes, keyword);

            var filterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            query = QueryHelper<VbRequestModel>.Filter(query, filterDictionary);

            var orderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            query = QueryHelper<VbRequestModel>.Order(query, orderDictionary);

            var pageable = new Pageable<VbRequestModel>(query, page - 1, size);
            //var pageable2 = new Pageable<RealizationVbModel>(query2, page - 1, size);

            var diffStatus = "";

            var data = query
               .Join(query2,
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
                   RealNo = real.VBNoRealize,
                   RealDate = real.Date,
                   VerDate = null,
                   DiffStatus = diffStatus,
                   DiffAmount = rqst.Amount - real.Amount,
                   ClearanceDate = rqst.CompleteDate,
                   IsPosted = rqst.Complete_Status,
                   Status = rqst.Complete_Status ? "Completed" : "Uncompleted",
                   LastModifiedUtc = real.LastModifiedUtc,
               })
               .OrderByDescending(s => s.LastModifiedUtc).ToList();
            //return data.ToList();

            int totalData = pageable.TotalCount;
            return new ReadResponse<ClearaceVBViewModel>(data, totalData, orderDictionary, new List<string>());
        }

        //public Task<List<VbRequestModel>> ReadByIdAsync(long id)
        //{
        //    var requestQuery = _RequestDbSet.AsQueryable();
        //    var realizationQuery = _RealizationDbSet.AsQueryable();

        //    var diffStatus = "";

        //    var result = requestQuery
        //       .Join(realizationQuery,
        //       (rqst) => rqst.VBNo,
        //       (real) => real.VBNo,
        //       (rqst, real) => new ClearaceVBViewModel()
        //       {
        //           Id = rqst.Id,
        //           RqstNo = rqst.VBNo,
        //           VBCategory = rqst.VBRequestCategory,
        //           RqstDate = rqst.Date,
        //           Unit = new Unit()
        //           {
        //               Id = rqst.Id,
        //               Name = rqst.UnitName,
        //           },
        //           RealNo = real.VBNoRealize,
        //           RealDate = real.Date,
        //           VerDate = null,
        //           DiffStatus = diffStatus,
        //           DiffAmount = rqst.Amount-real.Amount,
        //           ClearanceDate = rqst.CompleteDate,
        //           IsPosted = rqst.Complete_Status,
        //           Status = rqst.Complete_Status ? "Completed" : "Uncompleted",
        //           LastModifiedUtc = real.LastModifiedUtc,
        //       })
        //       .OrderByDescending(s => s.LastModifiedUtc).ToList();

        //    return result.ToList();
        //}
    }
}
