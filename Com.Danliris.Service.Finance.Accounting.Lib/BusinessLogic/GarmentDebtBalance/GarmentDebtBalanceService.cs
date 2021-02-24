using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentDebtBalance;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Moonlay.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentDebtBalance
{
    public class GarmentDebtBalanceService : IGarmentDebtBalanceService
    {
        private const string UserAgent = "finance-service";
        private readonly FinanceDbContext _dbContext;
        private readonly IIdentityService _identityService;

        public GarmentDebtBalanceService(IServiceProvider serviceProvider)
        {
            _dbContext = serviceProvider.GetService<FinanceDbContext>();
            _identityService = serviceProvider.GetService<IIdentityService>();
        }

        public int CreateFromCustoms(CustomsFormDto form)
        {
            var model = new GarmentDebtBalanceModel(form.PurchasingCategoryId, form.PurchasingCategoryName, form.BillsNo, form.PaymentBills, form.GarmentDeliveryOrderId, form.GarmentDeliveryOrderNo, form.SupplierId, form.SupplierName, form.CurrencyId, form.CurrencyCode, form.CurrencyRate);
            EntityExtension.FlagForCreate(model, _identityService.Username, UserAgent);
            _dbContext.GarmentDebtBalances.Add(model);

            _dbContext.SaveChanges();

            return model.Id;
        }

        public List<GarmentDebtBalanceCardDto> GetDebtBalanceCardDto(int supplierId, int month, int year)
        {
            throw new NotImplementedException();
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
