using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentDebtBalance;
using Com.Moonlay.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
//using AutoMapper.Extensions.Microsoft.DependencyInjection;
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
            var garmentDebtBalance = GetData(supplierId, month, year).ToList();

            //auto map
            //List<GarmentDebtBalanceCardDto> garmentDebtDto = Mapper.Map<List<Models.GarmentDebtBalance.GarmentDebtBalanceModel>,List<GarmentDebtBalanceCardDto>>(garmentDebtBalance);
            //List<GarmentDebtBalanceCardDto> garmentDebtDto = garmentDebtBalance.Select(s=> Mapper.Map<GarmentDebtBalanceCardDto>(s)).ToList();
            List<GarmentDebtBalanceCardDto> garmentDebtDto = garmentDebtBalance.Select(s => new GarmentDebtBalanceCardDto(s)).ToList();

            return garmentDebtDto;
        }

        public List<GarmentDebtBalanceCardDto> GetDebtBalanceCardWithBeforeBalanceAndTotalDto(int supplierId, int month, int year)
        {
            var garmentDebtBalance = GetData(supplierId, month, year).ToList();

            //get last balance
            var queryGetAllSaldoBefore = _dbContext.GarmentDebtBalances.Where(s => s.InvoiceDate.Month < month && s.InvoiceDate.Year < year).OrderByDescending(s => s.InvoiceDate).Select(s => new GarmentDebtBalance.GarmentDebtBalanceCardDto(s)).AsQueryable();
            var lastMonthAndYearSaldo = queryGetAllSaldoBefore.Select(s=> s.InvoiceDate).FirstOrDefault();
            //filter by last saldo year and month
            var querySaldoBefore = queryGetAllSaldoBefore.Where(s => s.InvoiceDate.Month < month && s.InvoiceDate.Year < year).OrderByDescending(s=> s.InvoiceDate).ToList(); 

            List<GarmentDebtBalanceCardDto> garmentDebtDto = garmentDebtBalance.Select(s => new GarmentDebtBalanceCardDto(s)).ToList();

            //insert
            var getLastQueryBefore = querySaldoBefore.FirstOrDefault();
            var lastBalance = getLastQueryBefore == null ? 0 : getLastQueryBefore.RemainBalance;
            garmentDebtDto.Add(new GarmentDebtBalanceCardDto("<<saldo awal>>", lastBalance));
            garmentDebtDto.Add(new GarmentDebtBalanceCardDto("<<total>>", 0,0,0));

            return garmentDebtDto.OrderBy(d=> d.InvoiceDate).ToList();
        }

        public int CreateFromCustoms(CustomsFormDto form)
        {
            var model = new GarmentDebtBalanceModel(form.PurchasingCategoryId, form.PurchasingCategoryName, form.BillsNo, form.PaymentBills, form.GarmentDeliveryOrderId, form.GarmentDeliveryOrderNo, form.SupplierId, form.SupplierName, form.CurrencyId, form.CurrencyCode, form.CurrencyRate);
            EntityExtension.FlagForCreate(model, _identityService.Username, UserAgent);
            _dbContext.GarmentDebtBalances.Add(model);

            _dbContext.SaveChanges();

            return model.Id;
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

        public GarmentDebtBalanceIndexDto GetDebtBalanceCardWithBalanceBeforeIndex(int supplierId, int month, int year)
        {
            var query = GetDebtBalanceCardWithBeforeBalanceAndTotalDto(supplierId, month, year);
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

        public int UpdateFromBankExpenditureNote(BankExpenditureNoteFormDto form)
        {
            throw new NotImplementedException();
        }

        public int UpdateFromInternalNote(InternalNoteFormDto form)
        {
            throw new NotImplementedException();
        }

        public int UpdateFromInvoice(InvoiceFormDto form)
        {
            throw new NotImplementedException();
        }
    }
}
