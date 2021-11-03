using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.PaymentDispositionNote;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.PaymentDispositionNote;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Com.Moonlay.Models;
using System.Threading.Tasks;
using System.Linq;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Newtonsoft.Json;
using Com.Moonlay.NetCore.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.PurchasingDispositionExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.PaymentDispositionNoteViewModel;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.JournalTransaction;
using System.Net.Http;
using Com.Danliris.Service.Finance.Accounting.Lib.Helpers;
using System.IO;
using System.Data;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.PaymentDispositionNote
{
    public class PaymentDispositionNoteService : IPaymentDispositionNoteService
    {
        private const string UserAgent = "finance-service";
        protected DbSet<PaymentDispositionNoteModel> DbSet;
        public IIdentityService IdentityService;
        private readonly IAutoDailyBankTransactionService _autoDailyBankTransactionService;
        public readonly IServiceProvider ServiceProvider;
        public FinanceDbContext DbContext;

        public PaymentDispositionNoteService(IServiceProvider serviceProvider, FinanceDbContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            DbSet = dbContext.Set<PaymentDispositionNoteModel>();
            IdentityService = serviceProvider.GetService<IIdentityService>();
            _autoDailyBankTransactionService = serviceProvider.GetService<IAutoDailyBankTransactionService>();
        }

        public void CreateModel(PaymentDispositionNoteModel model)
        {
            EntityExtension.FlagForCreate(model, IdentityService.Username, UserAgent);
            foreach (var item in model.Items)
            {
                PurchasingDispositionExpeditionModel expedition = DbContext.PurchasingDispositionExpeditions.FirstOrDefault(ex => ex.Id.Equals(item.PurchasingDispositionExpeditionId));
                EntityExtension.FlagForCreate(item, IdentityService.Username, UserAgent);
                expedition.IsPaid = true;
                expedition.BankExpenditureNoteNo = model.PaymentDispositionNo;
                expedition.BankExpenditureNoteDate = model.PaymentDate;
                foreach (var detail in item.Details)
                {
                    EntityExtension.FlagForCreate(detail, IdentityService.Username, UserAgent);
                }
            }
            DbSet.Add(model);
        }

        public async Task<int> CreateAsync(PaymentDispositionNoteModel model)
        {
            var timeOffset = new TimeSpan(IdentityService.TimezoneOffset, 0, 0);
            model.PaymentDispositionNo = await GetDocumentNo("K", model.BankCode, IdentityService.Username, model.PaymentDate.ToOffset(timeOffset).Date);

            if (model.BankCurrencyCode != "IDR")
            {
                var BICurrency = await GetBICurrency(model.BankCurrencyCode, model.PaymentDate);
                model.CurrencyRate = BICurrency.Rate.GetValueOrDefault();
            }

            CreateModel(model);
            //await _autoDailyBankTransactionService.AutoCreateFromPaymentDisposition(model);
            return await DbContext.SaveChangesAsync();
        }

        //private async Task<string> GetDocumentNo(string type, string bankCode, string username)
        //{
        //    var jsonSerializerSettings = new JsonSerializerSettings
        //    {
        //        MissingMemberHandling = MissingMemberHandling.Ignore
        //    };

        //    var http = ServiceProvider.GetService<IHttpClientService>();
        //    var uri = APIEndpoint.Purchasing + $"bank-expenditure-notes/bank-document-no?type={type}&bankCode={bankCode}&username={username}";
        //    var response = await http.GetAsync(uri);

        //    var result = new BaseResponse<string>();

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var responseContent = await response.Content.ReadAsStringAsync();
        //        result = JsonConvert.DeserializeObject<BaseResponse<string>>(responseContent, jsonSerializerSettings);
        //    }
        //    return result.data;
        //}
        private async Task<string> GetDocumentNo(string type, string bankCode, string username,DateTime date)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var http = ServiceProvider.GetService<IHttpClientService>();
            var uri = APIEndpoint.Purchasing + $"bank-expenditure-notes/bank-document-no-date?type={type}&bankCode={bankCode}&username={username}&date={date}";
            var response = await http.GetAsync(uri);

            var result = new BaseResponse<string>();

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<BaseResponse<string>>(responseContent, jsonSerializerSettings);
            }
            return result.data;
        }
        //private async Task<GarmentCurrency> GetGarmentCurrency(string codeCurrency)
        //{
        //    var date = DateTimeOffset.UtcNow.ToString("yyyy/MM/dd HH:mm:ss");
        //    var queryString = $"code={codeCurrency}&stringDate={date}";

        //    var http = ServiceProvider.GetService<IHttpClientService>();
        //    var response = await http.GetAsync(APIEndpoint.Core + $"master/garment-currencies/single-by-code-date?{queryString}");

        //    var responseString = await response.Content.ReadAsStringAsync();
        //    var jsonSerializationSetting = new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Ignore };

        //    var result = JsonConvert.DeserializeObject<APIDefaultResponse<GarmentCurrency>>(responseString, jsonSerializationSetting);

        //    return result.data;
        //}

        private async Task<GarmentCurrency> GetBICurrency(string codeCurrency, DateTimeOffset date)
        {
            string stringDate = date.ToString("yyyy/MM/dd HH:mm:ss");
            string queryString = $"code={codeCurrency}&stringDate={stringDate}";

            var http = ServiceProvider.GetService<IHttpClientService>();
            var response = await http.GetAsync(APIEndpoint.Core + $"master/bi-currencies/single-by-code-date?{queryString}");

            var responseString = await response.Content.ReadAsStringAsync();
            var jsonSerializationSetting = new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Ignore };

            var result = JsonConvert.DeserializeObject<APIDefaultResponse<GarmentCurrency>>(responseString, jsonSerializationSetting);

            return result.data;
        }

        //async Task<string> GenerateNo(PaymentDispositionNoteModel model, int clientTimeZoneOffset)
        //{
        //    DateTimeOffset Now = model.PaymentDate;
        //    string Year = Now.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("yy");
        //    string Month = Now.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("MM");
        //    string Day = Now.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd");
        //    //PD + 2 digit tahun + 2 digit bulan + 2 digit tgl + 3 digit urut
        //    string no = $"PD-{Year}-{Month}-{Day}-";
        //    int Padding = 3;

        //    var lastNo = await this.DbSet.Where(w => w.PaymentDispositionNo.StartsWith(no) && !w.IsDeleted).OrderByDescending(o => o.PaymentDispositionNo).FirstOrDefaultAsync();
        //    no = $"{no}";

        //    if (lastNo == null)
        //    {
        //        return no + "1".PadLeft(Padding, '0');
        //    }
        //    else
        //    {
        //        int lastNoNumber = int.Parse(lastNo.PaymentDispositionNo.Replace(no, "")) + 1;
        //        return no + lastNoNumber.ToString().PadLeft(Padding, '0');
        //    }
        //}

        public async Task DeleteModel(int id)
        {
            PaymentDispositionNoteModel model = await ReadByIdAsync(id);
            foreach (var item in model.Items)
            {
                PurchasingDispositionExpeditionModel expedition = DbContext.PurchasingDispositionExpeditions.FirstOrDefault(ex => ex.Id.Equals(item.PurchasingDispositionExpeditionId));
                EntityExtension.FlagForDelete(item, IdentityService.Username, UserAgent, true);
                expedition.IsPaid = false;
                expedition.BankExpenditureNoteNo = null;
                expedition.BankExpenditureNoteDate = DateTimeOffset.MinValue;
                foreach (var detail in item.Details)
                {
                    EntityExtension.FlagForDelete(detail, IdentityService.Username, UserAgent, true);
                }
            }
            EntityExtension.FlagForDelete(model, IdentityService.Username, UserAgent, true);
            DbSet.Update(model);
        }

        public async Task<int> DeleteAsync(int id)
        {
            var existingModel = DbSet
                        .Include(d => d.Items)
                        .ThenInclude(d => d.Details)
                        .Single(dispo => dispo.Id == id && !dispo.IsDeleted);

            //await _autoDailyBankTransactionService.AutoCreateFromPaymentDisposition(existingModel);
            await DeleteModel(id);
            return await DbContext.SaveChangesAsync();
        }

        public Task<PaymentDispositionNoteModel> ReadByIdAsync(int id)
        {
            return DbSet.Include(m => m.Items).ThenInclude(m => m.Details).FirstOrDefaultAsync(d => d.Id.Equals(id) && d.IsDeleted.Equals(false));
        }

        public async Task<int> UpdateAsync(int id, PaymentDispositionNoteModel model)
        {
            var existingModel = DbSet
                        .Include(d => d.Items)
                        .ThenInclude(d => d.Details)
                        .Single(dispo => dispo.Id == id && !dispo.IsDeleted);
            UpdateModel(id, model);
            //await _autoDailyBankTransactionService.AutoRevertFromPaymentDisposition(existingModel);
            //await _autoDailyBankTransactionService.AutoCreateFromPaymentDisposition(model);
            return await DbContext.SaveChangesAsync();
        }

        public void UpdateModel(int id, PaymentDispositionNoteModel model)
        {
            PaymentDispositionNoteModel exist = DbSet
                        .Include(d => d.Items)
                        .ThenInclude(d => d.Details)
                        .Single(dispo => dispo.Id == id && !dispo.IsDeleted);


            exist.BGCheckNumber = model.BGCheckNumber;
            exist.PaymentDate = model.PaymentDate;

            foreach (var item in exist.Items)
            {
                PaymentDispositionNoteItemModel itemModel = model.Items.FirstOrDefault(prop => prop.Id.Equals(item.Id));

                if (itemModel == null)
                {
                    PurchasingDispositionExpeditionModel expedition = DbContext.PurchasingDispositionExpeditions.FirstOrDefault(ex => ex.Id.Equals(item.PurchasingDispositionExpeditionId));
                    expedition.IsPaid = false;
                    expedition.BankExpenditureNoteNo = null;
                    expedition.BankExpenditureNoteDate = DateTimeOffset.MinValue;

                    EntityExtension.FlagForDelete(item, IdentityService.Username, UserAgent, true);

                    foreach (var detail in item.Details)
                    {
                        EntityExtension.FlagForDelete(detail, IdentityService.Username, UserAgent, true);
                        //DbContext.PaymentDispositionNoteDetails.Update(detail);
                    }

                    //DbContext.PaymentDispositionNoteItems.Update(item);
                }
                else
                {
                    EntityExtension.FlagForUpdate(item, IdentityService.Username, UserAgent);

                    foreach (var detail in DbContext.PaymentDispositionNoteDetails.AsNoTracking().Where(p => p.PaymentDispositionNoteItemId == item.Id))
                    {
                        EntityExtension.FlagForUpdate(detail, IdentityService.Username, UserAgent);

                    }
                }
            }

            EntityExtension.FlagForUpdate(exist, IdentityService.Username, UserAgent);
            //DbSet.Update(exist);
        }

        public ReadResponse<PaymentDispositionNoteModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<PaymentDispositionNoteModel> Query = this.DbSet.Include(m => m.Items).ThenInclude(m => m.Details);
            List<string> searchAttributes = new List<string>()
            {
                "PaymentDispositionNo", "Items.DispositionNo",  "SupplierName", "BankCurrencyCode","BankName"
            };

            Query = QueryHelper<PaymentDispositionNoteModel>.Search(Query, searchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<PaymentDispositionNoteModel>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<PaymentDispositionNoteModel>.Order(Query, OrderDictionary);

            Pageable<PaymentDispositionNoteModel> pageable = new Pageable<PaymentDispositionNoteModel>(Query, page - 1, size);
            List<PaymentDispositionNoteModel> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<PaymentDispositionNoteModel>(Data, TotalData, OrderDictionary, new List<string>());
        }

        public ReadResponse<PaymentDispositionNoteItemModel> ReadDetailsByEPOId(string epoId)
        {
            List<PaymentDispositionNoteItemModel> paymentDispositionNoteDetails = DbContext.PaymentDispositionNoteItems.Where(a => a.Details.Any(b => b.EPOId == epoId)).Distinct().ToList();

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>("{ }");

            int TotalData = paymentDispositionNoteDetails.Count;

            return new ReadResponse<PaymentDispositionNoteItemModel>(paymentDispositionNoteDetails, TotalData, OrderDictionary, new List<string>());
        }

        private async Task SetTrueDisposition(string dispositionNo)
        {
            var http = ServiceProvider.GetService<IHttpClientService>();
            await http.PutAsync(APIEndpoint.Purchasing + $"purchasing-dispositions/update/is-paid-true/{dispositionNo}", new StringContent("{}", Encoding.UTF8, General.JsonMediaType) );
        }

        public async Task<int> Post(PaymentDispositionNotePostDto form)
        {
            List<int> listIds = form.ListIds.Select(x => x.Id).ToList();

            foreach (int id in listIds)
            {
                var model = await ReadByIdAsync(id);

                if (model != null)
                {
                    model.SetIsPosted(IdentityService.Username, UserAgent);

                    foreach (var item in model.Items)
                    {
                        await SetTrueDisposition(item.DispositionNo);
                    }

                    await _autoDailyBankTransactionService.AutoCreateFromPaymentDisposition(model);
                }
            }

            var result = await DbContext.SaveChangesAsync();

            return result;
        }

        public List<ReportDto> GetReport(int bankExpenditureId, int dispositionId, int supplierId, int divisionId, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var expenditureQuery = DbContext.PaymentDispositionNotes.AsQueryable();
            var expenditureItemQuery = DbContext.PaymentDispositionNoteItems.AsQueryable();
            var query = from expenditure in expenditureQuery
                        join expenditureItem in expenditureItemQuery on expenditure.Id equals expenditureItem.PaymentDispositionNoteId into items
                        from item in items.DefaultIfEmpty()
                        select new
                        {
                            expenditure.Id,
                            expenditure.PaymentDispositionNo,
                            expenditure.PaymentDate,
                            item.DispositionId,
                            item.DispositionNo,
                            item.DispositionDate,
                            item.PaymentDueDate,
                            expenditure.BankId,
                            expenditure.BankName,
                            expenditure.BankCurrencyCode,
                            expenditure.CurrencyId,
                            expenditure.CurrencyCode,
                            expenditure.CurrencyRate,
                            expenditure.SupplierId,
                            expenditure.SupplierName,
                            expenditure.SupplierImport,
                            item.ProformaNo,
                            item.CategoryId,
                            item.CategoryName,
                            item.DivisionId,
                            item.DivisionName,
                            item.VatValue,
                            item.DPP,
                            expenditure.TransactionType,
                            expenditure.BankAccountNumber,
                            item.IncomeTaxValue,
                            item.PayToSupplier,
                            expenditure.Amount
                        };

            query = query.Where(entity => entity.PaymentDate >= startDate && entity.PaymentDate <= endDate);
            if (bankExpenditureId > 0)
                query = query.Where(entity => entity.Id == bankExpenditureId);

            if (dispositionId > 0)
                query = query.Where(entity => entity.DispositionId == dispositionId);

            if (supplierId > 0)
                query = query.Where(entity => entity.SupplierId == supplierId);

            if (divisionId > 0)
                query = query.Where(entity => entity.DivisionId == divisionId);

            var result = query.OrderBy(entity => entity.PaymentDate).ToList();

            return result.Select(element => new ReportDto(element.Id, element.PaymentDispositionNo, element.PaymentDate, element.DispositionId, element.DispositionNo, element.DispositionDate, element.PaymentDueDate, element.BankId, element.BankName, element.CurrencyId, element.CurrencyCode, element.SupplierId, element.SupplierName, element.SupplierImport, element.ProformaNo, element.CategoryId, element.CategoryName, element.DivisionId, element.DivisionName, element.VatValue, element.PayToSupplier, element.TransactionType, element.BankAccountNumber, element.CurrencyRate, element.BankCurrencyCode)).ToList();
        }

        public MemoryStream GetXls(List<ReportDto> data)
        {
            var dt = new DataTable();
            dt.Columns.Add(new DataColumn() { ColumnName = "No. Bukti Pembayaran Disposisi", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tanggal Bayar Disposisi", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "No. Disposisi", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl Disposisi", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl Jatuh Tempo", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Bank Bayar", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Supplier", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Nomor Proforma Invoice", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Kategori", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Divisi", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "PPN", DataType = typeof(double) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Jumlah dibayar ke Supplier", DataType = typeof(double) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Jenis Transaksi", DataType = typeof(string) });

            if (data.Count() == 0)
            {
                dt.Rows.Add("", "", "", "", "", "", "", "", "", "", "", 0, 0, "");
            }
            else
            {
                foreach (var item in data)
                {
                    dt.Rows.Add(
                        item.ExpenditureNo,
                        item.ExpenditureDate.AddHours(IdentityService.TimezoneOffset).ToString("dd/MM/yyyy"),
                        item.DispositionNo,
                        item.DispositionDate.AddHours(IdentityService.TimezoneOffset).ToString("dd/MM/yyyy"),
                        item.DispositionDueDate.AddHours(IdentityService.TimezoneOffset).ToString("dd/MM/yyyy"),
                        item.BankName,
                        item.CurrencyCode,
                        item.SupplierName,
                        item.ProformaNo,
                        item.CategoryName,
                        item.DivisionName,
                        item.VATAmount,
                        item.PaidAmount,
                        item.TransactionType
                        );
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(dt, "Laporan Pembayaran Disposisi") }, true);
        }
    }
}
