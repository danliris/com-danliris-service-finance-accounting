using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentDebtBalance
{
    public class GarmentDebtBalanceService : IGarmentDebtBalanceService
    {
        private const string UserAgent = "finance-service";
        private readonly FinanceDbContext _dbContext;
        private readonly IIdentityService _identityService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapper _mapper;

        public GarmentDebtBalanceService(IServiceProvider serviceProvider)
        {
            _dbContext = serviceProvider.GetService<FinanceDbContext>();
            _identityService = serviceProvider.GetService<IIdentityService>();
            _serviceProvider = serviceProvider;
            _mapper = serviceProvider.GetService<IMapper>();
        }

        public List<GarmentDebtBalanceCardDto> GetDebtBalanceCardDto(int supplierId, int month, int year)
        {
            var garmentDebtBalance = GetData(supplierId, month, year);

            //auto map
            List<GarmentDebtBalanceCardDto> garmentDebtDto = Mapper.Map<List<GarmentDebtBalanceCardDto>>(garmentDebtBalance);

            return garmentDebtDto;
        }

        public GarmentDebtBalanceIndexDto GetDebtBalanceCardIndex(int supplierId, int month , int year)
        {
            var query = GetDebtBalanceCardDto(supplierId, month, year);
            var result = new GarmentDebtBalanceIndexDto
            {
                Data = query,
                Count = query.Count,
                Order = new List<string>(),
                Selected = new List<string>()
            };
            return result;
        }

        private IQueryable<Models.GarmentDebtBalance.GarmentDebtBalanceModel> GetData(int supplierId, int month,int year)
        {
            var query = _dbContext.GarmentDebtBalances.AsQueryable();
            if (supplierId > 0)
                query = query.Where(s => s.SupplierId == supplierId);

            if (month > 0)
                query = query.Where(s => s.InvoiceDate.Month == month);

            if (year > 1)
                query = query.Where(s => s.InvoiceDate.Year == year);
            return query;
        }
    }
}
