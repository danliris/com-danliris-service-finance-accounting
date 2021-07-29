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

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Reports.ExportSalesDebtorReport
{
    public class ExportSalesDebtorReportService : IExportSalesDebtorReportService
    {
        private readonly IServiceProvider _serviceProvider;
        private const string UserAgent = "finance-service";
        private readonly FinanceDbContext _dbContext;
        private readonly IIdentityService _identityService;

        public ExportSalesDebtorReportService(IServiceProvider serviceProvider)
        {
            _dbContext = serviceProvider.GetService<FinanceDbContext>();
            _identityService = serviceProvider.GetService<IIdentityService>();
            _serviceProvider = serviceProvider;
        }

        public async Task<GarmentShippingPackingList> GetDataShippingInvoice(int month,int year)
        {
            GarmentShippingPackingList garmentShipping = new  GarmentShippingPackingList();
            var http = _serviceProvider.GetService<IHttpClientService>();
            var uri = APIEndpoint.PackingInventory + $"garment-shipping/invoices?Id={month}";
            var response = await http.GetAsync(uri);


            var result = new BaseResponse<string>();

            if (response.IsSuccessStatusCode)
            {
                var contentString = await response.Content.ReadAsStringAsync();
                Dictionary<string, object> content = JsonConvert.DeserializeObject<Dictionary<string, object>>(contentString);
                var dataString = content.GetValueOrDefault("data").ToString();
                var listData = JsonConvert.DeserializeObject<List<ShippingPackingDto>>(dataString);
                garmentShipping.data = listData;
            }
             

            else
            {
                var err = await response.Content.ReadAsStringAsync();

            }


            return garmentShipping;
        }
        

        public async  Task<List<ExportSalesDebtorReportViewModel>> GetMonitoring(int month, int year, int offset)
        {

            GarmentShippingPackingList garment = await GetDataShippingInvoice(month, year);
            List<ExportSalesDebtorReportViewModel> data = new List<ExportSalesDebtorReportViewModel>();
            return data;
        }
    }
}
