using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Com.Moonlay.NetCore.Lib;
using Com.Moonlay.Models;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.VbNonPORequest;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VbWIthPORequest
{
    public class VbWithPORequestService : IVbWithPORequestService
    {
        private readonly FinanceDbContext _dbContext;
        private readonly IIdentityService _identityService;
        private const string UserAgent = "finance-service";
        protected DbSet<VbRequestModel> _DbSet;
        protected DbSet<VbRequestDetailModel> _DetailDbSet;

        public VbWithPORequestService(FinanceDbContext dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _identityService = serviceProvider.GetService<IIdentityService>();

            _DbSet = _dbContext.Set<VbRequestModel>();
            _DetailDbSet = _dbContext.Set<VbRequestDetailModel>();
        }

        public ReadResponse<VbRequestList> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            var query = _dbContext.VbRequests.AsQueryable();

            var searchAttributes = new List<string>()
            {
                "VBNo",
                "UnitLoad",
                "CreatedBy"
            };

            query = QueryHelper<VbRequestModel>.Search(query, searchAttributes, keyword);

            var filterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            query = QueryHelper<VbRequestModel>.Filter(query, filterDictionary);

            var orderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            query = QueryHelper<VbRequestModel>.Order(query, orderDictionary);

            var pageable = new Pageable<VbRequestModel>(query, page - 1, size);
            var data = pageable.Data.Select(entity => new VbRequestList()
            {
                Id = entity.Id,
                VBNo = entity.VBNo,
                Date = entity.Date,
                UnitLoad = entity.UnitLoad,
                Amount = entity.Amount,
                //CurrencyCode = entity.CurrencyCode,
                UnitName = entity.UnitName,
                CreateBy = entity.CreatedBy,
                //Status_Post = entity.Status_Post,
                Approve_Status = entity.Apporve_Status,
                Complete_Status = entity.Complete_Status,
                VBRequestCategory = entity.VBRequestCategory

            }).ToList();

            int totalData = pageable.TotalCount;

            return new ReadResponse<VbRequestList>(data, totalData, orderDictionary, new List<string>());
        }

        public Task<int> CreateAsync(VbRequestModel model, VbWithPORequestViewModel viewmodel)
        {
            model.VBNo = GetVbNonPoNo(model);

            model.VBRequestCategory = "PO";

            //model.Status_Post = false;
            model.Apporve_Status = false;
            model.Complete_Status = false;

            foreach (var itm in viewmodel.Items)
            {
                model.UnitLoad = itm.unit.Name;
            }

            EntityExtension.FlagForCreate(model, _identityService.Username, UserAgent);

            _dbContext.VbRequests.Add(model);

            return _dbContext.SaveChangesAsync();
        }

        private string GetVbNonPoNo(VbRequestModel model)
        {
            var now = model.Date;
            var year = now.ToString("yy");
            var month = now.ToString("MM");


            //var unit = model.UnitCode.ToString().Split(" - ");


            var documentNo = $"VB-{month}{year}-";

            var countSameDocumentNo = _dbContext.VbRequests.Where(a => a.Date.Month == model.Date.Month).Count(entity => entity.UnitCode.Contains(model.UnitCode));

            if (countSameDocumentNo >= 0)
            {
                countSameDocumentNo += 1;

                documentNo += string.Format("{0:000}", countSameDocumentNo);
            }

            return documentNo;
        }

        public async Task<VbWithPORequestViewModel> ReadByIdAsync2(int id)
        {
            return await _dbContext.VbRequests.Include(entity => entity.VbRequestDetail).Where(entity => entity.Id == id)
                .Select(s =>
                new VbWithPORequestViewModel
                {
                    Id = s.Id,
                    CreatedAgent = s.CreatedAgent,
                    CreatedBy = s.CreatedBy,
                    LastModifiedAgent = s.LastModifiedAgent,
                    LastModifiedBy = s.LastModifiedBy,
                    VBNo = s.VBNo,
                    Date = s.Date,
                    DateEstimate = s.DateEstimate,
                    Unit = new Unit()
                    {
                        Id = s.UnitId,
                        Code = s.UnitCode,
                        Name = s.UnitName
                    },
                    Items = s.VbRequestDetail.GroupBy(
                            groupkey => new { groupkey.PONo, groupkey.UnitName },
                            item => item,
                            (grpkey,item) => new {Group = grpkey,Item = item}
                        ).Select(
                        t => new VbWithPORequestDetailViewModel
                        {
                            no = t.Group.PONo,
                            unit = new Unit()
                            {
                                Name = t.Group.UnitName
                            },

                            Details = t.Item.Select(
                                u => new VbWithPORequestDetailItemsViewModel
                                {
                                    Conversion = u.Conversion,
                                    dealQuantity = u.DealQuantity,
                                    dealUom = new dealUom()
                                    {
                                        _id = u.DealUOMId,
                                        unit = u.DealUOMUnit
                                    },
                                    defaultQuantity = u.DefaultQuantity,
                                    defaultUom = new defaultUom()
                                    {
                                        _id = u.DefaultUOMId,
                                        unit = u.DefaultUOMUnit
                                    },
                                    priceBeforeTax = u.Price,
                                    product = new Product_VB()
                                    {
                                        _id = u.ProductId,
                                        code = u.ProductCode,
                                        name = u.ProductName
                                    },
                                    productRemark = u.ProductRemark
                                }
                                ).ToList()
                        }
                        ).ToList()

                }
                ).FirstOrDefaultAsync();
        }

        public Task<int> UpdateAsync(int id, VbWithPORequestViewModel viewmodel)
        {
            var model = MappingData2(id, viewmodel);
            model.VBRequestCategory = "PO";

            //model.Status_Post = false;
            model.Apporve_Status = false;
            model.Complete_Status = false;

            EntityExtension.FlagForUpdate(model, _identityService.Username, UserAgent);

            _dbContext.VbRequests.Update(model);

            return _dbContext.SaveChangesAsync();
        }

        public VbRequestModel MappingData2(int id,VbWithPORequestViewModel viewModel)
        {
            var listDetail = new List<VbRequestDetailModel>();

            foreach(var itm1 in viewModel.Items)
            {
                foreach(var itm2 in itm1.Details)
                {
                    var item = new VbRequestDetailModel()
                    {
                        VBId = id,
                        POId = 0,
                        PONo = itm1.no,
                        UnitName = viewModel.Unit.Name,
                        Conversion = itm2.Conversion,
                        DealQuantity = itm2.dealQuantity,
                        DealUOMId = itm2.dealUom._id,
                        DealUOMUnit = itm2.dealUom.unit,
                        DefaultQuantity = itm2.defaultQuantity,
                        DefaultUOMId = itm2.defaultUom._id,
                        DefaultUOMUnit = itm2.defaultUom.unit,
                        Price = itm2.priceBeforeTax,
                        ProductCode = itm2.product.code,
                        ProductId = itm2.product._id,
                        ProductName = itm2.product.name,
                        ProductRemark = itm2.productRemark,
                        LastModifiedBy = viewModel.LastModifiedBy,
                        LastModifiedAgent = viewModel.LastModifiedAgent,
                        DeletedBy = "",
                        DeletedAgent = "",
                        CreatedBy = viewModel.CreatedBy,
                        CreatedAgent = viewModel.CreatedAgent
                    };

                    listDetail.Add(item);
                }
            }

            var result = new VbRequestModel()
            {
                VbRequestDetail = listDetail,
                Active = viewModel.Active,
                Id = viewModel.Id,
                Date = (DateTimeOffset)viewModel.Date,
                UnitId = viewModel.Unit.Id,
                UnitCode = viewModel.Unit.Code,
                UnitName = viewModel.Unit.Name,
                VBNo = viewModel.VBNo,
                CreatedBy = viewModel.CreatedBy,
                CreatedAgent = viewModel.CreatedAgent,
                LastModifiedAgent = viewModel.LastModifiedAgent,
                LastModifiedBy = viewModel.LastModifiedBy
            };

            return result;

        }

        public Task<int> DeleteAsync(int id)
        {
            var model = _dbContext.VbRequests.Where(entity => entity.Id == id).FirstOrDefault();

            if (model != null)
            {
                EntityExtension.FlagForDelete(model, _identityService.Username, UserAgent);

                _dbContext.VbRequests.Update(model);
            }

            return _dbContext.SaveChangesAsync();
        }

        public Task<int> MappingData(VbWithPORequestViewModel viewmodel)
        {
            var result = new List<VbRequestDetailModel>();

            int value = int.Parse(_DbSet.OrderByDescending(p => p.Id)
                            .Select(r => r.Id)
                            .First().ToString());

            foreach (var itm1 in viewmodel.Items)
            {

                foreach (var itm2 in itm1.Details)
                {
                    var item = new VbRequestDetailModel()
                    {
                        PONo = itm1.no,
                        UnitName = itm1.unit.Name,
                        VBId = value,
                        ProductId = itm2.product._id,
                        ProductCode = itm2.product.code,
                        ProductName = itm2.product.name,
                        DefaultQuantity = itm2.defaultQuantity,
                        DefaultUOMId = itm2.defaultUom._id,
                        DefaultUOMUnit = itm2.defaultUom.unit,
                        DealQuantity = itm2.dealQuantity,
                        DealUOMId = itm2.dealUom._id,
                        DealUOMUnit = itm2.dealUom.unit,
                        Conversion = itm2.Conversion,
                        Price = itm2.priceBeforeTax,
                        ProductRemark = itm2.productRemark

                    };
                    EntityExtension.FlagForCreate(item, _identityService.Username, UserAgent);
                    _dbContext.VbRequestsDetails.Add(item);
                }
            }

            return _dbContext.SaveChangesAsync();
            
        }
    }
}
