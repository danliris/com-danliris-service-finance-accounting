using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.GarmentPurchasingPphBankExpenditureNoteViewModels;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.DailyBankTransaction;
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
using AutoMapper;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentPurchasingPphBankExpenditureNote
{
    public class GarmentPurchasingPphBankExpenditureNoteService : IGarmentPurchasingPphBankExpenditureNoteService
    {
        private const string UserAgent = "finance-service";
        private readonly FinanceDbContext _dbContext;
        private readonly IIdentityService _identityService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDailyBankTransactionService _serviceDailyBankTransaction;
        private readonly IMapper _mapper;

        public GarmentPurchasingPphBankExpenditureNoteService(IServiceProvider serviceProvider)
        {
            _dbContext = serviceProvider.GetService<FinanceDbContext>();
            _identityService = serviceProvider.GetService<IIdentityService>();
            _serviceProvider = serviceProvider;
            _serviceDailyBankTransaction = serviceProvider.GetService<IDailyBankTransactionService>();
            _mapper = serviceProvider.GetService<IMapper>();
        }

        public async Task CreateAsync(FormInsert model)
        {
            var typeDocument = "K";
            var username = _identityService.Username;
            model.PphBankInvoiceNo = await _serviceDailyBankTransaction.GetDocumentNo("K",model.Bank.BankCode,username);

            var mapper = _mapper.Map<GarmentPurchasingPphBankExpenditureNoteModel>(model);

            EntityExtension.FlagForCreate(mapper, username, UserAgent);
            foreach (var item in mapper.Items)
            {
                EntityExtension.FlagForCreate(item, username, UserAgent);
                foreach(var invoice in item.GarmentPurchasingPphBankExpenditureNoteInvoices)
                {
                    EntityExtension.FlagForCreate(invoice, username, UserAgent);
                }
            }
            _dbContext.GarmentPurchasingPphBankExpenditureNotes.Add(mapper);
            await _dbContext.SaveChangesAsync();
        }

        //public Task DeleteAsync(int id)
        //{
        //    var existingModel = _dbContext.GarmentPurchasingPphBankExpenditureNotes
        //                .Include(d => d.Items)
        //                .ThenInclude(d => d.GarmentPurchasingPphBankExpenditureNoteInvoices)
        //                .Single(x => x.Id == id && !x.IsDeleted);
        //    GarmentPurchasingPphBankExpenditureNoteModel model = await ReadByIdAsync(id);
        //    foreach (var item in model.Items)
        //    {
        //        EntityExtension.FlagForDelete(item, IdentityService.Username, UserAgent, true);
        //    }
        //    EntityExtension.FlagForDelete(model, IdentityService.Username, UserAgent, true);
        //    DbSet.Update(model);

        //    return await DbContext.SaveChangesAsync();
        //}

        //public Task PostingDocument(int id)
        //{
        //    var existingModel = DbSet
        //                .Include(d => d.Items)
        //                .Single(x => x.Id == id && !x.IsDeleted);
        //    GarmentInvoicePaymentModel model = await ReadByIdAsync(id);
        //    foreach (var item in model.Items)
        //    {
        //        EntityExtension.FlagForDelete(item, IdentityService.Username, UserAgent, true);
        //    }
        //    EntityExtension.FlagForDelete(model, IdentityService.Username, UserAgent, true);
        //    DbSet.Update(model);

        //    return await DbContext.SaveChangesAsync();
        //}

        //public List<GarmentPurchasingPphBankExpenditureNoteDataViewModel> PrintInvoice(int id)
        //{
        //    var existingModel = DbSet
        //                .Include(d => d.Items)
        //                .Single(x => x.Id == id && !x.IsDeleted);
        //    GarmentInvoicePaymentModel model = await ReadByIdAsync(id);
        //    foreach (var item in model.Items)
        //    {
        //        EntityExtension.FlagForDelete(item, IdentityService.Username, UserAgent, true);
        //    }
        //    EntityExtension.FlagForDelete(model, IdentityService.Username, UserAgent, true);
        //    DbSet.Update(model);

        //    return await DbContext.SaveChangesAsync();
        //}

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

        public async Task<GarmentPurchasingPphBankExpenditureNoteModel> ReadByIdAsync(int id)
        {
            return await _dbContext.GarmentPurchasingPphBankExpenditureNotes.Include(element => element.Items).ThenInclude(invoice => invoice.GarmentPurchasingPphBankExpenditureNoteInvoices).FirstOrDefaultAsync(entity => entity.Id == id);
        }
    }
}
