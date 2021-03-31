using Com.Danliris.Service.Finance.Accounting.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentDispositionPaymentReport.ExcelGenerator
{
    public static class DispositionPaymentReportExcelGenerator
    {
        public static MemoryStream GenerateExcel(List<GarmentDispositionPaymentReportDto> data, DateTimeOffset startDate, DateTimeOffset endDate, int timezoneOffset)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn() { ColumnName = "No. Disposisi", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl. Disposisi", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl. Jatuh Tempo", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Nomor Proforma", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Supplier", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Kurs", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "DPP", DataType = typeof(double) });
            dt.Columns.Add(new DataColumn() { ColumnName = "PPN", DataType = typeof(double) });
            dt.Columns.Add(new DataColumn() { ColumnName = "PPh", DataType = typeof(double) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Biaya Lain-lain", DataType = typeof(double) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Total", DataType = typeof(double) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Kategori", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Posisi", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Alasan Retur", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl Pembelian Kirim", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tanggal Terima", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl Kirim", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Verifikator", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl Terima", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tgl Bayar", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "No. Bukti Pengeluaran Bank", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Nominal yang Dibayar", DataType = typeof(double) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "PO External", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Qty Disposisi", DataType = typeof(double) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Nomor Surat Jalan", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Qty Surat Jalan", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Nomor Surat Jalan", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Nomor BP Kecil", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Nomor BP Besar", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Nomor BeaCukai", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tanggal BeaCukai", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Nomor Bon Terima", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Nomor Nota Intern", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tanggal Nota Intern", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Staff", DataType = typeof(string) });

            if (data.Count == 0)
            {
                dt.Rows.Add("", "", "", "", "", "", 0, 0, 0, 0, 0, "", "", "", "", "", "", "", "", "", "", 0, "", "", 0, "", "", "","", "", "", "", "", "", "", "");
            }
            else
            {
                data = data.OrderByDescending(s => s.DispositionNoteDate).ToList();
                foreach (var item in data)
                {
                    //dt.Rows.Add(
                    //    item.InternalNoteNo,
                    //    item.InternalNoteDate.ToOffset(new TimeSpan(_identityService.TimezoneOffset, 0, 0)).ToString("d/M/yyyy", new CultureInfo("id-ID")),
                    //    item.SupplierName,
                    //    item.InternalNoteDueDate.ToOffset(new TimeSpan(_identityService.TimezoneOffset, 0, 0)).ToString("d/M/yyyy", new CultureInfo("id-ID")),
                    //    item.InvoicesNo,
                    //    item.DPP,
                    //    item.VAT,
                    //    item.IncomeTax,
                    //    item.TotalPaid,
                    //    item.PaymentType,
                    //    item.PaymentMethod,
                    //    item.PaymentDueDays.ToString(),
                    //    item.Position.ToDescriptionString(),
                    //    item.SendToVerificationDate?.ToOffset(new TimeSpan(_identityService.TimezoneOffset, 0, 0)).ToString("d/M/yyyy", new CultureInfo("id-ID")),
                    //    item.SendToVerificationBy,
                    //    item.VerificationAcceptedDate?.ToOffset(new TimeSpan(_identityService.TimezoneOffset, 0, 0)).ToString("d/M/yyyy", new CultureInfo("id-ID")),
                    //    item.SendToVerificationDate?.ToOffset(new TimeSpan(_identityService.TimezoneOffset, 0, 0)).ToString("d/M/yyyy", new CultureInfo("id-ID")),
                    //    item.VerificationAcceptedBy,
                    //    item.CashierAcceptedDate?.ToOffset(new TimeSpan(_identityService.TimezoneOffset, 0, 0)).ToString("d/M/yyyy", new CultureInfo("id-ID")),
                    //    null,
                    //    item.SendToPurchasingDate?.ToOffset(new TimeSpan(_identityService.TimezoneOffset, 0, 0)).ToString("d/M/yyyy", new CultureInfo("id-ID")),
                    //    item.SendToPurchasingBy,
                    //    item.SendToPurchasingRemark,
                    //    item.AccountingAcceptedDate?.ToOffset(new TimeSpan(_identityService.TimezoneOffset, 0, 0)).ToString("d/M/yyyy", new CultureInfo("id-ID")),
                    //    item.AccountingAcceptedBy
                    //    );
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(dt, "Laporan Ekspedisi Garment") }, true);
        }
    }
}
