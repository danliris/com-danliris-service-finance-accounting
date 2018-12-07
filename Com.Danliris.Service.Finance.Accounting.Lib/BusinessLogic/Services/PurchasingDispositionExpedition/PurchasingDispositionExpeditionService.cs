using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.PurchasingDispositionExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Enums.Expedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Helpers;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.PurchasingDispositionExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.IntegrationViewModel;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.PurchasingDispositionAcceptance;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.PurchasingDispositionReport;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.PurchasingDispositionVerification;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.PurchasingDispositionExpedition
{
    public class PurchasingDispositionExpeditionService : IPurchasingDispositionExpeditionService
    {
        private const string UserAgent = "finance-service";
        protected DbSet<PurchasingDispositionExpeditionModel> DbSet;
        public IIdentityService IdentityService;
        public readonly IServiceProvider ServiceProvider;
        public FinanceDbContext DbContext;

        public PurchasingDispositionExpeditionService(IServiceProvider serviceProvider, FinanceDbContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            DbSet = dbContext.Set<PurchasingDispositionExpeditionModel>();
            IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        public void CreateModel(PurchasingDispositionExpeditionModel m)
        {
            EntityExtension.FlagForCreate(m, IdentityService.Username, UserAgent);
            m.Position = ExpeditionPosition.SEND_TO_VERIFICATION_DIVISION;
            foreach (var item in m.Items)
            {
                EntityExtension.FlagForCreate(item, IdentityService.Username, UserAgent);
            }
            DbSet.Add(m);
        }

        public async Task<int> CreateAsync(PurchasingDispositionExpeditionModel m)
        {
            CreateModel(m);
            return await DbContext.SaveChangesAsync();
        }

        public async Task DeleteModel(int id)
        {
            PurchasingDispositionExpeditionModel model = await ReadByIdAsync(id);
            EntityExtension.FlagForDelete(model, IdentityService.Username, UserAgent, true);
            foreach (var item in model.Items)
            {
                EntityExtension.FlagForDelete(item, IdentityService.Username, UserAgent, true);
            }
            DbSet.Update(model);
        }

        public async Task<int> DeleteAsync(int id)
        {
            await DeleteModel(id);
            return await DbContext.SaveChangesAsync();
        }

        public ReadResponse<PurchasingDispositionExpeditionModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {

            IQueryable<PurchasingDispositionExpeditionModel> Query = this.DbSet.Include(m => m.Items);
            List<string> searchAttributes = new List<string>()
            {
                "DispositionId", "DispositionNo",  "SupplierName", "CurrencyCode"
            };

            Query = QueryHelper<PurchasingDispositionExpeditionModel>.Search(Query, searchAttributes, keyword);

            if (filter.Contains("verificationFilter"))
            {
                filter = "{}";
                List<ExpeditionPosition> positions = new List<ExpeditionPosition> { ExpeditionPosition.SEND_TO_PURCHASING_DIVISION, ExpeditionPosition.SEND_TO_ACCOUNTING_DIVISION, ExpeditionPosition.SEND_TO_CASHIER_DIVISION };
                Query = Query.Where(p => positions.Contains(p.Position));
            }

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<PurchasingDispositionExpeditionModel>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<PurchasingDispositionExpeditionModel>.Order(Query, OrderDictionary);

            Pageable<PurchasingDispositionExpeditionModel> pageable = new Pageable<PurchasingDispositionExpeditionModel>(Query, page - 1, size);
            List<PurchasingDispositionExpeditionModel> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<PurchasingDispositionExpeditionModel>(Data, TotalData, OrderDictionary, new List<string>());
        }

        public Task<PurchasingDispositionExpeditionModel> ReadByIdAsync(int id)
        {
            return DbSet.Include(m => m.Items).FirstOrDefaultAsync(d => d.Id.Equals(id) && d.IsDeleted.Equals(false));
        }

        public Task<int> Update(int id, PurchasingDispositionExpeditionModel m, string user, int clientTimeZoneOffset = 7)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(int id, PurchasingDispositionExpeditionModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<int> PurchasingDispositionAcceptance(PurchasingDispositionAcceptanceViewModel data)
        {

            int updated = 0;

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    List<string> dispositions = new List<string>();

                    if (data.Role.Equals("VERIFICATION"))
                    {
                        foreach (var item in data.PurchasingDispositionExpedition)
                        {
                            dispositions.Add(item.DispositionNo);
                            PurchasingDispositionExpeditionModel model = DbContext.PurchasingDispositionExpeditions.Single(x => x.Id == item.Id);
                            model.VerificationDivisionBy = IdentityService.Username;
                            model.VerificationDivisionDate = DateTimeOffset.UtcNow;
                            model.Position = ExpeditionPosition.VERIFICATION_DIVISION;

                            EntityExtension.FlagForUpdate(model, IdentityService.Username, UserAgent);
                        }

                        updated = await DbContext.SaveChangesAsync();
                        UpdateDispositionPosition(dispositions, ExpeditionPosition.VERIFICATION_DIVISION);
                    }
                    else if (data.Role.Equals("CASHIER"))
                    {
                        foreach (var item in data.PurchasingDispositionExpedition)
                        {
                            dispositions.Add(item.DispositionNo);
                            PurchasingDispositionExpeditionModel model = DbContext.PurchasingDispositionExpeditions.Single(x => x.Id == item.Id);
                            model.CashierDivisionBy = IdentityService.Username;
                            model.CashierDivisionDate = DateTimeOffset.UtcNow;
                            model.Position = ExpeditionPosition.CASHIER_DIVISION;

                            EntityExtension.FlagForUpdate(model, IdentityService.Username, UserAgent);
                        }

                        updated = await DbContext.SaveChangesAsync();
                        UpdateDispositionPosition(dispositions, ExpeditionPosition.CASHIER_DIVISION);
                    }
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }

            return updated;
        }

        public async Task<int> DeletePurchasingDispositionAcceptance(int id)
        {
            int count = 0;

            if (DbContext.PurchasingDispositionExpeditions.Count(x => x.Id == id && !x.IsDeleted).Equals(0))
            {
                return 0;
            }

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    PurchasingDispositionExpeditionModel purchasingDispositionExpedition = DbContext.PurchasingDispositionExpeditions.Single(x => x.Id == id);

                    if (purchasingDispositionExpedition.Position == ExpeditionPosition.VERIFICATION_DIVISION)
                    {
                        purchasingDispositionExpedition.VerificationDivisionBy = null;
                        purchasingDispositionExpedition.VerificationDivisionDate = null;
                        purchasingDispositionExpedition.Position = ExpeditionPosition.SEND_TO_VERIFICATION_DIVISION;

                        EntityExtension.FlagForUpdate(purchasingDispositionExpedition, IdentityService.Username, UserAgent);

                        count = await DbContext.SaveChangesAsync();
                        UpdateDispositionPosition(new List<string>() { purchasingDispositionExpedition.DispositionNo }, ExpeditionPosition.SEND_TO_VERIFICATION_DIVISION);
                    }
                    else if (purchasingDispositionExpedition.Position == ExpeditionPosition.CASHIER_DIVISION)
                    {
                        purchasingDispositionExpedition.CashierDivisionBy = null;
                        purchasingDispositionExpedition.CashierDivisionDate = null;
                        purchasingDispositionExpedition.Position = ExpeditionPosition.SEND_TO_CASHIER_DIVISION;

                        EntityExtension.FlagForUpdate(purchasingDispositionExpedition, IdentityService.Username, UserAgent);

                        count = await DbContext.SaveChangesAsync();
                        UpdateDispositionPosition(new List<string>() { purchasingDispositionExpedition.DispositionNo }, ExpeditionPosition.SEND_TO_CASHIER_DIVISION);
                    }

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }

            return count;
        }

        public async Task<int> PurchasingDispositionVerification(PurchasingDispositionVerificationViewModel data)
        {
            int updated = 0;

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    PurchasingDispositionExpeditionModel model;
                    if (data.Id == 0)
                    {
                        model = DbContext.PurchasingDispositionExpeditions.First(x => x.DispositionNo == data.DispositionNo);
                    }
                    else
                    {
                        model = DbContext.PurchasingDispositionExpeditions.Single(x => x.Id == data.Id);
                    }


                    if (data.SubmitPosition == ExpeditionPosition.SEND_TO_PURCHASING_DIVISION)
                    {
                        model.DispositionNo = data.DispositionNo;
                        model.VerifyDate = data.VerifyDate;
                        model.SendToPurchasingDivisionBy = IdentityService.Username;
                        model.SendToPurchasingDivisionDate = data.VerifyDate;
                        model.Position = ExpeditionPosition.SEND_TO_PURCHASING_DIVISION;
                        model.Active = false;
                        model.NotVerifiedReason = data.Reason;

                        EntityExtension.FlagForUpdate(model, IdentityService.Username, UserAgent);
                        updated = await DbContext.SaveChangesAsync();
                        UpdateDispositionPosition(new List<string>() { model.DispositionNo }, ExpeditionPosition.SEND_TO_PURCHASING_DIVISION);
                    }
                    else if (data.SubmitPosition == ExpeditionPosition.SEND_TO_CASHIER_DIVISION)
                    {
                        model.DispositionNo = data.DispositionNo;
                        model.VerifyDate = data.VerifyDate;
                        model.SendToCashierDivisionBy = IdentityService.Username;
                        model.SendToCashierDivisionDate = data.VerifyDate;
                        model.Position = ExpeditionPosition.SEND_TO_CASHIER_DIVISION;
                        model.Active = true;

                        EntityExtension.FlagForUpdate(model, IdentityService.Username, UserAgent);
                        updated = await DbContext.SaveChangesAsync();
                        UpdateDispositionPosition(new List<string>() { model.DispositionNo }, ExpeditionPosition.SEND_TO_CASHIER_DIVISION);
                    }


                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }

            return updated;
        }

        private void UpdateDispositionPosition(List<string> dispositions, ExpeditionPosition position)
        {
            string dispositionUri = "purchasing-dispositions/update/position";

            var data = new
            {
                Position = position,
                PurchasingDispositionNoes = dispositions
            };

            IHttpClientService httpClient = (IHttpClientService)this.ServiceProvider.GetService(typeof(IHttpClientService));
            var response = httpClient.PutAsync($"{APIEndpoint.Purchasing}{dispositionUri}", new StringContent(JsonConvert.SerializeObject(data).ToString(), Encoding.UTF8, General.JsonMediaType)).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(string.Format("{0}, {1}, {2}", response.StatusCode, response.Content, APIEndpoint.Purchasing));
            }
        }

        private PurchasingDispositionBaseResponseViewModel JoinReport(int page, int size, string order, string filter, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int offSet)
        {
            var expeditionData = DbSet.ToList();
            var purchasingDispositionResponse = GetPurchasingDisposition(page, size, order, filter);
            List<PurchasingDispositionViewModel> data = purchasingDispositionResponse.data;

            List<PurchasingDispositionReportViewModel> result = new List<PurchasingDispositionReportViewModel>();

            if (dateFrom == null && dateTo == null)
            {
                data = data
                    .Where(x => DateTimeOffset.UtcNow.AddDays(-30).Date <= x.CreatedUtc.AddHours(offSet).Date
                        && x.CreatedUtc.AddHours(offSet).Date <= DateTime.UtcNow.Date).ToList();
            }
            else if (dateFrom == null && dateTo != null)
            {
                data = data
                    .Where(x => dateTo.Value.AddDays(-30).Date <= x.CreatedUtc.AddHours(offSet).Date
                        && x.CreatedUtc.AddHours(offSet).Date <= dateTo.Value.Date).ToList();
            }
            else if (dateTo == null && dateFrom != null)
            {
                data = data
                    .Where(x => dateFrom.Value.Date <= x.CreatedUtc.AddHours(offSet).Date
                        && x.CreatedUtc.AddHours(offSet).Date <= dateFrom.Value.AddDays(30).Date).ToList();
            }
            else
            {
                data = data
                    .Where(x => dateFrom.Value.Date <= x.CreatedUtc.AddHours(offSet).Date
                        && x.CreatedUtc.AddHours(offSet).Date <= dateTo.Value.Date).ToList();
            }

            foreach (var item in data)
            {
                var expedition = expeditionData.FirstOrDefault(x => x.DispositionNo == item.DispositionNo);
                PurchasingDispositionReportViewModel vm = new PurchasingDispositionReportViewModel()
                {
                    BankExpenditureNoteDate = expedition == null || expedition.BankExpenditureNoteDate == DateTimeOffset.MinValue ? null : expedition.BankExpenditureNoteDate,
                    DispositionNo = item.DispositionNo,
                    BankExpenditureNoteNo = expedition?.BankExpenditureNoteNo,
                    BankExpenditureNotePPHDate = expedition == null || expedition.BankExpenditureNotePPHDate == DateTimeOffset.MinValue ? null : expedition.BankExpenditureNotePPHDate,
                    BankExpenditureNotePPHNo = expedition?.BankExpenditureNotePPHNo,
                    CashierDivisionDate = expedition == null || expedition.CashierDivisionDate == DateTimeOffset.MinValue ? null : expedition.CashierDivisionDate,
                    CreatedUtc = item.CreatedUtc,
                    InvoiceNo = item.InvoiceNo,
                    PaymentDueDate = item.PaymentDueDate,
                    Position = item.Position,
                    SentToVerificationDivisionDate = expedition == null ? null : new DateTimeOffset?(expedition.CreatedUtc),
                    SendDate = expedition == null ? null : ((expedition.Position == ExpeditionPosition.CASHIER_DIVISION || expedition.Position == ExpeditionPosition.SEND_TO_CASHIER_DIVISION) && expedition.SendToCashierDivisionDate != DateTimeOffset.MinValue) ? expedition.SendToCashierDivisionDate :
                    ((expedition.Position == ExpeditionPosition.SEND_TO_PURCHASING_DIVISION) && expedition.SendToPurchasingDivisionDate != DateTimeOffset.MinValue) ? expedition.SendToPurchasingDivisionDate : null,
                    SupplierName = item.Supplier.name,
                    VerificationDivisionDate = expedition == null || expedition.VerificationDivisionDate == DateTimeOffset.MinValue ? null : expedition.VerificationDivisionDate,
                    VerifyDate = expedition == null || expedition.VerifyDate == DateTimeOffset.MinValue ? null : expedition.VerifyDate
                };
                result.Add(vm);

            }
            return new PurchasingDispositionBaseResponseViewModel
            {
                info = purchasingDispositionResponse.info,
                data = result
            };
        }

        public MemoryStream GenerateExcel(int page, int size, string order, string filter, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int offSet)
        {
            var data = JoinReport(page, size, order, filter, dateFrom, dateTo, offSet);

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn() { ColumnName = "No. Disposisi", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl Disposisi", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl Jatuh Tempo", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Nomor Invoice", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Supplier", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Posisi", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl Pembelian Kirim", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl Terima Verifikasi", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl Cek Verifikasi", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl Kirim", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl Terima Kasir", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl Bayar Kasir", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "No Kuitansi Kasir", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl Bayar PPH Kasir", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "No Kuitansi PPHKasir", DataType = typeof(string) });

            if (data.data.Count == 0)
            {
                dt.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
            }
            else
            {
                foreach (var item in data.data)
                {
                    dt.Rows.Add(item.DispositionNo, item.CreatedUtc == null ? "-" : item.CreatedUtc.Value.AddHours(offSet).ToString("dd MMM yyyy"), item.PaymentDueDate == null ? "-" : item.PaymentDueDate.Value.AddHours(offSet).ToString("dd MMM yyyy"),
                        item.InvoiceNo, item.SupplierName, item.Position == 0 ? "-" : ((ExpeditionPosition)item.Position).ToDescriptionString(),item.SentToVerificationDivisionDate == null ? "-" : item.SentToVerificationDivisionDate.Value.AddHours(offSet).ToString("dd MMM yyyy"), item.VerificationDivisionDate == null ? "-" : item.VerificationDivisionDate.Value.AddHours(offSet).ToString("dd MMM yyyy"),
                        item.VerifyDate == null ? "-" : item.VerifyDate.Value.AddHours(offSet).ToString("dd MMM yyyy"), item.SendDate == null ? "-" : item.SendDate.Value.AddHours(offSet).ToString("dd MMM yyyy"),
                        item.CashierDivisionDate == null ? "-" : item.CashierDivisionDate.Value.AddHours(offSet).ToString("dd MMM yyyy"), item.BankExpenditureNoteDate == null ? "-" : item.BankExpenditureNoteDate.Value.AddHours(offSet).ToString("dd MMM yyyy"),
                        string.IsNullOrEmpty(item.BankExpenditureNoteNo) ? "-" : item.BankExpenditureNoteNo, item.BankExpenditureNotePPHDate == null ? "-" : item.BankExpenditureNotePPHDate.Value.AddHours(offSet).ToString("dd MMM yyyy"), string.IsNullOrEmpty(item.BankExpenditureNotePPHNo) ? "-" : item.BankExpenditureNotePPHNo);

                }
            }
            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(dt, "Disposisi Pembelian") }, true);
        }

        public ReadResponse<PurchasingDispositionReportViewModel> GetReport(int page, int size, string order, string filter, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int offSet)
        {
            var queries = JoinReport(page, size, order, filter, dateFrom, dateTo, offSet);
            Pageable<PurchasingDispositionReportViewModel> pageable = new Pageable<PurchasingDispositionReportViewModel>(queries.data, page - 1, size);
            List<PurchasingDispositionReportViewModel> data = pageable.Data.ToList();
            return new ReadResponse<PurchasingDispositionReportViewModel>(queries.data, queries.info.total, new Dictionary<string, string>(), new List<string>());
        }

        private PurchasingDispositionResponseViewModel GetPurchasingDisposition(int page, int size, string order, string filter)
        {
            string dispositionUri = "purchasing-dispositions";
            string queryUri = "?page=" + page + "&size=" + size + "&order=" + order + "&filter=" + filter;
            string uri = dispositionUri + queryUri;
            IHttpClientService httpClient = (IHttpClientService)this.ServiceProvider.GetService(typeof(IHttpClientService));
            var response = httpClient.GetAsync($"{APIEndpoint.Purchasing}{uri}").Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(string.Format("{0}, {1}, {2}", response.StatusCode, response.Content, APIEndpoint.Purchasing));
            }
            else
            {
                PurchasingDispositionResponseViewModel result = JsonConvert.DeserializeObject<PurchasingDispositionResponseViewModel>(response.Content.ReadAsStringAsync().Result);
                return result;
            }
        }

    }
}
