using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.GarmentPurchasingPphBankExpenditureNoteViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Newtonsoft.Json;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentPurchasingPphBankExpenditureNote;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentPurchasingPphBankExpenditureNote
{
    public class GarmentPurchasingPphBankExpenditureNoteService : IGarmentPurchasingPphBankExpenditureNoteService
    {
        private const string UserAgent = "finance-service";
        private readonly FinanceDbContext _dbContext;
        private readonly IIdentityService _identityService;
        private readonly IServiceProvider _serviceProvider;

        public GarmentPurchasingPphBankExpenditureNoteService(IServiceProvider serviceProvider)
        {
            _dbContext = serviceProvider.GetService<FinanceDbContext>();
            _identityService = serviceProvider.GetService<IIdentityService>();
            _serviceProvider = serviceProvider;
        }

        public ReadResponse<GarmentPurchasingPphBankExpenditureNoteDataViewModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            var query = _dbContext.GarmentPurchasingPphBankExpenditureNotes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(entity => entity.InvoiceOutNumber.Contains(keyword) || entity.BankName.Contains(keyword) || entity.IncomeTaxName.Contains(keyword) || entity.BankCurrencyCode.Contains(keyword) || entity.Items.Select(s=> s.InternalNotesNo).Contains(keyword));

            var orderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            query = QueryHelper<GarmentPurchasingPphBankExpenditureNoteModel>.Order(query, orderDictionary);

            var count = query.Count();

            var data = query
                .Skip((page - 1) * size)
                .Take(size)
                .Select(entity => new IndexDto(entity))
                .ToList();

            return new ReadResponse<IndexDto>(data, count, orderDictionary, new List<string>());
        }
    }
}
