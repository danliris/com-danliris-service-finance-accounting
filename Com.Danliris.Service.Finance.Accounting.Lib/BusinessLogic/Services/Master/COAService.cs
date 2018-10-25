using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.Master;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MasterCOA;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.BasicUploadCsvService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.MasterCOA;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.Master
{
    public class COAService : ICOAService, IBasicUploadCsvService<COAViewModel>
    {
        private const string UserAgent = "finance-service";
        protected DbSet<COAModel> DbSet;
        protected IIdentityService IdentityService;
        public FinanceDbContext DbContext;

        private readonly List<string> Header = new List<string>()
        {
            "Kode", "Nama", "Path", "Report Type", "Nature", "Cash Account"
        };
        public List<string> CsvHeader => Header;


        public sealed class COAMap : ClassMap<COAViewModel>
        {
            public COAMap()
            {
                Map(x => x.Code).Index(0);
                Map(x => x.Name).Index(1);
                Map(x => x.Path).Index(2);
                Map(x => x.ReportType).Index(3);
                Map(x => x.Nature).Index(4);
                Map(x => x.CashAccount).Index(5);
            }
        }

        public COAService(IServiceProvider serviceProvider, FinanceDbContext dbContext)
        {
            DbContext = dbContext;
            this.DbSet = dbContext.Set<COAModel>();
            this.IdentityService = serviceProvider.GetService<IIdentityService>();
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

        public async Task UploadData(List<COAModel> data)
        {
            await BulkInsert(data);
        }

        public void CreateModel(COAModel model)
        {
            EntityExtension.FlagForCreate(model, IdentityService.Username, UserAgent);
            DbSet.Add(model);
        }

        public Task<COAModel> ReadModelById(int id)
        {
            return DbSet.FirstOrDefaultAsync(d => d.Id.Equals(id) && d.IsDeleted.Equals(false));
        }

        public void UpdateModelAsync(int id, COAModel model)
        {
            EntityExtension.FlagForUpdate(model, IdentityService.Username, UserAgent);
            DbSet.Update(model);
        }

        public async Task DeleteModel(int id)
        {
            COAModel model = await ReadModelById(id);
            EntityExtension.FlagForDelete(model, IdentityService.Username, UserAgent, true);
            DbSet.Update(model);
        }

        public Task<int> BulkInsert(IEnumerable<COAModel> entities)
        {
            return Task.Factory.StartNew(async () =>
            {
                const int pageSize = 1000;
                int offset = 0;
                int processed = 0;

                var batch = entities.Where((data, index) => offset <= index && index < offset + pageSize);
                using (var transaction = DbContext.Database.BeginTransaction())
                {
                    while (batch.Count() > 0)
                    {
                        foreach (var item in batch)
                        {
                            EntityExtension.FlagForCreate(item, IdentityService.Username, UserAgent);
                        }
                        DbSet.AddRange(batch);
                        var result = await DbContext.SaveChangesAsync();
                        processed += batch.Count();
                        offset = pageSize;
                    };
                    transaction.Commit();
                }

                return processed;
            }).Unwrap();
        }

        public Tuple<bool, List<object>> UploadValidate(List<COAViewModel> Data, List<KeyValuePair<string, StringValues>> Body)
        {
            List<object> ErrorList = new List<object>();
            string ErrorMessage;
            bool Valid = true;



            foreach (COAViewModel coaVM in Data)
            {
                ErrorMessage = "";

                if (string.IsNullOrWhiteSpace(coaVM.Code))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh kosong, ");
                }
                else if (Data.Any(d => d != coaVM && d.Code.Equals(coaVM.Code)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh duplikat, ");
                }

                if (string.IsNullOrWhiteSpace(coaVM.Name))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh kosong, ");
                }
                else if (Data.Any(d => d != coaVM && d.Name.Equals(coaVM.Name)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh duplikat, ");
                }

                if (!string.IsNullOrEmpty(ErrorMessage))
                {
                    ErrorMessage = ErrorMessage.Remove(ErrorMessage.Length - 2);
                    var Error = new ExpandoObject() as IDictionary<string, object>;

                    Error.Add("Kode", coaVM.Code);
                    Error.Add("Nama", coaVM.Name);
                    Error.Add("Path", coaVM.Path);
                    Error.Add("Report Type", coaVM.ReportType);
                    Error.Add("Nature", coaVM.Nature);
                    Error.Add("Cash Account", coaVM.CashAccount);
                    Error.Add("Error", ErrorMessage);

                    ErrorList.Add(Error);
                }
            }

            if (ErrorList.Count > 0)
            {
                Valid = false;
            }

            return Tuple.Create(Valid, ErrorList);
        }

    }
}
