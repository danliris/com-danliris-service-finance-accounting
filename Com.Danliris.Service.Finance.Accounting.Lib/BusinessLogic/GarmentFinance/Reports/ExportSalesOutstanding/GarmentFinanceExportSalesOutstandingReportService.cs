using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Reports.ExportSalesDebtorReport;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.GarmentFinance.Report.DebtorCard;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.GarmentFinance.Report.ExportSalesOutstanding;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using static Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Reports.ExportSalesDebtorReport.GarmentShippingPackingList;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentFinance.Reports.ExportSalesOutstanding
{
    public class GarmentFinanceExportSalesOutstandingReportService : IGarmentFinanceExportSalesOutstandingReportService
    {
        private readonly IServiceProvider _serviceProvider;
        private const string UserAgent = "finance-service";
        private readonly FinanceDbContext _dbContext;
        private readonly IIdentityService _identityService;

        public GarmentFinanceExportSalesOutstandingReportService(IServiceProvider serviceProvider, FinanceDbContext dbContext)
        {
            _dbContext = dbContext;

            _identityService = serviceProvider.GetService<IIdentityService>();
            _serviceProvider = serviceProvider;
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

        public async Task<List<GarmentFinanceExportSalesOutstandingReportViewModel>> GetReportQuery(int month, int year, string buyer, int offset)
        {
            GarmentShippingPackingList invoicePackingListNow = await GetDataShippingInvoiceNow(month, year);

            DateTime date = new DateTime(year, month, 1);

            List<GarmentFinanceExportSalesOutstandingReportViewModel> data = new List<GarmentFinanceExportSalesOutstandingReportViewModel>();

            var invoice =  from a in invoicePackingListNow.data
                           select new GarmentFinanceExportSalesOutstandingReportViewModel
                           {
                               Amount = Convert.ToDecimal(a.amount),
                               InvoiceId =Convert.ToInt32( a.invoiceId),
                               TruckingDate = a.truckingDate
                           };
            var memorial = from a in (from aa in _dbContext.GarmentFinanceMemorialDetails where aa.MemorialDate.AddHours(7).Date.Month <= month && aa.MemorialDate.AddHours(7).Date.Year == year select new { aa.Id, aa.MemorialNo, aa.MemorialDate })
                          join c in _dbContext.GarmentFinanceMemorialDetailItems on a.Id equals c.MemorialDetailId
                          
                           where (buyer == null || (buyer != null && buyer != "" && c.BuyerCode == buyer))
                           select new GarmentFinanceExportSalesOutstandingReportViewModel
                          {
                              Amount = Convert.ToDecimal(-c.Amount),
                              InvoiceNo = c.InvoiceNo,
                              InvoiceId= c.InvoiceId
                              
                          };
            var bankCashReceipt = from a in (from aa in _dbContext.GarmentFinanceBankCashReceiptDetails where aa.BankCashReceiptDate.AddHours(7).Date.Month <= month && aa.BankCashReceiptDate.AddHours(7).Date.Year == year select new { aa.Id, aa.BankCashReceiptNo, aa.BankCashReceiptDate })
                                     join b in _dbContext.GarmentFinanceBankCashReceiptDetailItems on a.Id equals b.BankCashReceiptDetailId
                                  where (buyer == null || (buyer != null && buyer != "" && b.BuyerCode == buyer))
                                  select new GarmentFinanceExportSalesOutstandingReportViewModel
                                     {
                                         Amount = Convert.ToDecimal(-b.Amount),
                                         InvoiceNo = b.InvoiceNo,
                                         InvoiceId = b.InvoiceId
                                     };
            var unionQuery = memorial.Union(bankCashReceipt).Union(invoice);
            var querySum= unionQuery.GroupBy(a=> new { a.InvoiceId }, (key, group) => new
            {
                invoiveId = key.InvoiceId,
                balance = group.Sum(s => s.Amount)
            });
            decimal total = 0;
            foreach (var item in invoice)
            {
                GarmentFinanceExportSalesOutstandingReportViewModel model = new GarmentFinanceExportSalesOutstandingReportViewModel
                {
                    InvoiceNo = (from a in memorial where item.InvoiceId== a.InvoiceId select a.InvoiceNo).FirstOrDefault() == null ? (from a in bankCashReceipt where item.InvoiceId == a.InvoiceId select a.InvoiceNo).FirstOrDefault(): (from a in memorial where item.InvoiceId == a.InvoiceId select a.InvoiceNo).FirstOrDefault(),
                    TruckingDate = item.TruckingDate,
                    Amount = querySum.First(a => a.invoiveId == item.InvoiceId).balance
                };
                data.Add(model);
                total += model.Amount;
            }

            var lastRow = new GarmentFinanceExportSalesOutstandingReportViewModel
            {
                InvoiceNo = "Dipindahkan",
                TruckingDate = null,
                Amount=total
            };
            data.Add(lastRow);
            return data;
        }

        public Tuple<MemoryStream, string> GenerateExcel(int month, int year, string buyer, int offset)
        {
            throw new NotImplementedException();
        }

        public List<GarmentFinanceExportSalesOutstandingReportViewModel> GetMonitoring(int month, int year, string buyer, int offset)
        {
            var Data = GetReportQuery(month, year, buyer, offset);
            return Data;
        }
    }
}
