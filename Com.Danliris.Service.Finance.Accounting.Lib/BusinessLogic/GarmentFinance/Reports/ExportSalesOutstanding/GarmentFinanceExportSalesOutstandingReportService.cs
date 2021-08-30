using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.GarmentFinance.Report.DebtorCard;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.GarmentFinance.Report.ExportSalesOutstanding;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

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

        public List<GarmentShippingInvoiceViewModel> GetDataShippingInvoiceNow(int month, int year, string buyer)
        {
            var uri = APIEndpoint.PackingInventory + $"garment-shipping/invoices/packing-list-for-debtor-card-now?month={month}&year={year}&buyer={buyer}";
            var httpService = (IHttpClientService)_serviceProvider.GetService(typeof(IHttpClientService));
            List<GarmentShippingInvoiceViewModel> garmentShipping = new List<GarmentShippingInvoiceViewModel>();
            var http = _serviceProvider.GetService<IHttpClientService>();
            var response = httpService.GetAsync(uri).Result.Content.ReadAsStringAsync();
            Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Result);
            var json = result.Single(p => p.Key.Equals("data")).Value;
            var listData = JsonConvert.DeserializeObject<List<GarmentShippingInvoiceViewModel>>(json.ToString());
            foreach (var item in listData)
            {
                garmentShipping.Add(item);
            }

            return garmentShipping;
        }

        public List<GarmentFinanceExportSalesOutstandingReportViewModel> GetReportQuery(int month, int year, string buyer, int offset)
        {
            var invoicePackingListNow = GetDataShippingInvoiceNow(month, year, buyer);

            DateTime date = new DateTime(year, month, 1);

            List<GarmentFinanceExportSalesOutstandingReportViewModel> data = new List<GarmentFinanceExportSalesOutstandingReportViewModel>();

            var invoice =  from a in invoicePackingListNow
                           select new GarmentFinanceExportSalesOutstandingReportViewModel
                           {
                               Amount = Convert.ToDecimal(a.amount),
                               InvoiceNo = a.invoiceNo,
                               TruckingDate = a.truckingDate
                           };
            var memorial = from a in (from aa in _dbContext.GarmentFinanceMemorialDetails where aa.MemorialDate.AddHours(7).Date.Month <= month && aa.MemorialDate.AddHours(7).Date.Year == year select new { aa.Id, aa.MemorialNo, aa.MemorialDate })
                          join c in _dbContext.GarmentFinanceMemorialDetailItems on a.Id equals c.MemorialDetailId
                          where c.BuyerCode == buyer
                          select new GarmentFinanceExportSalesOutstandingReportViewModel
                          {
                              Amount = Convert.ToDecimal(-c.Amount),
                              InvoiceNo = c.InvoiceNo,
                          };
            var bankCashReceipt = from a in (from aa in _dbContext.GarmentFinanceBankCashReceiptDetails where aa.BankCashReceiptDate.AddHours(7).Date.Month <= month && aa.BankCashReceiptDate.AddHours(7).Date.Year == year select new { aa.Id, aa.BankCashReceiptNo, aa.BankCashReceiptDate })
                                     join b in _dbContext.GarmentFinanceBankCashReceiptDetailItems on a.Id equals b.BankCashReceiptDetailId
                                     where b.BuyerCode == buyer
                                     select new GarmentFinanceExportSalesOutstandingReportViewModel
                                     {
                                         Amount = Convert.ToDecimal(-b.Amount),
                                         InvoiceNo = b.InvoiceNo,
                                     };
            var unionQuery = memorial.Union(bankCashReceipt).Union(invoice);
            var querySum= unionQuery.GroupBy(a=> new { a.InvoiceNo }, (key, group) => new
            {
                invoiceNo = key.InvoiceNo,
                balance = group.Sum(s => s.Amount)
            });
            decimal total = 0;
            foreach (var item in invoice)
            {
                GarmentFinanceExportSalesOutstandingReportViewModel model = new GarmentFinanceExportSalesOutstandingReportViewModel
                {
                    InvoiceNo = item.InvoiceNo,
                    TruckingDate = item.TruckingDate,
                    Amount = querySum.First(a => a.invoiceNo == item.InvoiceNo).balance
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
