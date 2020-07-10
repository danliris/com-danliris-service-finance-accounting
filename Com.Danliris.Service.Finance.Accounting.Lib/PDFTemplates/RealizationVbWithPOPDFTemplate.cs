using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.PDFTemplates;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;

namespace Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.RealizationVBWIthPO
{
    public class RealizationVbWithPOPDFTemplate
    {
        public MemoryStream GeneratePdfTemplate(RealizationVbWithPOViewModel viewModel, int timeoffsset)
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
            PdfPTable headerTable3 = new PdfPTable(5);
            PdfPTable headerTable3a = new PdfPTable(9);
            PdfPTable headerTable4 = new PdfPTable(2);
            headerTable_A.SetWidths(new float[] { 10f, 10f });
            headerTable_A.WidthPercentage = 100;
            headerTable3.SetWidths(new float[] { 10f, 20f, 40f, 20f, 20f });
            headerTable3.WidthPercentage = 110;
            headerTable3a.SetWidths(new float[] { 20f, 3f, 20f, 20f, 3f, 20f, 20f, 3f, 20f});
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
            PdfPCell cellHeaderBody1b = new PdfPCell() { BorderWidthTop = 2 };
            PdfPCell cellHeaderBody2 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellHeaderBody3 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellHeaderBody4 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellHeaderBody5 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellHeaderBody6 = new PdfPCell() {  };

            //cellHeaderBody.Phrase = new Phrase("Kepada Yth.......", normal_font);
            //headerTable1.AddCell(cellHeaderBody);
            //cellHeaderBody.Phrase = new Phrase("Kasir PT. Danliris", normal_font);
            //headerTable1.AddCell(cellHeaderBody);
            //cellHeaderBody.Phrase = new Phrase("Di tempat", normal_font);
            //headerTable1.AddCell(cellHeaderBody);

            cellHeader1.AddElement(headerTable1);
            headerTable_A.AddCell(cellHeader1);

            cellHeader2.AddElement(headerTable2);
            headerTable_A.AddCell(cellHeader2);

            document.Add(headerTable_A);

            cellHeaderBody.HorizontalAlignment = Element.ALIGN_LEFT;
            cellHeaderBody1.HorizontalAlignment = Element.ALIGN_CENTER;
            cellHeaderBody1a.HorizontalAlignment = Element.ALIGN_CENTER;
            cellHeaderBody1b.HorizontalAlignment = Element.ALIGN_CENTER;
            cellHeaderBody2.HorizontalAlignment = Element.ALIGN_CENTER;
            cellHeaderBody3.HorizontalAlignment = Element.ALIGN_LEFT;
            cellHeaderBody4.HorizontalAlignment = Element.ALIGN_CENTER;
            cellHeaderBody5.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellHeaderBody6.HorizontalAlignment = Element.ALIGN_LEFT;

            cellHeaderBody2.Colspan = 5;
            cellHeaderBody2.Phrase = new Phrase("REALISASI VB", bold_font);
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

            cellHeaderBody3.Colspan = 5;
            cellHeaderBody3.Phrase = new Phrase($"Realisasi VB Bagian: {viewModel.numberVB.UnitName}", bold_font);
            headerTable3.AddCell(cellHeaderBody3);

            cellHeaderBody3.Colspan = 5;
            cellHeaderBody3.Phrase = new Phrase($"Tanggal: {viewModel.Date?.AddHours(timeoffsset).ToString("dd MMMM yyyy")}", bold_font);
            headerTable3.AddCell(cellHeaderBody3);

            cellHeaderBody1.Phrase = new Phrase("No", normal_font);
            headerTable3.AddCell(cellHeaderBody1);
            cellHeaderBody1.Phrase = new Phrase("Tanggal", normal_font);
            headerTable3.AddCell(cellHeaderBody1);
            cellHeaderBody1.Phrase = new Phrase("Keterangan", normal_font);
            headerTable3.AddCell(cellHeaderBody1);
            cellHeaderBody1.Phrase = new Phrase("Unit", normal_font);
            headerTable3.AddCell(cellHeaderBody1);
            cellHeaderBody1.Phrase = new Phrase("Jumlah", normal_font);
            headerTable3.AddCell(cellHeaderBody1);

            int index = 1;

