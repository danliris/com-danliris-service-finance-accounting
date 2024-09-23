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
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRealizationDocumentExpedition;

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

        private List<CashierReportViewModel> NewGetReportQuery(string divisionName, string isInklaring, string account, DateTimeOffset? approvalDateFrom, DateTimeOffset? approvalDateTo, int offSet)
        {
            var requestQuery = _DbContext.VBRequestDocuments.AsNoTracking().Where(s => s.ApprovalStatus == ApprovalStatus.Approved);
            var realizationQuery = _DbContext.VBRealizationDocuments.AsNoTracking();

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

            if (isInklaring == "YA")
            {
                requestQuery = requestQuery.Where(s => s.IsInklaring == true);
            }
            else if (isInklaring == "TIDAK")
            {
                requestQuery = requestQuery.Where(s => s.IsInklaring == false);
            }
            else
            {
                requestQuery = requestQuery.Where(s => s.IsInklaring == true || s.IsInklaring == false);
            }

            if (!string.IsNullOrWhiteSpace(account))
            {
                requestQuery = requestQuery.Where(w => w.ApprovedBy == account);
            }

            if (divisionName == "GARMENT")
            {
                requestQuery = requestQuery.Where(s => s.DocumentNo.Substring(0, 4) == "VB-G");
            }
            else if (divisionName == "TEXTILE")
            {
                requestQuery = requestQuery.Where(s => s.DocumentNo.Substring(0, 4) == "VB-T");
            }
            else
            {
                requestQuery = requestQuery.Where(s => s.DocumentNo.Substring(0, 4) == "VB-G" || s.DocumentNo.Substring(0, 4) == "VB-T");
            }

            IQueryable<CashierReportViewModel> result;

            result = (from rqst in requestQuery
                      join rlzd in realizationQuery on rqst.Id equals rlzd.VBRequestDocumentId into vbrealizations
                      from vbrealization in vbrealizations.DefaultIfEmpty()

                      select new CashierReportViewModel()
                      {
                          DocumentNo = rqst.DocumentNo,
                          RealizedNo = vbrealization == null ? "-": vbrealization.DocumentNo,
                          Position = vbrealization == null ? "-" : (((int)vbrealization.Position) == 1 ? "PEMBELIAN" : (((int)vbrealization.Position) == 2 ? "PENYERAHAN KE VERIFIKASI" : (((int)vbrealization.Position) == 3 ? "VERIFIKASI" : (((int)vbrealization.Position) == 4 ? "VERIFIKASI KE KASIR" : (((int)vbrealization.Position) == 5 ? "KASIR" : "RETUR"))))),
                          ApprovalDate = rqst.ApprovalDate.HasValue ? rqst.ApprovalDate.Value.ToOffset(new TimeSpan(offSet, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID")) : "-",
                          ApprovedBy = rqst.ApprovedBy,
                          CreateBy = rqst.CreatedBy,
                          Purpose = rqst.Purpose,
                          CurrencyCode = rqst.CurrencyCode,
                          Amount = rqst.Amount,
                          BankAccountName = rqst.BankAccountName,
                          TakenBy = rqst.TakenBy,
                          PhoneNumber = rqst.PhoneNumber,
                          Email = rqst.Email,
                          DivisioName = rqst.DocumentNo.Substring(0, 4) == "VB-G" ? "GARMENT" : "TEXTILE",
                          IsInklaring = rqst.IsInklaring == true ? "YA" : "TIDAK",
                          Aging = (int)(DateTimeOffset.Now.ToOffset(new TimeSpan(offSet, 0, 0)).Date - rqst.ApprovalDate.GetValueOrDefault().ToOffset(new TimeSpan(offSet, 0, 0)).Date).TotalDays,
                          CreatedUTC = rqst.CreatedUtc,
                          PositionVar  = vbrealization.Position
                      }).Where(s => s.PositionVar < VBRealizationPosition.Cashier).OrderBy(s => s.CreatedUTC).ThenBy(s => s.DocumentNo);

                 
            return result.ToList();
        }
    
    public List<CashierReportViewModel> GetReport(string divisionName, string isInklaring, string account, DateTimeOffset? approvalDateFrom, DateTimeOffset? approvalDateTo, int offSet)
    {
        var data = NewGetReportQuery(divisionName, isInklaring, account, approvalDateFrom, approvalDateTo, offSet);

        return data;
    }

    public MemoryStream GenerateExcel(string divisionName, string isInklaring, string account, DateTimeOffset? approvalDateFrom, DateTimeOffset? approvalDateTo, int offSet)
    {
        var data = NewGetReportQuery(divisionName, isInklaring, account, approvalDateFrom, approvalDateTo, offSet);

        var dt = new DataTable();

        dt.Columns.Add(new DataColumn() { ColumnName = "No.", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Aging (Hari)", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Tgl Approval VB", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Nama Approve", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Nomor VB", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Nomor Realisasi VB", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Posisi", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Jumlah VB", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Bank", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Keterangan", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Nama Pembuat VB", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Nama Pengambil VB", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "No Telepon", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Email Pembuat VB", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Divisi", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "InKlaring", DataType = typeof(string) });

        if (data.Count == 0)
        {
            dt.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
        }
        else
        {
            int index = 0;
            foreach (var item in data)
            {
               index++;
               dt.Rows.Add(index, item.Aging, item.ApprovalDate, item.ApprovedBy, item.DocumentNo, item.RealizedNo, item.Position, item.CurrencyCode, item.Amount.ToString("#,##0.#0"), item.BankAccountName, item.Purpose, item.CreateBy, item.TakenBy, item.PhoneNumber, item.Email, item.DivisioName, item.IsInklaring);
            }
        }

        return Excel.CreateExcelCashierReport(new KeyValuePair<DataTable, string>(dt, "Laporan Kasir VB"), approvalDateFrom.GetValueOrDefault(), approvalDateTo.GetValueOrDefault(), true);
    }

   }
}
