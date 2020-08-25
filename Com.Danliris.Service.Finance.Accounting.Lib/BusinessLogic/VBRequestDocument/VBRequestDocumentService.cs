using Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRequestDocument;
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

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRequestDocument
{
    public class VBRequestDocumentService : IVBRequestDocumentService
    {
        private const string UserAgent = "finance-service";
        public readonly FinanceDbContext _dbContext;

        private readonly IIdentityService _identityService;

        public VBRequestDocumentService(FinanceDbContext dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _identityService = serviceProvider.GetService<IIdentityService>();
        }

        private string GetDocumentNo(VBRequestDocumentNonPOFormDto form)
        {
            var now = form.Date.GetValueOrDefault().AddHours(_identityService.TimezoneOffset);
            var year = now.ToString("yy");
            var month = now.ToString("MM");


            //var unit = model.UnitCode.ToString().Split(" - ");

            var unitCode = "T";
            if (form.SuppliantUnit.Division.Name.ToUpper() == "GARMENT")
                unitCode = "G";


            var documentNo = $"VB-{unitCode}-{month}{year}-";

            var countSameDocumentNo = _dbContext.VBRequestDocuments.IgnoreQueryFilters().Where(a => a.Date.AddHours(_identityService.TimezoneOffset).Month == form.Date.GetValueOrDefault().AddHours(_identityService.TimezoneOffset).Month).Count();

            if (countSameDocumentNo >= 0)
            {
                countSameDocumentNo += 1;

                documentNo += string.Format("{0:000}", countSameDocumentNo);
            }

            return documentNo;
        }

        private string GetDocumentNo(VBRequestDocumentWithPOFormDto form)
        {
            var now = form.Date.GetValueOrDefault().AddHours(_identityService.TimezoneOffset);
            var year = now.ToString("yy");
            var month = now.ToString("MM");


            //var unit = model.UnitCode.ToString().Split(" - ");

            var unitCode = "T";
            if (form.SuppliantUnit.Division.Name.ToUpper() == "GARMENT")
                unitCode = "G";


            var documentNo = $"VB-{unitCode}-{month}{year}-";

            var countSameDocumentNo = _dbContext.VBRequestDocuments.Where(a => a.Date.AddHours(_identityService.TimezoneOffset).Month == form.Date.GetValueOrDefault().AddHours(_identityService.TimezoneOffset).Month).Count();

            if (countSameDocumentNo >= 0)
            {
                countSameDocumentNo += 1;

                documentNo += string.Format("{0:000}", countSameDocumentNo);
            }

            return documentNo;
        }

        //public int CreateNonPO(VBRequestDocumentNonPOFormDto form)
        public async Task<int> CreateNonPO(VBRequestDocumentNonPOFormDto form)
        {
            var internalTransaction = _dbContext.Database.CurrentTransaction == null;
            var transaction = !internalTransaction ? _dbContext.Database.CurrentTransaction : _dbContext.Database.BeginTransaction();
            
            try
            {
                var documentNo = GetDocumentNo(form);
                var model = new VBRequestDocumentModel(
                    documentNo,
                    form.Date.GetValueOrDefault(),
                    form.RealizationEstimationDate.GetValueOrDefault(),
                    form.SuppliantUnit.Id.GetValueOrDefault(),
                    form.SuppliantUnit.Code,
                    form.SuppliantUnit.Name,
                    form.SuppliantUnit.Division.Id.GetValueOrDefault(),
                    form.SuppliantUnit.Division.Code,
                    form.SuppliantUnit.Division.Name,
                    form.Currency.Id.GetValueOrDefault(),
                    form.Currency.Code,
                    form.Currency.Symbol,
                    form.Currency.Description,
                    form.Currency.Rate.GetValueOrDefault(),
                    form.Purpose,
                    form.Amount.GetValueOrDefault(),
                    false,
                    false,
                    false,
                    VBType.NonPO
                    );

                model.FlagForCreate(_identityService.Username, UserAgent);
                _dbContext.VBRequestDocuments.Add(model);
                await _dbContext.SaveChangesAsync();

                var items = AddNonPOItems(model.Id, form.Items);

                _dbContext.VBRequestDocumentItems.AddRange(items);
                await _dbContext.SaveChangesAsync();

                if (internalTransaction)
                    transaction.Commit();

                return model.Id;
            }
            catch (Exception ex)
            {
                if (internalTransaction)
                    transaction.Rollback();
                throw ex;
            }

        }

        private List<VBRequestDocumentItemModel> AddNonPOItems(int id, List<VBRequestDocumentNonPOItemFormDto> items)
        {
            var models = items.Select(element =>
            {
                var result = new VBRequestDocumentItemModel(
                    id,
                    element.Unit.Id.GetValueOrDefault(),
                    element.Unit.Name,
                    element.Unit.Code,
                    element.Unit.Division == null ? 0 : element.Unit.Division.Id.GetValueOrDefault(),
                    element.Unit.Division?.Name,
                    element.Unit.Division?.Code,
                    0,
                    string.Empty,
                    false,
                    0,
                    string.Empty,
                    0,
                    string.Empty,
                    0,
                    element.IsSelected,
                    element.Unit.VBDocumentLayoutOrder.GetValueOrDefault()
                    );

                result.FlagForCreate(_identityService.Username, UserAgent);
                return result;
            }).ToList();

            return models;
        }

        public int CreateWithPO(VBRequestDocumentWithPOFormDto form)
        {
            var documentNo = GetDocumentNo(form);

            var model = new VBRequestDocumentModel(
                documentNo,
                form.Date.GetValueOrDefault(),
                form.RealizationEstimationDate.GetValueOrDefault(),
                form.SuppliantUnit.Id.GetValueOrDefault(),
                form.SuppliantUnit.Code,
                form.SuppliantUnit.Name,
                form.SuppliantUnit.Division.Id.GetValueOrDefault(),
                form.SuppliantUnit.Division.Code,
                form.SuppliantUnit.Division.Name,
                form.Currency.Id.GetValueOrDefault(),
                form.Currency.Code,
                form.Currency.Symbol,
                form.Currency.Description,
                form.Currency.Rate.GetValueOrDefault(),
                form.Purpose,
                form.Amount.GetValueOrDefault(),
                false,
                false,
                false,
                VBType.WithPO
                );

            EntityExtension.FlagForCreate(model, _identityService.Username, UserAgent);
            _dbContext.VBRequestDocuments.Add(model);
            _dbContext.SaveChanges();

            AddItems(model.Id, form.Items);

            return model.Id;
        }

        private void AddItems(int id, List<VBRequestDocumentWithPOItemFormDto> items)
        {
            foreach (var item in items)
            {
                var documentItem = new VBRequestDocumentItemModel(
                    id,
                    item.PurchaseOrderExternal.unit._id.GetValueOrDefault(),
                    item.PurchaseOrderExternal.unit.name,
                    item.PurchaseOrderExternal.unit.code,
                    item.PurchaseOrderExternal.unit.division._id.GetValueOrDefault(),
                    item.PurchaseOrderExternal.unit.division.name,
                    item.PurchaseOrderExternal.unit.division.code,
                    item.PurchaseOrderExternal._id.GetValueOrDefault(),
                    item.PurchaseOrderExternal.no,
                    item.PurchaseOrderExternal.useIncomeTax,
                    item.PurchaseOrderExternal.useIncomeTax ? item.PurchaseOrderExternal.incomeTax._id.GetValueOrDefault() : 0,
                    item.PurchaseOrderExternal.useIncomeTax ? item.PurchaseOrderExternal.incomeTax.name : "",
                    item.PurchaseOrderExternal.useIncomeTax ? item.PurchaseOrderExternal.incomeTax.rate.GetValueOrDefault() : 0,
                    item.PurchaseOrderExternal.incomeTaxBy,
                    0,
                    false,
                    0
                    );

                EntityExtension.FlagForCreate(documentItem, _identityService.Username, UserAgent);
                _dbContext.VBRequestDocumentItems.Add(documentItem);
                _dbContext.SaveChanges();

                AddDetails(documentItem.Id, documentItem.EPOId, item.PurchaseOrderExternal.Details);
            }
        }

        private void AddDetails(int id, int epoId, List<PurchaseOrderExternalItem> items)
        {
            var models = items.Select(element =>
            {
                var result = new VBRequestDocumentEPODetailModel(
                    id,
                    epoId,
                    element.Product._id.GetValueOrDefault(),
                    element.Product.code,
                    element.Product.name,
                    element.DefaultQuantity.GetValueOrDefault(),
                    element.Product.uom._id.GetValueOrDefault(),
                    element.Product.uom.unit,
                    element.DealQuantity.GetValueOrDefault(),
                    element.DealUOM._id.GetValueOrDefault(),
                    element.DealUOM.unit,
                    element.Conversion.GetValueOrDefault(),
                    element.Price.GetValueOrDefault(),
                    element.UseVat,
                    string.Empty
                    );

                EntityExtension.FlagForCreate(result, _identityService.Username, UserAgent);
                return result;
            }).ToList();

            _dbContext.VBRequestDocumentEPODetails.AddRange(models);
            _dbContext.SaveChanges();

        }

        private void DeleteItemNonPO(int id)
        {
            var items = _dbContext.VBRequestDocumentItems.Where(s => s.VBRequestDocumentId == id).OrderBy(s => s.VBDocumentLayoutOrder).ToList();

            foreach (var item in items)
            {
                item.FlagForDelete(_identityService.Username, UserAgent);
            }
        }

        public Task<int> DeleteNonPO(int id)
        {
            var data = _dbContext.VBRequestDocuments.FirstOrDefault(s => s.Id == id);
            data.FlagForDelete(_identityService.Username, UserAgent);

            DeleteItemNonPO(id);

            return _dbContext.SaveChangesAsync();
        }

        public int DeleteWithPO(int id)
        {
            var model = _dbContext.VBRequestDocuments.FirstOrDefault(entity => entity.Id == id);
            EntityExtension.FlagForDelete(model, _identityService.Username, UserAgent);
            _dbContext.Update(model);

            var items = _dbContext.VBRequestDocumentItems.Where(entity => entity.VBRequestDocumentId == model.Id).ToList();
            items = items.Select(element =>
            {
                EntityExtension.FlagForDelete(element, _identityService.Username, UserAgent);
                return element;
            }).ToList();

            var itemIds = items.Select(element => element.Id).ToList();
            var details = _dbContext.VBRequestDocumentEPODetails.Where(entity => itemIds.Contains(entity.VBRequestDocumentItemId)).ToList();
            details = details.Select(element =>
            {
                EntityExtension.FlagForDelete(element, _identityService.Username, UserAgent);
                return element;
            }).ToList();

            _dbContext.SaveChanges();

            return model.Id;
        }

        public ReadResponse<VBRequestDocumentModel> Get(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            var query = _dbContext.Set<VBRequestDocumentModel>().AsQueryable(); ;

            List<string> searchAttributes = new List<string>()
            {
                "DocumentNo", "SuppliantUnitName"
            };

            query = QueryHelper<VBRequestDocumentModel>.Search(query, searchAttributes, keyword);

            var filterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            query = QueryHelper<VBRequestDocumentModel>.Filter(query, filterDictionary);

            var orderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            query = QueryHelper<VBRequestDocumentModel>.Order(query, orderDictionary);

            var pageable = new Pageable<VBRequestDocumentModel>(query, page - 1, size);
            var data = pageable.Data.ToList();

            int TotalData = pageable.TotalCount;

            return new ReadResponse<VBRequestDocumentModel>(data, TotalData, orderDictionary, new List<string>());
        }

        private VBRequestDocumentNonPODto MapToNonPODTO(VBRequestDocumentModel model, List<VBRequestDocumentItemModel> items)
        {
            return new VBRequestDocumentNonPODto()
            {
                Active = model.Active,
                Amount = model.Amount,
                DocumentNo = model.DocumentNo,
                CreatedAgent = model.CreatedAgent,
                Items = items.Select(s => new VBRequestDocumentNonPOItemDto()
                {
                    IsSelected = s.IsSelected,
                    Unit = new UnitDto()
                    {
                        Code = s.UnitCode,
                        Division = new DivisionDto()
                        {
                            Code = s.DivisionCode,
                            Id = s.DivisionId,
                            Name = s.DivisionName
                        },
                        Name = s.UnitName,
                        Id = s.UnitId,
                        VBDocumentLayoutOrder = s.VBDocumentLayoutOrder
                    },
                    Id = s.Id,
                    Active = s.Active,
                    CreatedAgent = s.CreatedAgent,
                    CreatedBy = s.CreatedBy,
                    CreatedUtc = s.CreatedUtc,
                    IsDeleted = s.IsDeleted,
                    LastModifiedAgent = s.LastModifiedAgent,
                    LastModifiedBy = s.LastModifiedBy,
                    LastModifiedUtc = s.LastModifiedUtc
                }).ToList(),
                LastModifiedUtc = model.LastModifiedUtc,
                LastModifiedBy = model.LastModifiedBy,
                LastModifiedAgent = model.LastModifiedAgent,
                IsDeleted = model.IsDeleted,
                CreatedUtc = model.CreatedUtc,
                CreatedBy = model.CreatedBy,
                Currency = new CurrencyDto()
                {
                    Code = model.CurrencyCode,
                    Description = model.CurrencyDescription,
                    Id = model.CurrencyId,
                    Rate = model.CurrencyRate,
                    Symbol = model.CurrencySymbol
                },
                Id = model.Id,
                Date = model.Date,
                Purpose = model.Purpose,
                RealizationEstimationDate = model.RealizationEstimationDate,
                SuppliantUnit = new UnitDto()
                {
                    Id = model.SuppliantUnitId,
                    Code = model.SuppliantUnitCode,
                    Name = model.SuppliantUnitName,
                    Division = new DivisionDto()
                    {
                        Name = model.SuppliantDivisionName,
                        Code = model.SuppliantDivisionCode,
                        Id = model.SuppliantDivisionId
                    }
                }
            };
        }

        public async Task<VBRequestDocumentNonPODto> GetNonPOById(int id)
        {
            var data = await _dbContext.VBRequestDocuments.FirstOrDefaultAsync(s => s.Id == id);

            if (data == null)
                return null;

            var items = _dbContext.VBRequestDocumentItems.Where(s => s.VBRequestDocumentId == id).OrderBy(s => s.VBDocumentLayoutOrder).ToList();

            var result = MapToNonPODTO(data, items);

            return result;
        }

        public VBRequestDocumentWithPODto GetWithPOById(int id)
        {
            var model = _dbContext.VBRequestDocuments.FirstOrDefault(entity => entity.Id == id);
            var items = _dbContext.VBRequestDocumentItems.Where(entity => entity.VBRequestDocumentId == id).ToList();

            var result = new VBRequestDocumentWithPODto()
            {
                Amount = model.Amount,
                Currency = new CurrencyDto()
                {
                    Code = model.CurrencyCode,
                    Description = model.CurrencyDescription,
                    Id = model.CurrencyId,
                    Rate = model.CurrencyRate,
                    Symbol = model.CurrencySymbol
                },
                Date = model.Date,
                RealizationEstimationDate = model.RealizationEstimationDate,
                SuppliantUnit = new UnitDto()
                {
                    Id = model.SuppliantUnitId,
                    Code = model.SuppliantUnitCode,
                    Name = model.SuppliantUnitName,
                    Division = new DivisionDto()
                    {
                        Code = model.SuppliantDivisionCode,
                        Id = model.SuppliantDivisionId,
                        Name = model.SuppliantDivisionName
                    }
                },
                DocumentNo = model.DocumentNo,
                Id = model.Id,
                Items = items.Select(element =>
                {
                    var details = _dbContext.VBRequestDocumentEPODetails.Where(entity => entity.VBRequestDocumentItemId == element.Id);
                    var elementResult = new VBRequestDocumentWithPOItemDto()
                    {
                        PurchaseOrderExternal = new PurchaseOrderExternal()
                        {
                            incomeTax = new IncomeTaxDto()
                            {
                                name = element.IncomeTaxName,
                                rate = element.IncomeTaxRate,
                                _id = element.IncomeTaxId
                            },
                            incomeTaxBy = element.IncomeTaxBy,
                            no = element.EPONo,
                            unit = new OldUnitDto()
                            {
                                name = element.UnitName,
                                _id = element.UnitId,
                                code = element.UnitCode,
                                division = new OldDivisionDto()
                                {
                                    code = element.DivisionCode,
                                    name = element.DivisionName,
                                    _id = element.DivisionId
                                }
                            },
                            useIncomeTax = element.UseIncomeTax,
                            _id = element.EPOId,
                            Details = details.Select(entity => new PurchaseOrderExternalItem()
                            {
                                Conversion = entity.Conversion,
                                DealQuantity = entity.DealQuantity,
                                DealUOM = new UnitOfMeasurement()
                                {
                                    unit = entity.DealUOMUnit,
                                    _id = entity.DealUOMId
                                },
                                DefaultQuantity = entity.DefaultQuantity,
                                Id = entity.Id,
                                Price = entity.Price,
                                Product = new Product()
                                {
                                    code = entity.ProductCode,
                                    name = entity.ProductName,
                                    uom = new UnitOfMeasurement()
                                    {
                                        unit = entity.DefaultUOMUnit,
                                        _id = entity.DefaultUOMId
                                    },
                                    _id = entity.ProductId
                                },
                                UseVat = entity.UseVat
                            }).ToList()
                        }
                    };

                    return elementResult;
                }).ToList()
            };

            return result;
        }

        public Task<int> UpdateNonPO(int id, VBRequestDocumentNonPOFormDto form)
        {
            var data = _dbContext.VBRequestDocuments.FirstOrDefault(s => s.Id == id);

            data.SetDate(form.Date.GetValueOrDefault(), _identityService.Username, UserAgent);
            data.SetRealizationEstimationDate(form.RealizationEstimationDate.GetValueOrDefault(), _identityService.Username, UserAgent);
            data.SetCurrency(form.Currency.Id.GetValueOrDefault(), form.Currency.Code, form.Currency.Symbol, form.Currency.Rate.GetValueOrDefault(), form.Currency.Description, _identityService.Username, UserAgent);
            data.SetAmount(form.Amount.GetValueOrDefault(), _identityService.Username, UserAgent);
            data.SetPurpose(form.Purpose, _identityService.Username, UserAgent);

            EditNonPOItems(id, form.Items);
            data.FlagForUpdate(_identityService.Username, UserAgent);
            return _dbContext.SaveChangesAsync();
        }

        private void EditNonPOItems(int id, List<VBRequestDocumentNonPOItemFormDto> formItems)
        {
            var items = _dbContext.VBRequestDocumentItems.Where(s => s.VBRequestDocumentId == id).OrderBy(s => s.VBDocumentLayoutOrder).ToList();
            foreach (var item in items)
            {
                var formItem = formItems.FirstOrDefault(s => s.Id == item.Id);
                if (formItem == null)
                {
                    item.FlagForDelete(_identityService.Username, UserAgent);
                }
                else
                {
                    item.SetIsSelected(formItem.IsSelected, _identityService.Username, UserAgent);
                    item.SetUnit(formItem.Unit.Id.GetValueOrDefault(), formItem.Unit.Name, formItem.Unit.Code, _identityService.Username, UserAgent);

                    if(formItem.Unit.Division == null)
                    {
                        item.SetDivision(0, null, null, _identityService.Username, UserAgent);

                    }
                    else
                    {
                        item.SetDivision(formItem.Unit.Division.Id.GetValueOrDefault(), formItem.Unit.Division.Name, formItem.Unit.Division.Code, _identityService.Username, UserAgent);
                    }
                    
                    //item.SetVBDocumentLayoutOrder(formItem.Unit.VBDocumentLayoutOrder.GetValueOrDefault(), _identityService.Username, UserAgent);
                }
            }

            var newItems = formItems.Where(s => s.Id == 0).Select(element =>
            {
                var result = new VBRequestDocumentItemModel(
                    id,
                    element.Unit.Id.GetValueOrDefault(),
                    element.Unit.Name,
                    element.Unit.Code,
                    element.Unit.Division == null ? 0 : element.Unit.Division.Id.GetValueOrDefault(),
                    element.Unit.Division?.Name,
                    element.Unit.Division?.Code,
                    0,
                    string.Empty,
                    false,
                    0,
                    string.Empty,
                    0,
                    string.Empty,
                    0,
                    element.IsSelected,
                    element.Unit.VBDocumentLayoutOrder.GetValueOrDefault()
                    );

                result.FlagForCreate(_identityService.Username, UserAgent);
                return result;
            }).ToList();

            _dbContext.VBRequestDocumentItems.AddRange(newItems);

            //return _dbContext.SaveChangesAsync();
        }

        public int UpdateWithPO(int id, VBRequestDocumentWithPOFormDto form)
        {
            throw new NotImplementedException();
        }
    }
}
