using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.MemoGarmentPurchasing;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MemoGarmentPurchasing;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.MemoGarmentPurchasing
{
    public class MemoGarmentPurchasingService : IMemoGarmentPurchasingService
    {
        private readonly FinanceDbContext _context;
        private readonly IIdentityService _identityService;
        private const string UserAgent = "finance-service";

        public MemoGarmentPurchasingService(FinanceDbContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _identityService = serviceProvider.GetService<IIdentityService>();
        }

        public async Task<int> CreateAsync(MemoGarmentPurchasingModel model)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                model.MemoNo = GetMemoNo(model);
                EntityExtension.FlagForCreate(model, _identityService.Username, UserAgent);

                foreach (var detail in model.MemoDetailGarmentPurchasings)
                    EntityExtension.FlagForCreate(detail, _identityService.Username, UserAgent);

                _context.Add(model);
                var result = await _context.SaveChangesAsync();

                transaction.Commit();
                return result;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw e;
            }
        }

        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public ReadResponse<MemoGarmentPurchasingModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            throw new NotImplementedException();
        }

        public Task<MemoGarmentPurchasingModel> ReadByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(int id, MemoGarmentPurchasingModel model)
        {
            throw new NotImplementedException();
        }

        private string GetMemoNo(MemoGarmentPurchasingModel model)
        {
            var date = DateTime.Now;
            var count = 1 + _context.MemoGarmentPurchasings.Count(x => x.CreatedUtc.Year.Equals(date.Year) && x.CreatedUtc.Month.Equals(date.Month));

            var generatedNo = $"{date.ToString("yy")}{date.ToString("MM")}.MG.{count.ToString("0000")}";

            return generatedNo;
        }
    }
}
