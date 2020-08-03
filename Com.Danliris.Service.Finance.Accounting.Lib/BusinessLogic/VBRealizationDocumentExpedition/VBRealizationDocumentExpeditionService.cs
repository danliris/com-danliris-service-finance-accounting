using Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRealizationDocumentExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRealizationDocumentExpedition
{
    public class VBRealizationDocumentExpeditionService : IVBRealizationDocumentExpeditionService
    {
        private const string UserAgent = "finance-accounting-service";
        private readonly FinanceDbContext _dbContext;
        private readonly IIdentityService _identityService;

        public VBRealizationDocumentExpeditionService(FinanceDbContext dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _identityService = serviceProvider.GetService<IIdentityService>();
        }

        public Task<int> CashierReceipt(List<int> vbRealizationIds)
        {
            var models = _dbContext.VBRealizationDocumentExpeditions.Where(entity => vbRealizationIds.Contains(entity.VBRealizationId)).ToList();

            models.ForEach(model =>
            {
                model.CashierVerification(_identityService.Username);
                EntityExtension.FlagForUpdate(model, _identityService.Username, UserAgent);
            });

            _dbContext.VBRealizationDocumentExpeditions.AddRange(models);

            return _dbContext.SaveChangesAsync();
        }

        public Task<VBRealizationDocumentExpeditionReportDto> GetReports(int vbId, int vbRealizationId, string vbRequestName, int unitId, DateTimeOffset dateStart, DateTimeOffset dateEnd, int page = 1, int size = 25)
        {
            throw new NotImplementedException();
        }

        public Task<int> InitializeExpedition(int vbRealizationId)
        {
            var realizationVB = _dbContext.RealizationVbs.FirstOrDefault(entity => entity.Id == vbRealizationId);

            var model = new VBRealizationDocumentExpeditionModel(
                realizationVB.Id,
                realizationVB.VBId,
                realizationVB.VBNo,
                realizationVB.VBNoRealize,
                realizationVB.Date,
                realizationVB.RequestVbName,
                realizationVB.UnitId,
                realizationVB.UnitName,
                realizationVB.DivisionId,
                realizationVB.DivisionName,
                realizationVB.Amount_VB,
                realizationVB.Amount,
                realizationVB.CurrencyCode,
                (double)realizationVB.CurrencyRate
                );

            EntityExtension.FlagForCreate(model, _identityService.Username, UserAgent);
            _dbContext.Add(model);
            return _dbContext.SaveChangesAsync();
        }

        public ReadResponse<VBRealizationDocumentExpeditionModel> Read(int page, int size, string order, string keyword, int position)
        {
            var query = _dbContext.Set<VBRealizationDocumentExpeditionModel>().AsQueryable();

            //query = query
            //    .Select(entity => new VBRealizationDocumentExpeditionIndexDto
            //    {
            //        Id = entity.Id,
            //        LastModifiedUtc = entity.LastModifiedUtc,
            //        VBNo = entity.VBNo,
            //        VBRealizationNo = entity.VBRealizationNo,
            //        VBRealizationDate = entity.VBRealizationDate,
            //        VBName = entity.VBRequestName,
            //        UnitId = entity.UnitId,
            //        UnitName = entity.UnitName,
            //        DivisionId = entity.DivisionId,
            //        DivisionName = entity.DivisionName,
            //        Currency

            //    });

            List<string> searchAttributes = new List<string>()
            {
                "VBRealizationNo", "VBRequestName", "UnitName"
            };

            query = QueryHelper<VBRealizationDocumentExpeditionModel>.Search(query, searchAttributes, keyword);

            //var filterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            //Query = QueryHelper<JournalTransactionModel>.Filter(Query, FilterDictionary);

            var orderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            query = QueryHelper<VBRealizationDocumentExpeditionModel>.Order(query, orderDictionary);

            var pageable = new Pageable<VBRealizationDocumentExpeditionModel>(query, page - 1, size);
            var data = pageable.Data.ToList();

            var totalData = pageable.TotalCount;

            return new ReadResponse<VBRealizationDocumentExpeditionModel>(data, totalData, orderDictionary, new List<string>());
        }

        public Task<int> Reject(int vbRealizationId, string reason)
        {
            var vbRealizationExpedtion = _dbContext.VBRealizationDocumentExpeditions.FirstOrDefault(entity => entity.VBRealizationId == vbRealizationId);

            vbRealizationExpedtion.VerificationRejected(_identityService.Username, reason);
            EntityExtension.FlagForUpdate(vbRealizationExpedtion, _identityService.Username, UserAgent);
            _dbContext.VBRealizationDocumentExpeditions.Update(vbRealizationExpedtion);
            return _dbContext.SaveChangesAsync();
        }

        public Task<int> SubmitToVerification(List<int> vbRealizationIds)
        {
            var models = _dbContext.VBRealizationDocumentExpeditions.Where(entity => vbRealizationIds.Contains(entity.VBRealizationId)).ToList();

            models.ForEach(model =>
            {
                model.SubmitToVerification(_identityService.Username);
                EntityExtension.FlagForUpdate(model, _identityService.Username, UserAgent);
            });

            _dbContext.VBRealizationDocumentExpeditions.AddRange(models);

            return _dbContext.SaveChangesAsync();
        }

        public Task<int> VerifiedToCashier(List<int> vbRealizationIds)
        {
            var models = _dbContext.VBRealizationDocumentExpeditions.Where(entity => vbRealizationIds.Contains(entity.VBRealizationId)).ToList();

            models.ForEach(model =>
            {
                model.SendToCashier(_identityService.Username);
                EntityExtension.FlagForUpdate(model, _identityService.Username, UserAgent);
            });

            _dbContext.VBRealizationDocumentExpeditions.AddRange(models);

            return _dbContext.SaveChangesAsync();
        }

        public Task<int> VerificationDocumentReceipt(List<int> vbRealizationIds)
        {
            var models = _dbContext.VBRealizationDocumentExpeditions.Where(entity => vbRealizationIds.Contains(entity.VBRealizationId)).ToList();

            models.ForEach(model =>
            {
                model.VerificationReceipt(_identityService.Username);
                EntityExtension.FlagForUpdate(model, _identityService.Username, UserAgent);
            });

            _dbContext.VBRealizationDocumentExpeditions.AddRange(models);

            return _dbContext.SaveChangesAsync();
        }

        public List<RealizationVbModel> ReadRelizationToVerification(int position)
        {
            var result = _dbContext.RealizationVbs.Where(entity => entity.Position == position).ToList();
            return result;
        }
    }
}
