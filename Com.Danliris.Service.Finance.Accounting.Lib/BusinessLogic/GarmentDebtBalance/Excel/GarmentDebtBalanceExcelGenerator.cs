using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentDebtBalance.Pdf;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentDebtBalance.Excel
{
    public static class GarmentDebtBalanceExcelGenerator
    {
        private static readonly List<MonthName> _months = new List<MonthName>()
        {
            new MonthName(1, "Januari"),
            new MonthName(2, "Februari"),
            new MonthName(3, "Maret"),
            new MonthName(4, "April"),
            new MonthName(5, "Mei"),
            new MonthName(6, "Juni"),
            new MonthName(7, "Juli"),
            new MonthName(8, "Agustus"),
            new MonthName(9, "September"),
            new MonthName(10, "Oktober"),
            new MonthName(11, "November"),
            new MonthName(12, "Desember"),
        };

        public static MemoryStream Generate(List<GarmentDebtBalanceSummaryDto> data, int month, int year, bool isForeignCurrency, bool supplierIsImport, int timezoneOffset)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet 1");

                SetTitle(worksheet, month, year, isForeignCurrency, supplierIsImport);
                SetTableHeader(worksheet, isForeignCurrency, supplierIsImport);
                SetData(worksheet, data, isForeignCurrency, supplierIsImport);

                worksheet.Cells[worksheet.Cells.Address].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);

                return stream;
            }
        }

        private static void SetData(ExcelWorksheet worksheet, List<GarmentDebtBalanceSummaryDto> data, bool isForeignCurrency, bool supplierIsImport)
        {
            if (!supplierIsImport)
            {
                var startedRow = 5;

                foreach (var item in data)
                {
                    worksheet.Cells[$"A{startedRow}"].Value = item.SupplierName;
                    worksheet.Cells[$"A{startedRow}"].Style.Font.Size = 14;
                    worksheet.Cells[$"B{startedRow}"].Value = item.CurrencyCode;
                    worksheet.Cells[$"B{startedRow}"].Style.Font.Size = 14;
                    worksheet.Cells[$"C{startedRow}"].Value = item.InitialBalance;
                    worksheet.Cells[$"C{startedRow}"].Style.Font.Size = 14;
                    worksheet.Cells[$"D{startedRow}"].Value = item.PurchaseAmount;
                    worksheet.Cells[$"D{startedRow}"].Style.Font.Size = 14;
                    worksheet.Cells[$"E{startedRow}"].Value = item.PaymentAmount;
                    worksheet.Cells[$"E{startedRow}"].Style.Font.Size = 14;
                    worksheet.Cells[$"F{startedRow}"].Value = item.CurrentBalance;
                    worksheet.Cells[$"F{startedRow}"].Style.Font.Size = 14;

                    startedRow++;
                }
            }
            else
            {
                var startedRow = 6;
                foreach (var item in data)
                {
                    worksheet.Cells[$"A{startedRow}"].Value = item.SupplierName;
                    worksheet.Cells[$"A{startedRow}"].Style.Font.Size = 14;
                    worksheet.Cells[$"B{startedRow}"].Value = item.CurrencyCode;
                    worksheet.Cells[$"B{startedRow}"].Style.Font.Size = 14;
                    worksheet.Cells[$"C{startedRow}"].Value = item.CurrencyInitialBalance;
                    worksheet.Cells[$"C{startedRow}"].Style.Font.Size = 14;
                    worksheet.Cells[$"D{startedRow}"].Value = item.CurrencyPurchaseAmount;
                    worksheet.Cells[$"D{startedRow}"].Style.Font.Size = 14;
                    worksheet.Cells[$"E{startedRow}"].Value = item.CurrencyPaymentAmount;
                    worksheet.Cells[$"E{startedRow}"].Style.Font.Size = 14;
                    worksheet.Cells[$"F{startedRow}"].Value = item.CurrencyCurrentBalance;
                    worksheet.Cells[$"F{startedRow}"].Style.Font.Size = 14;
                    worksheet.Cells[$"G{startedRow}"].Value = item.InitialBalance;
                    worksheet.Cells[$"G{startedRow}"].Style.Font.Size = 14;
                    worksheet.Cells[$"H{startedRow}"].Value = item.PurchaseAmount;
                    worksheet.Cells[$"H{startedRow}"].Style.Font.Size = 14;
                    worksheet.Cells[$"I{startedRow}"].Value = item.PaymentAmount;
                    worksheet.Cells[$"I{startedRow}"].Style.Font.Size = 14;
                    worksheet.Cells[$"J{startedRow}"].Value = item.CurrentBalance;
                    worksheet.Cells[$"J{startedRow}"].Style.Font.Size = 14;

                    startedRow++;
                }
            }
        }

        private static void SetTableHeader(ExcelWorksheet worksheet, bool isForeignCurrency, bool supplierIsImport)
        {
            if (!supplierIsImport)
            {
                worksheet.Cells["A4"].Value = "SUPPLIER";
                worksheet.Cells["A4"].Style.Font.Size = 14;
                worksheet.Cells["A4"].Style.Font.Bold = true;
                worksheet.Cells["B4"].Value = "MATA UANG";
                worksheet.Cells["B4"].Style.Font.Size = 14;
                worksheet.Cells["B4"].Style.Font.Bold = true;
                worksheet.Cells["C4"].Value = "SALDO AWAL";
                worksheet.Cells["C4"].Style.Font.Size = 14;
                worksheet.Cells["C4"].Style.Font.Bold = true;
                worksheet.Cells["D4"].Value = "PEMBELIAN";
                worksheet.Cells["D4"].Style.Font.Size = 14;
                worksheet.Cells["D4"].Style.Font.Bold = true;
                worksheet.Cells["E4"].Value = "PEMBAYARAN";
                worksheet.Cells["E4"].Style.Font.Size = 14;
                worksheet.Cells["E4"].Style.Font.Bold = true;
                worksheet.Cells["F4"].Value = "SALDO AKHIR";
                worksheet.Cells["F4"].Style.Font.Size = 14;
                worksheet.Cells["F4"].Style.Font.Bold = true;
            }
            else
            {
                worksheet.Cells["A4"].Value = "SUPPLIER";
                worksheet.Cells["A4:A5"].Merge = true;
                worksheet.Cells["A4:A5"].Style.Font.Size = 14;
                worksheet.Cells["A4:A5"].Style.Font.Bold = true;
                worksheet.Cells["B4"].Value = "MATA UANG";
                worksheet.Cells["B4:B5"].Merge = true;
                worksheet.Cells["B4:B5"].Style.Font.Size = 14;
                worksheet.Cells["B4:B5"].Style.Font.Bold = true;
                worksheet.Cells["C4"].Value = "SALDO AWAL";
                worksheet.Cells["C4:C5"].Merge = true;
                worksheet.Cells["C4:C5"].Style.Font.Size = 14;
                worksheet.Cells["C4:C5"].Style.Font.Bold = true;
                worksheet.Cells["D4"].Value = "PEMBELIAN";
                worksheet.Cells["D4:D5"].Merge = true;
                worksheet.Cells["D4:D5"].Style.Font.Size = 14;
                worksheet.Cells["D4:D5"].Style.Font.Bold = true;
                worksheet.Cells["E4"].Value = "PEMBAYARAN";
                worksheet.Cells["E4:E5"].Merge = true;
                worksheet.Cells["E4:E5"].Style.Font.Size = 14;
                worksheet.Cells["E4:E5"].Style.Font.Bold = true;
                worksheet.Cells["F4"].Value = "SALDO AKHIR";
                worksheet.Cells["F4:F5"].Merge = true;
                worksheet.Cells["F4:F5"].Style.Font.Size = 14;
                worksheet.Cells["F4:F5"].Style.Font.Bold = true;
                worksheet.Cells["G4"].Value = "DALAM RUPIAH";
                worksheet.Cells["G4:J4"].Merge = true;
                worksheet.Cells["G4:J4"].Style.Font.Size = 14;
                worksheet.Cells["G4:J4"].Style.Font.Bold = true;
                worksheet.Cells["G5"].Value = "SALDO AWAL";
                worksheet.Cells["G5"].Style.Font.Size = 14;
                worksheet.Cells["G5"].Style.Font.Bold = true;
                worksheet.Cells["H5"].Value = "PEMBELIAN";
                worksheet.Cells["H5"].Style.Font.Size = 14;
                worksheet.Cells["H5"].Style.Font.Bold = true;
                worksheet.Cells["I5"].Value = "PEMBAYARAN";
                worksheet.Cells["I5"].Style.Font.Size = 14;
                worksheet.Cells["I5"].Style.Font.Bold = true;
                worksheet.Cells["J5"].Value = "SALDO AKHIR";
                worksheet.Cells["J5"].Style.Font.Size = 14;
                worksheet.Cells["J5"].Style.Font.Bold = true;
            }
        }

        private static void SetTitle(ExcelWorksheet worksheet, int month, int year, bool isForeignCurrency, bool supplierIsImport)
        {
            var company = "PT DAN LIRIS";

            if (!supplierIsImport)
            {
                var title = "LEDGER HUTANG LOKAL";
                var monthName = _months.FirstOrDefault(element => element.Value == month);
                var period = $"PER {monthName.Name.ToUpper()} {year}";

                worksheet.Cells["A1"].Value = company;
                worksheet.Cells["A1:F1"].Merge = true;
                worksheet.Cells["A1:F1"].Style.Font.Size = 20;
                worksheet.Cells["A1:F1"].Style.Font.Bold = true;
                worksheet.Cells["A2"].Value = title;
                worksheet.Cells["A2:F2"].Merge = true;
                worksheet.Cells["A2:F2"].Style.Font.Size = 20;
                worksheet.Cells["A2:F2"].Style.Font.Bold = true;
                worksheet.Cells["A3"].Value = period;
                worksheet.Cells["A3:F3"].Merge = true;
                worksheet.Cells["A3:F3"].Style.Font.Size = 20;
                worksheet.Cells["A3:F3"].Style.Font.Bold = true;
            }
            else
            {
                var title = "LEDGER HUTANG IMPOR";
                var monthName = _months.FirstOrDefault(element => element.Value == month);
                var period = $"PER {monthName.Name.ToUpper()} {year}";

                worksheet.Cells["A1"].Value = company;
                worksheet.Cells["A1:J1"].Merge = true;
                worksheet.Cells["A1:J1"].Style.Font.Size = 20;
                worksheet.Cells["A1:J1"].Style.Font.Bold = true;
                worksheet.Cells["A2"].Value = title;
                worksheet.Cells["A2:J2"].Merge = true;
                worksheet.Cells["A2:J2"].Style.Font.Size = 20;
                worksheet.Cells["A2:J2"].Style.Font.Bold = true;
                worksheet.Cells["A3"].Value = period;
                worksheet.Cells["A3:J3"].Merge = true;
                worksheet.Cells["A3:J3"].Style.Font.Size = 20;
                worksheet.Cells["A3:J3"].Style.Font.Bold = true;
            }


        }
    }
}
