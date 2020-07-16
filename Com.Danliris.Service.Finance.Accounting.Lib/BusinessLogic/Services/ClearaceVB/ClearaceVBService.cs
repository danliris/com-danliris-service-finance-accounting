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
                "VBNo"
            };

            query = QueryHelper<VbRequestModel>.Search(query, SearchAttributes, keyword);

            var filterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            query = QueryHelper<VbRequestModel>.Filter(query, filterDictionary);

            var orderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            query = QueryHelper<VbRequestModel>.Order(query, orderDictionary);

            var pageable = new Pageable<VbRequestModel>(query, page - 1, size);
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
               .OrderByDescending(s => s.LastModifiedUtc).ToList();

            int totalData = pageable.TotalCount;
            return new ReadResponse<ClearaceVBViewModel>(data, totalData, orderDictionary, new List<string>());
        }
    }
}
