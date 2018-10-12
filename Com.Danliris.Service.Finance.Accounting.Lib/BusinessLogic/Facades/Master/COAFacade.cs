using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.Master;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MasterCOA;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseClass;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.MasterCOA;
using Com.Moonlay.NetCore.Lib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Facades.Master
{
    public class COAFacade : BaseLogic<COAModel>, ICOAFacade
    {
        public COAFacade(IIdentityService identityService, FinanceDbContext dbContext) : base(identityService, dbContext)
        {

        }

        public async Task<int> CreateAsync(COAModel model)
        {
            CreateModel(model);
            return await DbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            await DeleteModel(id);
            return await DbContext.SaveChangesAsync();
        }

        public ReadResponse<COAModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<COAModel> query = DbSet;

            List<string> searchAttributes = new List<string>()
            {
                "Code", "Name"
            };

            query = QueryHelper<COAModel>.Search(query, searchAttributes, keyword);

            Dictionary<string, object> filterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            query = QueryHelper<COAModel>.Filter(query, filterDictionary);

            List<string> selectedFields = new List<string>()
                {
                    "Id", "Name", "Code", "Path", "Nature", "CashAccount", "ReportType", "LastModifiedUtc"
                };

            Dictionary<string, string> orderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            query = QueryHelper<COAModel>.Order(query, orderDictionary);

            query = query.Select(x => new COAModel()
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Path = x.Path,
                CashAccount = x.CashAccount,
                Nature = x.Nature,
                ReportType = x.ReportType,
                LastModifiedUtc = x.LastModifiedUtc
            });

            Pageable<COAModel> pageable = new Pageable<COAModel>(query, page - 1, size);
            List<COAModel> data = pageable.Data.ToList();
            int totalData = pageable.TotalCount;

            return new ReadResponse<COAModel>(data, totalData, orderDictionary, selectedFields);
        }

        public async Task<COAModel> ReadByIdAsync(int id)
        {
            return await ReadModelById(id);
        }

        public async Task<int> UpdateAsync(int id, COAModel model)
        {
            UpdateModelAsync(id, model);
            return await DbContext.SaveChangesAsync();
        }

        public async Task UploadData(List<COAViewModel> data)
        {
            var modelData = Mapper.Map<List<COAViewModel>, List<COAModel>>(data);

            await BulkInsert(modelData);
        }
    }
}
