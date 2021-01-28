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
using System.Threading.Tasks;
using Com.Moonlay.Models;
using Microsoft.EntityFrameworkCore;

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

        public Task CreateAsync(GarmentPurchasingPphBankExpenditureNoteDataViewModel model)
        {
            model.No = await GenerateNo(model, 7);
            EntityExtension.FlagForCreate(model, IdentityService.Username, UserAgent);
            foreach (var item in model.Items)
            {
                EntityExtension.FlagForCreate(item, IdentityService.Username, UserAgent);
            }
            DbSet.Add(model);
            return await DbContext.SaveChangesAsync();
        }

        public Task DeleteAsync(int id)
        {
            var existingModel = DbSet
                        .Include(d => d.Items)
                        .Single(x => x.Id == id && !x.IsDeleted);
            GarmentInvoicePaymentModel model = await ReadByIdAsync(id);
            foreach (var item in model.Items)
            {
                EntityExtension.FlagForDelete(item, IdentityService.Username, UserAgent, true);
            }
            EntityExtension.FlagForDelete(model, IdentityService.Username, UserAgent, true);
            DbSet.Update(model);

            return await DbContext.SaveChangesAsync();
        }

        public Task PostingDocument(int id)
        {
            var existingModel = DbSet
                        .Include(d => d.Items)
                        .Single(x => x.Id == id && !x.IsDeleted);
            GarmentInvoicePaymentModel model = await ReadByIdAsync(id);
            foreach (var item in model.Items)
            {
                EntityExtension.FlagForDelete(item, IdentityService.Username, UserAgent, true);
            }
            EntityExtension.FlagForDelete(model, IdentityService.Username, UserAgent, true);
            DbSet.Update(model);

            return await DbContext.SaveChangesAsync();
        }

        public List<GarmentPurchasingPphBankExpenditureNoteDataViewModel> PrintInvoice(int id)
        {
            var existingModel = DbSet
                        .Include(d => d.Items)
                        .Single(x => x.Id == id && !x.IsDeleted);
            GarmentInvoicePaymentModel model = await ReadByIdAsync(id);
            foreach (var item in model.Items)
            {
                EntityExtension.FlagForDelete(item, IdentityService.Username, UserAgent, true);
            }
            EntityExtension.FlagForDelete(model, IdentityService.Username, UserAgent, true);
            DbSet.Update(model);

            return await DbContext.SaveChangesAsync();
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
                .Select(entity => new GarmentPurchasingPphBankExpenditureNoteDataViewModel(entity))
                .ToList();

            return new ReadResponse<GarmentPurchasingPphBankExpenditureNoteDataViewModel>(data, count, orderDictionary, new List<string>());
        }

        public Task<GarmentPurchasingPphBankExpenditureNoteDataViewModel> ReadByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
