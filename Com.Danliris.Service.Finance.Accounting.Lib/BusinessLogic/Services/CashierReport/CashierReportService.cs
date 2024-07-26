using Com.Danliris.Service.Finance.Accounting.Lib.Helpers;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using Com.Moonlay.Models;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRequestDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRealizationDocumentExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CashierReport;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.CashierReport;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.CashierReport
{
    public class CashierReportService : ICashierReportService
    {
        private const string _UserAgent = "finance-service";
        protected DbSet<VbRequestModel> _DbSet;
        protected DbSet<RealizationVbModel> _RealizationDbSet;
        protected DbSet<VBRealizationDocumentExpeditionModel> _RealizationXPDCDbSet;

        private readonly IServiceProvider _serviceProvider;
        protected IIdentityService _IdentityService;
        public FinanceDbContext _DbContext;

        public CashierReportService(IServiceProvider serviceProvider, FinanceDbContext dbContext)
        {
            _DbContext = dbContext;
            _DbSet = dbContext.Set<VbRequestModel>();
            _RealizationDbSet = dbContext.Set<RealizationVbModel>();
            _RealizationXPDCDbSet = dbContext.Set<VBRealizationDocumentExpeditionModel>();
            _serviceProvider = serviceProvider;
            _IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        private List<CashierReportViewModel> NewGetReportQuery(string divisionName, string isInklaring, DateTimeOffset? approvalDateFrom, DateTimeOffset? approvalDateTo, int offSet)
        {
            var requestQuery = _DbContext.VBRequestDocuments.AsNoTracking().Where(s => s.ApprovalStatus > ApprovalStatus.Draft);

            if (approvalDateFrom.HasValue && approvalDateTo.HasValue)
            {
                requestQuery = requestQuery.Where(s => approvalDateFrom.Value.Date <= s.ApprovalDate.GetValueOrDefault().AddHours(offSet).Date && s.ApprovalDate.GetValueOrDefault().AddHours(offSet).Date <= approvalDateTo.Value.Date);
            }
            else if (approvalDateFrom.HasValue && !approvalDateTo.HasValue)
            {
                requestQuery = requestQuery.Where(s => approvalDateFrom.Value.Date <= s.Date.AddHours(offSet).Date);
            }
            else if (!approvalDateFrom.HasValue && approvalDateTo.HasValue)
            {
                requestQuery = requestQuery.Where(s => s.Date.AddHours(offSet).Date <= approvalDateTo.Value.Date);
            }

            IQueryable<CashierReportViewModel> result;

            result = (from rqst in requestQuery
                      where divisionName == "GARMENT" ? rqst.DocumentNo.Substring(0, 4) == "VB-G" : rqst.DocumentNo.Substring(0, 4) == "VB-T"
                            && (string.IsNullOrWhiteSpace(isInklaring) ? true : (isInklaring == "YA" ? rqst.IsInklaring == true : rqst.IsInklaring == false))

                      select new CashierReportViewModel()
                      {
                          DocumentNo = rqst.DocumentNo,
                          ApprovalDate = rqst.ApprovalDate.HasValue ? rqst.ApprovalDate.Value.ToOffset(new TimeSpan(offSet, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID")) : "-",
                          CreateBy = rqst.CreatedBy,
                          Purpose = rqst.Purpose,
                          CurrencyCode = rqst.CurrencyCode,
                          Amount = rqst.Amount,
                          TakenBy = rqst.TakenBy,
                          PhoneNumber = rqst.PhoneNumber,
                          Email = rqst.Email,
                          DivisioName = rqst.DocumentNo.Substring(0, 4) == "VB-G" ? "GARMENT" : "TEXTILE",
                          IsInklaring = rqst.IsInklaring == true ? "YA" : "TIDAK",
                          Aging = (int)(DateTimeOffset.Now.ToOffset(new TimeSpan(offSet, 0, 0)).Date - rqst.ApprovalDate.GetValueOrDefault().ToOffset(new TimeSpan(offSet, 0, 0)).Date).TotalDays,
                          CreatedUTC = rqst.CreatedUtc,

                      }).OrderBy(s => s.CreatedUTC).ThenBy(s => s.DocumentNo);

            return result.ToList();
        }
    
    public List<CashierReportViewModel> GetReport(string divisionName, string isInklaring, DateTimeOffset? approvalDateFrom, DateTimeOffset? approvalDateTo, int offSet)
    {
        var data = NewGetReportQuery(divisionName, isInklaring, approvalDateFrom, approvalDateTo, offSet);

        return data;
    }

    public MemoryStream GenerateExcel(string divisionName, string isInklaring, DateTimeOffset? approvalDateFrom, DateTimeOffset? approvalDateTo, int offSet)
    {
        var data = NewGetReportQuery(divisionName, isInklaring, approvalDateFrom, approvalDateTo, offSet);

        var dt = new DataTable();

        dt.Columns.Add(new DataColumn() { ColumnName = "No.", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Aging (Hari)", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Tgl Approval VB", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Nomor VB", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Jumlah VB", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Keterangan", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Nama Pembuat VB", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Nama Pengambil VB", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "No Telepon", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Email Pembuat VB", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Divisi", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "InKlaring", DataType = typeof(string) });

        if (data.Count == 0)
        {
            dt.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "");
        }
        else
        {
            int index = 0;
            foreach (var item in data)
            {
               index++;
               dt.Rows.Add(index, item.Aging, item.ApprovalDate, item.DocumentNo, item.CurrencyCode, item.Amount.ToString("#,##0.###0"), item.Purpose, item.CreateBy, item.TakenBy, item.PhoneNumber, item.Email, item.DivisioName, item.IsInklaring);
            }
        }

        return Excel.CreateExcelCashierReport(new KeyValuePair<DataTable, string>(dt, "Laporan Kasir VB"), approvalDateFrom.GetValueOrDefault(), approvalDateTo.GetValueOrDefault(), true);
    }

   }
}
