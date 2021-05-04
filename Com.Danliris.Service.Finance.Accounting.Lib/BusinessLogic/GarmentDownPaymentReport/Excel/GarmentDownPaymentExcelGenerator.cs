using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentDownPaymentReport.Excel
{
    public class GarmentDownPaymentExcelGenerator
    {
        public static MemoryStream Generate(List<GarmentDownPaymentReportDto> data, DateTimeOffset date, int timezoneOffset)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet 1");

                SetTitle(worksheet, date, timezoneOffset);
                SetTableHeader(worksheet, timezoneOffset);
                SetData(worksheet, data, timezoneOffset);

                worksheet.Cells[worksheet.Cells.Address].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);

                return stream;
            }
        }

        private static void SetData(ExcelWorksheet worksheet, List<GarmentDownPaymentReportDto> data, int timezoneOffset)
        {
            //CultureInfo cultureInfo = new CultureInfo("en-us");
            //var currentRow = 6;
            //var index = 1;
            //foreach (var datum in data)
            //{
            //    //compare max count disposisiton payment and memo
            //    var countDispo = datum.DispositionPayments.Count;
            //    var countMemo = datum.MemoDocuments.Count;
            //    var maxRow = 0;
            //    if(countDispo > countMemo)
            //    {
            //        maxRow = countDispo;
            //    }
            //    else
            //    {
            //        maxRow = countMemo;
            //    }

            //    for(int item=0;item<maxRow)

            //    worksheet.Cells[$"A{currentRow}"].Value = index;
            //    worksheet.Cells[$"A{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"B{currentRow}"].Value = datum.ExpenditureNoteNo;
            //    worksheet.Cells[$"B{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"C{currentRow}"].Value = datum.ExpenditureDate.AddHours(timezoneOffset).ToString("dd/MM/yyyy");
            //    worksheet.Cells[$"C{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"D{currentRow}"].Value = datum.Amount.ToString("N2", cultureInfo);
            //    worksheet.Cells[$"D{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"D{currentRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            //    worksheet.Cells[$"E{currentRow}"].Value = datum.CategoryName;
            //    worksheet.Cells[$"E{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"F{currentRow}"].Value = datum.PaymentMethod;
            //    worksheet.Cells[$"F{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"G{currentRow}"].Value = datum.InvoiceAmount.ToString("N2", cultureInfo);
            //    worksheet.Cells[$"G{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"G{currentRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            //    worksheet.Cells[$"H{currentRow}"].Value = (datum.InvoiceAmount * 0.1).ToString("N2", cultureInfo);
            //    worksheet.Cells[$"H{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"H{currentRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            //    worksheet.Cells[$"I{currentRow}"].Value = 0.0;
            //    worksheet.Cells[$"I{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"J{currentRow}"].Value = (datum.InvoiceAmount + (datum.InvoiceAmount * 0.1)).ToString("N2", cultureInfo);
            //    worksheet.Cells[$"J{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"J{currentRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            //    worksheet.Cells[$"K{currentRow}"].Value = datum.CurrencyCode;
            //    worksheet.Cells[$"K{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"L{currentRow}"].Value = datum.BankName;
            //    worksheet.Cells[$"L{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"M{currentRow}"].Value = datum.SupplierCode;
            //    worksheet.Cells[$"M{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"N{currentRow}"].Value = datum.SupplierName;
            //    worksheet.Cells[$"N{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"O{currentRow}"].Value = datum.InternalNoteNo;
            //    worksheet.Cells[$"O{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"P{currentRow}"].Value = datum.InvoiceNo;
            //    worksheet.Cells[$"P{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"Q{currentRow}"].Value = datum.InvoiceAmount.ToString("N2", cultureInfo);
            //    worksheet.Cells[$"Q{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"Q{currentRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            //    worksheet.Cells[$"R{currentRow}"].Value = datum.InvoiceAmount.ToString("N2", cultureInfo);
            //    worksheet.Cells[$"R{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"R{currentRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            //    worksheet.Cells[$"S{currentRow}"].Value = datum.OutstandingAmount;
            //    worksheet.Cells[$"S{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"T{currentRow}"].Value = datum.OutstandingAmount;
            //    worksheet.Cells[$"T{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"U{currentRow}"].Value = datum.DeliveryOrdersNo;
            //    worksheet.Cells[$"U{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"V{currentRow}"].Value = datum.BillsNo;
            //    worksheet.Cells[$"V{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"W{currentRow}"].Value = datum.PaymentBills;
            //    worksheet.Cells[$"W{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"X{currentRow}"].Value = datum.InvoiceAmount.ToString("N2", cultureInfo);
            //    worksheet.Cells[$"X{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"X{currentRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            //    worksheet.Cells[$"Y{currentRow}"].Value = datum.CurrencyRate;
            //    worksheet.Cells[$"Y{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"Z{currentRow}"].Value = datum.CurrencyRate;
            //    worksheet.Cells[$"Z{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"AA{currentRow}"].Value = (datum.InvoiceAmount * datum.CurrencyRate).ToString("N2", cultureInfo);
            //    worksheet.Cells[$"AA{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"AA{currentRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            //    worksheet.Cells[$"AB{currentRow}"].Value = (datum.InvoiceAmount * datum.CurrencyRate).ToString("N2", cultureInfo);
            //    worksheet.Cells[$"AB{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"AB{currentRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            //    worksheet.Cells[$"AC{currentRow}"].Value = (datum.InvoiceAmount - (datum.InvoiceAmount * datum.CurrencyRate)).ToString("N2", cultureInfo);
            //    worksheet.Cells[$"AC{currentRow}"].Style.Font.Size = 14;
            //    worksheet.Cells[$"AC{currentRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            //    index++;
            //    currentRow++;
            //}

            //worksheet.Cells[$"A6:AC{currentRow}"].AutoFitColumns();
            //worksheet.Cells[$"A6:AC{currentRow}"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            //worksheet.Cells[$"A6:AC{currentRow}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            //worksheet.Cells[$"A6:AC{currentRow}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            //worksheet.Cells[$"A6:AC{currentRow}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        }

        private static void SetTableHeader(ExcelWorksheet worksheet, int timezoneOffset)
        {
            worksheet.Cells["A5"].Value = "NO";
            worksheet.Cells["A5"].Style.Font.Size = 14;
            worksheet.Cells["A5"].Style.Font.Bold = true;
            worksheet.Cells["A5"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells["A5"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            worksheet.Cells["A5:A6"].Merge = true;
            worksheet.Cells["A5:A6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["B5"].Value = "BUKTI";
            worksheet.Cells["B5"].Style.Font.Size = 14;
            worksheet.Cells["B5"].Style.Font.Bold = true;
            worksheet.Cells["B5:C5"].Merge = true;
            worksheet.Cells["B5:C5"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            worksheet.Cells["B6"].Value = "TGL.";
            worksheet.Cells["B6"].Style.Font.Size = 14;
            worksheet.Cells["B6"].Style.Font.Bold = true;
            worksheet.Cells["B6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            worksheet.Cells["C6"].Value = "NO.";
            worksheet.Cells["C6"].Style.Font.Size = 14;
            worksheet.Cells["C6"].Style.Font.Bold = true;
            worksheet.Cells["C6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["D5"].Value = "DISPOSISI";
            worksheet.Cells["D5"].Style.Font.Size = 14;
            worksheet.Cells["D5"].Style.Font.Bold = true;
            worksheet.Cells["D5:D6"].Merge = true;
            worksheet.Cells["D5:D6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);



            worksheet.Cells["E5"].Value = "SUPPLIER";
            worksheet.Cells["E5"].Style.Font.Size = 14;
            worksheet.Cells["E5"].Style.Font.Bold = true;
            worksheet.Cells["E5:E6"].Merge = true;
            worksheet.Cells["E5:E6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);



            worksheet.Cells["F5"].Value = "UMUR UANG MUKA";
            worksheet.Cells["F5"].Style.Font.Size = 14;
            worksheet.Cells["F5"].Style.Font.Bold = true;
            worksheet.Cells["F5:F6"].Merge = true;
            worksheet.Cells["F5:F6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);



            worksheet.Cells["G5"].Value = "SALDO AWAL";
            worksheet.Cells["G5"].Style.Font.Size = 14;
            worksheet.Cells["G5"].Style.Font.Bold = true;
            worksheet.Cells["G5:J5"].Merge = true;
            worksheet.Cells["G5:J5"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["G6"].Value = "NILAI DISPO";
            worksheet.Cells["G6"].Style.Font.Size = 14;
            worksheet.Cells["G6"].Style.Font.Bold = true;
            worksheet.Cells["G6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            worksheet.Cells["H6"].Value = "NILAI BAYAR";
            worksheet.Cells["H6"].Style.Font.Size = 14;
            worksheet.Cells["H6"].Style.Font.Bold = true;
            worksheet.Cells["H6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["I6"].Value = "KURS";
            worksheet.Cells["I6"].Style.Font.Size = 14;
            worksheet.Cells["I6"].Style.Font.Bold = true;
            worksheet.Cells["I6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["J6"].Value = "RUPIAH";
            worksheet.Cells["J6"].Style.Font.Size = 14;
            worksheet.Cells["J6"].Style.Font.Bold = true;
            worksheet.Cells["J6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["K5"].Value = "PEMASUKAN UANG MUKA";
            worksheet.Cells["K5"].Style.Font.Size = 14;
            worksheet.Cells["K5"].Style.Font.Bold = true;
            worksheet.Cells["K5:N5"].Merge = true;
            worksheet.Cells["K5:N5"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["K6"].Value = "NILAI DISPO";
            worksheet.Cells["K6"].Style.Font.Size = 14;
            worksheet.Cells["K6"].Style.Font.Bold = true;
            worksheet.Cells["K6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["L6"].Value = "NILAI BAYAR";
            worksheet.Cells["L6"].Style.Font.Size = 14;
            worksheet.Cells["L6"].Style.Font.Bold = true;
            worksheet.Cells["L6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["M6"].Value = "KURS";
            worksheet.Cells["M6"].Style.Font.Size = 14;
            worksheet.Cells["M6"].Style.Font.Bold = true;
            worksheet.Cells["M6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["N6"].Value = "RUPIAH";
            worksheet.Cells["N6"].Style.Font.Size = 14;
            worksheet.Cells["N6"].Style.Font.Bold = true;
            worksheet.Cells["N6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["O5"].Value = "MEMO";
            worksheet.Cells["O5"].Style.Font.Size = 14;
            worksheet.Cells["O5"].Style.Font.Bold = true;
            worksheet.Cells["O5:P5"].Merge = true;
            worksheet.Cells["O5:P5"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["O6"].Value = "NO MEMO";
            worksheet.Cells["O6"].Style.Font.Size = 14;
            worksheet.Cells["O6"].Style.Font.Bold = true;
            worksheet.Cells["O6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["P6"].Value = "TGL";
            worksheet.Cells["P6"].Style.Font.Size = 14;
            worksheet.Cells["P6"].Style.Font.Bold = true;
            worksheet.Cells["P6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["Q5"].Value = "REALISASI UANG MUKA";
            worksheet.Cells["Q5"].Style.Font.Size = 14;
            worksheet.Cells["Q5"].Style.Font.Bold = true;
            worksheet.Cells["Q5:S5"].Merge = true;
            worksheet.Cells["Q5:S5"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["Q6"].Value = "JUMLAH VALAS";
            worksheet.Cells["Q6"].Style.Font.Size = 14;
            worksheet.Cells["Q6"].Style.Font.Bold = true;
            worksheet.Cells["Q6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["R6"].Value = "KURS";
            worksheet.Cells["R6"].Style.Font.Size = 14;
            worksheet.Cells["R6"].Style.Font.Bold = true;
            worksheet.Cells["R6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            worksheet.Cells["S6"].Value = "RUPIAH";
            worksheet.Cells["S6"].Style.Font.Size = 14;
            worksheet.Cells["S6"].Style.Font.Bold = true;
            worksheet.Cells["S6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["T5"].Value = "TGL NOTA";
            worksheet.Cells["T5"].Style.Font.Size = 14;
            worksheet.Cells["T5"].Style.Font.Bold = true;
            worksheet.Cells["T5:T6"].Merge = true;
            worksheet.Cells["T5:T6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["U5"].Value = "NO NI.";
            worksheet.Cells["U5"].Style.Font.Size = 14;
            worksheet.Cells["U5"].Style.Font.Bold = true;
            worksheet.Cells["U5:U6"].Merge = true;
            worksheet.Cells["U5:U6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["V5"].Value = "SURAT JALAN";
            worksheet.Cells["V5"].Style.Font.Size = 14;
            worksheet.Cells["V5"].Style.Font.Bold = true;
            worksheet.Cells["V5:V6"].Merge = true;
            worksheet.Cells["V5:V6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["W5"].Value = "TGL SJ";
            worksheet.Cells["W5"].Style.Font.Size = 14;
            worksheet.Cells["W5"].Style.Font.Bold = true;
            worksheet.Cells["W5:W6"].Merge = true;
            worksheet.Cells["W5:W6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["X5"].Value = "NO. BP Kecil";
            worksheet.Cells["X5"].Style.Font.Size = 14;
            worksheet.Cells["X5"].Style.Font.Bold = true;
            worksheet.Cells["X5:X6"].Merge = true;
            worksheet.Cells["X5:X6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["Y5"].Value = "NO. BP";
            worksheet.Cells["Y5"].Style.Font.Size = 14;
            worksheet.Cells["Y5"].Style.Font.Bold = true;
            worksheet.Cells["Y5:AB5"].Merge = true;
            worksheet.Cells["Y5:AB5"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["Y6"].Value = "KET";
            worksheet.Cells["Y6"].Style.Font.Size = 14;
            worksheet.Cells["Y6"].Style.Font.Bold = true;
            worksheet.Cells["Y6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["Z6"].Value = "MATA UANG";
            worksheet.Cells["Z6"].Style.Font.Size = 14;
            worksheet.Cells["Z6"].Style.Font.Bold = true;
            worksheet.Cells["Z6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["AA6"].Value = "KURS";
            worksheet.Cells["AA6"].Style.Font.Size = 14;
            worksheet.Cells["AA6"].Style.Font.Bold = true;
            worksheet.Cells["AA6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["AB6"].Value = "RUPIAH";
            worksheet.Cells["AB6"].Style.Font.Size = 14;
            worksheet.Cells["AB6"].Style.Font.Bold = true;
            worksheet.Cells["AB6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["AC5"].Value = "Selisih Kurs";
            worksheet.Cells["AC5"].Style.Font.Size = 14;
            worksheet.Cells["AC5"].Style.Font.Bold = true;
            worksheet.Cells["AC5:AC6"].Merge = true;
            worksheet.Cells["AC5:AC6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["AD5"].Value = "SALDO AKHIR";
            worksheet.Cells["AD5"].Style.Font.Size = 14;
            worksheet.Cells["AD5"].Style.Font.Bold = true;
            worksheet.Cells["AD5:AF5"].Merge = true;
            worksheet.Cells["AD5:AF5"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["AD6"].Value = "MATA UANG";
            worksheet.Cells["AD6"].Style.Font.Size = 14;
            worksheet.Cells["AD6"].Style.Font.Bold = true;
            worksheet.Cells["AD6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["AE6"].Value = "KURS";
            worksheet.Cells["AE6"].Style.Font.Size = 14;
            worksheet.Cells["AE6"].Style.Font.Bold = true;
            worksheet.Cells["AE6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["AF6"].Value = "RUPIAH";
            worksheet.Cells["AF6"].Style.Font.Size = 14;
            worksheet.Cells["AF6"].Style.Font.Bold = true;
            worksheet.Cells["AF6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


            worksheet.Cells["A5:AF6"].AutoFitColumns();
            worksheet.Cells["A5:AF6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A5:AF6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;



        }

        private static void SetTitle(ExcelWorksheet worksheet, DateTimeOffset date, int timezoneOffset)
        {
            var company = "";
            var title = "SALDO UANG MUKA PEMBELIAN IMPORT GARMENT";
            var timeSpanOffset = new TimeSpan(timezoneOffset, 0, 0);
            var dateReport = date.ToOffset(timeSpanOffset);
            var cultureInfo = new CultureInfo("id-ID");
            var dateReportStr = $"PER: {dateReport.ToString("dd-MM-yyyy")}";

            worksheet.Cells["A1"].Value = company;
            worksheet.Cells["A1:AC1"].Merge = true;
            worksheet.Cells["A1:AC1"].Style.Font.Size = 20;
            worksheet.Cells["A1:AC1"].Style.Font.Bold = true;
            worksheet.Cells["A2"].Value = title;
            worksheet.Cells["A2:AC2"].Merge = true;
            worksheet.Cells["A2:AC2"].Style.Font.Size = 20;
            worksheet.Cells["A2:AC2"].Style.Font.Bold = true;
            worksheet.Cells["A3"].Value = dateReportStr;
            worksheet.Cells["A3:AC3"].Merge = true;
            worksheet.Cells["A3:AC3"].Style.Font.Size = 20;
            worksheet.Cells["A3:AC3"].Style.Font.Bold = true;
        }
    }
}
