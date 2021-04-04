using Com.Danliris.Service.Finance.Accounting.Lib.Enums.Expedition;
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
        public static MemoryStream GenerateExcel(List<GarmentDispositionPaymentReportDto> data, DateTimeOffset startDate, DateTimeOffset endDate, int timezoneOffset, GarmentPurchasingExpeditionPosition position)
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
            dt.Columns.Add(new DataColumn() { ColumnName = "Nomor SJ", DataType = typeof(string) });
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
                    dt.Rows.Add(
                        item.DispositionNoteNo,
                        item.DispositionNoteDate.AddHours(timezoneOffset).ToString("dd/MM/yyyy"),
                        item.DispositionNoteDueDate.AddHours(timezoneOffset).ToString("dd/MM/yyyy"),
                        item.ProformaNo,
                        item.SupplierName,
                        item.CurrencyCode,
                        item.DPPAmount,
                        item.VATAmount,
                        item.IncomeTaxAmount,
                        item.OthersExpenditureAmount,
                        item.TotalAmount,
                        item.CategoryName,
                        item.PositionDescription,
                        item.SendToPurchasingRemark,
                        item.SendToVerificationDate != null ? item.SendToVerificationDate.GetValueOrDefault().AddHours(timezoneOffset).ToString("dd/MM/yyyy") : "",
                        item.VerificationAcceptedDate != null ? item.VerificationAcceptedDate.GetValueOrDefault().AddHours(timezoneOffset).ToString("dd/MM/yyyy") : "",
                        item.VerifiedDate != null ? item.VerifiedDate.GetValueOrDefault().AddHours(timezoneOffset).ToString("dd/MM/yyyy") : "",
                        item.VerifiedBy,
                        item.CashierAcceptedDate != null ? item.CashierAcceptedDate.GetValueOrDefault().AddHours(timezoneOffset).ToString("dd/MM/yyyy") : "",
                        item.BankExpenditureNoteDate != null ? item.BankExpenditureNoteDate.GetValueOrDefault().AddHours(timezoneOffset).ToString("dd/MM/yyyy") : "",
                        item.BankExpenditureNoteNo,
                        item.PaidAmount,
                        item.CurrencyCode,
                        item.ExternalPurchaseOrderNo,
                        item.DispositionQuantity,
                        item.DeliveryOrderNo,
                        item.DeliveryOrderQuantity,
                        item.DeliveryOrderNo,
                        item.BillsNo,
                        item.PaymentBillsNo,
                        item.CustomsNoteNo,
                        item.CustomsNoteDate != null ? item.CustomsNoteDate.GetValueOrDefault().AddHours(timezoneOffset).ToString("dd/MM/yyyy") : "",
                        item.UnitReceiptNoteNo,
                        item.InternalNoteNo,
                        item.InternalNoteDate != null ? item.InternalNoteDate.GetValueOrDefault().AddHours(timezoneOffset).ToString("dd/MM/yyyy") : "",
                        item.SendToVerificationby
                        );
                }
            }

            return Excel.CreateExcelDispositionReport(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(dt, "Laporan Ekspedisi Garment") }, startDate, endDate, timezoneOffset, position, true);
        }
    }
}
