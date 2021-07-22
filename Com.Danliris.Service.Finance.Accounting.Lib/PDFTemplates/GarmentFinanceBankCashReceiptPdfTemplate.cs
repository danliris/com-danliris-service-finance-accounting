using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.GarmentFinance.BankCashReceipt;
using Com.Danliris.Service.Sales.Lib.Utilities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.PDFTemplates
{
    public class GarmentFinanceBankCashReceiptPdfTemplate
    {
        public MemoryStream GeneratePdfTemplate(BankCashReceiptViewModel viewModel, int clientTimeZoneOffset)
        {
            const int MARGIN = 8;

            Font header_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 18);
            Font normal_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 11);
            Font bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 11);
            Font note_font = FontFactory.GetFont(BaseFont.HELVETICA_OBLIQUE, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
            Font bold_italic_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLDOBLIQUE, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 12);
            Font Title_bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 13);

            Document document = new Document(PageSize.A5.Rotate(), MARGIN, MARGIN, MARGIN, MARGIN);
            MemoryStream stream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            document.Open();

            #region CustomModel
            string TotalPaidString = "";
            if (viewModel != null)
            {
                if(viewModel.Currency.Code == "IDR")
                {
                    if (viewModel.Amount.ToString().EndsWith(",00")){
                        TotalPaidString = NumberToTextIDN.terbilang((double)viewModel.Amount) + " Rupiah";
                    } else
                    {
                        TotalPaidString = NumberToTextIDN.terbilang((double)viewModel.Amount) + " " + NumberToTextIDN.terbilangKoma((double)viewModel.Amount) + " Rupiah";
                    }
                    
                } else
                {
                    if (viewModel.Amount.ToString().EndsWith(",00"))
                    {
                        TotalPaidString = NumberToTextIDN.terbilang((double)viewModel.Amount) + " Dollar";
                    }
                    else
                    {
                        TotalPaidString = NumberToTextIDN.terbilang((double)viewModel.Amount) + " Dollar " + NumberToTextIDN.terbilangKoma((double)viewModel.Amount) + " Sen";
                        TotalPaidString = TotalPaidString.Replace("koma", "");
                    }
                }
            }
            var arrayRemarks = new List<string>();
            foreach(var item in viewModel.Items)
            {
                arrayRemarks.Add(item.Remarks);
            }

            string payment = String.Join(", ", arrayRemarks);


            #endregion CustomModel

            #region Header

            PdfPTable headerTable_A = new PdfPTable(1);
            PdfPTable headerTable_B = new PdfPTable(1);
            PdfPTable headerTable1 = new PdfPTable(1);
            PdfPTable headerTable2 = new PdfPTable(1);
            PdfPTable headerTable3 = new PdfPTable(3);
            PdfPTable headerTable4 = new PdfPTable(2);
            PdfPTable headerTitle = new PdfPTable(1);
            PdfPTable headerReceiptNo = new PdfPTable(1);
            headerTable_A.SetWidths(new float[] { 15f });
            headerTable_A.WidthPercentage = 100;
            headerTable3.SetWidths(new float[] { 40f, 4f, 100f });
            headerTable3.WidthPercentage = 100;
            headerTable4.SetWidths(new float[] { 10f, 40f });
            headerTable4.WidthPercentage = 100;

            PdfPCell cellHeader1 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellHeader2 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellHeader3 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellHeader4 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellHeaderBody = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellMoney = new PdfPCell() { Border = Rectangle.BOX };
            cellMoney.FixedHeight = 50f;

            cellHeaderBody.Phrase = new Phrase("PT. DAN LIRIS", Title_bold_font);
            headerTable1.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase("Kel. Banaran, Kec. Grogol, Kab. Dati II Sukoharjo", normal_font);
            headerTable1.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase("PO BOX 166 Solo - 57100 Jawa Tengah Indonesia", normal_font);
            headerTable1.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase("Telp. 0271-714400, Fax. 0271-717178", normal_font);
            headerTable1.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("", normal_font);
            headerTable1.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase("", normal_font);
            headerTable1.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase("", normal_font);
            headerTable1.AddCell(cellHeaderBody);

            cellHeader1.AddElement(headerTable1);
            headerTable_A.AddCell(cellHeader1);

            document.Add(headerTable_A);

            cellHeaderBody.HorizontalAlignment = Element.ALIGN_CENTER;

            cellHeaderBody.Phrase = new Phrase("K W I T A N S I ", header_font);
            headerTitle.AddCell(cellHeaderBody);

            document.Add(headerTitle);

            cellHeaderBody.HorizontalAlignment = Element.ALIGN_RIGHT;

            cellHeaderBody.Phrase = new Phrase(viewModel?.ReceiptNo, header_font);
            headerReceiptNo.AddCell(cellHeaderBody);

            document.Add(headerReceiptNo);

            cellHeaderBody.HorizontalAlignment = Element.ALIGN_LEFT;

            cellHeaderBody.Phrase = new Phrase("BANK ", normal_font);
            headerTable3.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase(":", normal_font);
            headerTable3.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase(viewModel?.Bank.BankName + " - " + viewModel?.Bank.AccountNumber, normal_font);
            headerTable3.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("TELAH TERIMA DARI ", normal_font);
            headerTable3.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase(":", normal_font);
            headerTable3.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase(viewModel?.Remarks, normal_font);
            headerTable3.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("", normal_font);
            headerTable3.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase("", normal_font);
            headerTable3.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase("", normal_font);
            headerTable3.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("BANYAKNYA UANG ", normal_font);
            headerTable3.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase(":", normal_font);
            headerTable3.AddCell(cellHeaderBody);
            cellMoney.Phrase = new Phrase(TotalPaidString, normal_font);
            headerTable3.AddCell(cellMoney);

            cellHeaderBody.Phrase = new Phrase("UNTUK PEMBAYARAN ", normal_font);
            headerTable3.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase(":", normal_font);
            headerTable3.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase(payment);
            headerTable3.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("", normal_font);
            headerTable3.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase("", normal_font);
            headerTable3.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase("", normal_font);
            headerTable3.AddCell(cellHeaderBody);

            cellHeader3.AddElement(headerTable3);
            headerTable_B.AddCell(cellHeader3);

            cellHeader4.AddElement(headerTable4);
            headerTable_B.AddCell(cellHeader4);

            document.Add(headerTable_B);
            #endregion

            #region Footer
            PdfPTable footerTable = new PdfPTable(2);
            PdfPTable footerTable1 = new PdfPTable(1);
            PdfPTable footerTable2 = new PdfPTable(2);

            footerTable.SetWidths(new float[] { 10f, 10f });
            footerTable.WidthPercentage = 100;
            footerTable1.WidthPercentage = 80;
            footerTable2.SetWidths(new float[] { 30f, 80f });
            footerTable2.WidthPercentage = 100;

            PdfPCell cellFooterLeft1 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellFooterLeft2 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellHeaderFooter = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellTerbilang = new PdfPCell() { Border = Rectangle.NO_BORDER };



            cellHeaderFooter.HorizontalAlignment = Element.ALIGN_CENTER;
            cellTerbilang.HorizontalAlignment = Element.ALIGN_CENTER;

            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable1.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable1.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable1.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable1.AddCell(cellHeaderFooter);

            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable1.AddCell(cellHeaderFooter);

            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable1.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable1.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable1.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable1.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable1.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable1.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable1.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable1.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable1.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable1.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable1.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable1.AddCell(cellHeaderFooter);

            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable1.AddCell(cellHeaderFooter);
            cellTerbilang.Border = Rectangle.BOX;
            cellTerbilang.FixedHeight = 30f;
            cellTerbilang.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellTerbilang.Phrase = new Phrase("T E R B I L A N G : " + viewModel?.Currency.Code + " " + viewModel?.Amount.ToString("#,##0.00", new CultureInfo("id-ID")), bold_font);
            footerTable1.AddCell(cellTerbilang);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable1.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", note_font);
            footerTable1.AddCell(cellHeaderFooter);

            cellFooterLeft1.AddElement(footerTable1);
            footerTable.AddCell(cellFooterLeft1);

            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("SOLO, " + viewModel?.ReceiptDate.AddHours(clientTimeZoneOffset).ToString("dd MMMM yyyy", new CultureInfo("id-ID")), normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("Dicetak Tanggal :" + DateTimeOffset.Now.AddHours(clientTimeZoneOffset).ToString("dd MMMM yyyy / HH:mm:ss", new CultureInfo("id-ID")), normal_font);
            footerTable2.AddCell(cellHeaderFooter);
            cellHeaderFooter.Phrase = new Phrase("", normal_font);
            footerTable2.AddCell(cellHeaderFooter);

            cellFooterLeft2.AddElement(footerTable2);
            footerTable.AddCell(cellFooterLeft2);

            document.Add(footerTable);

            #endregion Footer

            document.Close();
            byte[] byteInfo = stream.ToArray();
            stream.Write(byteInfo, 0, byteInfo.Length);
            stream.Position = 0;

            return stream;
        }
    }
}
