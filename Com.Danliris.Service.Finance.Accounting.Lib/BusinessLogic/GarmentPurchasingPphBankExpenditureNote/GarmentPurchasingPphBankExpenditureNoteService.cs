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
using Com.Danliris.Service.Finance.Accounting.Lib.Models.DailyBankTransaction;

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
            model.PphBankInvoiceNo = await _serviceDailyBankTransaction.GetDocumentNo("K", model.Bank.BankCode, username);
            //model.PphBankInvoiceNo = "test";

            var mapper = _mapper.Map<GarmentPurchasingPphBankExpenditureNoteModel>(model);

            EntityExtension.FlagForCreate(mapper, username, UserAgent);
            foreach (var item in mapper.Items)
            {
                EntityExtension.FlagForCreate(item, username, UserAgent);
                foreach (var invoice in item.GarmentPurchasingPphBankExpenditureNoteInvoices)
                {
                    EntityExtension.FlagForCreate(invoice, username, UserAgent);
                }
            }
            _dbContext.GarmentPurchasingPphBankExpenditureNotes.Add(mapper);
            await _dbContext.SaveChangesAsync();


        }

        public async Task DeleteAsync(int id)
        {
            var existingModel = _dbContext.GarmentPurchasingPphBankExpenditureNotes
                        .Include(d => d.Items)
                        .ThenInclude(d => d.GarmentPurchasingPphBankExpenditureNoteInvoices)
                        .Single(x => x.Id == id && !x.IsDeleted);
            var model = await ReadByIdAsync(id);
            foreach (var item in existingModel.Items)
            {
                EntityExtension.FlagForDelete(item, _identityService.Username, UserAgent, true);
            }
            EntityExtension.FlagForDelete(existingModel, _identityService.Username, UserAgent, true);
            _dbContext.GarmentPurchasingPphBankExpenditureNotes.Update(existingModel);

            await _dbContext.SaveChangesAsync();
        }

        public async Task PostingDocument(int id)
        {
            var existingModel = _dbContext.GarmentPurchasingPphBankExpenditureNotes
                        .Include(d => d.Items)
                        .Single(x => x.Id == id && !x.IsDeleted);

            existingModel.IsPosted = true;


            foreach (var item in existingModel.Items)
            {
                EntityExtension.FlagForDelete(item, _identityService.Username, UserAgent, true);
            }
            EntityExtension.FlagForDelete(existingModel, _identityService.Username, UserAgent, true);
            _dbContext.GarmentPurchasingPphBankExpenditureNotes.Update(existingModel);

            var model = await ReadByIdAsync(id);
            // insert DailyBankTransaction
            var dailyBankMap = _mapper.Map<DailyBankTransactionModel>(model);
            var insertDailyBank = await _serviceDailyBankTransaction.CreateAsync(dailyBankMap);

            await _dbContext.SaveChangesAsync();
        }

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
            //var query = _dbContext.GarmentPurchasingPphBankExpenditureNotes.Include(s=> s.Items).ThenInclude(s=> s.GarmentPurchasingPphBankExpenditureNoteInvoices).AsQueryable();
            var query = _dbContext.GarmentPurchasingPphBankExpenditureNoteItems.Include(s => s.GarmentPurchasingPphBankExpenditureNote).Include(s=>s.GarmentPurchasingPphBankExpenditureNoteInvoices).AsQueryable();


            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(entity => entity.GarmentPurchasingPphBankExpenditureNote.InvoiceOutNumber.Contains(keyword) || entity.GarmentPurchasingPphBankExpenditureNote.BankName.Contains(keyword) || entity.IncomeTaxName.Contains(keyword) || entity.GarmentPurchasingPphBankExpenditureNote.BankCurrencyCode.Contains(keyword) || keyword.Contains(entity.InternalNotesNo));

            var orderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            query = QueryHelper<GarmentPurchasingPphBankExpenditureNoteItemModel>.Order(query, orderDictionary);

            var count = query.Count();

            var data = query
                .Skip((page - 1) * size)
                .Take(size)
                .Select(entity => new GarmentPurchasingPphBankExpenditureNoteDataViewModel(entity))
                .ToList();

            return new ReadResponse<GarmentPurchasingPphBankExpenditureNoteDataViewModel>(data, count, orderDictionary, new List<string>());
        }

        public async Task<FormInsert> ReadByIdAsync(int id)
        {
            return await _dbContext.GarmentPurchasingPphBankExpenditureNotes
                .Include(element => element.Items)
                .ThenInclude(invoice => invoice.GarmentPurchasingPphBankExpenditureNoteInvoices)
                .Select(s => new FormInsert
                {
                    Id = s.Id,
                    Bank = new Bank
                    {
                        AccountCOA = s.AccountBankCOA,
                        AccountName = s.AccountBankName,
                        AccountNumber = s.AccountBankNumber,
                        BankAddress = s.BankAddress,
                        BankCode = s.BankCode,
                        BankName = s.BankName,
                        Code = s.BankCode1,
                        Currency = new Currency
                        {
                            Code = s.BankCurrencyCode,
                            Id = s.BankCurrencyId
                        },
                        SwiftCode = s.BankSwiftCode,
                    },
                    Date = s.InvoiceOutDate,
                    DateFrom = s.DueDateStart,
                    DateTo = s.DueDateEnd,
                    PphBankInvoiceNo = s.InvoiceOutNumber,
                    IncomeTax = new ViewModels.GarmentPurchasingPphBankExpenditureNoteViewModels.IncomeTax
                    {
                        Name = s.IncomeTaxName,
                        Rate = Convert.ToDecimal(s.IncomeTaxRate),
                        Id = s.IncomeTaxId
                    },
                    PPHBankExpenditureNoteItems = s.Items.Select(item => new FormAdd
                    {
                        CurrencyCode = item.CurrencyCode,
                        CurrencyId = item.CurrencyId,
                        SupplierId = item.SupplierId,
                        SupplierName = item.SupplierName,
                        INDate = item.Date,
                        INDueDate = item.DueDate,
                        INNo = item.InternalNotesNo,
                        INId = item.InternalNotesId,
                        Items =
                        new List<Item>()
                        {
                            new Item
                            {
                                TotalAmount = item.TotalPaid,
                                Details = item.GarmentPurchasingPphBankExpenditureNoteInvoices.Select(detail => new Detail {
                                            InvoiceDate = detail.InvoicesDate,
                                            InvoiceId = Convert.ToInt32(detail.InvoicesId),
                                            InvoiceNo = detail.InvoicesNo,
                                            InvoiceTotalAmount = Convert.ToDouble(detail.Total),
                                            ProductCode = detail.ProductCode,
                                            ProductId = Convert.ToInt32(detail.ProductId),
                                            ProductCategory = detail.ProductCategory,
                                            ProductName = detail.ProductName,
                                            PriceTotal = detail.Total,
                                            UnitCode = detail.UnitCode,
                                            UnitId = detail.UnitId,
                                            UnitName = detail.UnitName
                                            }).ToList(),
                                TotalIncomeTax = item.TotalPaid *(item.IncomeTaxTotal/100)
                            }
                        }
                    }).ToList()
                })
                .FirstOrDefaultAsync(entity => entity.Id == id);
        }

        

        public ReadResponse<GarmentPurchasingPphBankExpenditureNoteReportViewDto> GetReportView(int page, int size, string order, GarmentPurchasingPphBankExpenditureNoteFilterReportDto filter)
        {
            var query = _dbContext.GarmentPurchasingPphBankExpenditureNoteItems.Include(s => s.GarmentPurchasingPphBankExpenditureNote).Include(s=> s.GarmentPurchasingPphBankExpenditureNoteInvoices).AsQueryable();

            if (!string.IsNullOrEmpty(filter.InvoiceOutNo))
            {
                query = query.Where(entity => entity.GarmentPurchasingPphBankExpenditureNote.InvoiceOutNumber.Contains(filter.InvoiceOutNo));
            }
            
            if(!string.IsNullOrEmpty(filter.InvoiceNo))
            {
                query = query.Where(entity => entity.GarmentPurchasingPphBankExpenditureNoteInvoices.Any(t => t.InvoicesNo.Contains(filter.InvoiceNo)));
            }

            if (!string.IsNullOrEmpty(filter.INNo))
            {
                query = query.Where(entity => entity.InternalNotesNo.Contains(filter.INNo));
            }

            if (!string.IsNullOrEmpty(filter.SupplierName))
            {
                query = query.Where(entity => entity.SupplierName.Contains(filter.SupplierName));
            }

            if(filter.DateStart.HasValue)
            {
                query = query.Where(entity => entity.Date >= filter.DateStart);
            }

            if(filter.DateEnd.HasValue)
            {
                query = query.Where(entity => entity.Date <= filter.DateEnd);
            }

            //var orderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            //query = QueryHelper<GarmentPurchasingPphBankExpenditureNoteItemModel>.Order(query, orderDictionary);

            var count = query.Count();

            var data = query.SelectMany(s=> s.GarmentPurchasingPphBankExpenditureNoteInvoices)
                .Skip((page - 1) * size)
                .Take(size)
                .ToList()
                                .GroupBy(
                invoice => new { invoice.InvoicesId, invoice.InvoicesNo, invoice.InvoicesDate },
                groupInvoice => groupInvoice,
                (key, grp) => new GarmentPurchasingPphBankExpenditureNoteReportViewDto
                {
                    BankName = grp.FirstOrDefault().GarmentPurchasingPphBankExpenditureNoteItem.GarmentPurchasingPphBankExpenditureNote.BankName,
                    Category = grp.FirstOrDefault().ProductCategory,
                    INNO = grp.FirstOrDefault().GarmentPurchasingPphBankExpenditureNoteItem.InternalNotesNo,
                    InvoiceNo = key.InvoicesNo,
                    CurrencyCode = grp.FirstOrDefault().GarmentPurchasingPphBankExpenditureNoteItem.CurrencyCode,
                    InvoiceOutNo = grp.FirstOrDefault().GarmentPurchasingPphBankExpenditureNoteItem.GarmentPurchasingPphBankExpenditureNote.InvoiceOutNumber,
                    PaidDate = grp.FirstOrDefault().GarmentPurchasingPphBankExpenditureNoteItem.GarmentPurchasingPphBankExpenditureNote.InvoiceOutDate,
                    SupplierName = grp.FirstOrDefault().GarmentPurchasingPphBankExpenditureNoteItem.SupplierName,
                    PPH = grp.FirstOrDefault().GarmentPurchasingPphBankExpenditureNoteItem.TotalPaid * (grp.FirstOrDefault().GarmentPurchasingPphBankExpenditureNoteItem.IncomeTaxTotal / 100)
                }).ToList()
                ;

            return new ReadResponse<GarmentPurchasingPphBankExpenditureNoteReportViewDto>(data, count, new Dictionary<string, string>(), new List<string>());
        }

        public List<GarmentPurchasingPphBankExpenditureNoteLoaderInternNote> GetLoaderInterNotePPH(string keyword)
        {
            var query = _dbContext.GarmentPurchasingPphBankExpenditureNoteItems.Where(s=> s.InternalNotesNo.Contains(keyword)).Select(s => new GarmentPurchasingPphBankExpenditureNoteLoaderInternNote
            {
                Id = s.InternalNotesId,
                Name = s.InternalNotesNo,
            })
                .Skip((1 - 1) * 10)
                .Take(10);
            return query.ToList();
        }

        public List<GarmentPurchasingPphBankExpenditureLoaderSupplierDto> GetLoaderSupplier(string keyword)
        {
            var query = _dbContext.GarmentPurchasingPphBankExpenditureNoteItems.Where(s => s.SupplierName.Contains(keyword)).Select(s => new GarmentPurchasingPphBankExpenditureLoaderSupplierDto
            {
                Id = s.SupplierId,
                Name = s.SupplierName,
            })
                .Skip((1 - 1) * 10)
                .Take(10);
            return query.ToList();
        }

        public List<GarmentPurchasingPphBankExpenditureNoteLoaderInvoiceDto> GetLoaderInvoice(string keyword)
        {
            var query = _dbContext.GarmentPurchasingPphBankExpenditureNoteInvoices.Where(s => s.InvoicesNo.Contains(keyword)).Select(s => new GarmentPurchasingPphBankExpenditureNoteLoaderInvoiceDto
            {
                Id = s.InvoicesId,
                Name = s.InvoicesNo,
            })
                .Skip((1 - 1) * 10)
                .Take(10);
            return query.ToList();
        }
        public List<GarmentPurchasingPphBankExpenditureNoteLoaderInvoiceOutDto> GetLoaderInvoiceOut(string keyword)
        {
            var query = _dbContext.GarmentPurchasingPphBankExpenditureNotes.Where(s => s.InvoiceOutNumber.Contains(keyword)).Select(s => new GarmentPurchasingPphBankExpenditureNoteLoaderInvoiceOutDto
            {
                Id = s.Id,
                Name = s.InvoiceOutNumber,
            })
                .Skip((1 - 1) * 10)
                .Take(10);
            return query.ToList();
        }
    }
}
