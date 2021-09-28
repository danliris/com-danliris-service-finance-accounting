using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.Rreports.ExportSalesDebtorReportController;
using Microsoft.Extensions.DependencyInjection;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.IO;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.Reports.LocalSalesDebtorReport;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentFinance.MemorialDetailLocal;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentFinance.BankCashReceiptDetailLocal;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.Reports.LocalSalesDebtorReport;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Reports.ExportSalesDebtorReport;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.JournalTransaction;
using Newtonsoft.Json;
using static Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Reports.ExportSalesDebtorReport.GarmentShippingPackingList;
using static Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.Reports.LocalSalesDebtorReport.GarmentShippingLocalSalesNote;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.Reports.LocalSalesDebtorReport
{
    public class LocalSalesDebtorReportService : ILocalSalesDebtorReportService
    {
        private readonly IServiceProvider _serviceProvider;
        private const string UserAgent = "finance-service";
        private readonly FinanceDbContext _dbContext;
        private readonly IIdentityService _identityService;
        protected DbSet<GarmentFinanceMemorialDetailLocalModel> DbSetMemoDetailLocal;
        protected DbSet<GarmentFinanceMemorialDetailLocalItemModel> DbSetDetailLocalItemMemo;
        protected DbSet<GarmentFinanceBankCashReceiptDetailLocalItemModel> DbSetBankCashDetailLocalItem;
        protected DbSet<GarmentFinanceBankCashReceiptDetailLocalModel> DBSetBankCashDetailLocal;

        public LocalSalesDebtorReportService(IServiceProvider serviceProvider, FinanceDbContext dbContext)
        {
            _dbContext = dbContext;
            _identityService = serviceProvider.GetService<IIdentityService>();
            _serviceProvider = serviceProvider;
        }

        public Task<MemoryStream> GenerateExcel(int month, int year, string type)
        {
            throw new NotImplementedException();
        }

        public async Task<List<LocalSalesDebtorReportViewModel>> GetMonitoring(int month, int year, int offset)
        {
            var data = await GetReportQuery(month, year);
            return data;
        }

        public async Task<List<LocalSalesDebtorReportViewModel>> GetReportQuery(int month, int year)
        {
            //GarmentShippingPackingList balance = await GetDataBalance();
            GarmentShippingLocalSalesNote salesBalanceNow = await GetDataLocalSalesNote("now", month, year);
            GarmentShippingLocalSalesNote salesBalance = await GetDataLocalSalesNote("balance", month, year);

            List<LocalSalesDebtorReportViewModel> data = new List<LocalSalesDebtorReportViewModel>();
            //GarmentCurrency garmentCurrency = await GetCurrency();

            var receiptBankCashReceiptDetailLocal = from a in (from aa in _dbContext.GarmentFinanceBankCashReceiptDetailLocals where aa.BankCashReceiptDate.AddHours(7).Month == month && aa.BankCashReceiptDate.AddHours(7).Year == year select new { aa.Id })
                                          join b in _dbContext.GarmentFinanceBankCashReceiptDetailLocalItems on a.Id equals b.BankCashReceiptDetailLocalId

                                          select new LocalSalesDebtorReportViewModel
                                          {
                                              buyerCode = b.BuyerCode,
                                              buyerName = b.BuyerName,
                                              beginingBalance = 0,
                                              receipt = Convert.ToDouble(b.Amount),
                                              sales = 0,
                                              endBalance = 0,
                                              normal = 0,
                                              oneThirty = 0,
                                              thirtySixty = 0,
                                              sixtyNinety = 0,
                                              moreThanNinety = 0
                                          };

            var receiptMemorialDetailLocal = from a in (from aa in _dbContext.GarmentFinanceMemorialDetailLocals where aa.MemorialDate.AddHours(7).Month == month && aa.MemorialDate.AddHours(7).Year == year select new { aa.Id })
                                             join c in _dbContext.GarmentFinanceMemorialDetailLocalItems on a.Id equals c.MemorialDetailLocalId

                                             select new LocalSalesDebtorReportViewModel
                                             {
                                                 buyerCode = c.BuyerCode,
                                                 buyerName = c.BuyerName,
                                                 beginingBalance = 0,
                                                 receipt = Convert.ToDouble(c.Amount),
                                                 sales = 0,
                                                 endBalance = 0,
                                                 normal = 0,
                                                 oneThirty = 0,
                                                 thirtySixty = 0,
                                                 sixtyNinety = 0,
                                                 moreThanNinety = 0
                                             };

            var salesBalanceLocal = from a in salesBalanceNow.data

                                  select new LocalSalesDebtorReportViewModel
                                  {
                                      buyerCode = a.buyer.Code,
                                      buyerName = a.buyer.Name,
                                      beginingBalance = 0,
                                      receipt = 0,
                                      sales = a.amount,
                                      endBalance = 0,
                                      normal = 0,
                                      oneThirty = 0,
                                      thirtySixty = 0,
                                      sixtyNinety = 0,
                                      moreThanNinety = 0
                                  };

            var localDebtorBalance = from a in _dbContext.GarmentLocalDebiturBalances

                                    select new LocalSalesDebtorReportViewModel
                                    {
                                        buyerCode = a.BuyerCode,
                                        buyerName = a.BuyerName,
                                        beginingBalance = Convert.ToDecimal(a.BalanceAmount),
                                        receipt = 0,
                                        sales = 0,
                                        endBalance = 0,
                                        normal = 0,
                                        oneThirty = 0,
                                        thirtySixty = 0,
                                        sixtyNinety = 0,
                                        moreThanNinety = 0
                                    };

            var beginningBalanceSales = from a in salesBalance.data

                                    select new LocalSalesDebtorReportViewModel
                                    {
                                        buyerCode = a.buyer.Code,
                                        buyerName = a.buyer.Name,
                                        beginingBalance = Convert.ToDecimal(a.amount),
                                        receipt = 0,
                                        sales = 0,
                                        endBalance = 0,
                                        normal = 0,
                                        oneThirty = 0,
                                        thirtySixty = 0,
                                        sixtyNinety = 0,
                                        moreThanNinety = 0
                                    };

            var beginningBalanceReceiptBankCashReceiptDetailLocal = from a in (from aa in _dbContext.GarmentFinanceBankCashReceiptDetailLocals where aa.BankCashReceiptDate.AddHours(7).Month < month && aa.BankCashReceiptDate.AddHours(7).Year == year select new { aa.Id })
                                                    join b in _dbContext.GarmentFinanceBankCashReceiptDetailLocalItems on a.Id equals b.BankCashReceiptDetailLocalId

                                                    select new LocalSalesDebtorReportViewModel
                                                    {
                                                        buyerCode = b.BuyerCode,
                                                        buyerName = b.BuyerName,
                                                        beginingBalance = -b.Amount,
                                                        receipt = 0,
                                                        sales = 0,
                                                        endBalance = 0,
                                                        normal = 0,
                                                        oneThirty = 0,
                                                        thirtySixty = 0,
                                                        sixtyNinety = 0,
                                                        moreThanNinety = 0
                                                    };

            var beginningBalanceReceiptMemorialDetailLocal = from a in (from aa in _dbContext.GarmentFinanceMemorialDetailLocals where aa.MemorialDate.AddHours(7).Month < month && aa.MemorialDate.AddHours(7).Year == year select new { aa.Id })
                                             join c in _dbContext.GarmentFinanceMemorialDetailLocalItems on a.Id equals c.MemorialDetailLocalId

                                             select new LocalSalesDebtorReportViewModel
                                             {
                                                 buyerCode = c.BuyerCode,
                                                 buyerName = c.BuyerName,
                                                 beginingBalance = -c.Amount,
                                                 receipt = 0,
                                                 sales = 0,
                                                 endBalance = 0,
                                                 normal = 0,
                                                 oneThirty = 0,
                                                 thirtySixty = 0,
                                                 sixtyNinety = 0,
                                                 moreThanNinety = 0
                                             };

            /*
             var beginingMemo = from a in (from aa in _dbContext.GarmentFinanceMemorialDetailLocals where aa.MemorialDate.AddHours(7).Month < month && aa.MemorialDate.AddHours(7).Year == year select new { aa.Id })
                               join c in _dbContext.GarmentFinanceMemorialDetailLocalItems on a.Id equals c.MemorialDetailLocalId

                               select new LocalSalesDebtorReportViewModel
                               {
                                   buyerCode = c.BuyerCode,
                                   buyerName = c.BuyerName,
                                   beginingBalance = Convert.ToDecimal(-c.Amount),
                                   receipt = 0,
                                   sales = 0,
                                   endBalance = 0,
                                   normal = 0,
                                   oneThirty = 0,
                                   thirtySixty = 0,
                                   sixtyNinety = 0,
                                   moreThanNinety = 0

                               };

            var beginingBankCashReceipt = from a in (from aa in _dbContext.GarmentFinanceBankCashReceiptDetailLocals where aa.BankCashReceiptDate.AddHours(7).Month < month && aa.BankCashReceiptDate.AddHours(7).Year == year select new { aa.Id })
                                          join b in _dbContext.GarmentFinanceBankCashReceiptDetailLocalItems on a.Id equals b.BankCashReceiptDetailLocalId

                                          select new LocalSalesDebtorReportViewModel
                                          {
                                              buyerCode = b.BuyerCode,
                                              buyerName = b.BuyerName,
                                              beginingBalance = Convert.ToDecimal(-b.Amount),
                                              receipt = 0,
                                              sales = 0,
                                              endBalance = 0,
                                              normal = 0,
                                              oneThirty = 0,
                                              thirtySixty = 0,
                                              sixtyNinety = 0,
                                              moreThanNinety = 0
                                          };

            var memoNow = from a in (from aa in _dbContext.GarmentFinanceMemorialDetailLocals where aa.MemorialDate.AddHours(7).Month == month && aa.MemorialDate.AddHours(7).Year == year select new { aa.Id })
                          join c in _dbContext.GarmentFinanceMemorialDetailLocalItems on a.Id equals c.MemorialDetailLocalId

                          select new LocalSalesDebtorReportViewModel
                          {
                              buyerCode = c.BuyerCode,
                              buyerName = c.BuyerName,
                              beginingBalance = 0,
                              receipt = Convert.ToDouble(c.Amount),
                              sales = 0,
                              endBalance = 0,
                              normal = 0,
                              oneThirty = 0,
                              thirtySixty = 0,
                              sixtyNinety = 0,
                              moreThanNinety = 0

                          };
            var bankCashReceiptNow = from a in (from aa in _dbContext.GarmentFinanceBankCashReceiptDetailLocals where aa.BankCashReceiptDate.AddHours(7).Month == month && aa.BankCashReceiptDate.AddHours(7).Year == year select new { aa.Id })
                                     join b in _dbContext.GarmentFinanceBankCashReceiptDetailLocalItems on a.Id equals b.BankCashReceiptDetailLocalId

                                     select new LocalSalesDebtorReportViewModel
                                     {
                                         buyerCode = b.BuyerCode,
                                         buyerName = b.BuyerName,
                                         beginingBalance = 0,
                                         receipt = Convert.ToDouble(b.Amount),
                                         sales = 0,
                                         endBalance = 0,
                                         normal = 0,
                                         oneThirty = 0,
                                         thirtySixty = 0,
                                         sixtyNinety = 0,
                                         moreThanNinety = 0
                                     };*/




            var queryUnion = localDebtorBalance
                .Union(salesBalanceLocal)
                .Union(receiptMemorialDetailLocal)
                .Union(receiptBankCashReceiptDetailLocal)
                .Union(beginningBalanceSales)
                .Union(beginningBalanceReceiptBankCashReceiptDetailLocal)
                .Union(beginningBalanceReceiptMemorialDetailLocal);

            var querySum = queryUnion.ToList()
                   .GroupBy(x => new { x.buyerCode, x.buyerName }, (key, group) => new
                   {
                       buyerCode = key.buyerCode,
                       buyerName = key.buyerName,
                       beginingBalance = group.Sum(s => s.beginingBalance),
                       receipt = group.Sum(s => s.receipt),
                       sales = group.Sum(s => s.sales),
                       endBalance = group.Sum(s => s.endBalance),
                       normal = group.Sum(s => s.normal),
                       oneThirty = group.Sum(s => s.oneThirty),
                       thirtySixty = group.Sum(s => s.thirtySixty),
                       sixtyNinety = group.Sum(s => s.sixtyNinety),
                       moreThanNinety = group.Sum(s => s.moreThanNinety)
                   }).OrderByDescending(s => s.buyerName);
            int index = 1;
            foreach (var item in querySum)
            {
                LocalSalesDebtorReportViewModel model = new LocalSalesDebtorReportViewModel
                {
                    index = index.ToString(),
                    buyerCode = item.buyerCode,
                    buyerName = item.buyerName,
                    beginingBalance = item.beginingBalance,
                    receipt = item.receipt,
                    sales = item.sales,
                    endBalance = Convert.ToDouble(item.beginingBalance) + item.sales - item.receipt,
                    normal = item.normal,
                    oneThirty = item.oneThirty,
                    thirtySixty = item.thirtySixty,
                    sixtyNinety = item.sixtyNinety,
                    moreThanNinety = item.moreThanNinety,
                    total = "TOTAL"
                };
                data.Add(model);
                index++;
            }
            var queryTOTAL = data.ToList()
                   .GroupBy(x => new { x.total }, (key, group) => new
                   {

                       beginingBalance = group.Sum(s => s.beginingBalance),
                       receipt = group.Sum(s => s.receipt),
                       sales = group.Sum(s => s.sales),
                       endBalance = group.Sum(s => s.endBalance),
                       normal = group.Sum(s => s.normal),
                       oneThirty = group.Sum(s => s.oneThirty),
                       thirtySixty = group.Sum(s => s.thirtySixty),
                       sixtyNinety = group.Sum(s => s.sixtyNinety),
                       moreThanNinety = group.Sum(s => s.moreThanNinety)
                   });
            foreach (var item in queryTOTAL)
            {
                LocalSalesDebtorReportViewModel model = new LocalSalesDebtorReportViewModel
                {
                    index = "",
                    buyerCode = "TOTAL",
                    buyerName = "",
                    beginingBalance = item.beginingBalance,
                    receipt = item.receipt,
                    sales = item.sales,
                    endBalance = item.endBalance,
                    normal = item.normal,
                    oneThirty = item.oneThirty,
                    thirtySixty = item.thirtySixty,
                    sixtyNinety = item.sixtyNinety,
                    moreThanNinety = item.moreThanNinety
                };
                data.Add(model);
            }

            return data;
        }

        public async Task<GarmentShippingLocalSalesNote> GetDataLocalSalesNote(string type, int month, int year)
        {
            GarmentShippingLocalSalesNote garmentShippingLocalSalesNote = new GarmentShippingLocalSalesNote();
            var http = _serviceProvider.GetService<IHttpClientService>();
            var uri = APIEndpoint.PackingInventory + $"garment-shipping/local-sales-notes/localSalesDebtor?type={type}&month={month}&year={year}";
            var response = await http.GetAsync(uri);


            var result = new BaseResponse<string>();

            if (response.IsSuccessStatusCode)
            {
                if (response.Content != null)
                {
                    var contentString = await response.Content.ReadAsStringAsync();
                    Dictionary<string, object> content = JsonConvert.DeserializeObject<Dictionary<string, object>>(contentString);
                    var dataString = content.GetValueOrDefault("data").ToString();
                    var listData = JsonConvert.DeserializeObject<List<ShippingLocalSalesNoteDto>>(dataString);
                    garmentShippingLocalSalesNote.data = listData;
                }
            }
            else
            {
                var err = await response.Content.ReadAsStringAsync();
            }


            return garmentShippingLocalSalesNote;
        }

        class timeSpanInvoice
        {
            internal string buyerCode { get; set; }
            internal decimal amount { get; set; }
            internal int day { get; set; }
            internal string type { get; set; }
            internal int invoiceId { get; set; }
        }
    }
}
