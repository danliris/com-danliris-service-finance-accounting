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
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CashierVBRealizationReport;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.CashierVBRealizationReport;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.CashierVBRealizationReport
{
    public class CashierVBRealizationReportService : ICashierVBRealizationReportService
    {
        private const string _UserAgent = "finance-service";
        protected DbSet<VbRequestModel> _DbSet;
        protected DbSet<RealizationVbModel> _RealizationDbSet;
        protected DbSet<VBRealizationDocumentExpeditionModel> _RealizationXPDCDbSet;

        private readonly IServiceProvider _serviceProvider;
        protected IIdentityService _IdentityService;
        public FinanceDbContext _DbContext;

        public CashierVBRealizationReportService(IServiceProvider serviceProvider, FinanceDbContext dbContext)
        {
            _DbContext = dbContext;
            _DbSet = dbContext.Set<VbRequestModel>();
            _RealizationDbSet = dbContext.Set<RealizationVbModel>();
            _RealizationXPDCDbSet = dbContext.Set<VBRealizationDocumentExpeditionModel>();
            _serviceProvider = serviceProvider;
            _IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        private List<CashierVBRealizationViewModel> NewGetReportQuery(string divisionName, string isInklaring, DateTimeOffset? approvalDateFrom, DateTimeOffset? approvalDateTo, DateTimeOffset? realizeDateFrom, DateTimeOffset? realizeDateTo, int offSet)
        {
            var requestQuery = _DbContext.VBRequestDocuments.AsNoTracking().Where(s => s.ApprovalStatus > ApprovalStatus.Draft);

            var realizationQuery = _DbContext.VBRealizationDocuments.AsNoTracking();

            var expeditionQuery = _DbContext.VBRealizationDocumentExpeditions.AsNoTracking().Where(entity => entity.Position == VBRealizationDocumentExpedition.VBRealizationPosition.Cashier);

            //-------------------------------------------------------

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

            if (realizeDateFrom.HasValue && realizeDateTo.HasValue)
            {
                expeditionQuery = expeditionQuery.Where(s => realizeDateFrom.Value.Date <= s.CashierReceiptDate.GetValueOrDefault().AddHours(offSet).Date && s.CashierReceiptDate.GetValueOrDefault().AddHours(offSet).Date <= realizeDateTo.Value.Date);
            }
            else if (realizeDateFrom.HasValue && !realizeDateTo.HasValue)
            {
                expeditionQuery = expeditionQuery.Where(s => realizeDateFrom.Value.Date <= s.CreatedUtc.AddHours(offSet).Date);
            }
            else if (!realizeDateFrom.HasValue && realizeDateTo.HasValue)
            {
                expeditionQuery = expeditionQuery.Where(s => s.CreatedUtc.AddHours(offSet).Date <= realizeDateTo.Value.Date);
            }

            IQueryable<CashierVBRealizationViewModel> result;

            result = (from rqst in requestQuery
                      join realization in realizationQuery on rqst.Id equals realization.VBRequestDocumentId 
                      join expedition in expeditionQuery on realization.Id equals expedition.VBRealizationId

                      where divisionName == "GARMENT" ? realization.DocumentNo.Substring(0, 3) == "R-G" : realization.DocumentNo.Substring(0, 4) == "R-T"
                            && (string.IsNullOrWhiteSpace(isInklaring) ? true : (isInklaring == "YA" ? rqst.IsInklaring == true : rqst.IsInklaring == false))

                      select new CashierVBRealizationViewModel()
                      {
                          DocumentNo = realization.DocumentNo,
                          RealizationDate = expedition != null ? expedition.CashierReceiptDate : null,
                          CurrencyCode = rqst.CurrencyCode,
                          Amount = realization.Amount,
                          CreateBy = realization.CreatedBy,
                          Remark = realization.Remark,
                          TakenBy = realization.TakenBy,
                          PhoneNumber = realization.PhoneNumber,
                          Email = realization.Email,
                          DivisioName = realization.DocumentNo.Substring(0, 3) == "R-G" ? "GARMENT" : "TEXTILE",
                          IsInklaring = rqst.IsInklaring == true ? "YA" : "TIDAK",
                          CreatedUTC = realization.CreatedUtc,
                          CompletedBy = realization.CompletedBy,
                          BankAccountName = rqst.BankAccountName,

                      }).OrderBy(s => s.CreatedUTC).ThenBy(s => s.DocumentNo);

            return result.ToList();
        }
    
    public List<CashierVBRealizationViewModel> GetReport(string divisionName, string isInklaring, DateTimeOffset? approvalDateFrom, DateTimeOffset? approvalDateTo, DateTimeOffset? realizeDateFrom, DateTimeOffset? realizeDateTo, int offSet)
    {
        var data = NewGetReportQuery(divisionName, isInklaring, approvalDateFrom, approvalDateTo, realizeDateFrom, realizeDateTo, offSet);

        return data;
    }

    public MemoryStream GenerateExcel(string divisionName, string isInklaring, DateTimeOffset? approvalDateFrom, DateTimeOffset? approvalDateTo, DateTimeOffset? realizeDateFrom, DateTimeOffset? realizeDateTo, int offSet)
    {
        var data = NewGetReportQuery(divisionName, isInklaring, approvalDateFrom, approvalDateTo, realizeDateFrom, realizeDateTo, offSet);

        var dt = new DataTable();

        dt.Columns.Add(new DataColumn() { ColumnName = "No.", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Tgl Approval Realisasi VB", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Nomor Realisasi VB", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Nama Approve", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Jumlah Realiasi VB", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Bank", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Keterangan", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Nama Pembuat Realiasi VB", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Nama Pengambil Realisasi VB", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "No Telepon", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Email Pembuat Realisasi VB", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "Divisi", DataType = typeof(string) });
        dt.Columns.Add(new DataColumn() { ColumnName = "InKlaring", DataType = typeof(string) });

        if (data.Count == 0)
        {
            dt.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "");
        }
        else
        {
            int index = 0;
            foreach (var item in data)
            {
               index++;

               string RealizeDate = item.RealizationDate.Value.ToOffset(new TimeSpan(offSet, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID"));
                     
               dt.Rows.Add(index, RealizeDate, item.DocumentNo, item.CompletedBy, item.CurrencyCode, item.Amount.ToString("#,##0.###0"), item.BankAccountName, item.Remark, item.CreateBy, item.TakenBy, item.PhoneNumber, item.Email, item.DivisioName, item.IsInklaring);
            }
        }

        return Excel.CreateExcelCashierVBRealizationReport(new KeyValuePair<DataTable, string>(dt, "Laporan Kasir Realisasi VB"), realizeDateFrom.GetValueOrDefault(), realizeDateTo.GetValueOrDefault(), true);
    }

   }
}
