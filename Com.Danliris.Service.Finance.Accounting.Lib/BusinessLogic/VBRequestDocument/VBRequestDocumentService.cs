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
            var now = form.Date.GetValueOrDefault();
            var year = now.ToString("yy");
            var month = now.ToString("MM");


            //var unit = model.UnitCode.ToString().Split(" - ");

            var unitCode = "T";
            if (form.SuppliantUnit.Division.Name.ToUpper() == "GARMENT")
                unitCode = "G";


            var documentNo = $"VB-{unitCode}-{month}{year}-";

            var countSameDocumentNo = _dbContext.VBRequestDocuments.Where(a => a.Date.Month == form.Date.GetValueOrDefault().Month).Count();

            if (countSameDocumentNo >= 0)
            {
                countSameDocumentNo += 1;

                documentNo += string.Format("{0:000}", countSameDocumentNo);
            }

            return documentNo;
        }

        public async Task<int> CreateNonPO(VBRequestDocumentNonPOFormDto form)
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
                form.Currency.Rate,
                form.Purpose,
                form.Amount.GetValueOrDefault(),
                false,
                false,
                false,
                VBType.NonPO
                );

            EntityExtension.FlagForCreate(model, _identityService.Username, UserAgent);

            _dbContext.VBRequestDocuments.Add(model);
            await _dbContext.SaveChangesAsync();

            var items = AddNonPOItems(model.Id, form.Items);

            _dbContext.VBRequestDocumentItems.AddRange(items);
            await _dbContext.SaveChangesAsync();

            return model.Id;
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
                    element.Unit.Division.Id.GetValueOrDefault(),
                    element.Unit.Division.Name,
                    element.Unit.Division.Code,
                    0,
                    string.Empty,
                    false,
                    0,
                    string.Empty,
                    0,
                    string.Empty,
                    0,
                    element.IsSelected,
                    element.Unit.VBDocumentLayoutOrder
                    );

                EntityExtension.FlagForCreate(result, _identityService.Username, UserAgent);
                return result;
            }).ToList();

            return models;
        }

        public int CreateWithPO(VBRequestDocumentWithPOFormDto form)
        {
            throw new NotImplementedException();
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
            var data =  _dbContext.VBRequestDocuments.FirstOrDefault(s => s.Id == id);
            data.FlagForDelete(_identityService.Username, UserAgent);

            DeleteItemNonPO(id);

            return _dbContext.SaveChangesAsync();
        }

        public int DeleteWithPO(int id)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public Task<int> UpdateNonPO(int id, VBRequestDocumentNonPOFormDto form)
        {
            var data = _dbContext.VBRequestDocuments.FirstOrDefault(s => s.Id == id);
            
            data.SetDate(form.Date.GetValueOrDefault(), _identityService.Username, UserAgent);
            data.SetRealizationEstimationDate(form.RealizationEstimationDate.GetValueOrDefault(), _identityService.Username, UserAgent);
            data.SetCurrency(form.Currency.Id.GetValueOrDefault(), form.Currency.Code, form.Currency.Symbol, form.Currency.Rate, form.Currency.Description, _identityService.Username, UserAgent);
            data.SetAmount(form.Amount.GetValueOrDefault(), _identityService.Username, UserAgent);
            data.SetPurpose(form.Purpose, _identityService.Username, UserAgent);
            
            EditNonPOItems(id, form.Items);
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
                    item.SetDivision(formItem.Unit.Division.Id.GetValueOrDefault(), formItem.Unit.Division.Name, formItem.Unit.Division.Code, _identityService.Username, UserAgent);
                    item.SetVBDocumentLayoutOrder(formItem.Unit.VBDocumentLayoutOrder, _identityService.Username, UserAgent);
                }
            }

            var newItems = formItems.Where(s => s.Id == 0).Select(element =>
            {
                var result = new VBRequestDocumentItemModel(
                    id,
                    element.Unit.Id.GetValueOrDefault(),
                    element.Unit.Name,
                    element.Unit.Code,
                    element.Unit.Division.Id.GetValueOrDefault(),
                    element.Unit.Division.Name,
                    element.Unit.Division.Code,
                    0,
                    string.Empty,
                    false,
                    0,
                    string.Empty,
                    0,
                    string.Empty,
                    0,
                    element.IsSelected,
                    element.Unit.VBDocumentLayoutOrder
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
