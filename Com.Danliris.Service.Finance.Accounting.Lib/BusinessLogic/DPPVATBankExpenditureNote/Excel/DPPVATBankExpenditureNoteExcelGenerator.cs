using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.DPPVATBankExpenditureNote.Excel
{
    public class DPPVATBankExpenditureNoteExcelGenerator
    {
        public static MemoryStream Generate(List<ReportDto> data, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet 1");

                SetTitle(worksheet, startDate, endDate);
                SetTableHeader(worksheet);
                SetData(worksheet, data);

                worksheet.Cells[worksheet.Cells.Address].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);

                return stream;
            }
        }

        private static void SetData(ExcelWorksheet worksheet, List<ReportDto> data)
        {
            var currentRow = 6;
            var index = 1;
            foreach (var datum in data)
            {
                worksheet.Cells[$"A{currentRow}"].Value = index;
                worksheet.Cells[$"A{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"A{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"B{currentRow}"].Value = datum.ExpenditureNoteNo;
                worksheet.Cells[$"B{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"B{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"C{currentRow}"].Value = datum.ExpenditureDate.ToString("dd/MM/yyyy");
                worksheet.Cells[$"C{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"C{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"D{currentRow}"].Value = datum.Amount;
                worksheet.Cells[$"D{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"D{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"E{currentRow}"].Value = datum.CategoryName;
                worksheet.Cells[$"E{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"E{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"F{currentRow}"].Value = datum.PaymentMethod;
                worksheet.Cells[$"F{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"F{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"G{currentRow}"].Value = datum.DPP;
                worksheet.Cells[$"G{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"G{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"H{currentRow}"].Value = datum.VAT;
                worksheet.Cells[$"H{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"H{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"I{currentRow}"].Value = "";
                worksheet.Cells[$"I{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"I{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"J{currentRow}"].Value = datum.InternalNoteAmount;
                worksheet.Cells[$"J{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"J{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"K{currentRow}"].Value = datum.CurrencyCode;
                worksheet.Cells[$"K{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"K{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"L{currentRow}"].Value = datum.BankName;
                worksheet.Cells[$"L{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"L{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"M{currentRow}"].Value = datum.DeliveryOrdersNo;
                worksheet.Cells[$"M{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"M{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"N{currentRow}"].Value = datum.SupplierName;
                worksheet.Cells[$"N{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"N{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"O{currentRow}"].Value = datum.InternalNoteNo;
                worksheet.Cells[$"O{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"O{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"P{currentRow}"].Value = datum.InvoiceNo;
                worksheet.Cells[$"P{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"P{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"Q{currentRow}"].Value = datum.InvoiceAmount;
                worksheet.Cells[$"Q{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"Q{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"R{currentRow}"].Value = datum.Amount;
                worksheet.Cells[$"R{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"R{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"S{currentRow}"].Value = datum.OutstandingAmount;
                worksheet.Cells[$"S{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"S{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"T{currentRow}"].Value = datum.Amount - datum.OutstandingAmount;
                worksheet.Cells[$"T{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"T{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"U{currentRow}"].Value = "";
                worksheet.Cells[$"U{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"U{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"V{currentRow}"].Value = datum.BillsNo;
                worksheet.Cells[$"V{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"V{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"W{currentRow}"].Value = datum.PaymentBills;
                worksheet.Cells[$"W{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"W{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"X{currentRow}"].Value = 0;
                worksheet.Cells[$"X{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"X{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"Y{currentRow}"].Value = datum.CurrencyRate;
                worksheet.Cells[$"Y{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"Y{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"Z{currentRow}"].Value = datum.CurrencyRate;
                worksheet.Cells[$"Z{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"Z{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"AA{currentRow}"].Value = 0;
                worksheet.Cells[$"AA{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"AA{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"AB{currentRow}"].Value = 0;
                worksheet.Cells[$"AB{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"AB{currentRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"AC{currentRow}"].Value = 0;
                worksheet.Cells[$"AC{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"AC{currentRow}"].Style.Font.Bold = true;

                index++;
            }
        }

        private static void SetTableHeader(ExcelWorksheet worksheet)
        {
            worksheet.Cells["A5"].Value = "No";
            worksheet.Cells["A5"].Style.Font.Size = 14;
            worksheet.Cells["A5"].Style.Font.Bold = true;
            worksheet.Cells["B5"].Value = "No Bukti Pengeluaran Bank";
            worksheet.Cells["B5"].Style.Font.Size = 14;
            worksheet.Cells["B5"].Style.Font.Bold = true;
            worksheet.Cells["C5"].Value = "Tanggal Bayar DPP+PPN";
            worksheet.Cells["C5"].Style.Font.Size = 14;
            worksheet.Cells["C5"].Style.Font.Bold = true;
            worksheet.Cells["D5"].Value = "Nilai Pembayaran Keluar";
            worksheet.Cells["D5"].Style.Font.Size = 14;
            worksheet.Cells["D5"].Style.Font.Bold = true;
            worksheet.Cells["E5"].Value = "Category";
            worksheet.Cells["E5"].Style.Font.Size = 14;
            worksheet.Cells["E5"].Style.Font.Bold = true;
            worksheet.Cells["F5"].Value = "Cara Pembayaran";
            worksheet.Cells["F5"].Style.Font.Size = 14;
            worksheet.Cells["F5"].Style.Font.Bold = true;
            worksheet.Cells["G5"].Value = "DPP";
            worksheet.Cells["G5"].Style.Font.Size = 14;
            worksheet.Cells["G5"].Style.Font.Bold = true;
            worksheet.Cells["H5"].Value = "PPN";
            worksheet.Cells["H5"].Style.Font.Size = 14;
            worksheet.Cells["H5"].Style.Font.Bold = true;
            worksheet.Cells["I5"].Value = "NK";
            worksheet.Cells["I5"].Style.Font.Size = 14;
            worksheet.Cells["I5"].Style.Font.Bold = true;
            worksheet.Cells["J5"].Value = "Total NI";
            worksheet.Cells["J5"].Style.Font.Size = 14;
            worksheet.Cells["J5"].Style.Font.Bold = true;
            worksheet.Cells["K5"].Value = "Mata Uang";
            worksheet.Cells["K5"].Style.Font.Size = 14;
            worksheet.Cells["K5"].Style.Font.Bold = true;
            worksheet.Cells["L5"].Value = "Bank Bayar PPh";
            worksheet.Cells["L5"].Style.Font.Size = 14;
            worksheet.Cells["L5"].Style.Font.Bold = true;
            worksheet.Cells["M5"].Value = "Kode Supplier";
            worksheet.Cells["M5"].Style.Font.Size = 14;
            worksheet.Cells["M5"].Style.Font.Bold = true;
            worksheet.Cells["N5"].Value = "Nama Supplier";
            worksheet.Cells["N5"].Style.Font.Size = 14;
            worksheet.Cells["N5"].Style.Font.Bold = true;
            worksheet.Cells["O5"].Value = "No SPB";
            worksheet.Cells["O5"].Style.Font.Size = 14;
            worksheet.Cells["O5"].Style.Font.Bold = true;
            worksheet.Cells["P5"].Value = "No Invoice";
            worksheet.Cells["P5"].Style.Font.Size = 14;
            worksheet.Cells["P5"].Style.Font.Bold = true;
            worksheet.Cells["Q5"].Value = "Nilai Invoice";
            worksheet.Cells["Q5"].Style.Font.Size = 14;
            worksheet.Cells["Q5"].Style.Font.Bold = true;
            worksheet.Cells["R5"].Value = "Nilai Bayar";
            worksheet.Cells["R5"].Style.Font.Size = 14;
            worksheet.Cells["R5"].Style.Font.Bold = true;
            worksheet.Cells["S5"].Value = "Outstanding";
            worksheet.Cells["S5"].Style.Font.Size = 14;
            worksheet.Cells["S5"].Style.Font.Bold = true;
            worksheet.Cells["T5"].Value = "Selisih yang Dibayar";
            worksheet.Cells["T5"].Style.Font.Size = 14;
            worksheet.Cells["T5"].Style.Font.Bold = true;
            worksheet.Cells["U5"].Value = "No SJ";
            worksheet.Cells["U5"].Style.Font.Size = 14;
            worksheet.Cells["U5"].Style.Font.Bold = true;
            worksheet.Cells["V5"].Value = "No BP";
            worksheet.Cells["V5"].Style.Font.Size = 14;
            worksheet.Cells["V5"].Style.Font.Bold = true;
            worksheet.Cells["W5"].Value = "No BB";
            worksheet.Cells["W5"].Style.Font.Size = 14;
            worksheet.Cells["W5"].Style.Font.Bold = true;
            worksheet.Cells["X5"].Value = "Nilai SJ";
            worksheet.Cells["X5"].Style.Font.Size = 14;
            worksheet.Cells["X5"].Style.Font.Bold = true;
            worksheet.Cells["Y5"].Value = "Rate Bayar Bulan Berjalan";
            worksheet.Cells["Y5"].Style.Font.Size = 14;
            worksheet.Cells["Y5"].Style.Font.Bold = true;
            worksheet.Cells["Z5"].Value = "Rate (disaat NOTA/BP diakui Hutang)";
            worksheet.Cells["Z5"].Style.Font.Size = 14;
            worksheet.Cells["Z5"].Style.Font.Bold = true;
            worksheet.Cells["AA5"].Value = "Nilai Bayar Rate Berjalan";
            worksheet.Cells["AA5"].Style.Font.Size = 14;
            worksheet.Cells["AA5"].Style.Font.Bold = true;
            worksheet.Cells["AB5"].Value = "Nilai Bayar Saat diakui Hutang";
            worksheet.Cells["AB5"].Style.Font.Size = 14;
            worksheet.Cells["AB5"].Style.Font.Bold = true;
            worksheet.Cells["AC5"].Value = "Selisih Kurs";
            worksheet.Cells["AC5"].Style.Font.Size = 14;
            worksheet.Cells["AC5"].Style.Font.Bold = true;
        }

        private static void SetTitle(ExcelWorksheet worksheet, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var company = "PT DAN LIRIS";
            var title = "LAPORAN BUKTI PENGELUARAN BANK DPP + PPN";

            var cultureInfo = new CultureInfo("id-ID");
            var date = $"PERIODE: {startDate.DateTime.ToString("dd MMMM yyyy", cultureInfo)} sampai dengan {endDate.DateTime.ToString("dd MMMM yyyy", cultureInfo)}";

            worksheet.Cells["A1"].Value = company;
            worksheet.Cells["A1:AC1"].Merge = true;
            worksheet.Cells["A1:AC1"].Style.Font.Size = 20;
            worksheet.Cells["A1:AC1"].Style.Font.Bold = true;
            worksheet.Cells["A2"].Value = title;
            worksheet.Cells["A2:AC2"].Merge = true;
            worksheet.Cells["A2:AC2"].Style.Font.Size = 20;
            worksheet.Cells["A2:AC2"].Style.Font.Bold = true;
            worksheet.Cells["A3"].Value = date;
            worksheet.Cells["A3:AC3"].Merge = true;
            worksheet.Cells["A3:AC3"].Style.Font.Size = 20;
            worksheet.Cells["A3:AC3"].Style.Font.Bold = true;
        }
    }
}
