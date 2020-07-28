using Com.Danliris.Service.Finance.Accounting.Lib.Helpers;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.VBStatusReport;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.VBStatusReport;
using Com.Moonlay.Models;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.VBStatusReport
{
    public class VBStatusReportService : IVBStatusReportService
    {
        private const string _UserAgent = "finance-service";
        protected DbSet<VbRequestModel> _DbSet;
        protected DbSet<RealizationVbModel> _RealizationDbSet;
        private readonly IServiceProvider _serviceProvider;
        protected IIdentityService _IdentityService;
        public FinanceDbContext _DbContext;

        public VBStatusReportService(IServiceProvider serviceProvider, FinanceDbContext dbContext)
        {
            _DbContext = dbContext;
            _DbSet = dbContext.Set<VbRequestModel>();
            _RealizationDbSet = dbContext.Set<RealizationVbModel>();
            _serviceProvider = serviceProvider;
            _IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        private async Task<List<VBStatusReportViewModel>> GetReportQuery(int unitId, long vbRequestId, string applicantName, string clearanceStatus, DateTimeOffset? requestDateFrom, DateTimeOffset? requestDateTo, DateTimeOffset? realizeDateFrom, DateTimeOffset? realizeDateTo, int offSet)
        {
            var requestQuery = _DbSet.AsQueryable().Where(s => s.IsDeleted == false);

            if (unitId != 0)
            {
                requestQuery = requestQuery.Where(s => s.UnitId == unitId);
            }

            if (vbRequestId != 0)
            {
                requestQuery = requestQuery.Where(s => s.Id == vbRequestId);
            }

            if (!string.IsNullOrEmpty(applicantName))
            {
                requestQuery = requestQuery.Where(s => s.CreatedBy == applicantName);
            }

            if (requestDateFrom.HasValue && requestDateTo.HasValue)
            {
                requestQuery = requestQuery.Where(s => requestDateFrom.Value.Date <= s.Date.AddHours(offSet).Date && s.Date.AddHours(offSet).Date <= requestDateTo.Value.Date);
            }
            else if (requestDateFrom.HasValue && !requestDateTo.HasValue)
            {
                requestQuery = requestQuery.Where(s => requestDateFrom.Value.Date <= s.Date.AddHours(offSet).Date);
            }
            else if (!requestDateFrom.HasValue && requestDateTo.HasValue)
            {
                requestQuery = requestQuery.Where(s => s.Date.AddHours(offSet).Date <= requestDateTo.Value.Date);
            }

            var realizationQuery = _RealizationDbSet.AsQueryable().Where(s => s.IsDeleted == false);

            if (realizeDateFrom.HasValue && realizeDateTo.HasValue)
            {
                realizationQuery = realizationQuery.Where(s => realizeDateFrom.Value.Date <= s.Date.AddHours(offSet).Date && s.Date.AddHours(offSet).Date <= realizeDateTo.Value.Date);
            }
            else if (realizeDateFrom.HasValue && !realizeDateTo.HasValue)
            {
                realizationQuery = realizationQuery.Where(s => realizeDateFrom.Value.Date <= s.Date.AddHours(offSet).Date);
            }
            else if (!realizeDateFrom.HasValue && realizeDateTo.HasValue)
            {
                realizationQuery = realizationQuery.Where(s => s.Date.AddHours(offSet).Date <= realizeDateTo.Value.Date);
            }

            var result = new List<VBStatusReportViewModel>();

            switch (clearanceStatus.ToUpper())
            {
                case "CLEARANCE":
                    result = requestQuery
                    .Join(realizationQuery,
                    (rqst) => rqst.VBNo,
                    (real) => real.VBNo,
                    (rqst, real) => new VBStatusReportViewModel()
                    {
                        Id = rqst.Id,
                        VBNo = rqst.VBNo,
                        Date = rqst.Date,
                        DateEstimate = real.DateEstimate,
                        Unit = new Unit()
                        {
                            Id = rqst.Id,
                            Name = rqst.UnitName,
                        },
                        CreateBy = rqst.CreatedBy,
                        RealizationNo = real.VBNoRealize,
                        RealizationDate = real.Date,
                        Usage = rqst.Usage,
                        Aging = rqst.Complete_Status ? (int)(real.Date - rqst.Date).TotalDays : (int)(requestDateTo.GetValueOrDefault() - rqst.Date).TotalDays,
                        Amount = rqst.Amount,
                        RealizationAmount = real.Amount,
                        Difference = rqst.Amount - real.Amount,
                        Status = rqst.Complete_Status ? "Clearance" : "Outstanding",
                        LastModifiedUtc = real.LastModifiedUtc,
                    })
                    .Where(t => t.Status == "Clearance")
                    .OrderByDescending(s => s.LastModifiedUtc)
                    .ToList();
                    break;

                case "OUTSTANDING":
                    //decimal amount = 0;
                    //result = requestQuery.OrderBy(s => s.LastModifiedUtc)
                    //.Select(s => new VBStatusReportViewModel()
                    //{
                    //    Id = s.Id,
                    //    VBNo = s.VBNo,
                    //    Date = s.Date,
                    //    DateEstimate = s.DateEstimate,
                    //    Unit = new Unit()
                    //    {
                    //        Id = s.Id,
                    //        Name = s.UnitName,
                    //    },
                    //    CreateBy = s.CreatedBy,
                    //    Usage = s.Usage,
                    //    Aging = (int)(requestDateTo.GetValueOrDefault() - s.Date).TotalDays,
                    //    Amount = s.Amount,
                    //    RealizationAmount = amount,
                    //    Difference = amount,
                    //    Status = s.Complete_Status ? "Clearance" : "Outstanding",
                    //    LastModifiedUtc = s.LastModifiedUtc,
                    //})
                    //.Where(t => t.Status == "Outstanding")
                    //.OrderByDescending(s => s.LastModifiedUtc)
                    //.ToList();
                    result = requestQuery
                    .Join(realizationQuery,
                    (rqst) => rqst.VBNo,
                    (real) => real.VBNo,
                    (rqst, real) => new VBStatusReportViewModel()
                    {
                        Id = rqst.Id,
                        VBNo = rqst.VBNo,
                        Date = rqst.Date,
                        DateEstimate = real.DateEstimate,
                        Unit = new Unit()
                        {
                            Id = rqst.Id,
                            Name = rqst.UnitName,
                        },
                        CreateBy = rqst.CreatedBy,
                        RealizationNo = real.VBNoRealize,
                        RealizationDate = real.Date,
                        Usage = rqst.Usage,
                        Aging = (int)(requestDateTo.GetValueOrDefault() - rqst.Date).TotalDays,
                        Amount = rqst.Amount,
                        RealizationAmount = real.Amount,
                        Difference = rqst.Amount - real.Amount,
                        Status = rqst.Complete_Status ? "Clearance" : "Outstanding",
                        LastModifiedUtc = real.LastModifiedUtc,
                    })
                    .Where(t => t.Status == "Outstanding")
                    .OrderByDescending(s => s.LastModifiedUtc)
                    .ToList();
                    break;

                case "ALL":
                    result = requestQuery
                    .Join(realizationQuery,
                    (rqst) => rqst.VBNo,
                    (real) => real.VBNo,
                    (rqst, real) => new VBStatusReportViewModel()
                    {
                        Id = rqst.Id,
                        VBNo = rqst.VBNo,
                        Date = rqst.Date,
                        DateEstimate = real.DateEstimate,
                        Unit = new Unit()
                        {
                            Id = rqst.Id,
                            Name = rqst.UnitName,
                        },
                        CreateBy = rqst.CreatedBy,
                        RealizationNo = real.VBNoRealize,
                        RealizationDate = real.Date,
                        Usage = rqst.Usage,
                        Aging = rqst.Complete_Status ? (int)(real.Date - rqst.Date).TotalDays : (int)(requestDateTo.GetValueOrDefault() - rqst.Date).TotalDays,
                        Amount = rqst.Amount,
                        RealizationAmount = real.Amount,
                        Difference = rqst.Amount - real.Amount,
                        Status = rqst.Complete_Status ? "Clearance" : "Outstanding",
                        LastModifiedUtc = real.LastModifiedUtc,
                    })
                    .Where(t => t.Status == "Clearance" || t.Status == "Outstanding")
                    .OrderByDescending(s => s.LastModifiedUtc)
                    .ToList();
                    break;

            }

            return result.ToList();
        }

        public async Task<List<VBStatusReportViewModel>> GetReport(int unitId, long vbRequestId, string applicantName, string clearanceStatus, DateTimeOffset? requestDateFrom, DateTimeOffset? requestDateTo, DateTimeOffset? realizeDateFrom, DateTimeOffset? realizeDateTo, int offSet)
        {
            var data = await GetReportQuery(unitId, vbRequestId, applicantName, clearanceStatus, requestDateFrom, requestDateTo, realizeDateFrom, realizeDateTo, offSet);

            return data;
        }

        public async Task<MemoryStream> GenerateExcel(int unitId, long vbRequestId, string applicantName, string clearanceStatus, DateTimeOffset? requestDateFrom, DateTimeOffset? requestDateTo, DateTimeOffset? realizeDateFrom, DateTimeOffset? realizeDateTo, int offSet)
        {
            var data = await GetReportQuery(unitId, vbRequestId, applicantName, clearanceStatus, requestDateFrom, requestDateTo, realizeDateFrom, realizeDateTo, offSet);

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn() { ColumnName = "No VB", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tanggal VB", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Estimasi Tgl Realisasi", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Unit", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Pemohon VB", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "No Realisasi", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl Realisasi", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Keperluan VB", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Aging (Hari)", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Jumlah VB", DataType = typeof(double) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Realisasi", DataType = typeof(double) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Sisa (Kurang/Lebih)", DataType = typeof(double) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Status", DataType = typeof(string) });

            if (data.Count == 0)
            {
                dt.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "");
            }
            else
            {
                data = data.OrderByDescending(s => s.LastModifiedUtc).ToList();
                foreach (var item in data)
                {
                    dt.Rows.Add(item.VBNo, item.Date.ToOffset(new TimeSpan(offSet, 0, 0)).ToString("d/M/yyyy", new CultureInfo("id-ID")),
                    item.DateEstimate.ToOffset(new TimeSpan(offSet, 0, 0)).ToString("d/M/yyyy", new CultureInfo("id-ID")), item.Unit.Name, item.CreateBy, item.RealizationNo,
                    item.RealizationDate.ToOffset(new TimeSpan(offSet, 0, 0)).ToString("d/M/yyyy", new CultureInfo("id-ID")),
                    item.Usage, item.Aging, item.Amount, item.RealizationAmount, item.Difference, item.Status);
                }
            }

            return Excel.CreateExcelVBStatusReport(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(dt, "Status VB") }, requestDateFrom.GetValueOrDefault(), requestDateTo.GetValueOrDefault(), true);
        }

        public Task<int> CreateAsync(VbRequestModel model)
        {
            EntityExtension.FlagForCreate(model, _IdentityService.Username, _UserAgent);

            _DbContext.VbRequests.Add(model);

            return _DbContext.SaveChangesAsync();
        }

        public Task<VbRequestModel> ReadByIdAsync(int id)
        {
            return _DbContext.VbRequests.Where(entity => entity.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<VbRequestModel>> GetByApplicantName(string applicantName)
        {
            var data = await _DbContext.VbRequests.Where(entity => entity.CreatedBy == applicantName).ToListAsync();
            return data;
        }
    }
}
