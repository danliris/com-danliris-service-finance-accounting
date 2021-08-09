using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.Rreports.ExportSalesDebtorReportController;
using Newtonsoft.Json;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using static Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Reports.ExportSalesDebtorReport.GarmentShippingPackingList;
using Microsoft.Extensions.DependencyInjection;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentFinance.Memorial;
using Microsoft.EntityFrameworkCore;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentFinance.BankCashReceipt;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentFinance.MemorialDetail;
using System.Linq;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentFinance.BankCashReceiptDetail;
using System.IO;
using System.Data;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Reports.ExportSalesDebtorReport
{
    public class ExportSalesDebtorReportService : IExportSalesDebtorReportService
    {
        private readonly IServiceProvider _serviceProvider;
        private const string UserAgent = "finance-service";
        private readonly FinanceDbContext _dbContext;
        private readonly IIdentityService _identityService;
        protected DbSet<GarmentFinanceMemorialDetailModel> DbSetMemoDetail;
        protected DbSet<GarmentFinanceMemorialDetailItemModel> DbSetDetailItemMemo;
        protected DbSet<BankCashReceiptDetailItemModel> DbSetBankCashDetailItem;
        protected DbSet<BankCashReceiptDetailModel> DBSetBankCashDetail;



        public ExportSalesDebtorReportService(IServiceProvider serviceProvider, FinanceDbContext dbContext)
        {
            _dbContext = dbContext;
            
            _identityService = serviceProvider.GetService<IIdentityService>();
            _serviceProvider = serviceProvider;
        }

        public async Task<GarmentShippingPackingList> GetDataShippingInvoice(int month, int year)
        {
            GarmentShippingPackingList garmentShipping = new GarmentShippingPackingList();
            var http = _serviceProvider.GetService<IHttpClientService>();
            var uri = APIEndpoint.PackingInventory + $"garment-shipping/invoices/exportSalesDebtor?month={month}&year={year}";
            var response = await http.GetAsync(uri);


            var result = new BaseResponse<string>();

            if (response.IsSuccessStatusCode)
            {
                if (response.Content != null)
                {
                    var contentString = await response.Content.ReadAsStringAsync();
                    Dictionary<string, object> content = JsonConvert.DeserializeObject<Dictionary<string, object>>(contentString);
                    var dataString = content.GetValueOrDefault("data").ToString();
                    var listData = JsonConvert.DeserializeObject<List<ShippingPackingDto>>(dataString);
                    garmentShipping.data = listData;
                }
            }


            else
            {
                var err = await response.Content.ReadAsStringAsync();

            }


            return garmentShipping;
        }
        public async Task<GarmentShippingPackingList> GetDataShippingInvoiceNow(int month, int year)
        {
            GarmentShippingPackingList garmentShipping = new GarmentShippingPackingList();
            var http = _serviceProvider.GetService<IHttpClientService>();
            var uri = APIEndpoint.PackingInventory + $"garment-shipping/invoices/exportSalesDebtorNow?month={month}&year={year}";
            var response = await http.GetAsync(uri);


            var result = new BaseResponse<string>();

            if (response.IsSuccessStatusCode)
            {
                if (response.Content != null)
                {
                    var contentString = await response.Content.ReadAsStringAsync();
                    Dictionary<string, object> content = JsonConvert.DeserializeObject<Dictionary<string, object>>(contentString);
                    var dataString = content.GetValueOrDefault("data").ToString();
                    var listData = JsonConvert.DeserializeObject<List<ShippingPackingDto>>(dataString);
                    garmentShipping.data = listData;
                }
            }


            else
            {
                var err = await response.Content.ReadAsStringAsync();

            }


            return garmentShipping;
        }

        public async Task<GarmentShippingPackingList> GetDataBalance()
        {
            GarmentShippingPackingList garmentShipping = new GarmentShippingPackingList();
            var http = _serviceProvider.GetService<IHttpClientService>();
            var uri = APIEndpoint.PackingInventory + $"garment-shipping/garment-debitur-balances";
            var response = await http.GetAsync(uri);


            var result = new BaseResponse<string>();

            if (response.IsSuccessStatusCode)
            {
                if (response.Content != null)
                {
                    var contentString = await response.Content.ReadAsStringAsync();
                    Dictionary<string, object> content = JsonConvert.DeserializeObject<Dictionary<string, object>>(contentString);
                    var dataString = content.GetValueOrDefault("data").ToString();
                    var listData = JsonConvert.DeserializeObject<List<ShippingPackingDto>>(dataString);
                    garmentShipping.data = listData;

                }
            }


            else
            {
                var err = await response.Content.ReadAsStringAsync();

            }


            return garmentShipping;
        }
        private async Task<GarmentCurrency> GetCurrencyByCurrencyCodeDate(string currencyCode)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var httpClient = (IHttpClientService)_serviceProvider.GetService(typeof(IHttpClientService));

            var currencyUri = APIEndpoint.Core + $"master/bi-currencies?code={currencyCode}";
            var currencyResponse = await httpClient.GetAsync(currencyUri);

            var currencyResult = new BaseResponse<GarmentCurrency>()
            {
                data = new GarmentCurrency()
            };

            if (currencyResponse.IsSuccessStatusCode)
            {
                currencyResult = JsonConvert.DeserializeObject<BaseResponse<GarmentCurrency>>(currencyResponse.Content.ReadAsStringAsync().Result, jsonSerializerSettings);
            }

            return currencyResult.data;
        }

        public async Task<List<ExportSalesDebtorReportViewModel>> GetMonitoring(int month, int year,string type, int offset)
        {

            GarmentShippingPackingList invoicePackingListBalance = await GetDataShippingInvoice(month, year);
            GarmentShippingPackingList invoicePackingListNow = await GetDataShippingInvoiceNow(month, year);
            GarmentShippingPackingList balance = await GetDataBalance();

            List<ExportSalesDebtorReportViewModel> data = new List<ExportSalesDebtorReportViewModel>();
            GarmentCurrency garmentCurrency = await GetCurrencyByCurrencyCodeDate("USD");

            var beginingMemo = from a in (from aa in _dbContext.GarmentFinanceMemorialDetails where aa.MemorialDate.Month < month && aa.MemorialDate.Year == year select new { aa.Id })
                               join c in _dbContext.GarmentFinanceMemorialDetailItems on a.Id equals c.MemorialDetailId
                               select new ExportSalesDebtorReportViewModel
                               {
                                   buyerCode = c.BuyerCode,
                                   buyerName = c.BuyerName,
                                   beginingBalance = Convert.ToDecimal(-c.Amount),
                                   receipt = 0,
                                   sales = 0,
                                   endBalance = 0,
                                   lessThan = 0,
                                   between = 0,
                                   moreThan = 0

                               };
            var beginingBankCashReceipt = from a in (from aa in _dbContext.GarmentFinanceBankCashReceiptDetails where aa.BankCashReceiptDate.Month < month && aa.BankCashReceiptDate.Year == year select new { aa.Id })
                                          join b in _dbContext.GarmentFinanceBankCashReceiptDetailItems on a.Id equals b.BankCashReceiptDetailId

                                          select new ExportSalesDebtorReportViewModel
                                          {
                                              buyerCode = b.BuyerCode,
                                              buyerName = b.BuyerName,
                                              beginingBalance = Convert.ToDecimal(-b.Amount),
                                              receipt = 0,
                                              sales = 0,
                                              endBalance = 0,
                                              lessThan = 0,
                                              between = 0,
                                              moreThan = 0

                                          };
            var beginingInvoice = from a in invoicePackingListBalance.data
                                  select new ExportSalesDebtorReportViewModel
                                  {
                                      buyerCode = a.buyerAgentCode,
                                      buyerName = a.buyerAgentName,
                                      beginingBalance = Convert.ToDecimal(a.amount),
                                      receipt = 0,
                                      sales = 0,
                                      endBalance = 0,
                                      lessThan = 0,
                                      between = 0,
                                      moreThan = 0

                                  };
            var beginingBalance = from a in balance.data
                                  select new ExportSalesDebtorReportViewModel
                                  {
                                      buyerCode = a.buyerAgentCode,
                                      buyerName = a.buyerAgentName,
                                      beginingBalance = Convert.ToDecimal(a.amount),
                                      receipt = 0,
                                      sales = 0,
                                      endBalance = 0,
                                      lessThan = 0,
                                      between = 0,
                                      moreThan = 0

                                  };
            var salesNow = from a in invoicePackingListNow.data
                           select new ExportSalesDebtorReportViewModel
                           {
                               buyerCode = a.buyerAgentCode,
                               buyerName = a.buyerAgentName,
                               beginingBalance = 0,
                               receipt = 0,
                               sales = Convert.ToDouble(a.amount),
                               endBalance = 0,
                               lessThan = 0,
                               between = 0,
                               moreThan = 0
                           };
            var memoNow = from a in (from aa in _dbContext.GarmentFinanceMemorialDetails where aa.MemorialDate.Month == month && aa.MemorialDate.Year == year select new { aa.Id })
                          join c in _dbContext.GarmentFinanceMemorialDetailItems on a.Id equals c.MemorialDetailId
                          select new ExportSalesDebtorReportViewModel
                          {
                              buyerCode = c.BuyerCode,
                              buyerName = c.BuyerName,
                              beginingBalance = 0,
                              receipt = Convert.ToDouble(c.Amount),
                              sales = 0,
                              endBalance = 0,
                              lessThan = 0,
                              between = 0,
                              moreThan = 0

                          };
            var bankCashReceiptNow = from a in (from aa in _dbContext.GarmentFinanceBankCashReceiptDetails where aa.BankCashReceiptDate.Month == month && aa.BankCashReceiptDate.Year == year select new { aa.Id })
                                     join b in _dbContext.GarmentFinanceBankCashReceiptDetailItems on a.Id equals b.BankCashReceiptDetailId

                                     select new ExportSalesDebtorReportViewModel
                                     {
                                         buyerCode = b.BuyerCode,
                                         buyerName = b.BuyerName,
                                         beginingBalance = 0,
                                         receipt = Convert.ToDouble(b.Amount),
                                         sales = 0,
                                         endBalance = 0,
                                         lessThan = 0,
                                         between = 0,
                                         moreThan = 0

                                     };
            var periodeMemo = from a in (from aa in _dbContext.GarmentFinanceMemorialDetails where aa.MemorialDate.Month <= month && aa.MemorialDate.Year == year select new { aa.Id })
                               join c in _dbContext.GarmentFinanceMemorialDetailItems on a.Id equals c.MemorialDetailId
                               select new ExportSalesDebtorReportViewModel
                               {
                                   buyerCode = c.BuyerCode,
                                   buyerName = c.BuyerName,
                                   beginingBalance =0,
                                   receipt = 0,
                                   sales = 0,
                                   endBalance = 0,
                                   lessThan = -c.Amount,
                                   between = -c.Amount,
                                   moreThan = -c.Amount

                               };
            var periodeBankCashReceipt = from a in (from aa in _dbContext.GarmentFinanceBankCashReceiptDetails where aa.BankCashReceiptDate.Month <= month && aa.BankCashReceiptDate.Year == year select new { aa.Id })
                                          join b in _dbContext.GarmentFinanceBankCashReceiptDetailItems on a.Id equals b.BankCashReceiptDetailId

                                          select new ExportSalesDebtorReportViewModel
                                          {
                                              buyerCode = b.BuyerCode,
                                              buyerName = b.BuyerName,
                                              beginingBalance = 0,
                                              receipt = 0,
                                              sales = 0,
                                              endBalance = 0,
                                              lessThan = Convert.ToDouble(-b.Amount),
                                              between = Convert.ToDouble(-b.Amount),
                                              moreThan = Convert.ToDouble(-b.Amount)

                                          };
            var queryUnion =beginingBalance.Union(beginingMemo).Union(beginingBankCashReceipt).Union(beginingInvoice)
                        .Union(memoNow).Union(bankCashReceiptNow).Union(salesNow)
                        .Union(periodeBankCashReceipt).Union(periodeMemo);

            var _invoice = invoicePackingListBalance.data.Union(invoicePackingListNow.data);

            var querySum = queryUnion.ToList()
                   .GroupBy(x => new { x.buyerCode, x.buyerName }, (key, group) => new
                   {
                       buyerCode = key.buyerCode,
                       buyerName = key.buyerName,
                       beginingBalance = group.Sum(s => s.beginingBalance),
                       receipt = group.Sum(s => s.receipt),
                       sales = group.Sum(s => s.sales),
                       endBalance = group.Sum(s => s.endBalance),
                       lessThan = group.Sum(s => s.lessThan),
                       between = group.Sum(s => s.between),
                       moreThan = group.Sum(s => s.moreThan)

                   }).OrderByDescending(s => s.buyerName);
            int index = 1;
            var querytimeSpan = from aa in _invoice
                                select new timeSpanInvoice
                                {
                                    buyerCode = aa.buyerAgentCode,
                                    amount = Convert.ToDecimal(aa.amount),
                                    day = (DateTimeOffset.Now - aa.truckingDate).Days
                                };
            var querySumTS = querytimeSpan.ToList()
                   .GroupBy(x => new { x.buyerCode, x.day }, (key, group) => new
                   {
                       buyerCode = key.buyerCode,
                       amount = group.Sum(s => s.amount)
                   });
            foreach (var item in querySum)
            {
                ExportSalesDebtorReportViewModel model = new ExportSalesDebtorReportViewModel
                {
                    index = index,
                    buyerCode = item.buyerCode,
                    buyerName = item.buyerName,
                    beginingBalance = item.beginingBalance,
                    receipt = item.receipt,
                    sales = item.sales,
                    endBalance = Convert.ToDouble(item.beginingBalance) + item.sales - item.receipt,
                    lessThan =  Convert.ToDouble((from aa in querytimeSpan where aa.buyerCode == item.buyerCode && aa.day <=30 select aa.amount).FirstOrDefault()),
                    between = Convert.ToDouble((from aa in querytimeSpan where aa.buyerCode == item.buyerCode && (aa.day > 30 && aa.day <60)  select aa.amount).FirstOrDefault()),
                    moreThan = Convert.ToDouble((from aa in querytimeSpan where aa.buyerCode == item.buyerCode && aa.day > 60 select aa.amount).FirstOrDefault()),
                };
                data.Add(model);
                index++;
            }
            

            return data;
        }
        class timeSpanInvoice
        {
             internal string buyerCode { get; set; }
             internal decimal amount { get; set; } 
             internal int day { get; set; }
        }
        public async Task<MemoryStream> GenerateExcel(int month, int year,string type)
        {


            GarmentShippingPackingList invoicePackingListBalance = await GetDataShippingInvoice(month, year);
            GarmentShippingPackingList invoicePackingListNow = await GetDataShippingInvoiceNow(month, year);
            GarmentShippingPackingList balance = await GetDataShippingInvoice(month, year);
            GarmentCurrency garmentCurrency = await GetCurrencyByCurrencyCodeDate("USD");
            List<ExportSalesDebtorReportViewModel> data = new List<ExportSalesDebtorReportViewModel>();

            var beginingMemo = from a in (from aa in _dbContext.GarmentFinanceMemorialDetails where aa.MemorialDate.Month < month && aa.MemorialDate.Year == year select new { aa.Id })
                               join c in _dbContext.GarmentFinanceMemorialDetailItems on a.Id equals c.MemorialDetailId
                               select new ExportSalesDebtorReportViewModel
                               {
                                   buyerCode = c.BuyerCode,
                                   buyerName = c.BuyerName,
                                   beginingBalance = Convert.ToDecimal(-c.Amount),
                                   receipt = 0,
                                   sales = 0,
                                   endBalance = 0,
                                   lessThan = 0,
                                   between = 0,
                                   moreThan = 0

                               };
            var beginingBankCashReceipt = from a in (from aa in _dbContext.GarmentFinanceBankCashReceiptDetails where aa.BankCashReceiptDate.Month < month && aa.BankCashReceiptDate.Year == year select new { aa.Id })
                                          join b in _dbContext.GarmentFinanceBankCashReceiptDetailItems on a.Id equals b.BankCashReceiptDetailId

                                          select new ExportSalesDebtorReportViewModel
                                          {
                                              buyerCode = b.BuyerCode,
                                              buyerName = b.BuyerName,
                                              beginingBalance = Convert.ToDecimal(-b.Amount),
                                              receipt = 0,
                                              sales = 0,
                                              endBalance = 0,
                                              lessThan = 0,
                                              between = 0,
                                              moreThan = 0

                                          };
            var beginingInvoice = from a in invoicePackingListBalance.data
                                  select new ExportSalesDebtorReportViewModel
                                  {
                                      buyerCode = a.buyerAgentCode,
                                      buyerName = a.buyerAgentName,
                                      beginingBalance = Convert.ToDecimal(a.amount),
                                      receipt = 0,
                                      sales = 0,
                                      endBalance = 0,
                                      lessThan = 0,
                                      between = 0,
                                      moreThan = 0

                                  };
            var beginingBalance = from a in balance.data
                                  select new ExportSalesDebtorReportViewModel
                                  {
                                      buyerCode = a.buyerAgentCode,
                                      buyerName = a.buyerAgentName,
                                      beginingBalance = Convert.ToDecimal(a.amount),
                                      receipt = 0,
                                      sales = 0,
                                      endBalance = 0,
                                      lessThan = 0,
                                      between = 0,
                                      moreThan = 0

                                  };
            var salesNow = from a in invoicePackingListNow.data
                           select new ExportSalesDebtorReportViewModel
                           {
                               buyerCode = a.buyerAgentCode,
                               buyerName = a.buyerAgentName,
                               beginingBalance = 0,
                               receipt = 0,
                               sales = Convert.ToDouble(a.amount),
                               endBalance = 0,
                               lessThan = 0,
                               between = 0,
                               moreThan = 0
                           };
            var memoNow = from a in (from aa in _dbContext.GarmentFinanceMemorialDetails where aa.MemorialDate.Month == month && aa.MemorialDate.Year == year select new { aa.Id })
                          join c in _dbContext.GarmentFinanceMemorialDetailItems on a.Id equals c.MemorialDetailId
                          select new ExportSalesDebtorReportViewModel
                          {
                              buyerCode = c.BuyerCode,
                              buyerName = c.BuyerName,
                              beginingBalance = 0,
                              receipt = Convert.ToDouble(c.Amount),
                              sales = 0,
                              endBalance = 0,
                              lessThan = 0,
                              between = 0,
                              moreThan = 0

                          };
            var bankCashReceiptNow = from a in (from aa in _dbContext.GarmentFinanceBankCashReceiptDetails where aa.BankCashReceiptDate.Month == month && aa.BankCashReceiptDate.Year == year select new { aa.Id })
                                     join b in _dbContext.GarmentFinanceBankCashReceiptDetailItems on a.Id equals b.BankCashReceiptDetailId

                                     select new ExportSalesDebtorReportViewModel
                                     {
                                         buyerCode = b.BuyerCode,
                                         buyerName = b.BuyerName,
                                         beginingBalance = 0,
                                         receipt = Convert.ToDouble(b.Amount),
                                         sales = 0,
                                         endBalance = 0,
                                         lessThan = 0,
                                         between = 0,
                                         moreThan = 0

                                     };

            var periodeMemo = from a in (from aa in _dbContext.GarmentFinanceMemorialDetails where aa.MemorialDate.Month <= month && aa.MemorialDate.Year == year select new { aa.Id })
                              join c in _dbContext.GarmentFinanceMemorialDetailItems on a.Id equals c.MemorialDetailId
                              select new ExportSalesDebtorReportViewModel
                              {
                                  buyerCode = c.BuyerCode,
                                  buyerName = c.BuyerName,
                                  beginingBalance = 0,
                                  receipt = 0,
                                  sales = 0,
                                  endBalance = 0,
                                  lessThan = -c.Amount,
                                  between = -c.Amount,
                                  moreThan = -c.Amount

                              };
            var periodeBankCashReceipt = from a in (from aa in _dbContext.GarmentFinanceBankCashReceiptDetails where aa.BankCashReceiptDate.Month <= month && aa.BankCashReceiptDate.Year == year select new { aa.Id })
                                         join b in _dbContext.GarmentFinanceBankCashReceiptDetailItems on a.Id equals b.BankCashReceiptDetailId

                                         select new ExportSalesDebtorReportViewModel
                                         {
                                             buyerCode = b.BuyerCode,
                                             buyerName = b.BuyerName,
                                             beginingBalance = 0,
                                             receipt = 0,
                                             sales = 0,
                                             endBalance = 0,
                                             lessThan = Convert.ToDouble(-b.Amount),
                                             between = Convert.ToDouble(-b.Amount),
                                             moreThan = Convert.ToDouble(-b.Amount)

                                         };
            var queryUnion = beginingMemo.Union(beginingBankCashReceipt).Union(beginingInvoice)
                        .Union(memoNow).Union(bankCashReceiptNow).Union(salesNow)
                        .Union(periodeBankCashReceipt).Union(periodeMemo);

            var _invoice = invoicePackingListBalance.data.Union(invoicePackingListNow.data);

            var querySum = queryUnion.ToList()
                   .GroupBy(x => new { x.buyerCode, x.buyerName }, (key, group) => new
                   {
                       buyerCode = key.buyerCode,
                       buyerName = key.buyerName,
                       beginingBalance = group.Sum(s => s.beginingBalance),
                       receipt = group.Sum(s => s.receipt),
                       sales = group.Sum(s => s.sales),
                       endBalance = group.Sum(s => s.endBalance),
                       lessThan = group.Sum(s => s.lessThan),
                       between = group.Sum(s => s.between),
                       moreThan = group.Sum(s => s.moreThan)

                   }).OrderByDescending(s => s.buyerName);
            int index = 1;
            var querytimeSpan = from aa in _invoice
                                select new timeSpanInvoice
                                {
                                    buyerCode = aa.buyerAgentCode,
                                    amount = Convert.ToDecimal(aa.amount),
                                    day = (DateTimeOffset.Now - aa.truckingDate).Days
                                };
            var querySumTS = querytimeSpan.ToList()
                   .GroupBy(x => new { x.buyerCode, x.day }, (key, group) => new
                   {
                       buyerCode = key.buyerCode,
                       amount = group.Sum(s => s.amount)
                   });
            foreach (var item in querySum)
            {
                ExportSalesDebtorReportViewModel model = new ExportSalesDebtorReportViewModel
                {
                    index = index,
                    buyerCode = item.buyerCode,
                    buyerName = item.buyerName,
                    beginingBalance = item.beginingBalance,
                    receipt = item.receipt,
                    sales = item.sales,
                    endBalance = Convert.ToDouble(item.beginingBalance) + item.sales - item.receipt,
                    lessThan = Convert.ToDouble((from aa in querytimeSpan where aa.buyerCode == item.buyerCode && aa.day <= 30 select aa.amount).FirstOrDefault()),
                    between = Convert.ToDouble((from aa in querytimeSpan where aa.buyerCode == item.buyerCode && (aa.day > 30 && aa.day < 60) select aa.amount).FirstOrDefault()),
                    moreThan = Convert.ToDouble((from aa in querytimeSpan where aa.buyerCode == item.buyerCode && aa.day > 60 select aa.amount).FirstOrDefault()),
                };
                data.Add(model);
                index++;
            }
          
            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Buyer", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo Awal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Penjualan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Penerimaan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo Akhir", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Umur Piutang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Umur Piutang1", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Umur Piutang2", DataType = typeof(String) });
          
            int counter = 0;
            result.Rows.Add("",
                    "", "", "", "", "", "", "< 30 hari", " 31- 60 hari", "> 61 hari");
            if (data.Count() == 0)
                result.Rows.Add("", "", "", 0, 0, 0, 0, 0, 0, 0); // to allow column name to be generated properly for empty data as template
            else
            {

                foreach (var item in data)
                {
                    counter++;
                    //DateTimeOffset date = item.date ?? new DateTime(1970, 1, 1);
                    //string dateString = date == new DateTime(1970, 1, 1) ? "-" : date.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(item.index, item.buyerCode, item.buyerName, Math.Round(item.beginingBalance,2),Math.Round(item.sales,2), Math.Round(item.receipt,2), Math.Round(item.endBalance,2), Math.Round(item.lessThan,2), Math.Round(item.between,2), Math.Round(item.moreThan,2));
                }
            }
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet 1");
                string monthValue = "";
                switch (month)
                {
                    case 1:
                        monthValue="JANUARI";
                        break;
                    case 2:
                        monthValue = "FEBRUARI";
                        break;
                    case 3:
                        monthValue = "MARET";
                        break;
                    case 4:
                        monthValue = "APRIL";
                        break;
                    case 5:
                        monthValue = "MEI";
                        break;
                    case 6:
                        monthValue = "JUNI";
                        break;
                    case 7:
                        monthValue = "JULI";
                        break;
                    case 8:
                        monthValue = "AGUSTUS";
                        break;
                    case 9:
                        monthValue = "SEPTEMBER";
                        break;
                    case 10:
                        monthValue = "OKTOBER";
                        break;
                    case 11:
                        monthValue = "NOVEMBER";
                        break;
                    default:
                        monthValue = "DESEMBER";
                        break;
                }
                worksheet.Cells["A1"].Value = "BULAN " +  monthValue + " " + year ;
                worksheet.Cells["A1"].Style.Font.Size = 14;
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A2"].LoadFromDataTable(result, true);
                worksheet.Cells["A" + 2 + ":A" + 3 + ""].Merge = true;
                worksheet.Cells["B" + 2 + ":B" + 3 + ""].Merge = true;
                worksheet.Cells["C" + 2 + ":C" + 3 + ""].Merge = true;
                worksheet.Cells["D" + 2 + ":D" + 3 + ""].Merge = true;
                worksheet.Cells["E" + 2 + ":E" + 3 + ""].Merge = true;
                worksheet.Cells["F" + 2 + ":F" + 3 + ""].Merge = true;
                worksheet.Cells["G" + 2 + ":G" + 3 + ""].Merge = true;
                worksheet.Cells["A" + 2 + ":J" + (counter + 3) + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A" + 2 + ":J" + (counter + 3) + ""].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A" + 2 + ":J" + (counter + 3) + ""].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A" + 2 + ":J" + (counter + 3) + ""].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A" + 2 + ":J" + 3 + ""].Style.Font.Bold = true;
                worksheet.Cells["H" + 2 + ":J" + 2 + ""].Merge = true;

                worksheet.Cells["A" + 2 + ":J" + 2 + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                
                foreach (var cell in worksheet.Cells["D" + 4 + ":J" + (counter + 3 ) + ""])
                {
                    cell.Value = Convert.ToDecimal(cell.Value);
                    cell.Style.Numberformat.Format = "#,##0.00";
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                }
                worksheet.Cells["A" + 2 + ":J" + (counter + 3) + ""].AutoFitColumns();

                 
                var stream = new MemoryStream();

                package.SaveAs(stream);

                return stream;
            }

        }
    }
}
