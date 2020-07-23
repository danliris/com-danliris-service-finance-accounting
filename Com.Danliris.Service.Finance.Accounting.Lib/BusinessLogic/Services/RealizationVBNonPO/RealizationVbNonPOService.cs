using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.RealizationVBNonPO;
using Com.Moonlay.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.IntegrationViewModel;
using Newtonsoft.Json;
using Com.Moonlay.NetCore.Lib;
using System.Net.Http.Headers;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.RealizationVBNonPO
{
    public class RealizationVbNonPOService : IRealizationVbNonPOService
    {
        private readonly FinanceDbContext _dbContext;
        private readonly IIdentityService _identityService;
        private const string UserAgent = "finance-service";
        protected DbSet<RealizationVbModel> _DbSet;
        protected DbSet<RealizationVbDetailModel> _DetailDbSet;

        public RealizationVbNonPOService(FinanceDbContext dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _identityService = serviceProvider.GetService<IIdentityService>();

            _DbSet = _dbContext.Set<RealizationVbModel>();
            _DetailDbSet = _dbContext.Set<RealizationVbDetailModel>();
        }

        public Task<int> CreateAsync(RealizationVbModel model, RealizationVbNonPOViewModel viewmodel)
        {
            var updateTotalRequestVb = _dbContext.VbRequests.FirstOrDefault(x => x.VBNo == model.VBNo && x.IsDeleted == false);
            updateTotalRequestVb.Realization_Status = true;

            model.VBNoRealize = GetVbRealizeNo(model);
            model.isVerified = false;
            model.isClosed = false;
            model.isNotVeridied = true;
            decimal temp_total = 0;
            decimal convert_total = 0;

            foreach (var item2 in viewmodel.Items)
            {
                decimal count_total;
                if (item2.isGetPPn == true)
                {
                    decimal temp = item2.Amount * 0.1m;
                    count_total = item2.Amount + temp;
                    convert_total += ConvertRate(count_total, viewmodel);
                    temp_total += count_total;
                }
                else
                {
                    count_total = item2.Amount;
                    convert_total += ConvertRate(count_total, viewmodel);
                    temp_total += item2.Amount;
                }
            }

            model.Amount = temp_total;

            var ResultDiffReqReal = viewmodel.numberVB.Amount - convert_total;

            if (ResultDiffReqReal > 0)
            {
                model.DifferenceReqReal = ResultDiffReqReal;
                model.StatusReqReal = "Sisa";
            }
            else if (ResultDiffReqReal == 0)
            {
                model.DifferenceReqReal = ResultDiffReqReal;
                model.StatusReqReal = "Sesuai";
            }
            else
            {
                model.DifferenceReqReal = ResultDiffReqReal * -1;
                model.StatusReqReal = "Kurang";
            }

            

            EntityExtension.FlagForCreate(model, _identityService.Username, UserAgent);

            _dbContext.RealizationVbs.Add(model);

            return _dbContext.SaveChangesAsync();
        }

        private decimal ConvertRate(decimal count, RealizationVbNonPOViewModel viewmodel)
        {
            double convertCurrency;
            if (viewmodel.numberVB.CurrencyCode == "IDR")
            {
                convertCurrency = (double)count;
            }
            else
            {
                convertCurrency = (Math.Round((double)count * (double)viewmodel.numberVB.CurrencyRate));
            }


            return (decimal)convertCurrency;
        }

        private string GetVbRealizeNo(RealizationVbModel model)
        {
            var now = model.Date.LocalDateTime;
            var year = now.ToString("yy");
            var month = now.ToString("MM");

            var documentNo = $"R-{month}{year}-";

            var countSameDocumentNo = _dbContext.RealizationVbs.Where(a => a.Date.Month == model.Date.Month).Count();

            if (countSameDocumentNo >= 0)
            {
                countSameDocumentNo += 1;

                documentNo += string.Format("{0:000}", countSameDocumentNo);
            }

            return documentNo;
        }

        public Task<int> DeleteAsync(int id)
        {
            var model = _dbContext.RealizationVbs.Where(entity => entity.Id == id).FirstOrDefault();

            var updateTotalRequestVb = _dbContext.VbRequests.FirstOrDefault(x => x.VBNo == model.VBNo && x.IsDeleted == false);
            updateTotalRequestVb.Realization_Status = false;

            if (model != null)
            {
                EntityExtension.FlagForDelete(model, _identityService.Username, UserAgent);

                _dbContext.RealizationVbs.Update(model);
            }

            return _dbContext.SaveChangesAsync();
        }

        public Task<int> MappingData(RealizationVbNonPOViewModel viewmodel)
        {
            var result = new List<RealizationVbDetailModel>();

            int value = int.Parse(_DbSet.OrderByDescending(p => p.Id)
                            .Select(r => r.Id)
                            .First().ToString());

            foreach (var itm1 in viewmodel.Items)
            {
                var item = new RealizationVbDetailModel()
                {
                    VBRealizationId = value,
                    AmountNonPO = itm1.Amount,
                    DateNonPO = itm1.DateDetail,
                    Remark = itm1.Remark,
                    isGetPPn = itm1.isGetPPn
                };

                EntityExtension.FlagForCreate(item, _identityService.Username, UserAgent);
                _dbContext.RealizationVbDetails.Add(item);
            }

            return _dbContext.SaveChangesAsync();
        }

        public ReadResponse<RealizationVbList> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            var query = _dbContext.RealizationVbs.AsQueryable();

            var searchAttributes = new List<string>()
            {
                "VBNo",
                "VBNoRealize",
                "RequestVbName"
            };

            query = QueryHelper<RealizationVbModel>.Search(query, searchAttributes, keyword);

            var filterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            query = QueryHelper<RealizationVbModel>.Filter(query, filterDictionary);

            var orderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            query = QueryHelper<RealizationVbModel>.Order(query, orderDictionary);

            var pageable = new Pageable<RealizationVbModel>(query, page - 1, size);
            var data = pageable.Data.Select(entity => new RealizationVbList()
            {
                Id = entity.Id,
                VBNo = entity.VBNo,
                VBNoRealize = entity.VBNoRealize,
                Date = entity.Date,
                DateEstimate = entity.DateEstimate,
                RequestVbName = entity.RequestVbName,
                isVerified = entity.isVerified,
                VBRealizeCategory = entity.VBRealizeCategory

            }).Where(entity => entity.VBRealizeCategory == "NONPO").ToList();

            int totalData = pageable.TotalCount;

            return new ReadResponse<RealizationVbList>(data, totalData, orderDictionary, new List<string>());
        }

        public async Task<RealizationVbNonPOViewModel> ReadByIdAsync2(int id)
        {
            var model = await _dbContext.RealizationVbs.Include(entity => entity.RealizationVbDetail).Where(entity => entity.Id == id).FirstOrDefaultAsync();
            var result = new RealizationVbNonPOViewModel()
            {
                Id = model.Id,
                CreatedAgent = model.CreatedAgent,
                CreatedBy = model.CreatedBy,
                LastModifiedAgent = model.LastModifiedAgent,
                LastModifiedBy = model.LastModifiedBy,
                VBRealizationNo = model.VBNoRealize,
                Date = model.Date,
                numberVB = new DetailRequestNonPO()
                {
                    Amount = model.Amount_VB,
                    CreateBy = model.RequestVbName,
                    CurrencyCode = model.CurrencyCode,
                    CurrencyRate = model.CurrencyRate,
                    Date = model.DateVB,
                    DateEstimate = model.DateEstimate,
                    UnitCode = model.UnitCode,
                    UnitLoad = model.UnitLoad,
                    UnitName = model.UnitName,
                    VBNo = model.VBNo,
                    VBRequestCategory = model.VBRealizeCategory
                },
                Items = model.RealizationVbDetail.Select(s => new VbNonPORequestDetailViewModel()
                {

                    DateDetail = s.DateNonPO,
                    Remark = s.Remark,
                    Amount = s.AmountNonPO,
                    isGetPPn = s.isGetPPn

                }).ToList()
            };

            return result;
        }

        public Task<int> UpdateAsync(int id, RealizationVbNonPOViewModel viewModel)
        {
            var model = MappingData2(id, viewModel);

            var updateTotalRequestVb = _dbContext.VbRequests.FirstOrDefault(x => x.VBNo == model.VBNo && x.IsDeleted == false);
            updateTotalRequestVb.Realization_Status = true;

            EntityExtension.FlagForUpdate(model, _identityService.Username, UserAgent);

            var itemIds = _dbContext.RealizationVbDetails.Where(entity => entity.VBRealizationId == id).Select(entity => entity.Id).ToList();

            foreach (var itemId in itemIds)
            {
                var item = model.RealizationVbDetail.FirstOrDefault(element => element.Id == itemId);

                if (item == null)
                {
                    var itemToDelete = _dbContext.RealizationVbDetails.FirstOrDefault(entity => entity.Id == itemId);
                    EntityExtension.FlagForDelete(itemToDelete, _identityService.Username, UserAgent);
                    _dbContext.RealizationVbDetails.Update(itemToDelete);
                }
                else
                {
                    EntityExtension.FlagForUpdate(item, _identityService.Username, UserAgent);
                    _dbContext.RealizationVbDetails.Update(item);
                }
            }

            foreach (var item in model.RealizationVbDetail)
            {
                if (item.Id <= 0)
                {
                    EntityExtension.FlagForCreate(item, _identityService.Username, UserAgent);
                    _dbContext.RealizationVbDetails.Add(item);
                }
            }

            _dbContext.RealizationVbs.Update(model);

            return _dbContext.SaveChangesAsync();
        }

        public RealizationVbModel MappingData2(int id, RealizationVbNonPOViewModel viewModel)
        {
            var listDetail = new List<RealizationVbDetailModel>();

            decimal temp_total = 0;
            decimal convert_total = 0;
            foreach (var itm in viewModel.Items)
            {

                decimal count_total;
                if (itm.isGetPPn == true)
                {
                    decimal temp = itm.Amount * 0.1m;
                    count_total = itm.Amount + temp;
                    convert_total += ConvertRate(count_total, viewModel);
                    temp_total += count_total;
                }
                else
                {
                    count_total = itm.Amount;
                    convert_total += ConvertRate(count_total, viewModel);
                    temp_total += itm.Amount;
                }

                var item = new RealizationVbDetailModel()
                {
                    VBRealizationId = id,
                    LastModifiedBy = viewModel.LastModifiedBy,
                    LastModifiedAgent = viewModel.LastModifiedAgent,
                    DeletedBy = "",
                    DeletedAgent = "",
                    CreatedBy = viewModel.CreatedBy,
                    CreatedAgent = viewModel.CreatedAgent,
                    DateNonPO = itm.DateDetail,
                    Remark = itm.Remark,
                    AmountNonPO = itm.Amount,
                    isGetPPn = itm.isGetPPn
                };

                listDetail.Add(item);
            }

            var result = new RealizationVbModel()
            {
                RealizationVbDetail = listDetail,
                Active = viewModel.Active,
                Id = viewModel.Id,
                Date = (DateTimeOffset)viewModel.Date,
                UnitCode = viewModel.numberVB.UnitCode,
                UnitName = viewModel.numberVB.UnitName,
                VBNo = viewModel.numberVB.VBNo,
                VBNoRealize = viewModel.VBRealizationNo,
                DateEstimate = (DateTimeOffset)viewModel.numberVB.DateEstimate,
                DateVB = (DateTimeOffset)viewModel.numberVB.Date,
                CurrencyCode = viewModel.numberVB.CurrencyCode,
                CurrencyRate = viewModel.numberVB.CurrencyRate,
                UnitLoad = viewModel.numberVB.UnitLoad,
                RequestVbName = viewModel.numberVB.CreateBy,
                UsageVBRequest = viewModel.numberVB.Usage,
                Amount = temp_total,
                Amount_VB = viewModel.numberVB.Amount,
                isVerified = false,
                isClosed = false,
                isNotVeridied = true,
                CreatedBy = viewModel.CreatedBy,
                CreatedAgent = viewModel.CreatedAgent,
                LastModifiedAgent = viewModel.LastModifiedAgent,
                LastModifiedBy = viewModel.LastModifiedBy,
                VBRealizeCategory = viewModel.numberVB.VBRequestCategory,
                DifferenceReqReal = convert_total,
                
            };

            return result;
        }
    }
}