            foreach(var itm in viewModel.Items)
            {
                cellHeaderBody1.Phrase = new Phrase(index.ToString(), normal_font);
                headerTable3.AddCell(cellHeaderBody1);
                index++;
                
                cellHeaderBody1.Phrase = new Phrase(itm.date?.AddHours(timeoffsset).ToString("dd/MM/yyyy"), normal_font);
                headerTable3.AddCell(cellHeaderBody1);

                cellHeaderBody1.Phrase = new Phrase(itm.no, normal_font);
                headerTable3.AddCell(cellHeaderBody1);

                cellHeaderBody1.Phrase = new Phrase("", normal_font);
                headerTable3.AddCell(cellHeaderBody1);

                cellHeaderBody1.Phrase = new Phrase("", normal_font);
                headerTable3.AddCell(cellHeaderBody1);
            }

            cellHeaderBody1b.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody1b);
            cellHeaderBody1b.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody1b);
            cellHeaderBody1a.Colspan = 2;
            cellHeaderBody1a.Phrase = new Phrase("Total Realisasi", normal_font);
            headerTable3.AddCell(cellHeaderBody1a);
            cellHeaderBody1b.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody1b);
            //cellHeaderBody1.Phrase = new Phrase(" ", normal_font);
            //headerTable3.AddCell(cellHeaderBody1);

            //cellHeaderBody1.Colspan = 2;
            cellHeaderBody1.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody1);
            cellHeaderBody1.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody1);
            cellHeaderBody6.Colspan = 2;
            cellHeaderBody6.Phrase = new Phrase($"No. VB {viewModel.numberVB.VBNo}", normal_font);
            headerTable3.AddCell(cellHeaderBody6);
            cellHeaderBody1.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody1);
            //cellHeaderBody1.Phrase = new Phrase(" ", normal_font);
            //headerTable3.AddCell(cellHeaderBody1);

            cellHeaderBody5.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody5);
            cellHeaderBody5.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody5);
            cellHeaderBody5.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody5);
            //cellHeaderBody5.Colspan = 4;
            cellHeaderBody5.Phrase = new Phrase("Kurang/Sisa", normal_font);
            headerTable3.AddCell(cellHeaderBody5);
            cellHeaderBody5.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody5);

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
            cellHeaderBody4.Phrase = new Phrase("Terbilang : ", normal_font);
            headerTable3.AddCell(cellHeaderBody4);
            cellHeaderBody4.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody4);
            cellHeaderBody4.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody4);
            cellHeaderBody4.Phrase = new Phrase(" ", normal_font);
            headerTable3.AddCell(cellHeaderBody4);

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
            //cellHeaderBody.Phrase = new Phrase("VB Uang", normal_font);
            //headerTable3.AddCell(cellHeaderBody);
            //cellHeaderBody.Phrase = new Phrase(":", normal_font);
            //headerTable3.AddCell(cellHeaderBody);

            //decimal convertCurrency = 0;
            //string Usage = "";

            ////foreach (var itm1 in viewModel.Items)
            ////{
            ////    foreach (var itm2 in itm1.Details)
            ////    {
            ////        convertCurrency += itm2.priceBeforeTax;
            ////        Usage += itm2.product.name + ", ";
            ////    }
            ////}
            ////Usage = Usage.Remove(Usage.Length - 2);

            //cellHeaderBody.Phrase = new Phrase("Rp. "/* + convertCurrency.ToString("#,##0.00", new CultureInfo("id-ID"))*/, normal_font);
            //headerTable3.AddCell(cellHeaderBody);
            //cellHeaderBody.Phrase = new Phrase(" ", normal_font);
            //headerTable3.AddCell(cellHeaderBody);
            //cellHeaderBody.Phrase = new Phrase(" ", normal_font);
            //headerTable3.AddCell(cellHeaderBody);


            //cellHeaderBody.Phrase = new Phrase("Terbilang", normal_font);
            //headerTable3.AddCell(cellHeaderBody);
            //cellHeaderBody.Phrase = new Phrase(":", normal_font);
            //headerTable3.AddCell(cellHeaderBody);


            //string TotalPaidString = "";/*NumberToTextIDN.terbilang(Decimal.ToDouble(convertCurrency))*/;

            //cellHeaderBody.Phrase = new Phrase(TotalPaidString + " Rupiah", normal_font);
            //headerTable3.AddCell(cellHeaderBody);
            //cellHeaderBody.Phrase = new Phrase(" ", normal_font);
            //headerTable3.AddCell(cellHeaderBody);
            //cellHeaderBody.Phrase = new Phrase(" ", normal_font);
            //headerTable3.AddCell(cellHeaderBody);

            //cellHeaderBody.Phrase = new Phrase("Kegunaan", normal_font);
            //headerTable3.AddCell(cellHeaderBody);
            //cellHeaderBody.Phrase = new Phrase(":", normal_font);
            //headerTable3.AddCell(cellHeaderBody);
            //cellHeaderBody.Phrase = new Phrase(Usage, normal_font);
            //headerTable3.AddCell(cellHeaderBody);
            //cellHeaderBody.Phrase = new Phrase(" ", normal_font);
            //headerTable3.AddCell(cellHeaderBody);
            //cellHeaderBody.Phrase = new Phrase(" ", normal_font);
            //headerTable3.AddCell(cellHeaderBody);

            //cellHeaderBody.Phrase = new Phrase(" ", normal_font);
            //headerTable3.AddCell(cellHeaderBody);
            //cellHeaderBody.Phrase = new Phrase(" ", normal_font);
            //headerTable3.AddCell(cellHeaderBody);
            //cellHeaderBody.Phrase = new Phrase(" ", normal_font);
            //headerTable3.AddCell(cellHeaderBody);
            //cellHeaderBody.Phrase = new Phrase(" ", normal_font);
            //headerTable3.AddCell(cellHeaderBody);
            //cellHeaderBody.Phrase = new Phrase(" ", normal_font);
            //headerTable3.AddCell(cellHeaderBody);

            //cellHeaderBody.Phrase = new Phrase("Beban Unit  :", normal_font);
            //headerTable3.AddCell(cellHeaderBody);
            //cellHeaderBody.Phrase = new Phrase(" ", normal_font);
            //headerTable3.AddCell(cellHeaderBody);
            //cellHeaderBody.Phrase = new Phrase(" ", normal_font);
            //headerTable3.AddCell(cellHeaderBody);
            //cellHeaderBody.Phrase = new Phrase(" ", normal_font);
            //headerTable3.AddCell(cellHeaderBody);
            //cellHeaderBody.Phrase = new Phrase(" ", normal_font);
            //headerTable3.AddCell(cellHeaderBody);

            cellHeader3.AddElement(headerTable3);
            headerTable_B.AddCell(cellHeader3);

            cellHeader4.AddElement(headerTable4);
            headerTable_B.AddCell(cellHeader4);

            document.Add(headerTable_B);
            //writer.AddAnnotation(_checkGroup);
            #endregion Header

            #region CheckBox
            string unit = "";
            //foreach (var itm in viewModel.Items)
            //{
            //    unit += itm.unit.Name + ",";
            //}
            //unit = unit.Remove(unit.Length - 1);
            //var items = unit.Split(",");

            //string lastitem = items[items.Length - 1];

            //lastitem = lastitem.Trim();
            cellHeaderBody.Phrase = new Phrase("Spinning 1", normal_font_8);
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

            if (unit.ToUpper().Contains("SPINNING 1"))
            {
                _radioG.Checked = true;
            }
            else
            {
                _radioG.Checked = false;
            }
            _radioG.Rotation = 90;
            _radioG.Options = TextField.READ_ONLY;
            _radioField1 = _radioG.CheckField;

            cellform.CellEvent
                 = new BebanUnitEvent(_checkGroup, _radioField1, 1);
            headerTable3a.AddCell(cellform);
            cellHeaderBody.Phrase = new Phrase(":Rp...........", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("Printing", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("", normal_font_8);
            PdfPCell cellform11 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            cellform11.FixedHeight = 5f;
            //initiate form checkbox
            PdfFormField _checkGroup11 = PdfFormField.CreateEmpty(writer);
            RadioCheckField _radioG11;
            PdfFormField _radioField111;
            Rectangle kotak11 = new Rectangle(100, 100);
            _radioG11 = new RadioCheckField(writer, kotak11, "abc", "Yes");
            _radioG11.CheckType = RadioCheckField.TYPE_CHECK;
            _radioG11.BorderStyle = PdfBorderDictionary.STYLE_SOLID;
            _radioG11.BorderColor = BaseColor.Black;
            _radioG11.BorderWidth = BaseField.BORDER_WIDTH_MEDIUM;
            if (unit.ToUpper().Contains("PRINTING"))
            {
                _radioG11.Checked = true;
            }
            else
            {
                _radioG11.Checked = false;
            }
            _radioG11.Rotation = 90;
            _radioG11.Options = TextField.READ_ONLY;
            _radioField111 = _radioG11.CheckField;
            cellform11.CellEvent
                 = new BebanUnitEvent(_checkGroup11, _radioField111, 1);
            headerTable3a.AddCell(cellform11);
            cellHeaderBody.Phrase = new Phrase(":Rp...........", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("Konfeksi 2 B", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("", normal_font_8);
            PdfPCell cellform8 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            cellform8.FixedHeight = 5f;
            //initiate form checkbox
            PdfFormField _checkGroup8 = PdfFormField.CreateEmpty(writer);
            RadioCheckField _radioG8;
            PdfFormField _radioField18;
            Rectangle kotak8 = new Rectangle(100, 100);
            _radioG8 = new RadioCheckField(writer, kotak8, "abc", "Yes");
            _radioG8.CheckType = RadioCheckField.TYPE_CHECK;
            _radioG8.BorderStyle = PdfBorderDictionary.STYLE_SOLID;
            _radioG8.BorderColor = BaseColor.Black;
            _radioG8.BorderWidth = BaseField.BORDER_WIDTH_MEDIUM;
            if (unit.ToUpper().Contains("KONFEKSI 2B"))
            {
                _radioG8.Checked = true;
            }
            else
            {
                _radioG8.Checked = false;
            }
            _radioG8.Rotation = 90;
            _radioG8.Options = TextField.READ_ONLY;
            _radioField18 = _radioG8.CheckField;
            cellform8.CellEvent
                 = new BebanUnitEvent(_checkGroup8, _radioField18, 1);
            headerTable3a.AddCell(cellform8);
            cellHeaderBody.Phrase = new Phrase(":Rp...........", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("Spinning 2", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("", normal_font_8);
            PdfPCell cellform5 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            cellform5.FixedHeight = 5f;
            //initiate form checkbox
            PdfFormField _checkGroup5 = PdfFormField.CreateEmpty(writer);
            RadioCheckField _radioG5;
            PdfFormField _radioField15;
            Rectangle kotak5 = new Rectangle(100, 100);
            _radioG5 = new RadioCheckField(writer, kotak5, "abc", "Yes");
            _radioG5.CheckType = RadioCheckField.TYPE_CHECK;
            _radioG5.BorderStyle = PdfBorderDictionary.STYLE_SOLID;
            _radioG5.BorderColor = BaseColor.Black;
            _radioG5.BorderWidth = BaseField.BORDER_WIDTH_MEDIUM;
            if (unit.ToUpper().Contains("SPINNING 2"))
            {
                _radioG5.Checked = true;
            }
            else
            {
                _radioG5.Checked = false;
            }
            _radioG5.Rotation = 90;
            _radioG5.Options = TextField.READ_ONLY;
            _radioField15 = _radioG5.CheckField;
            cellform5.CellEvent
                 = new BebanUnitEvent(_checkGroup5, _radioField15, 1);
            headerTable3a.AddCell(cellform5);
            cellHeaderBody.Phrase = new Phrase(":Rp...........", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("Dyeing", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("", normal_font_8);
            PdfPCell cellform5a = new PdfPCell() { Border = Rectangle.NO_BORDER };
            cellform5.FixedHeight = 5f;
            //initiate form checkbox
            PdfFormField _checkGroup5a = PdfFormField.CreateEmpty(writer);
            RadioCheckField _radioG5a;
            PdfFormField _radioField15a;
            Rectangle kotak5a = new Rectangle(100, 100);
            _radioG5a = new RadioCheckField(writer, kotak5a, "abc", "Yes");
            _radioG5a.CheckType = RadioCheckField.TYPE_CHECK;
            _radioG5a.BorderStyle = PdfBorderDictionary.STYLE_SOLID;
            _radioG5a.BorderColor = BaseColor.Black;
            _radioG5a.BorderWidth = BaseField.BORDER_WIDTH_MEDIUM;
            if (unit.ToUpper().Contains("SPINNING 2"))
            {
                _radioG5a.Checked = true;
            }
            else
            {
                _radioG5a.Checked = false;
            }
            _radioG5a.Rotation = 90;
            _radioG5a.Options = TextField.READ_ONLY;
            _radioField15a = _radioG5a.CheckField;
            cellform5a.CellEvent
                 = new BebanUnitEvent(_checkGroup5a, _radioField15a, 1);
            headerTable3a.AddCell(cellform5a);
            cellHeaderBody.Phrase = new Phrase(":Rp...........", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("Konfeksi 2C", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("", normal_font_8);
            PdfPCell cellform13 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            cellform13.FixedHeight = 5f;
            //initiate form checkbox
            PdfFormField _checkGroup13 = PdfFormField.CreateEmpty(writer);
            RadioCheckField _radioG13;
            PdfFormField _radioField113;
            Rectangle kotak13 = new Rectangle(100, 100);
            _radioG13 = new RadioCheckField(writer, kotak13, "abc", "Yes");
            _radioG13.CheckType = RadioCheckField.TYPE_CHECK;
            _radioG13.BorderStyle = PdfBorderDictionary.STYLE_SOLID;
            _radioG13.BorderColor = BaseColor.Black;
            _radioG13.BorderWidth = BaseField.BORDER_WIDTH_MEDIUM;
            if (unit.ToUpper().Contains("KONFEKSI 2C"))
            {
                _radioG13.Checked = true;
            }
            else
            {
                _radioG13.Checked = false;
            }
            _radioG13.Rotation = 90;
            _radioG13.Options = TextField.READ_ONLY;
            _radioField113 = _radioG13.CheckField;
            cellform13.CellEvent
                 = new BebanUnitEvent(_checkGroup13, _radioField113, 1);
            headerTable3a.AddCell(cellform13);
            cellHeaderBody.Phrase = new Phrase(":Rp...........", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("Spinning 3", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("", normal_font_8);
            PdfPCell cellform10 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            cellform10.FixedHeight = 5f;
            //initiate form checkbox
            PdfFormField _checkGroup10 = PdfFormField.CreateEmpty(writer);
            RadioCheckField _radioG10;
            PdfFormField _radioField110;
            Rectangle kotak10 = new Rectangle(100, 100);
            _radioG10 = new RadioCheckField(writer, kotak10, "abc", "Yes");
            _radioG10.CheckType = RadioCheckField.TYPE_CHECK;
            _radioG10.BorderStyle = PdfBorderDictionary.STYLE_SOLID;
            _radioG10.BorderColor = BaseColor.Black;
            _radioG10.BorderWidth = BaseField.BORDER_WIDTH_MEDIUM;
            if (unit.ToUpper().Contains("SPINNING 3"))
            {
                _radioG10.Checked = true;
            }
            else
            {
                _radioG10.Checked = false;
            }
            _radioG10.Rotation = 90;
            _radioG10.Options = TextField.READ_ONLY;
            _radioField110 = _radioG10.CheckField;
            cellform10.CellEvent
                 = new BebanUnitEvent(_checkGroup10, _radioField110, 1);
            headerTable3a.AddCell(cellform10);
            cellHeaderBody.Phrase = new Phrase(":Rp...........", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("Konfeksi 1A", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("", normal_font_8);
            PdfPCell cellform7 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            cellform7.FixedHeight = 5f;
            //initiate form checkbox
            PdfFormField _checkGroup7 = PdfFormField.CreateEmpty(writer);
            RadioCheckField _radioG7;
            PdfFormField _radioField17;
            Rectangle kotak7 = new Rectangle(100, 100);
            _radioG7 = new RadioCheckField(writer, kotak7, "abc", "Yes");
            _radioG7.CheckType = RadioCheckField.TYPE_CHECK;
            _radioG7.BorderStyle = PdfBorderDictionary.STYLE_SOLID;
            _radioG7.BorderColor = BaseColor.Black;
            _radioG7.BorderWidth = BaseField.BORDER_WIDTH_MEDIUM;
            if (unit.ToUpper().Contains("KONFEKSI 1A"))
            {
                _radioG7.Checked = true;
            }
            else
            {
                _radioG7.Checked = false;
            }
            _radioG7.Rotation = 90;
            _radioG7.Options = TextField.READ_ONLY;
            _radioField17 = _radioG7.CheckField;
            cellform7.CellEvent
                 = new BebanUnitEvent(_checkGroup7, _radioField17, 1);
            headerTable3a.AddCell(cellform7);
            cellHeaderBody.Phrase = new Phrase(":Rp...........", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("Umum", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("", normal_font_8);
            PdfPCell cellform4 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            cellform4.FixedHeight = 5f;
            //initiate form checkbox
            PdfFormField _checkGroup4 = PdfFormField.CreateEmpty(writer);
            RadioCheckField _radioG4;
            PdfFormField _radioField14;
            Rectangle kotak4 = new Rectangle(100, 100);
            _radioG4 = new RadioCheckField(writer, kotak4, "abc", "Yes");
            _radioG4.CheckType = RadioCheckField.TYPE_CHECK;
            _radioG4.BorderStyle = PdfBorderDictionary.STYLE_SOLID;
            _radioG4.BorderColor = BaseColor.Black;
            _radioG4.BorderWidth = BaseField.BORDER_WIDTH_MEDIUM;
            if (unit.ToUpper().Contains("UMUM"))
            {
                _radioG4.Checked = true;
            }
            else
            {
                _radioG4.Checked = false;
            }
            _radioG4.Rotation = 90;
            _radioG4.Options = TextField.READ_ONLY;
            _radioField14 = _radioG4.CheckField;
            cellform4.CellEvent
                 = new BebanUnitEvent(_checkGroup4, _radioField14, 1);
            headerTable3a.AddCell(cellform4);
            cellHeaderBody.Phrase = new Phrase(":Rp...........", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("Weaving 1", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("", normal_font_8);
            PdfPCell cellform1 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            cellform1.FixedHeight = 5f;
            //initiate form checkbox 

            PdfFormField _checkGroup1 = PdfFormField.CreateEmpty(writer);
            RadioCheckField _radioG1;
            PdfFormField _radioField11;
            Rectangle kotak1 = new Rectangle(100, 100);
            _radioG1 = new RadioCheckField(writer, kotak1, "abc", "Yes");
            _radioG1.CheckType = RadioCheckField.TYPE_CHECK;
            _radioG1.BorderStyle = PdfBorderDictionary.STYLE_SOLID;
            _radioG1.BorderColor = BaseColor.Black;
            _radioG1.BorderWidth = BaseField.BORDER_WIDTH_MEDIUM;
            if (unit.ToUpper().Contains("WEAVING 1"))
            {
                _radioG1.Checked = true;
            }
            else
            {
                _radioG1.Checked = false;
            }
            _radioG1.Rotation = 90;
            _radioG1.Options = TextField.READ_ONLY;
            _radioField11 = _radioG1.CheckField;

            cellform1.CellEvent
                 = new BebanUnitEvent(_checkGroup1, _radioField11, 1);
            headerTable3a.AddCell(cellform1);
            cellHeaderBody.Phrase = new Phrase(":Rp...........", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("Konfeksi 1B", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("", normal_font_8);
            PdfPCell cellform12 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            cellform12.FixedHeight = 5f;
            //initiate form checkbox
            PdfFormField _checkGroup12 = PdfFormField.CreateEmpty(writer);
            RadioCheckField _radioG12;
            PdfFormField _radioField112;
            Rectangle kotak12 = new Rectangle(100, 100);
            _radioG12 = new RadioCheckField(writer, kotak12, "abc", "Yes");
            _radioG12.CheckType = RadioCheckField.TYPE_CHECK;
            _radioG12.BorderStyle = PdfBorderDictionary.STYLE_SOLID;
            _radioG12.BorderColor = BaseColor.Black;
            _radioG12.BorderWidth = BaseField.BORDER_WIDTH_MEDIUM;
            if (unit.ToUpper().Contains("KONFEKSI 1B"))
            {
                _radioG12.Checked = true;
            }
            else
            {
                _radioG12.Checked = false;
            }
            _radioG12.Rotation = 90;
            _radioG12.Options = TextField.READ_ONLY;
            _radioField112 = _radioG12.CheckField;
            cellform12.CellEvent
                 = new BebanUnitEvent(_checkGroup12, _radioField112, 1);
            headerTable3a.AddCell(cellform12);
            cellHeaderBody.Phrase = new Phrase(":Rp...........", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("Kosong", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase("", normal_font_8);
            PdfPCell cellform9 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            cellform9.FixedHeight = 5f;
            //initiate form checkbox
            PdfFormField _checkGroup9 = PdfFormField.CreateEmpty(writer);
            RadioCheckField _radioG9;
            PdfFormField _radioField19;
            Rectangle kotak9 = new Rectangle(100, 100);
            _radioG9 = new RadioCheckField(writer, kotak9, "abc", "Yes");
            _radioG9.CheckType = RadioCheckField.TYPE_CHECK;
            _radioG9.BorderStyle = PdfBorderDictionary.STYLE_SOLID;
            _radioG9.BorderColor = BaseColor.Black;
            _radioG9.BorderWidth = BaseField.BORDER_WIDTH_MEDIUM;

            //string res;
            //if (lastitem.ToUpper() == "SPINNING 1" || lastitem.ToUpper() == "SPINNING 2" || lastitem.ToUpper() == "SPINNING 3" || lastitem.ToUpper() == "WEAVING 1" || lastitem.ToUpper() == "WEAVING 2" &&
            //    lastitem.ToUpper() == "PRINTING" || lastitem.ToUpper() == "FINISHING" || lastitem.ToUpper() == "KONFEKSI 1A" || lastitem.ToUpper() == "KONFEKSI 1B"
            //    || lastitem.ToUpper() == "KONFEKSI 2A" || lastitem.ToUpper() == "KONFEKSI 2B" || lastitem.ToUpper() == "KONFEKSI 2C" || lastitem.ToUpper() == "UMUM")
            //{
            //    _radioG9.Checked = false;
            //    res = ".......";
            //}
            //else
            //{
            //    _radioG9.Checked = true;
            //    res = lastitem;
            //}

            _radioG9.Rotation = 90;
            _radioG9.Options = TextField.READ_ONLY;
            _radioField19 = _radioG9.CheckField;
            cellform9.CellEvent
                 = new BebanUnitEvent(_checkGroup9, _radioField19, 1);
            headerTable3a.AddCell(cellform9);
            //cellHeaderBody.Phrase = new Phrase("", normal_font_8);
            //headerTable3a.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase(":Rp...........", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("Weaving 2", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("", normal_font_8);
            PdfPCell cellform6 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            cellform6.FixedHeight = 5f;
            //initiate form checkbox
            PdfFormField _checkGroup6 = PdfFormField.CreateEmpty(writer);
            RadioCheckField _radioG6;
            PdfFormField _radioField16;
            Rectangle kotak6 = new Rectangle(100, 100);
            _radioG6 = new RadioCheckField(writer, kotak6, "abc", "Yes");
            _radioG6.CheckType = RadioCheckField.TYPE_CHECK;
            _radioG6.BorderStyle = PdfBorderDictionary.STYLE_SOLID;
            _radioG6.BorderColor = BaseColor.Black;
            _radioG6.BorderWidth = BaseField.BORDER_WIDTH_MEDIUM;
            if (unit.ToUpper().Contains("WEAVING 2"))
            {
                _radioG6.Checked = true;
            }
            else
            {
                _radioG6.Checked = false;
            }
            _radioG6.Rotation = 90;
            _radioG6.Options = TextField.READ_ONLY;
            _radioField16 = _radioG6.CheckField;
            cellform6.CellEvent
                 = new BebanUnitEvent(_checkGroup6, _radioField16, 1);
            headerTable3a.AddCell(cellform6);
            cellHeaderBody.Phrase = new Phrase(":Rp...........", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("Konfeksi 2 A", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("", normal_font_8);
            PdfPCell cellform3 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            cellform3.FixedHeight = 5f;
            //initiate form checkbox
            PdfFormField _checkGroup3 = PdfFormField.CreateEmpty(writer);
            RadioCheckField _radioG3;
            PdfFormField _radioField13;
            Rectangle kotak3 = new Rectangle(100, 100);
            _radioG3 = new RadioCheckField(writer, kotak3, "abc", "Yes");
            _radioG3.CheckType = RadioCheckField.TYPE_CHECK;
            _radioG3.BorderStyle = PdfBorderDictionary.STYLE_SOLID;
            _radioG3.BorderColor = BaseColor.Black;
            _radioG3.BorderWidth = BaseField.BORDER_WIDTH_MEDIUM;
            if (unit.ToUpper().Contains("KONFEKSI 2A"))
            {
                _radioG3.Checked = true;
            }
            else
            {
                _radioG3.Checked = false;
            }
            _radioG3.Rotation = 90;
            _radioG3.Options = TextField.READ_ONLY;
            _radioField13 = _radioG3.CheckField;
            cellform3.CellEvent
                 = new BebanUnitEvent(_checkGroup3, _radioField13, 1);
            headerTable3a.AddCell(cellform3);
            cellHeaderBody.Phrase = new Phrase(":Rp...........", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);

            
            cellHeaderBody.Phrase = new Phrase("", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase("", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase("", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);





            //================================================




            //================================================

            cellHeaderBody.Phrase = new Phrase(" ", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase(" ", normal_font_8);
            headerTable3a.AddCell(cellHeaderBody);

            cellHeader3a.AddElement(headerTable3a);
            headerTable_C.AddCell(cellHeader3a);
            document.Add(headerTable_C);
            writer.AddAnnotation(_checkGroup);
            writer.AddAnnotation(_checkGroup1);
            //writer.AddAnnotation(_checkGroup2);
            writer.AddAnnotation(_checkGroup3);
            writer.AddAnnotation(_checkGroup4);
            writer.AddAnnotation(_checkGroup5);
            writer.AddAnnotation(_checkGroup5a);
            writer.AddAnnotation(_checkGroup6);
            writer.AddAnnotation(_checkGroup7);
            writer.AddAnnotation(_checkGroup8);
            writer.AddAnnotation(_checkGroup9);
            writer.AddAnnotation(_checkGroup10);
            writer.AddAnnotation(_checkGroup11);
            writer.AddAnnotation(_checkGroup12);
            writer.AddAnnotation(_checkGroup13);
            #endregion

            #region Footer

            PdfPTable table = new PdfPTable(5)
            {
                WidthPercentage = 100
            };
            float[] widths = new float[] { 1f, 1f, 1f, 1f, 1f };
            table.SetWidths(widths);
            PdfPCell cell = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
            };

            PdfPCell cellLeft = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
            };

            PdfPCell cellColspan = new PdfPCell()
            {
                Colspan = 4,
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
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
            cell.Phrase = new Phrase("", normal_font);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Menyetujui,", normal_font);
            table.AddCell(cell);
            cell.Phrase = new Phrase("Diperiksa,", normal_font);
            table.AddCell(cell);
            cell.Phrase = new Phrase("Mengetahui,", normal_font);
            table.AddCell(cell);
            cell.Phrase = new Phrase("", normal_font);
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
                cell.Phrase = new Phrase("", normal_font);
                table.AddCell(cell);
            }

            cell.Phrase = new Phrase("(..................)", normal_font);
            table.AddCell(cell);
            cell.Phrase = new Phrase("(..................)", normal_font);
            table.AddCell(cell);
            cell.Phrase = new Phrase("(..................)", normal_font);
            table.AddCell(cell);
            cell.Phrase = new Phrase("(..................)", normal_font);
            table.AddCell(cell);
            cell.Phrase = new Phrase($"({viewModel.numberVB.CreateBy})", normal_font);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Kasir", normal_font);
            table.AddCell(cell);
            cell.Phrase = new Phrase("Verifikasi", normal_font);
            table.AddCell(cell);
            cell.Phrase = new Phrase("..................", normal_font);
            table.AddCell(cell);
            cell.Phrase = new Phrase("..................", normal_font);
            table.AddCell(cell);
            cell.Phrase = new Phrase(viewModel.numberVB.UnitName, normal_font);
            table.AddCell(cell);

            document.Add(table);
            #endregion Footer

            document.Close();
            byte[] byteInfo = stream.ToArray();
            stream.Write(byteInfo, 0, byteInfo.Length);
            stream.Position = 0;

            return stream;
        }
    }
}