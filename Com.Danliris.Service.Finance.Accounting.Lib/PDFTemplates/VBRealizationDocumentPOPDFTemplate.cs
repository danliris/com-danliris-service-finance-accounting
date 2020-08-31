using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRealizationDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRequestDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
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
    public class VBRealizationDocumentPOPDFTemplate
    {
        public MemoryStream GeneratePdfTemplate(VBRealizationPdfDto viewModel, int timeoffsset)
        {
            const int MARGIN = 20;
            const int MARGIN_VERTICAL = 10;

            Font header_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 18);
            Font normal_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 11);
            Font normal_font_8 = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9);
            Font bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 11);
            Font bold_font_8 = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
            Font note_font = FontFactory.GetFont(BaseFont.HELVETICA_OBLIQUE, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
            Font bold_italic_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLDOBLIQUE, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 12);
            Font Title_bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 13);

            Document document = new Document(PageSize.A4, MARGIN_VERTICAL, MARGIN_VERTICAL, MARGIN, MARGIN);
            MemoryStream stream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            document.Open();

            #region Header

            PdfPTable headerTable_A = new PdfPTable(2);
            PdfPTable headerTable_B = new PdfPTable(1);
            PdfPTable headerTable_C = new PdfPTable(1);
            PdfPTable headerTable1 = new PdfPTable(1);
            PdfPTable headerTable2 = new PdfPTable(1);
            PdfPTable headerTable3 = new PdfPTable(8);
            PdfPTable headerTable3a = new PdfPTable(9);
            PdfPTable headerTable4 = new PdfPTable(2);
            headerTable_A.SetWidths(new float[] { 10f, 10f });
            headerTable_A.WidthPercentage = 100;
            headerTable3.SetWidths(new float[] { 10f, 20f, 40f, 20f, 20f, 20f, 20f, 20f });
            headerTable3.WidthPercentage = 110;
            headerTable3a.SetWidths(new float[] { 20f, 3f, 20f, 20f, 3f, 20f, 20f, 3f, 20f });
            headerTable3a.WidthPercentage = 120;
            headerTable4.SetWidths(new float[] { 10f, 40f });
            headerTable4.WidthPercentage = 100;

            PdfPCell cellHeader1 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellHeader2 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellHeader3 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellHeader4 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellHeader3a = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellHeaderBody = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellHeaderBody1 = new PdfPCell() { };
            PdfPCell cellHeaderBody1a = new PdfPCell() { BorderWidthTop = 2 };
            PdfPCell cellHeaderBody1a1 = new PdfPCell() { };
            PdfPCell cellHeaderBody1a2 = new PdfPCell() { };
            PdfPCell cellHeaderBody1b = new PdfPCell() { BorderWidthTop = 2 };
            PdfPCell cellHeaderBody1b1 = new PdfPCell() { };
            PdfPCell cellHeaderBody1b2 = new PdfPCell() { };
            PdfPCell cellHeaderBody1c = new PdfPCell() { };
            PdfPCell cellHeaderBody2 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellHeaderBody3 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellHeaderBody4 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellHeaderBody4a = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellHeaderBody4b = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellHeaderBody5 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellHeaderBody5a = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellHeaderBody6 = new PdfPCell() { };

            cellHeader1.AddElement(headerTable1);
            headerTable_A.AddCell(cellHeader1);

            cellHeader2.AddElement(headerTable2);
            headerTable_A.AddCell(cellHeader2);

            document.Add(headerTable_A);

            cellHeaderBody.HorizontalAlignment = Element.ALIGN_LEFT;
            cellHeaderBody1.HorizontalAlignment = Element.ALIGN_CENTER;
            cellHeaderBody1a.HorizontalAlignment = Element.ALIGN_CENTER;
            cellHeaderBody1a1.HorizontalAlignment = Element.ALIGN_CENTER;
            cellHeaderBody1a2.HorizontalAlignment = Element.ALIGN_CENTER;
            cellHeaderBody1b.HorizontalAlignment = Element.ALIGN_CENTER;
            cellHeaderBody1b1.HorizontalAlignment = Element.ALIGN_CENTER;
            cellHeaderBody1b2.HorizontalAlignment = Element.ALIGN_CENTER;
            cellHeaderBody1c.HorizontalAlignment = Element.ALIGN_CENTER;
            cellHeaderBody2.HorizontalAlignment = Element.ALIGN_CENTER;
            cellHeaderBody3.HorizontalAlignment = Element.ALIGN_LEFT;
            cellHeaderBody4.HorizontalAlignment = Element.ALIGN_CENTER;
            cellHeaderBody4a.HorizontalAlignment = Element.ALIGN_CENTER;
            cellHeaderBody4b.HorizontalAlignment = Element.ALIGN_LEFT;
            cellHeaderBody5.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellHeaderBody5a.HorizontalAlignment = Element.ALIGN_CENTER;
            cellHeaderBody6.HorizontalAlignment = Element.ALIGN_LEFT;

            cellHeaderBody2.Colspan = 8;
            cellHeaderBody2.Phrase = new Phrase("REALISASI VB TANPA PO", bold_font);
            headerTable3.AddCell(cellHeaderBody2);

            cellHeaderBody.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody);

            cellHeaderBody3.Colspan = 8;
            cellHeaderBody3.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellHeaderBody3.Phrase = new Phrase($"{viewModel.Header.DocumentNo}", bold_font);
            headerTable3.AddCell(cellHeaderBody3);

            cellHeaderBody3.Colspan = 8;
            cellHeaderBody3.HorizontalAlignment = Element.ALIGN_LEFT;
            cellHeaderBody3.Phrase = new Phrase($"Realisasi VB Bagian: {viewModel.Header.SuppliantUnitName}", bold_font);
            headerTable3.AddCell(cellHeaderBody3);

            cellHeaderBody3.Colspan = 8;
            cellHeaderBody3.Phrase = new Phrase($"Tanggal: {viewModel.Header.Date.AddHours(timeoffsset).ToString("dd MMMM yyyy", new CultureInfo("id-ID"))}", bold_font);
            headerTable3.AddCell(cellHeaderBody3);

            cellHeaderBody1.Phrase = new Phrase("No", normal_font);
            headerTable3.AddCell(cellHeaderBody1);
            cellHeaderBody1.Phrase = new Phrase("Tanggal", normal_font);
            headerTable3.AddCell(cellHeaderBody1);
            cellHeaderBody1.Phrase = new Phrase("Keterangan", normal_font);
            headerTable3.AddCell(cellHeaderBody1);
            cellHeaderBody1.Phrase = new Phrase("Divisi", normal_font);
            headerTable3.AddCell(cellHeaderBody1);
            cellHeaderBody1.Phrase = new Phrase("Harga", normal_font);
            headerTable3.AddCell(cellHeaderBody1);
            cellHeaderBody1.Phrase = new Phrase("PPn", normal_font);
            headerTable3.AddCell(cellHeaderBody1);
            cellHeaderBody1.Phrase = new Phrase("PPh", normal_font);
            headerTable3.AddCell(cellHeaderBody1);
            //cellHeaderBody1.Phrase = new Phrase("Unit", normal_font);
            //headerTable3.AddCell(cellHeaderBody1);
            cellHeaderBody1.Phrase = new Phrase("Jumlah", normal_font);
            headerTable3.AddCell(cellHeaderBody1);

            int index = 1;
            decimal count_price = 0;
            decimal total_realization = 0;

            //var items = viewModel.numberVB.UnitLoad.Split(",");

            //string lastitem = items[items.Length - 1];

            //lastitem = lastitem.Trim();

            decimal total_all = 0;
            //foreach (var item in viewModel.Items)
            //{
            //    if (item.IsGetPPn)
            //    {
            //        var temp = item.Amount * 0.1m;
            //        total_all += item.Amount + temp;
            //    }
            //    else
            //    {
            //        total_all += item.Amount;
            //    }
            //}

            var currencyCode = viewModel.Header.CurrencyCode;
            var currencydescription = viewModel.Header.CurrencyDescription;

            foreach (var itm in viewModel.Items)
            {
                cellHeaderBody1.Phrase = new Phrase(index.ToString(), normal_font);
                headerTable3.AddCell(cellHeaderBody1);
                index++;

                cellHeaderBody1.Phrase = new Phrase(itm.Date.AddHours(timeoffsset).ToString("dd/MM/yyyy"), normal_font);
                headerTable3.AddCell(cellHeaderBody1);

                cellHeaderBody1.Phrase = new Phrase(itm.UnitPaymentOrderNo, normal_font);
                headerTable3.AddCell(cellHeaderBody1);

                cellHeaderBody1.Phrase = new Phrase(" ", normal_font);
                headerTable3.AddCell(cellHeaderBody1);

                cellHeaderBody1.Phrase = new Phrase(itm.Amount.ToString("#,##0.00", new CultureInfo("id-ID")), normal_font);
                headerTable3.AddCell(cellHeaderBody1);

                cellHeaderBody1.Phrase = new Phrase(itm.UseVat ? "Ya" : "Tidak", normal_font);
                headerTable3.AddCell(cellHeaderBody1);

                cellHeaderBody1.Phrase = new Phrase(itm.UseIncomeTax ? "Ya" : "Tidak", normal_font);
                headerTable3.AddCell(cellHeaderBody1);
                if (itm.UseVat)
                {
                    var temp = itm.Amount * 0.1m;
                    total_all = itm.Amount + temp;

                    //cellHeaderBody1.Phrase = new Phrase(temp.ToString("#,##0.00", new CultureInfo("id-ID")), normal_font);
                    //headerTable3.AddCell(cellHeaderBody1);

                }
                else
                {
                    total_all = itm.Amount;
                    //cellHeaderBody1.Phrase = new Phrase(0.ToString("#,##0.00", new CultureInfo("id-ID")), normal_font);
                    //headerTable3.AddCell(cellHeaderBody1);
                }

                //if (itm.UseIncomeTax && itm.IncomeTaxBy == "Supplier")
                //{
                //    var pphTemp = itm.Amount * ((decimal)itm.IncomeTaxRate / 100);

                //    cellHeaderBody1.Phrase = new Phrase(pphTemp.ToString("#,##0.00", new CultureInfo("id-ID")), normal_font);
                //    headerTable3.AddCell(cellHeaderBody1);
                //}
                //else
                //{
                //    cellHeaderBody1.Phrase = new Phrase(0.ToString("#,##0.00", new CultureInfo("id-ID")), normal_font);
                //    headerTable3.AddCell(cellHeaderBody1);
                //}

                cellHeaderBody1.Phrase = new Phrase($"{currencyCode}        " + itm.Amount.ToString("#,##0.00", new CultureInfo("id-ID")), normal_font);
                headerTable3.AddCell(cellHeaderBody1);
                count_price += total_all;
                total_realization += itm.Amount;
            }

            cellHeaderBody1b.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody1b);
            cellHeaderBody1b.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody1b);
            //cellHeaderBody1b.Phrase = new Phrase(" ", normal_font);
            //headerTable3.AddCell(cellHeaderBody1b);
            cellHeaderBody1a.Colspan = 5;
            cellHeaderBody1a.Phrase = new Phrase("Total Realisasi", normal_font);
            headerTable3.AddCell(cellHeaderBody1a);
            cellHeaderBody1b.Phrase = new Phrase($"{currencyCode}       " + total_realization.ToString("#,##0.00", new CultureInfo("id-ID")), normal_font);
            headerTable3.AddCell(cellHeaderBody1b);



            cellHeaderBody1b1.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody1b1);
            //cellHeaderBody1b.Phrase = new Phrase(" ", normal_font);
            //headerTable3.AddCell(cellHeaderBody1b);
            cellHeaderBody1a1.Colspan = 6;
            cellHeaderBody1a1.Phrase = new Phrase("PPn", normal_font);
            headerTable3.AddCell(cellHeaderBody1a1);
            cellHeaderBody1b1.Phrase = new Phrase($"{currencyCode}       " + (count_price - total_realization).ToString("#,##0.00", new CultureInfo("id-ID")), normal_font);
            headerTable3.AddCell(cellHeaderBody1b1);

            cellHeaderBody1b1.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody1b1);
            //cellHeaderBody1b.Phrase = new Phrase(" ", normal_font);
            //headerTable3.AddCell(cellHeaderBody1b);
            cellHeaderBody1a1.Colspan = 6;
            cellHeaderBody1a1.Phrase = new Phrase("PPh", normal_font);
            headerTable3.AddCell(cellHeaderBody1a1);
            cellHeaderBody1b1.Phrase = new Phrase($"{currencyCode}       " + (GetPPhValue(viewModel)).ToString("#,##0.00", new CultureInfo("id-ID")), normal_font);
            headerTable3.AddCell(cellHeaderBody1b1);

            cellHeaderBody1b2.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody1b2);
            //cellHeaderBody1b.Phrase = new Phrase(" ", normal_font);
            //headerTable3.AddCell(cellHeaderBody1b);
            cellHeaderBody1a2.Colspan = 6;
            cellHeaderBody1a2.Phrase = new Phrase("Total Keseluruhan", normal_font);
            var grandTotal = count_price - GetPPhValue(viewModel);
            headerTable3.AddCell(cellHeaderBody1a2);
            cellHeaderBody1b2.Phrase = new Phrase($"{currencyCode}       " + grandTotal.ToString("#,##0.00", new CultureInfo("id-ID")), normal_font);
            headerTable3.AddCell(cellHeaderBody1b2);

            //cellHeaderBody1.Colspan = 2;
            cellHeaderBody6.Colspan = 2;
            //cellHeaderBody6.Phrase = new Phrase(" ", normal_font);
            //headerTable3.AddCell(cellHeaderBody6);
            var vbDate = viewModel.Header.VBRequestDocumentDate?.AddHours(timeoffsset).ToString("dd-MMMM-yy", new CultureInfo("id-ID"));
            if (viewModel.Header.DocumentType == RealizationDocumentType.NonVB)
                if (!string.IsNullOrWhiteSpace(viewModel.Header.VBRequestDocumentNo))
                    vbDate = viewModel.Header.VBRequestDocumentDate?.AddHours(timeoffsset).ToString("dd-MMMM-yy", new CultureInfo("id-ID"));
                else
                    vbDate = "";

            cellHeaderBody6.Phrase = new Phrase($"Tanggal VB : {vbDate}", normal_font);
            headerTable3.AddCell(cellHeaderBody6);
            //
            cellHeaderBody1.Colspan = 5;
            cellHeaderBody1.Phrase = new Phrase($"No.VB: {viewModel.Header.VBRequestDocumentNo}", normal_font);
            headerTable3.AddCell(cellHeaderBody1);

            if (viewModel.Header.VBRequestDocumentId == 0)
            {
                cellHeaderBody1.Phrase = new Phrase($"{currencyCode}        " + 0.ToString("#,##0.00", new CultureInfo("id-ID")), normal_font);
                headerTable3.AddCell(cellHeaderBody1);
            }
            else
            {
                cellHeaderBody1.Phrase = new Phrase($"{currencyCode}        " + viewModel.Header.VBRequestDocumentAmount.ToString("#,##0.00", new CultureInfo("id-ID")), normal_font);
                headerTable3.AddCell(cellHeaderBody1);
            }



            var priceterbilang = count_price;

            var res = (count_price - GetPPhValue(viewModel)) - (viewModel.Header.VBRequestDocumentId == 0 ? 0 : viewModel.Header.VBRequestDocumentAmount);

            cellHeaderBody5.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody5);
            cellHeaderBody5.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody5);
            cellHeaderBody5.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody5);
            cellHeaderBody5.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody5);
            cellHeaderBody5.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody5);
            cellHeaderBody5.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody5);
            //cellHeaderBody5.Phrase = new Phrase(" ", normal_font);
            //headerTable3.AddCell(cellHeaderBody5);


            if (res > 0)
            {
                cellHeaderBody5.Phrase = new Phrase("Kurang", bold_font);
                headerTable3.AddCell(cellHeaderBody5);

                cellHeaderBody5a.Phrase = new Phrase($"({currencyCode}      " + res.ToString("#,##0.00", new CultureInfo("id-ID")) + ")", normal_font);
                headerTable3.AddCell(cellHeaderBody5a);
            }
            else
            {
                cellHeaderBody5.Phrase = new Phrase("Sisa", bold_font);
                headerTable3.AddCell(cellHeaderBody5);

                cellHeaderBody5a.Phrase = new Phrase($"{currencyCode}       " + (res * -1).ToString("#,##0.00", new CultureInfo("id-ID")), normal_font);
                headerTable3.AddCell(cellHeaderBody5a);
            }

            //count_price /= items.Length;
            string total = count_price.ToString("#,##0.00", new CultureInfo("id-ID"));

            cellHeaderBody4.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody4);
            cellHeaderBody4.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody4);
            cellHeaderBody4.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody4);
            cellHeaderBody4.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody4);
            cellHeaderBody4.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody4);
            cellHeaderBody4.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody4);
            cellHeaderBody4.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody4);
            cellHeaderBody4.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody4);

            cellHeaderBody4a.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody4a);
            cellHeaderBody4a.Phrase = new Phrase("Terbilang : ", normal_font);
            headerTable3.AddCell(cellHeaderBody4a);
            cellHeaderBody4b.Colspan = 6;
            cellHeaderBody4b.Phrase = new Phrase(Nom(grandTotal, currencyCode, currencydescription), normal_font);
            headerTable3.AddCell(cellHeaderBody4b);
            //cellHeaderBody4b.Phrase = new Phrase(" ", normal_font);
            //headerTable3.AddCell(cellHeaderBody4b);

            cellHeaderBody4.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody4);
            cellHeaderBody4.Phrase = new Phrase("Beban Unit :", bold_font);
            headerTable3.AddCell(cellHeaderBody4);
            cellHeaderBody4.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody4);
            cellHeaderBody4.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody4);
            cellHeaderBody4.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody4);
            cellHeaderBody4.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody4);
            cellHeaderBody4.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody4);
            cellHeaderBody4.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody4);

            cellHeader3.AddElement(headerTable3);
            headerTable_B.AddCell(cellHeader3);

            cellHeader4.AddElement(headerTable4);
            headerTable_B.AddCell(cellHeader4);

            document.Add(headerTable_B);

            #endregion Header

            #region NewCheckbox

            var unitCosts = viewModel.UnitCosts.GroupBy(s => s.UnitId).OrderBy(s => s.Key);

            List<PdfFormField> annotations = new List<PdfFormField>();
            foreach (var item in unitCosts)
            {

                if (item.Key == 0)
                {
                    cellHeaderBody.Phrase = new Phrase("......", normal_font_8);
                }
                else
                {
                    cellHeaderBody.Phrase = new Phrase(item.First().UnitName, normal_font_8);
                }

                headerTable3a.AddCell(cellHeaderBody);

                cellHeaderBody.Phrase = new Phrase("", normal_font_8);

                //Create_Box(writer,headerTable3a);


                PdfPCell cellform = new PdfPCell() { Border = Rectangle.NO_BORDER };
                cellform.FixedHeight = 5f;
                //initiate form checkbox 

                PdfFormField _checkGroup = PdfFormField.CreateEmpty(writer);
                RadioCheckField _radioG;
                PdfFormField _radioField1;
                Rectangle kotak = new Rectangle(100, 100);
                _radioG = new RadioCheckField(writer, kotak, "abc", "Yes");
                _radioG.CheckType = RadioCheckField.TYPE_CHECK;
                _radioG.BorderStyle = PdfBorderDictionary.STYLE_SOLID;
                _radioG.BorderColor = BaseColor.Black;
                _radioG.BorderWidth = BaseField.BORDER_WIDTH_MEDIUM;

                _radioG.Checked = true;
                bool flag = true;


                _radioG.Rotation = 0;
                _radioG.Options = TextField.READ_ONLY;
                _radioField1 = _radioG.CheckField;

                cellform.CellEvent
                     = new BebanUnitEvent(_checkGroup, _radioField1, 1);
                headerTable3a.AddCell(cellform);

                if (!flag)
                {
                    cellHeaderBody.Phrase = new Phrase($"...........", normal_font_8);
                    headerTable3a.AddCell(cellHeaderBody);
                }
                else
                {
                    var nom = item.Sum(s => s.Amount).ToString("#,##0.00", new CultureInfo("id-ID"));

                    cellHeaderBody.Phrase = new Phrase($"{currencyCode}   {nom}", normal_font_8); //total
                    headerTable3a.AddCell(cellHeaderBody);
                }

                annotations.Add(_checkGroup);
            }

            for (var i = 0; i < 9 - (3 * (unitCosts.Count() % 3)); i++)
            {
                cellHeaderBody.Phrase = new Phrase(" ", normal_font);
                headerTable3a.AddCell(cellHeaderBody);
            }

            cellHeader3a.AddElement(headerTable3a);
            headerTable_C.AddCell(cellHeader3a);
            document.Add(headerTable_C);

            foreach (var annotation in annotations)
            {
                writer.AddAnnotation(annotation);
            }

            #endregion


            #region Footer

            PdfPTable table = new PdfPTable(4)
            {
                WidthPercentage = 100
            };
            float[] widths = new float[] { 1f, 1f, 1f, 1f };
            table.SetWidths(widths);
            PdfPCell cell = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
            };

            cell.Phrase = new Phrase("", normal_font);
            table.AddCell(cell);
            cell.Phrase = new Phrase("", normal_font);
            table.AddCell(cell);
            cell.Phrase = new Phrase("", normal_font);
            table.AddCell(cell);
            cell.Phrase = new Phrase("", normal_font);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Menyetujui,", normal_font);
            table.AddCell(cell);
            cell.Phrase = new Phrase("Diperiksa,", normal_font);
            table.AddCell(cell);
            cell.Phrase = new Phrase("Mengetahui,", normal_font);
            table.AddCell(cell);
            cell.Phrase = new Phrase("Pembuat laporan,", normal_font);
            table.AddCell(cell);

            for (var i = 0; i < 11; i++)
            {

                cell.Phrase = new Phrase("", normal_font);
                table.AddCell(cell);
                cell.Phrase = new Phrase("", normal_font);
                table.AddCell(cell);
                cell.Phrase = new Phrase("", normal_font);
                table.AddCell(cell);
                cell.Phrase = new Phrase("", normal_font);
                table.AddCell(cell);
            }

            cell.Phrase = new Phrase("(..................)", normal_font);
            table.AddCell(cell);
            cell.Phrase = new Phrase("(..................)", normal_font);
            table.AddCell(cell);
            cell.Phrase = new Phrase("(..................)", normal_font);
            table.AddCell(cell);
            cell.Phrase = new Phrase($"(..................)", normal_font);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Kasir", normal_font);
            table.AddCell(cell);
            cell.Phrase = new Phrase("Verifikasi", normal_font);
            table.AddCell(cell);
            cell.Phrase = new Phrase($"..................", normal_font);
            table.AddCell(cell);
            cell.Phrase = new Phrase(viewModel.Header.SuppliantUnitName, normal_font);
            table.AddCell(cell);

            document.Add(table);
            #endregion Footer

            document.Close();
            byte[] byteInfo = stream.ToArray();
            stream.Write(byteInfo, 0, byteInfo.Length);
            stream.Position = 0;

            return stream;
        }


        private string Nom(decimal total, string symbol, string description)
        {
            string TotalPaidString;
            string CurrencySay;
            if (symbol == "Rp" || symbol.ToUpper() == "IDR")
            {
                TotalPaidString = NumberToTextIDN.terbilang((double)total);
                CurrencySay = "Rupiah";
            }
            else
            {
                TotalPaidString = NumberToTextIDN.terbilang((double)total);
                CurrencySay = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(description.ToLower());
            }

            return TotalPaidString + " " + CurrencySay;
        }


        private decimal GetPPhValue(VBRealizationPdfDto viewModel)
        {
            decimal val = 0;

            foreach (var itm in viewModel.Items)
            {
                if (itm.UseIncomeTax == true && itm.IncomeTaxBy == "Supplier")
                {
                    val += itm.Amount * ((decimal)itm.IncomeTaxRate / 100);
                }
            }

            return val;
        }
    }
}
