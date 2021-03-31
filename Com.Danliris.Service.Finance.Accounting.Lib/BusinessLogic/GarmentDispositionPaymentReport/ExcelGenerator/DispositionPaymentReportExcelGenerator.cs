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
                    dt.Rows.Add(
                        item.DispositionNoteNo,
                        item.DispositionNoteDate.AddHours(timezoneOffset),
                        item.DispositionNoteDueDate.AddHours(timezoneOffset),
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
                        item.SendToVerificationDate,
                        item.VerifiedDate,
                        item.CashierAcceptedDate,
                        item.BankExpenditureNoteDate,
                        item.BankExpenditureNoteNo,
                        item.PaidAmount,
                        item.CurrencyCode,
                        item.ExternalPurchaseOrderNo,
                        item.DispositionQuantity,
                        item.DeliveryOrderNo,
                        item.DeliveryOrderQuantity,
                        item.BillsNo,
                        item.PaymentBillsNo,
                        item.CustomsNoteNo,
                        item.CustomsNoteDate,
                        item.UnitReceiptNoteNo,
                        item.InternalNoteNo,
                        item.InternalNoteDate,
                        item.SendToVerificationby
                        );
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(dt, "Laporan Ekspedisi Garment") }, true);
        }
    }
}
