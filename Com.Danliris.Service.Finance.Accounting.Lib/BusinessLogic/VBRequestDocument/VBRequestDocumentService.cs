using Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRequestDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        private string GetDocumentNo(VBRequestDocumentWithPOFormDto form)
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

        public int CreateNonPO(VBRequestDocumentNonPOFormDto form)
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
            _dbContext.SaveChanges();

            AddItems(model.Id, form.Items);
            throw new NotImplementedException();
        }

        private void AddItems(int id, List<VBRequestDocumentNonPOItemFormDto> items)
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
                    element.IsSelected
                    );

                EntityExtension.FlagForCreate(result, _identityService.Username, UserAgent);
                return result;
            }).ToList();

            _dbContext.VBRequestDocumentItems.AddRange(models);
            _dbContext.SaveChanges();
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
                form.Currency.Rate,
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
                    false
                    );

                EntityExtension.FlagForCreate(documentItem, _identityService.Username, UserAgent);
                _dbContext.VBRequestDocumentItems.Add(documentItem);
                _dbContext.SaveChanges();

                AddDetails(documentItem.Id, documentItem.EPOId, item.PurchaseOrderExternal.Items);
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

        public int DeleteNonPO(int id)
        {
            throw new NotImplementedException();
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
                "DocumentNo", "ReferenceNo", "Description"
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

        public VBRequestDocumentNonPODto GetNonPOById(int id)
        {
            throw new NotImplementedException();
        }

        public VBRequestDocumentWithPODto GetWithPOById(int id)
        {
            throw new NotImplementedException();
        }

        public int UpdateNonPO(int id, VBRequestDocumentNonPOFormDto form)
        {
            throw new NotImplementedException();
        }

        public int UpdateWithPO(int id, VBRequestDocumentWithPOFormDto form)
        {
            throw new NotImplementedException();
        }
    }
}
