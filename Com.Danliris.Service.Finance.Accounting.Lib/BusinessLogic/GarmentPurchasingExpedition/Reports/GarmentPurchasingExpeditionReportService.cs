using Com.Danliris.Service.Finance.Accounting.Lib.Enums.Expedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Helpers;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentPurchasingExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentPurchasingExpedition.Reports
{
    public class GarmentPurchasingExpeditionReportService : IGarmentPurchasingExpeditionReportService
    {
        private const string UserAgent = "finance-service";
        private readonly FinanceDbContext _dbContext;
        private readonly IIdentityService _identityService;
        private readonly IServiceProvider _serviceProvider;

        public GarmentPurchasingExpeditionReportService(IServiceProvider serviceProvider)
        {
            _dbContext = serviceProvider.GetService<FinanceDbContext>();
            _identityService = serviceProvider.GetService<IIdentityService>();
            _serviceProvider = serviceProvider;
        }

        public MemoryStream GenerateExcel(int internalNoteId, int supplierId, GarmentPurchasingExpeditionPosition position, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var query = GetQuery(internalNoteId, supplierId, position, startDate, endDate);

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn() { ColumnName = "No. NI", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl. NI", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Supplier", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl. Jatuh Tempo", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Nomor Invoice", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "DPP", DataType = typeof(double) });
            dt.Columns.Add(new DataColumn() { ColumnName = "PPN", DataType = typeof(double) });
            dt.Columns.Add(new DataColumn() { ColumnName = "PPh", DataType = typeof(double) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Total", DataType = typeof(double) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tipe Bayar", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Term Pembayaran", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tempo", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Posisi", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl. Pembelian Kirim", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Admin", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl. Verif Terima", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl. Verif Kirim", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Verifikator", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl. Terima Kasir", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "No. Bukti Pengeluaran Bank", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl. Terima Pembelian", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Admin Pembelian", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Alasan", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl. Terima Accounting", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Admin Accounting", DataType = typeof(string) });

            if (query.Count() == 0)
            {
                dt.Rows.Add("", "", "", "", "", 0, 0, 0, 0, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
            }
            else
            {
                var data = query.OrderByDescending(s => s.LastModifiedUtc).ToList();
                foreach (var item in data)
                {
                    dt.Rows.Add(
                        item.InternalNoteNo,
                        item.InternalNoteDate.ToOffset(new TimeSpan(_identityService.TimezoneOffset, 0, 0)).ToString("d/M/yyyy", new CultureInfo("id-ID")),
                        item.SupplierName,
                        item.InternalNoteDueDate.ToOffset(new TimeSpan(_identityService.TimezoneOffset, 0, 0)).ToString("d/M/yyyy", new CultureInfo("id-ID")),
                        item.InvoicesNo,
                        item.DPP,
                        item.VAT,
                        item.IncomeTax,
                        item.TotalPaid,
                        item.PaymentType,
                        item.PaymentMethod,
                        item.PaymentDueDays.ToString(),
                        item.Position.ToDescriptionString(),
                        item.SendToVerificationDate?.ToOffset(new TimeSpan(_identityService.TimezoneOffset, 0, 0)).ToString("d/M/yyyy", new CultureInfo("id-ID")),
                        item.SendToVerificationBy,
                        item.VerificationAcceptedDate?.ToOffset(new TimeSpan(_identityService.TimezoneOffset, 0, 0)).ToString("d/M/yyyy", new CultureInfo("id-ID")),
                        item.SendToVerificationDate?.ToOffset(new TimeSpan(_identityService.TimezoneOffset, 0, 0)).ToString("d/M/yyyy", new CultureInfo("id-ID")),
                        item.VerificationAcceptedBy,
                        item.CashierAcceptedDate?.ToOffset(new TimeSpan(_identityService.TimezoneOffset, 0, 0)).ToString("d/M/yyyy", new CultureInfo("id-ID")),
                        null,
                        item.SendToPurchasingDate?.ToOffset(new TimeSpan(_identityService.TimezoneOffset, 0, 0)).ToString("d/M/yyyy", new CultureInfo("id-ID")),
                        item.SendToPurchasingBy,
                        item.SendToPurchasingRemark,
                        item.AccountingAcceptedDate?.ToOffset(new TimeSpan(_identityService.TimezoneOffset, 0, 0)).ToString("d/M/yyyy", new CultureInfo("id-ID")),
                        item.AccountingAcceptedBy
                        );
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(dt, "Laporan Ekspedisi Garment") }, true);
        }

        public List<GarmentPurchasingExpeditionModel> GetReport(int internalNoteId, int supplierId, GarmentPurchasingExpeditionPosition position, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var query = GetQuery(internalNoteId, supplierId, position, startDate, endDate);
            return query.ToList();
        }

        private IQueryable<GarmentPurchasingExpeditionModel> GetQuery(int internalNoteId, int supplierId, GarmentPurchasingExpeditionPosition position, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var result = _dbContext.GarmentPurchasingExpeditions.Where(entity => entity.InternalNoteDate >= startDate && entity.InternalNoteDate <= endDate);

            if (internalNoteId > 0)
                result = result.Where(entity => entity.InternalNoteId == internalNoteId);

            if (supplierId > 0)
                result = result.Where(entity => entity.SupplierId == supplierId);

            if (position > 0)
                result = result.Where(entity => entity.Position == position);

            if (position == GarmentPurchasingExpeditionPosition.Purchasing)
                result = result.Where(entity => !string.IsNullOrWhiteSpace(entity.SendToPurchasingRemark));

            if (internalNoteId > 0)
                result = result.Where(entity => entity.InternalNoteId == internalNoteId);

            return result;
        }
    }
}
