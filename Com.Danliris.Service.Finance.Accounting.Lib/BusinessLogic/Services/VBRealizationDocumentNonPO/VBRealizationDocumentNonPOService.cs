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

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.VBRealizationDocumentNonPO
{
    public class VBRealizationDocumentNonPOService : IVBRealizationDocumentNonPOService
    {
        private const string _UserAgent = "finance-service";
        protected DbSet<VBRealizationDocumentModel> _dbSet;
        protected IIdentityService _IdentityService;
        public FinanceDbContext _DbContext;

        public VBRealizationDocumentNonPOService(IServiceProvider serviceProvider, FinanceDbContext dbContext)
        {
            _DbContext = dbContext;
            _dbSet = dbContext.Set<VBRealizationDocumentModel>();
            _IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        public Task<int> CreateAsync(VBRealizationDocumentModel model)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public ReadResponse<VBRealizationDocumentModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            throw new NotImplementedException();
        }

        public Task<VBRealizationDocumentModel> ReadByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(int id, VBRealizationDocumentModel model)
        {
            throw new NotImplementedException();
        }
    }
}
