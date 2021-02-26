using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentDebtBalance;
using Com.Moonlay.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
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
            List<GarmentDebtBalanceCardDto> garmentDebtDto = Mapper.Map<List<Models.GarmentDebtBalance.GarmentDebtBalanceModel>, List<GarmentDebtBalanceCardDto>>(garmentDebtBalance);

            return garmentDebtDto;
        }

        public int CreateFromCustoms(CustomsFormDto form)
        {
            var model = new GarmentDebtBalanceModel(form.PurchasingCategoryId, form.PurchasingCategoryName, form.BillsNo, form.PaymentBills, form.GarmentDeliveryOrderId, form.GarmentDeliveryOrderNo, form.SupplierId, form.SupplierCode, form.SupplierName, form.SupplierIsImport, form.CurrencyId, form.CurrencyCode, form.CurrencyRate);
            EntityExtension.FlagForCreate(model, _identityService.Username, UserAgent);
            _dbContext.GarmentDebtBalances.Add(model);

            _dbContext.SaveChanges();

            return model.Id;
        }

        public GarmentDebtBalanceIndexDto GetDebtBalanceCardIndex(int supplierId, int month, int year)
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

        private IQueryable<Models.GarmentDebtBalance.GarmentDebtBalanceModel> GetData(int supplierId, int month, int year)
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

        public int UpdateFromBankExpenditureNote(int deliveryOrderId, BankExpenditureNoteFormDto form)
        {
            var model = _dbContext.GarmentDebtBalances.FirstOrDefault(entity => entity.GarmentDeliveryOrderId == deliveryOrderId);

            model.SetBankExpenditureNote(form.BankExpenditureNoteId, form.BankExpenditureNoteNo, form.BankExpenditureNoteInvoiceAmount);
            EntityExtension.FlagForUpdate(model, _identityService.Username, UserAgent);

            _dbContext.GarmentDebtBalances.Update(model);

            _dbContext.SaveChanges();

            return model.Id;
        }

        public int UpdateFromInternalNote(int deliveryOrderId, InternalNoteFormDto form)
        {
            var model = _dbContext.GarmentDebtBalances.FirstOrDefault(entity => entity.GarmentDeliveryOrderId == deliveryOrderId);

            model.SetInternalNote(form.InternalNoteId, form.InternalNoteNo);
            EntityExtension.FlagForUpdate(model, _identityService.Username, UserAgent);

            _dbContext.GarmentDebtBalances.Update(model);

            _dbContext.SaveChanges();

            return model.Id;
        }

        public int UpdateFromInvoice(int deliveryOrderId, InvoiceFormDto form)
        {
            var model = _dbContext.GarmentDebtBalances.FirstOrDefault(entity => entity.GarmentDeliveryOrderId == deliveryOrderId);

            model.SetInvoice(form.InvoiceId, form.InvoiceDate, form.InvoiceNo, form.DPPAmount, form.CurrencyDPPAmount, form.VATAmount, form.IncomeTaxAmount, form.IsPayVAT, form.IsPayIncomeTax);
            EntityExtension.FlagForUpdate(model, _identityService.Username, UserAgent);

            _dbContext.GarmentDebtBalances.Update(model);

            _dbContext.SaveChanges();

            return model.Id;
        }

        public List<GarmentDebtBalanceSummaryDto> GetDebtBalanceSummary(int supplierId, int month, int year, bool isForeignCurrency, bool supplierIsImport)
        {
            var query = _dbContext.GarmentDebtBalances.Where(entity => (entity.DPPAmount + entity.CurrencyDPPAmount + entity.VATAmount - entity.IncomeTaxAmount) - entity.BankExpenditureNoteInvoiceAmount != 0);

            var beginningOfMonth = new DateTimeOffset(year, month, 1, 0, 0, 0, 0, TimeSpan.Zero);

            query = query.Where(entity => entity.InvoiceDate != DateTimeOffset.MinValue && (entity.InvoiceDate.AddHours(_identityService.TimezoneOffset).Month == month && entity.InvoiceDate.AddHours(_identityService.TimezoneOffset).Year == year));

            if (supplierId > 0)
                query = query.Where(entity => entity.SupplierId == supplierId);

            if (supplierIsImport)
                query = query.Where(entity => entity.SupplierIsImport);

            if (isForeignCurrency)
                query = query.Where(entity => !entity.SupplierIsImport && entity.CurrencyCode != "IDR");

            if (!isForeignCurrency && !supplierIsImport)
                query = query.Where(entity => !entity.SupplierIsImport && entity.CurrencyCode == "IDR");

            var tempResult = query
                .GroupBy(entity => new { entity.SupplierId, entity.CurrencyId })
                .Select(entity => new
                {
                    SupplierId = entity.Key.SupplierId,
                    SupplierCode = entity.FirstOrDefault().SupplierCode,
                    SupplierName = entity.FirstOrDefault().SupplierName,
                    SupplierIsImport = entity.FirstOrDefault().SupplierIsImport,
                    CurrencyId = entity.Key.CurrencyId,
                    CurrencyCode = entity.FirstOrDefault().CurrencyCode,
                    PurchaseAmount = entity.Sum(sum => sum.DPPAmount + sum.CurrencyDPPAmount + sum.VATAmount - sum.IncomeTaxAmount),
                    PaymentAmount = entity.Sum(sum => sum.BankExpenditureNoteInvoiceAmount)
                })
                .ToList();

            var initialBalances = _dbContext
                .GarmentDebtBalances
                .Where(entity => entity.InvoiceDate < beginningOfMonth)
                .GroupBy(entity => new { entity.SupplierId, entity.CurrencyId })
                .Select(entity => new
                {
                    SupplierId = entity.Key.SupplierId,
                    SupplierCode = entity.FirstOrDefault().SupplierCode,
                    SupplierName = entity.FirstOrDefault().SupplierName,
                    CurrencyId = entity.Key.CurrencyId,
                    CurrencyCode = entity.FirstOrDefault().CurrencyCode,
                    PurchaseAmount = entity.Sum(sum => sum.DPPAmount + sum.CurrencyDPPAmount + sum.VATAmount - sum.IncomeTaxAmount),
                    PaymentAmount = entity.Sum(sum => sum.BankExpenditureNoteInvoiceAmount)
                })
                .ToList();

            var result = new List<GarmentDebtBalanceSummaryDto>();
            foreach (var item in tempResult)
            {
                var initialBalance = initialBalances.FirstOrDefault(element => element.CurrencyId == item.CurrencyId && element.SupplierId == item.SupplierId);
                var initialBalanceAmount = 0.0;

                if (initialBalance != null)
                    initialBalanceAmount = initialBalance.PaymentAmount - initialBalance.PurchaseAmount;

                var currentBalance = initialBalanceAmount + (item.PaymentAmount - item.PurchaseAmount);

                result.Add(new GarmentDebtBalanceSummaryDto(item.SupplierId, item.SupplierCode, item.SupplierName, item.SupplierIsImport, item.CurrencyId, item.CurrencyCode, initialBalanceAmount, item.PurchaseAmount, item.PaymentAmount, currentBalance));
                
            }

            return result;
        }
    }
}
