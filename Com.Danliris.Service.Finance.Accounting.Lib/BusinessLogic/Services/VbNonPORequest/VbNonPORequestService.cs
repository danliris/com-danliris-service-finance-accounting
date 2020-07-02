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

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.VbNonPORequest
{
    public class VbNonPORequestService : IVbNonPORequestService
    {
        private readonly FinanceDbContext _dbContext;
        private readonly IIdentityService _identityService;
        private readonly List<string> _alphabets;
        private const string UserAgent = "finance-service";
        private readonly DbSet<VbRequestModel> dbSet;

        public VbNonPORequestService(FinanceDbContext dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _identityService = serviceProvider.GetService<IIdentityService>();

            _alphabets = GetAlphabets();
            dbSet = _dbContext.Set<VbRequestModel>();
        }

        public List<string> GetAlphabets()
        {
            //Declare string container for alphabet
            var result = new List<string>();

            //Loop through the ASCII characters 65 to 90
            for (int i = 65; i <= 90; i++)
            {
                // Convert the int to a char to get the actual character behind the ASCII code
                result.Add(((char)i).ToString());
            }

            return result;
        }

        public ReadResponse<VbRequestList> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            var query = _dbContext.VbRequests.AsQueryable();

            var searchAttributes = new List<string>()
            {
                "VBNo",
                "UnitLoad",
                "CreatedBy",
                "Status_Post",
                "Apporve_Status",
                "Complete_Status"
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
                CreateBy = entity.CreatedBy,
                Status_Post = entity.Status_Post,
                Approve_Status = entity.Apporve_Status,
                Complete_Status = entity.Complete_Status,
                VBRequestCategory = entity.VBRequestCategory

            }).ToList();

            int totalData = pageable.TotalCount;

            return new ReadResponse<VbRequestList>(data, totalData, orderDictionary, new List<string>());
        }

        public Task<int> CreateAsync(VbRequestModel model, VbNonPORequestViewModel viewmodel)
        {
            model.VBNo = GetVbNonPoNo(model);

            model.VBRequestCategory = "NONPO";

            model.Status_Post = "Belum";
            model.Apporve_Status = "Not Approve";
            model.Complete_Status = "Not Complete";

            model.UnitLoad = GetUnitLoad(viewmodel);

            EntityExtension.FlagForCreate(model, _identityService.Username, UserAgent);

            _dbContext.VbRequests.Add(model);

            return _dbContext.SaveChangesAsync();
        }
        public Task<int> MappingData(VbNonPORequestViewModel viewModel)
        {
            var result = new List<VbRequestDetailModel>();
            var myProperty = viewModel.GetType().GetProperties();

            int value = int.Parse(dbSet.OrderByDescending(p => p.Id)
                            .Select(r => r.Id)
                            .First().ToString());

            foreach (var prop in myProperty.Where(x => x.PropertyType == typeof(bool)))
            {
                if ((bool)prop.GetValue(viewModel))
                {
                    var item = new VbRequestDetailModel() { VBId = value, UnitName = prop.Name };
                    if (prop.Name.ToUpper() == "OTHERS")
                        item.DetailOthers = viewModel.DetailOthers;

                    EntityExtension.FlagForCreate(item, _identityService.Username, UserAgent);
                    _dbContext.VbRequestsDetails.Add(item);
                    //_dbContext.SaveChangesAsync();

                }
            }

            return _dbContext.SaveChangesAsync();

        }

        public VbRequestModel MappingData2(VbNonPORequestViewModel viewModel)
        {
            var listDetail = new List<VbRequestDetailModel>();
            
            var myProperty = viewModel.GetType().GetProperties();

            int value = int.Parse(dbSet.OrderByDescending(p => p.Id)
                            .Select(r => r.Id)
                            .First().ToString());

            foreach (var prop in myProperty.Where(x => x.PropertyType == typeof(bool)))
            {
                if ((bool)prop.GetValue(viewModel))
                {
                    var item = new VbRequestDetailModel() { VBId = value, UnitName = prop.Name,
                        LastModifiedBy = viewModel.LastModifiedBy,
                        LastModifiedAgent = viewModel.LastModifiedAgent,
                        DeletedBy = "",
                        DeletedAgent = "",
                        CreatedBy = viewModel.CreatedBy,
                        CreatedAgent = viewModel.CreatedAgent,
                    };
                    if (prop.Name.ToUpper() == "OTHERS")
                        item.DetailOthers = viewModel.DetailOthers;
                    listDetail.Add(item);
                    //EntityExtension.FlagForCreate(item, _identityService.Username, UserAgent);
                    //_dbContext.VbRequestsDetails.Add(item);
                    //_dbContext.SaveChangesAsync();

                }
            }
            var result = new VbRequestModel() {
                VbRequestDetail = listDetail,
                Active = viewModel.Active,
                Id = viewModel.Id,
                Date = (DateTimeOffset)viewModel.Date,
                VBCode = viewModel.VBCode,
                VBNo = viewModel.VBNo,
                CurrencyId  = viewModel.Currency.Id,
                CurrencyCode = viewModel.Currency.Code,
                CurrencyRate = viewModel.Currency.Rate,
                CurrencySymbol = viewModel.Currency.Symbol,
                Amount = viewModel.Amount,
                Usage = viewModel.Usage,
                UnitLoad = viewModel.UnitLoad,
                CreatedBy = viewModel.CreatedBy,
                CreatedAgent = viewModel.CreatedAgent,
                LastModifiedAgent = viewModel.LastModifiedAgent,
                LastModifiedBy = viewModel.LastModifiedBy
            };

            return result;

        }

        private string GetUnitLoad(VbNonPORequestViewModel viewmodel)
        {
            var UnitLoad = "";
            if (viewmodel.Spinning1 == true)
            {
                UnitLoad += "Spinning 1, ";
            }

            if (viewmodel.Spinning2 == true)
            {
                UnitLoad += "Spinning 2, ";
            }

            if (viewmodel.Spinning3 == true)
            {
                UnitLoad += "Spinning 3, ";
            }

            if (viewmodel.Weaving1 == true)
            {
                UnitLoad += "Weaving 1, ";
            }

            if (viewmodel.Weaving2 == true)
            {
                UnitLoad += "Weaving 2, ";
            }

            if (viewmodel.Finishing == true)
            {
                UnitLoad += "Finishing, ";
            }

            if (viewmodel.Printing == true)
            {
                UnitLoad += "Printing, ";
            }

            if (viewmodel.Konfeksi1A == true)
            {
                UnitLoad += "Konfeksi 1A, ";
            }

            if (viewmodel.Konfeksi1B == true)
            {
                UnitLoad += "Konfeksi 1B, ";
            }

            if (viewmodel.Konfeksi2A == true)
            {
                UnitLoad += "Konfeksi 2A, ";
            }

            if (viewmodel.Konfeksi2B == true)
            {
                UnitLoad += "Konfeksi 2B, ";
            }

            if (viewmodel.Konfeksi2C == true)
            {
                UnitLoad += "Konfeksi 2C, ";
            }

            if (viewmodel.Umum == true)
            {
                UnitLoad += "Umum, ";
            }

            if (viewmodel.Others == true)
            {
                UnitLoad += $"{viewmodel.DetailOthers}, ";
            }

            UnitLoad = UnitLoad.Remove(UnitLoad.Length - 2);

            return UnitLoad;
        }

        private string GetVbNonPoNo(VbRequestModel model)
        {
            var now = DateTime.Now;
            var year = now.ToString("yy");
            var month = now.ToString("MM");


            var unit = model.VBCode.ToString().Split(" - ");


            var documentNo = $"VB{unit[0]}{month}{year}";

            var countSameDocumentNo =  _dbContext.VbRequests.Count(entity => entity.VBCode.Contains(model.VBCode));

            if (countSameDocumentNo >= 0)
            {
                countSameDocumentNo += 1;

                documentNo += string.Format("{0:000}", countSameDocumentNo);
            }

            return documentNo;
        }

        public Task<VbRequestModel> ReadByIdAsync(int id)
        {
            return _dbContext.VbRequests.Where(entity => entity.Id == id).FirstOrDefaultAsync();
            //return _dbContext.VbRequests.Include(entity => entity.VbRequestDetail).Where(entity => entity.Id == id).FirstOrDefaultAsync();
        }

        public Task<VbNonPORequestViewModel> ReadByIdAsync2(int id)
        {
            //return _dbContext.VbRequests.Where(entity => entity.Id == id).FirstOrDefaultAsync();
            return _dbContext.VbRequests.Include(entity => entity.VbRequestDetail).Where(entity => entity.Id == id)
                .Select(s =>
                   new VbNonPORequestViewModel
                   {
                       Id = s.Id,
                       CreatedAgent = s.CreatedAgent,
                       CreatedBy = s.CreatedBy,
                       LastModifiedAgent = s.LastModifiedAgent,
                       LastModifiedBy = s.LastModifiedBy,
                       UnitLoad = s.UnitLoad,
                       VBNo = s.VBNo,
                       Date = s.Date,
                       VBCode = s.VBCode,
                       Currency = new CurrencyVBRequest()
                       {
                           Id = s.CurrencyId,
                           Code = s.CurrencyCode,
                           Rate = s.CurrencyRate,
                           Symbol = s.CurrencySymbol
                       },
                       Amount = s.Amount,
                       Usage = s.Usage,
                       Active = s.Active,
                       Spinning1 = s.VbRequestDetail.Where(t => t.UnitName.ToUpper() == "SPINNING1").Count() >= 1,
                       Spinning2 = s.VbRequestDetail.Where(t => t.UnitName.ToUpper() == "SPINNING2").Count() >= 1,
                       Spinning3 = s.VbRequestDetail.Where(t => t.UnitName.ToUpper() == "SPINNING3").Count() >= 1,
                       Weaving1 = s.VbRequestDetail.Where(t => t.UnitName.ToUpper() == "WEAVING1").Count() >= 1,
                       Weaving2 = s.VbRequestDetail.Where(t => t.UnitName.ToUpper() == "WEAVING2").Count() >= 1,
                       Finishing = s.VbRequestDetail.Where(t => t.UnitName.ToUpper() == "FINISHING").Count() >= 1,
                       Printing = s.VbRequestDetail.Where(t => t.UnitName.ToUpper() == "PRINTING").Count() >= 1,
                       Konfeksi1A = s.VbRequestDetail.Where(t => t.UnitName.ToUpper() == "KONFEKSI1A").Count() >= 1,
                       Konfeksi1B = s.VbRequestDetail.Where(t => t.UnitName.ToUpper() == "KONFEKSI1B").Count() >= 1,
                       Konfeksi2A = s.VbRequestDetail.Where(t => t.UnitName.ToUpper() == "KONFEKSI2A").Count() >= 1,
                       Konfeksi2B = s.VbRequestDetail.Where(t => t.UnitName.ToUpper() == "KONFEKSI2B").Count() >= 1,
                       Konfeksi2C = s.VbRequestDetail.Where(t => t.UnitName.ToUpper() == "KONFEKSI2C").Count() >= 1,
                       Umum = s.VbRequestDetail.Where(t => t.UnitName.ToUpper() == "UMUM").Count() >= 1,
                       Others = s.VbRequestDetail.Where(t => t.UnitName.ToUpper() == "OTHERS").Count() >= 1,
                       DetailOthers = s.VbRequestDetail.FirstOrDefault(t => t.UnitName.ToUpper() == "OTHERS") == null ?
                       string.Empty: s.VbRequestDetail.FirstOrDefault(t => t.UnitName.ToUpper() == "OTHERS").DetailOthers
                   }
                )
                .FirstOrDefaultAsync();
        }

        //public Task<int> UpdateAsync(int id, VbRequestModel model)
        //{
        //    model.VBRequestCategory = "NONPO";
        //    //model.UnitLoad = GetUnitLoad(model);

        //    model.Status_Post = "Belum";
        //    model.Apporve_Status = "Not Approve";
        //    model.Complete_Status = "Not Complete";

        //    EntityExtension.FlagForUpdate(model, _identityService.Username, UserAgent);

        //    _dbContext.VbRequests.Update(model);

        //    return _dbContext.SaveChangesAsync();
        //}
        public Task<int> UpdateAsync(int id, VbNonPORequestViewModel viewModel)
        {
            var model = MappingData2(viewModel);
            model.VBRequestCategory = "NONPO";
            model.UnitLoad = GetUnitLoad(viewModel);

            model.Status_Post = "Belum";
            model.Apporve_Status = "Not Approve";
            model.Complete_Status = "Not Complete";

            EntityExtension.FlagForUpdate(model, _identityService.Username, UserAgent);

            _dbContext.VbRequests.Update(model);

            return _dbContext.SaveChangesAsync();
        }

        public Task<int> UpdateStatusAsync(List<VbRequestModel> ListVbRequestViewModel, string user)
        {

            int Updated = 0;
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var Ids = ListVbRequestViewModel.Select(d => d.Id).ToList();
                    var listData = dbSet
                        .Where(m => Ids.Contains(m.Id) && !m.IsDeleted)
                        .ToList();
                    listData.ForEach(m =>
                    {
                        EntityExtension.FlagForUpdate(m, user, UserAgent);
                        m.Status_Post = "Sudah";

                        //foreach (var item in m.Items)
                        //{
                        //    EntityExtension.FlagForUpdate(item, user, USER_AGENT);
                        //}
                    });

                    Updated = _dbContext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

                return _dbContext.SaveChangesAsync();
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
    }
}
