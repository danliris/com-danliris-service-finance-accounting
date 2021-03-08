using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentDebtBalance.Excel
{
    public static class GarmentDebtBalanceDetailExcelGenerator
    {

        public static MemoryStream Generate(List<GarmentDebtBalanceDetailDto> data, int timezoneOffset)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet 1");

                SetTitle(worksheet);
                SetTableHeader(worksheet);
                SetData(worksheet, data, timezoneOffset);

                worksheet.Cells[worksheet.Cells.Address].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);

                return stream;
            }
        }

        private static void SetData(ExcelWorksheet worksheet, List<GarmentDebtBalanceDetailDto> data, int timezoneOffset)
        {
            var currentRow = 5;

            foreach (var item in data)
            {
                worksheet.Cells[$"A{currentRow}"].Value = $"{item.SupplierCode} - {item.SupplierName}";
                worksheet.Cells[$"A{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"B{currentRow}"].Value = item.BillNo;
                worksheet.Cells[$"B{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"C{currentRow}"].Value = item.PaymentBill;
                worksheet.Cells[$"C{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"D{currentRow}"].Value = item.DeliveryOrderNo;
                worksheet.Cells[$"D{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"E{currentRow}"].Value = item.PaymentType;
                worksheet.Cells[$"E{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"F{currentRow}"].Value = item.ArrivalDate.AddHours(timezoneOffset);
                worksheet.Cells[$"F{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"G{currentRow}"].Value = item.DebtAging;
                worksheet.Cells[$"G{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"H{currentRow}"].Value = item.InternalNoteNo;
                worksheet.Cells[$"H{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"I{currentRow}"].Value = item.InvoiceNo;
                worksheet.Cells[$"I{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"J{currentRow}"].Value = "";
                worksheet.Cells[$"J{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"K{currentRow}"].Value = item.CurrencyDPPAmount == 0 ? item.DPPAmount : item.CurrencyDPPAmount;
                worksheet.Cells[$"K{currentRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"K{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"L{currentRow}"].Value = item.CurrencyVATAmount == 0 ? item.VATAmount : item.CurrencyVATAmount;
                worksheet.Cells[$"L{currentRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"L{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"M{currentRow}"].Value = item.CurrencyIncomeTaxAmount == 0 ? item.IncomeTaxAmount : item.CurrencyIncomeTaxAmount;
                worksheet.Cells[$"M{currentRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"M{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"N{currentRow}"].Value = item.CurrencyTotal == 0 ? item.Total : item.CurrencyTotal;
                worksheet.Cells[$"N{currentRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"N{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"O{currentRow}"].Value = item.CurrencyCode;
                worksheet.Cells[$"O{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"P{currentRow}"].Value = item.CurrencyRate;
                worksheet.Cells[$"P{currentRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"P{currentRow}"].Style.Font.Size = 14;
                worksheet.Cells[$"Q{currentRow}"].Value = item.Total;
                worksheet.Cells[$"Q{currentRow}"].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[$"Q{currentRow}"].Style.Font.Size = 14;

                currentRow++;
            }

            worksheet.Cells[$"A5:F{currentRow}"].AutoFitColumns();
            worksheet.Cells[$"A5:F{currentRow}"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[$"A5:F{currentRow}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[$"A5:F{currentRow}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[$"A5:F{currentRow}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
        }

        private static void SetTableHeader(ExcelWorksheet worksheet)
        {
            worksheet.Cells["A4"].Value = "SUPPLIER";
            worksheet.Cells["A4"].Style.Font.Size = 14;
            worksheet.Cells["A4"].Style.Font.Bold = true;
            worksheet.Cells["B4"].Value = "NO. BP BESAR";
            worksheet.Cells["B4"].Style.Font.Size = 14;
            worksheet.Cells["B4"].Style.Font.Bold = true;
            worksheet.Cells["C4"].Value = "NO. BP KECIL";
            worksheet.Cells["C4"].Style.Font.Size = 14;
            worksheet.Cells["C4"].Style.Font.Bold = true;
            worksheet.Cells["D4"].Value = "NO. SJ";
            worksheet.Cells["D4"].Style.Font.Size = 14;
            worksheet.Cells["D4"].Style.Font.Bold = true;
            worksheet.Cells["E4"].Value = "TIPE BAYAR";
            worksheet.Cells["E4"].Style.Font.Size = 14;
            worksheet.Cells["E4"].Style.Font.Bold = true;
            worksheet.Cells["F4"].Value = "TGL. NOTA";
            worksheet.Cells["F4"].Style.Font.Size = 14;
            worksheet.Cells["F4"].Style.Font.Bold = true;
            worksheet.Cells["G4"].Value = "UMUR HUTANG";
            worksheet.Cells["G4"].Style.Font.Size = 14;
            worksheet.Cells["G4"].Style.Font.Bold = true;
            worksheet.Cells["H4"].Value = "NOTA INTERN";
            worksheet.Cells["H4"].Style.Font.Size = 14;
            worksheet.Cells["H4"].Style.Font.Bold = true;
            worksheet.Cells["I4"].Value = "NO. INVOICE";
            worksheet.Cells["I4"].Style.Font.Size = 14;
            worksheet.Cells["I4"].Style.Font.Bold = true;
            worksheet.Cells["J4"].Value = "NO. FAKTUR";
            worksheet.Cells["J4"].Style.Font.Size = 14;
            worksheet.Cells["J4"].Style.Font.Bold = true;
            worksheet.Cells["K4"].Value = "DPP";
            worksheet.Cells["K4"].Style.Font.Size = 14;
            worksheet.Cells["K4"].Style.Font.Bold = true;
            worksheet.Cells["L4"].Value = "PPN";
            worksheet.Cells["L4"].Style.Font.Size = 14;
            worksheet.Cells["L4"].Style.Font.Bold = true;
            worksheet.Cells["M4"].Value = "PPh";
            worksheet.Cells["M4"].Style.Font.Size = 14;
            worksheet.Cells["M4"].Style.Font.Bold = true;
            worksheet.Cells["N4"].Value = "TOTAL (DPP + PPN - PPh)";
            worksheet.Cells["N4"].Style.Font.Size = 14;
            worksheet.Cells["N4"].Style.Font.Bold = true;
            worksheet.Cells["O4"].Value = "MATA UANG";
            worksheet.Cells["O4"].Style.Font.Size = 14;
            worksheet.Cells["O4"].Style.Font.Bold = true;
            worksheet.Cells["P4"].Value = "RATE";
            worksheet.Cells["P4"].Style.Font.Size = 14;
            worksheet.Cells["P4"].Style.Font.Bold = true;
            worksheet.Cells["Q4"].Value = "TOTAL (IDR)";
            worksheet.Cells["Q4"].Style.Font.Size = 14;
            worksheet.Cells["Q4"].Style.Font.Bold = true;

            worksheet.Cells["A4:Q4"].AutoFitColumns();
            worksheet.Cells["A4:Q4"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A4:Q4"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A4:Q4"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A4:Q4"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
        }

        private static void SetTitle(ExcelWorksheet worksheet)
        {
            var company = "PT DAN LIRIS";

            var title = "LEDGER HUTANG LOKAL";

            worksheet.Cells["A1"].Value = company;
            worksheet.Cells["A1:Q1"].Merge = true;
            worksheet.Cells["A1:Q1"].Style.Font.Size = 20;
            worksheet.Cells["A1:Q1"].Style.Font.Bold = true;
            worksheet.Cells["A2"].Value = title;
            worksheet.Cells["A2:Q2"].Merge = true;
            worksheet.Cells["A2:Q2"].Style.Font.Size = 20;
            worksheet.Cells["A2:Q2"].Style.Font.Bold = true;
        }
    }
}
