using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.VBRealizationDocumentNonPO;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRealizationDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Com.Moonlay.Models;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.VBRealizationDocumentNonPO;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRequestDocument;
using Newtonsoft.Json;
using Com.Moonlay.NetCore.Lib;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.VBRealizationDocumentNonPO
{
    public class VBRealizationDocumentNonPOService : IVBRealizationDocumentNonPOService
    {
        private const string UserAgent = "finance-service";
        protected DbSet<VBRealizationDocumentModel> _dbSet;
        protected IIdentityService _identityService;
        public FinanceDbContext _dbContext;

        public VBRealizationDocumentNonPOService(IServiceProvider serviceProvider, FinanceDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<VBRealizationDocumentModel>();
            _identityService = serviceProvider.GetService<IIdentityService>();
        }

        private Tuple<string, int> GetDocumentNo(VBRealizationDocumentNonPOViewModel form, VBRealizationDocumentModel existingData)
        {
            var now = form.Date.GetValueOrDefault().AddHours(_identityService.TimezoneOffset);
            var year = now.ToString("yy");
            var month = now.ToString("MM");

            var documentNo = $"R-{month}{year}-";

            var index = 1;

            if (existingData != null)
            {
                index = existingData.Index + 1;
            }

            documentNo += string.Format("{0:000}", index);


            return new Tuple<string, int>(documentNo, index);
        }

        private List<VBRealizationDocumentExpenditureItemModel> AddItems(int id, IEnumerable<VBRealizationDocumentNonPOExpenditureItemViewModel> items)
        {
            var models = items.Select(element =>
            {
                var result = new VBRealizationDocumentExpenditureItemModel(id, element);

                result.FlagForCreate(_identityService.Username, UserAgent);
                return result;
            }).ToList();

            return models;
        }

        private List<VBRealizationDocumentUnitCostsItemModel> AddUnitCosts(int id, IEnumerable<VBRealizationDocumentNonPOUnitCostViewModel> unitCosts)
        {
            var models = unitCosts.Select(element =>
            {
                var result = new VBRealizationDocumentUnitCostsItemModel(id, element);

                result.FlagForCreate(_identityService.Username, UserAgent);
                return result;
            }).ToList();

            return models;
        }

        public async Task<int> CreateAsync(VBRealizationDocumentNonPOViewModel vm)
        {
            var internalTransaction = _dbContext.Database.CurrentTransaction == null;
            var transaction = !internalTransaction ? _dbContext.Database.CurrentTransaction : _dbContext.Database.BeginTransaction();

            try
            {
                var existingData = _dbContext.VBRealizationDocuments.Where(a => a.Date.AddHours(_identityService.TimezoneOffset).Month == vm.Date.GetValueOrDefault().AddHours(_identityService.TimezoneOffset).Month).OrderByDescending(s => s.Index).FirstOrDefault();
                var documentNo = GetDocumentNo(vm, existingData);
                vm.DocumentNo = documentNo.Item1;
                vm.Index = documentNo.Item2;
                var model = new VBRealizationDocumentModel(vm);

                model.FlagForCreate(_identityService.Username, UserAgent);
                _dbContext.VBRealizationDocuments.Add(model);
                await _dbContext.SaveChangesAsync();


                var items = AddItems(model.Id, vm.Items);

                _dbContext.VBRealizationDocumentExpenditureItems.AddRange(items);
                await _dbContext.SaveChangesAsync();

                var unitCosts = AddUnitCosts(model.Id, vm.UnitCosts);

                _dbContext.VBRealizationDocumentUnitCostsItems.AddRange(unitCosts);
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

        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public ReadResponse<VBRealizationDocumentModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            var query = _dbContext.VBRealizationDocuments.Where(entity => entity.Type == VBType.NonPO).AsQueryable();

            var searchAttributes = new List<string>()
            {
                "DocumentNo",
                "VBRequestDocumentNo",
                "VBRequestDocumentCreatedBy"
            };

            query = QueryHelper<VBRealizationDocumentModel>.Search(query, searchAttributes, keyword);

            var filterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            query = QueryHelper<VBRealizationDocumentModel>.Filter(query, filterDictionary);

            var orderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            query = QueryHelper<VBRealizationDocumentModel>.Order(query, orderDictionary);

            var pageable = new Pageable<VBRealizationDocumentModel>(query, page - 1, size);
            var data = pageable.Data.ToList();

            int totalData = pageable.TotalCount;

            return new ReadResponse<VBRealizationDocumentModel>(data, totalData, orderDictionary, new List<string>());
        }

        public Task<VBRealizationDocumentNonPOViewModel> ReadByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(int id, VBRealizationDocumentNonPOViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}
