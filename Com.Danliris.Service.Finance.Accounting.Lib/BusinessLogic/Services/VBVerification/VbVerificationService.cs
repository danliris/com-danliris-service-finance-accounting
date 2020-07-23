using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Newtonsoft.Json;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Moonlay.NetCore.Lib;
using Com.Moonlay.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.VbNonPORequest;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.VbNonPORequest;
using OfficeOpenXml.FormulaParsing.Utilities;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.VBVerification
{
    public class VbVerificationService : IVbVerificationService
    {
        private readonly FinanceDbContext _dbContext;
        private readonly IIdentityService _identityService;

        private const string UserAgent = "finance-service";
        private readonly DbSet<RealizationVbModel> dbSet;
        private readonly DbSet<VbRequestModel> _RequestDbSet;

        public VbVerificationService(FinanceDbContext dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _identityService = serviceProvider.GetService<IIdentityService>();

            dbSet = _dbContext.Set<RealizationVbModel>();
            _RequestDbSet = _dbContext.Set<VbRequestModel>();
        }

        public ReadResponse<VbVerificationList> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            var query = _dbContext.RealizationVbs.AsQueryable();
            var query2 = _RequestDbSet.AsQueryable();

            var searchAttributes = new List<string>()
            {
                "VBNo",
                "UnitLoad",
                "VBNoRealize",
                "RequestVbName",
                "CurrencyCodeNonPO",
                "UnitName",
                "VBRealizeCategory"
            };

            query = QueryHelper<RealizationVbModel>.Search(query, searchAttributes, keyword);

            var filterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            query = QueryHelper<RealizationVbModel>.Filter(query, filterDictionary);

            var orderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            query = QueryHelper<RealizationVbModel>.Order(query, orderDictionary);

            var pageable = new Pageable<RealizationVbModel>(query, page - 1, size);

            var data = query.Include(s => s.RealizationVbDetail).Join(query2,
               (real) => real.VBNo,
               (rqst) => rqst.VBNo,
               (real, rqst) => new VbVerificationList()
               {
                   Id = real.Id,
                   VBNo = real.VBNo,
                   DateRealization = real.Date,
                   DateEstimate = real.DateEstimate,
                   UnitLoad = real.UnitLoad,
                   Amount_Realization = real.Amount,
                   VBNoRealize = real.VBNoRealize,
                   RequestVbName = real.RequestVbName,
                   DateVB = real.DateVB,
                   Currency = real.CurrencyCodeNonPO,
                   UnitName = real.UnitName,
                   VBRealizeCategory = real.VBRealizeCategory.Contains("NONPO") ? "Non PO" : "PO",
                   Diff = real.DifferenceReqReal,

                   Usage = rqst.Usage,
                   Amount_Request = rqst.Amount,

                   DetailItems = real.RealizationVbDetail.Select(s => new ModelVbItem 
                   {

                       Date = s.DateNonPO,
                       Remark = s.Remark,
                       Amount = s.AmountNonPO,
                       isGetPPn = s.isGetPPn

                   }).ToList()

               }).ToList();

            int totalData = pageable.TotalCount;

            return new ReadResponse<VbVerificationList>(data, totalData, orderDictionary, new List<string>());
        }

        public ReadResponse<VbVerificationResultList> ReadVerification(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            var query = _dbContext.RealizationVbs.AsQueryable();

            var searchAttributes = new List<string>()
            {
                "VBNoRealize",
                "RequestVbName",
                "UnitName",
                "VBNo",
                "VbNo",
                "VBRealizeCategory",
                "CurrencyCodeNonPO"
            };

            query = QueryHelper<RealizationVbModel>.Search(query, searchAttributes, keyword);

            var filterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            query = QueryHelper<RealizationVbModel>.Filter(query, filterDictionary);

            var orderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            query = QueryHelper<RealizationVbModel>.Order(query, orderDictionary);

            var pageable = new Pageable<RealizationVbModel>(query, page - 1, size);

            var data = pageable.Data.Select(entity => new VbVerificationResultList()
            {
                DateVerified = entity.VerifiedDate,
                RealizeNo = entity.VBNoRealize,
                DateRealize = entity.Date,
                RequestName = entity.RequestVbName,
                UnitRequest = entity.UnitName,
                SendTo = entity.UnitName,
                VbNo = entity.VBNo,
                VBCategory = entity.VBRealizeCategory.Contains("NONPO") ? "Non PO" : "PO",
                Currency = entity.CurrencyCodeNonPO,
                isVerified = entity.isVerified

            }).Where(entity => entity.isVerified == true).ToList();

            int totalData = pageable.TotalCount;

            return new ReadResponse<VbVerificationResultList>(data, totalData, orderDictionary, new List<string>());
        }
    }
}
