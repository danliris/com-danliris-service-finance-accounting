using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.DPPVATBankExpenditureNote.PDF
{
    public static class DPPVATBankExpenditureNotePDFGenerator
    {
        private static readonly Font _headerFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 11);
        private static readonly Font _subHeaderFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);
        private static readonly Font _normalFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9);
        private static readonly Font _smallFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
        private static readonly Font _smallerFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);
        private static readonly Font _normalBoldFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9);
        private static readonly Font _normalBoldWhiteFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9, 0, BaseColor.White);
        private static readonly Font _smallBoldFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
        private static readonly Font _smallerBoldFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);
        private static readonly Font _smallerBoldWhiteFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7, 0, BaseColor.White);

        public static MemoryStream Generate(DPPVATBankExpenditureNoteDto data, int timezoneOffset)
        {
            var document = new Document(PageSize.A4.Rotate(), 20, 20, 20, 20);
            var stream = new MemoryStream();
            var writer = PdfWriter.GetInstance(document, stream);
            document.Open();

            SetTitle(document, data, timezoneOffset);
            SetTable(document, data, timezoneOffset);

            document.Close();
            byte[] byteInfo = stream.ToArray();
            stream.Write(byteInfo, 0, byteInfo.Length);
            stream.Position = 0;

            return stream;
        }

        private static void SetTable(Document document, DPPVATBankExpenditureNoteDto data, int timezoneOffset)
        {
            var table = new PdfPTable(7)
            {
                WidthPercentage = 100,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            table.SetWidths(new float[] { 1f, 3f, 3f, 3f, 2f, 2f, 6f });

            var cellCenter = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellLeft = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellRight = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            cellCenter.Phrase = new Phrase("No.", _subHeaderFont);
            table.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("No. SPB", _subHeaderFont);
            table.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Kategori Barang", _subHeaderFont);
            table.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Divisi", _subHeaderFont);
            table.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Unit", _subHeaderFont);
            table.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Mata Uang", _subHeaderFont);
            table.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Jumlah", _subHeaderFont);
            table.AddCell(cellCenter);

            var rowNumber = 1;
            foreach (var item in data.Items)
            {
                cellCenter.Phrase = new Phrase(rowNumber.ToString(), _normalFont);
                table.AddCell(cellCenter);
                cellLeft.Phrase = new Phrase(item.InternalNote.DocumentNo, _normalFont);
                table.AddCell(cellLeft);
                cellLeft.Phrase = new Phrase(string.Join("\n", item.InternalNote.Items.Select(element => $"- {element.Invoice.Category.Name}")), _subHeaderFont);
                table.AddCell(cellLeft);
                cellCenter.Phrase = new Phrase("", _subHeaderFont);
                table.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase("", _subHeaderFont);
                table.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase(item.InternalNote.Currency.Code, _subHeaderFont);
                table.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase(item.InternalNote.Items.Sum(itemInvoice => itemInvoice.Invoice.Amount).ToString(), _subHeaderFont);
                table.AddCell(cellCenter);
            }
        }

        private static void SetTitle(Document document, DPPVATBankExpenditureNoteDto data, int timezoneOffset)
        {
            var table = new PdfPTable(3)
            {
                WidthPercentage = 100,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            table.SetWidths(new float[] { 6f, 3f, 3f });

            var cellCenter = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellLeft = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            cellCenter.Colspan = 3;
            cellCenter.Phrase = new Phrase("BUKTI PENGELUARAN BANK", _headerFont);
            table.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("\n", _headerFont);
            table.AddCell(cellCenter);
            cellCenter.Colspan = 1;

            cellLeft.Phrase = new Phrase("PT DAN LIRIS", _subHeaderFont);
            table.AddCell(cellLeft);
            cellLeft.Phrase = new Phrase("Tanggal", _subHeaderFont);
            table.AddCell(cellLeft);
            cellLeft.Phrase = new Phrase($": {data.Date.AddHours(timezoneOffset).ToString("dd/MMMM/yyyy")}", _subHeaderFont);

            cellLeft.Phrase = new Phrase("Kel. Banaran, Kec. Grogol", _subHeaderFont);
            table.AddCell(cellLeft);
            cellLeft.Phrase = new Phrase("NO", _subHeaderFont);
            table.AddCell(cellLeft);
            cellLeft.Phrase = new Phrase($": {data.DocumentNo}", _subHeaderFont);

            cellLeft.Phrase = new Phrase("Sukoharjo - 57100", _subHeaderFont);
            table.AddCell(cellLeft);
            cellLeft.Phrase = new Phrase("Dibayarkan ke", _subHeaderFont);
            table.AddCell(cellLeft);
            cellLeft.Phrase = new Phrase($": {data.Supplier.Name}", _subHeaderFont);

            cellLeft.Phrase = new Phrase("", _subHeaderFont);
            table.AddCell(cellLeft);
            cellLeft.Phrase = new Phrase("Bank", _subHeaderFont);
            table.AddCell(cellLeft);
            cellLeft.Phrase = new Phrase($": {data.Bank.BankName} {data.Currency.Code} - A/C : {data.Bank.AccountNumber}", _subHeaderFont);

            document.Add(table);
        }
    }
}
