using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CreditorAccount;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.PDFTemplates
{
    public static class CreditorAccountPDFTemplate
    {
        private static readonly Font _headerFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 18);
        private static readonly Font _normalFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9);
        private static readonly Font _smallFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
        private static readonly Font _smallerFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);
        private static readonly Font _normalBoldFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9);
        private static readonly Font _smallBoldFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
        private static readonly Font _smallerBoldFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);

        public static MemoryStream GeneratePdfTemplate(List<CreditorAccountViewModel> data, string suplierName, int month, int year, int offSet)
        {
            var document = new Document(PageSize.A4.Rotate(), 25, 25, 25, 25);
            var stream = new MemoryStream();
            PdfWriter.GetInstance(document, stream);
            document.Open();

            SetHeader(document, suplierName, month, year, offSet);

            SetReportTable(document, data, suplierName, month, year, offSet);

            /*SetFooter(document, data);*/

            document.Close();
            byte[] byteInfo = stream.ToArray();
            stream.Write(byteInfo, 0, byteInfo.Length);
            stream.Position = 0;

            return stream;
        }

        private static void SetHeader(Document document, string suplierName,  int month, int year, int offSet)
        {
            var table = new PdfPTable(1)
            {
                WidthPercentage = 100
            };

            var cell = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            cell.Phrase = new Phrase("PT. DAN LIRIS", _normalBoldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Kartu Hutang Lokal", _normalBoldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase(suplierName, _normalBoldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Periode: " + new DateTime(year, month, DateTime.DaysInMonth(year, month)).ToString("dd MMMM yyyy"), _normalBoldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("", _normalBoldFont);
            table.AddCell(cell);

            document.Add(table);
        }

        private static void SetReportTableHeader(PdfPTable table)
        {
            var cell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
            };

            cell.Rowspan = 2;
            cell.Phrase = new Phrase("TANGGAL", _normalBoldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("NAMA BARANG", _normalBoldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("NO. BON PENERIMAAN", _normalBoldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("NO. INVOICE", _normalBoldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("NO. NI/SPB", _normalBoldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("NO. VOUCHER", _normalBoldFont);
            table.AddCell(cell);

            cell.Rowspan = 1;
            cell.Colspan = 3;
            cell.Phrase = new Phrase("MUTASI", _normalBoldFont);
            table.AddCell(cell);

            cell.Colspan = 1;
            cell.Phrase = new Phrase("PEMBELIAN", _normalBoldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("PEMBAYARAN", _normalBoldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("SALDO", _normalBoldFont);
            table.AddCell(cell);

        }

        private static void SetReportTable(Document document, List<CreditorAccountViewModel> data, string suplierName, int month, int year, int offSet)
        {
            var table = new PdfPTable(9)
            {
                WidthPercentage = 100
            };
            table.SetWidths(new float[] { 5f, 15f, 15f, 15f, 15f, 15f, 15f, 15f, 15f });

            SetReportTableHeader(table);

            foreach (var item in data)
            {
                var cell = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };

                var cellAlignLeft = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };

                var cellAlignRight = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };

                cell.Phrase = new Phrase(item.Date.ToString(), _normalFont);
                table.AddCell(cell);

                cell.Phrase = new Phrase(item.Products, _smallerFont);
                table.AddCell(cell);

                cell.Phrase = new Phrase(item.UnitReceiptNoteNo, _smallerFont);
                table.AddCell(cell);

                cell.Phrase = new Phrase(item.InvoiceNo, _normalFont);
                table.AddCell(cell);

                cell.Phrase = new Phrase(item.MemoNo, _normalFont);
                table.AddCell(cell);

                cell.Phrase = new Phrase(item.BankExpenditureNoteNo, _normalFont);
                table.AddCell(cell);

                decimal? purchase = 0;
                decimal? payment = 0;
                if (item.Mutation > 0)
                {
                    purchase += item.Mutation;
                }
                else
                {
                    payment += item.Mutation;
                    
                }

                cellAlignRight.Phrase = new Phrase(purchase.ToString(), _normalFont);
                table.AddCell(cellAlignRight);

                cellAlignRight.Phrase = new Phrase(payment.ToString(), _normalFont);
                table.AddCell(cellAlignRight);

                cellAlignRight.Phrase = new Phrase(item.FinalBalance.ToString(), _normalFont);
                table.AddCell(cellAlignRight);
            }

            document.Add(table);
        }

        /*private static void SetFooter(Document document, List<CreditorAccountViewModel> data)
        {
            var table = new PdfPTable(9)
            {
                WidthPercentage = 100
            };
            table.SetWidths(new float[] { 5f, 15f, 15f, 15f, 15f, 15f, 15f, 15f, 15f });

            var groupedByCurrency = data.GroupBy(item => item.Currency).Select(element => new
            {
                currency = element.Key,
                currencyRate = element.First().CurrencyRate,
                totalStartBalance = element.Sum(curr => curr.StartBalance),
                totalPurchase = element.Sum(curr => curr.Purchase),
                totalPayment = element.Sum(curr => curr.Payment),
                totalFinalBalance = element.Sum(curr => curr.FinalBalance)
            });

            var cell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            var cellAlignRight = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            var length = groupedByCurrency.Count();

            cell.Colspan = 2;
            cell.Rowspan = length;
            cell.Phrase = new Phrase("SUB TOTAL", _normalBoldFont);
            table.AddCell(cell);

            foreach (var item in groupedByCurrency)
            {
                cell.Colspan = 1;
                cell.Rowspan = 1;
                cell.Phrase = new Phrase(item.currency, _normalBoldFont);
                table.AddCell(cell);

                cellAlignRight.Phrase = new Phrase(item.totalStartBalance.ToString("#,##0.#0"), _normalBoldFont);
                table.AddCell(cellAlignRight);

                cellAlignRight.Phrase = new Phrase(item.totalPurchase.ToString("#,##0.#0"), _normalBoldFont);
                table.AddCell(cellAlignRight);

                cellAlignRight.Phrase = new Phrase(item.totalPayment.ToString("#,##0.#0"), _normalBoldFont);
                table.AddCell(cellAlignRight);

                cellAlignRight.Phrase = new Phrase(item.totalFinalBalance.ToString("#,##0.#0"), _normalBoldFont);
                table.AddCell(cellAlignRight);

                cellAlignRight.Phrase = new Phrase((item.totalStartBalance * item.currencyRate).ToString("#,##0.#0"), _normalBoldFont);
                table.AddCell(cellAlignRight);

                cellAlignRight.Phrase = new Phrase((item.totalPurchase * item.currencyRate).ToString("#,##0.#0"), _normalBoldFont);
                table.AddCell(cellAlignRight);

                cellAlignRight.Phrase = new Phrase((item.totalPayment * item.currencyRate).ToString("#,##0.#0"), _normalBoldFont);
                table.AddCell(cellAlignRight);

                cellAlignRight.Phrase = new Phrase((item.totalFinalBalance * item.currencyRate).ToString("#,##0.#0"), _normalBoldFont);
                table.AddCell(cellAlignRight);
            }

            document.Add(table);
        }
    */
    }
}