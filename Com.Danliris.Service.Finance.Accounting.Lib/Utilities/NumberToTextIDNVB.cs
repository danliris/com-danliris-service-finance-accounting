using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Utilities
{
    public static class NumberToTextIDNVB
    {
        static string[] satuan = { "Nol", "Satu", "Dua", "Tiga", "Empat", "Lima", "Enam", "Tujuh", "Delapan", "Sembilan" };
        static string[] belasan = { "Sepuluh", "Sebelas", "Dua Belas", "Tiga Belas", "Empat Belas", "Lima Belas", "Enam Belas", "Tujuh Belas", "Delapan Belas", "Sembilan Belas" };
        static string[] puluhan = { "", "", "Dua Puluh", "Tiga Puluh", "Empat Puluh", "Lima Puluh", "Enam Puluh", "Tujuh Puluh", "Delapan Puluh", "Sembilan Puluh" };
        static string[] ribuan = { "", "ribu", "juta", "milyar", "triliyun", "kuadrilyun", "kuintiliun", "sekstiliun", "septiliun", "oktiliun", "noniliun", "desiliun" };

        public static string terbilang(double d, string symbol, string description)
        {
            var strHasil = "";
            var isNegative = (d) < 0;
            d = Convert.ToDouble((Math.Round(d * 100) / 100).ToString("N2"));
            if (isNegative)
            {
                d = d * -1;
            }

            if (d.ToString().IndexOf(".") != -1)
            {
                var a = Convert.ToDouble((d.ToString().Substring(d.ToString().IndexOf(".") + 1)));
                if (a != 0)
                {
                    if (symbol == "Rp" || symbol.ToUpper() == "IDR")
                    {
                        strHasil = "rupiah " + terbilangKoma(d);
                    }
                    else
                    {
                        strHasil = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(description.ToLower())+ " " + terbilangKoma(d);
                    }
                       
                }
                d = Convert.ToDouble(d.ToString().Substring(0, d.ToString().IndexOf(".")));
            }
            else
            {
                if (symbol == "Rp" || symbol.ToUpper() == "IDR")
                {
                    strHasil = strHasil + "rupiah ";
                }
                else
                {
                    strHasil = strHasil +  CultureInfo.CurrentCulture.TextInfo.ToTitleCase(description.ToLower());
                }
            }

            var nDigit = 0;
            var nPosisi = 0;

            var strTemp = Math.Truncate(d).ToString();
            for (var i = strTemp.Length; i > 0; i--)
            {
                var tmpBuff = "";
                nDigit = Convert.ToInt32(strTemp.Substring(i - 1, 1), 10);
                nPosisi = (strTemp.Length - i) + 1;
                switch (nPosisi % 3)
                {
                    case 1:
                        var bAllZeros = false;
                        if (i == 1)
                            tmpBuff = satuan[nDigit] + " ";
                        else if (strTemp.Substring(i - 2, 1) == "1")
                            tmpBuff = belasan[nDigit] + " ";
                        else if (nDigit > 0)
                            tmpBuff = satuan[nDigit] + " ";
                        else
                        {
                            bAllZeros = true;
                            if (i > 1)
                                if (strTemp.Substring(i - 2, 1) != "0")
                                    bAllZeros = false;
                            if (i > 2)
                                if (strTemp.Substring(i - 3, 1) != "0")
                                    bAllZeros = false;
                            tmpBuff = "";
                        }

                        if ((!bAllZeros) && (nPosisi > 1))
                            if ((strTemp.Length == 4) && (strTemp.Substring(0, 1) == "1"))
                                tmpBuff = "Se" + ribuan[Convert.ToInt32(Math.Round(Convert.ToDecimal(nPosisi) / 3))] + " ";
                            else
                                tmpBuff = tmpBuff + ribuan[Convert.ToInt32(Math.Round(Convert.ToDecimal(nPosisi) / 3))] + " ";
                        strHasil = tmpBuff + strHasil;
                        break;
                    case 2:
                        if (nDigit > 0)
                            strHasil = (puluhan[nDigit] + " " + strHasil).Trim();
                        break;
                    case 0:
                        if (nDigit > 0)
                            if (nDigit == 1)
                                strHasil = "Seratus " + strHasil;
                            else
                                strHasil = satuan[nDigit] + " Ratus " + strHasil;
                        break;
                }

            }

            if (strTemp.Length > 0)
            {
                if (isNegative)
                {
                    strHasil = $"minus { strHasil}";
                }
            }

            strHasil = strHasil.Trim().ToLower();
            if (strHasil.Length > 0)
            {
                strHasil = strHasil.Substring(0, 1).ToUpper() +
                    strHasil.Substring(1, strHasil.Length - 1);
            }

            return strHasil;
        }

        public static string terbilangKoma(double frac)
        {
            var stringFrac = frac.ToString();
            var a = "";

            if (stringFrac.IndexOf(".") > -1)
            {
                a = frac.ToString().Substring(frac.ToString().IndexOf(".") + 1);
            }
            else if (stringFrac.IndexOf(",") > -1)
            {
                a = frac.ToString().Substring(frac.ToString().IndexOf(",") + 1);
            }

            string fixNumber = "";

            if (stringFrac.IndexOf(".") > -1)
            {
                fixNumber = (frac.ToString("N2")).ToString().Substring((frac.ToString("N2")).ToString().IndexOf(".") + 1);
            }
            else if (stringFrac.IndexOf(",") > -1)
            {
                fixNumber = (frac.ToString("N2")).ToString().Substring((frac.ToString("N2")).ToString().IndexOf(",") + 1);
            }

            var strHasil = "";


            var nDigit = 0;
            var nPosisi = 0;

            var strTemp = fixNumber;
            for (var i = strTemp.Length; i > 0; i--)
            {
                var tmpBuff = "";
                nDigit = Convert.ToInt32(strTemp.Substring(i - 1, 1), 10);
                nPosisi = (strTemp.Length - i) + 1;
                switch (nPosisi % 3)
                {
                    case 1:
                        var bAllZeros = false;
                        if (i == 1)
                            tmpBuff = satuan[nDigit] + " ";
                        else if (strTemp.Substring(i - 2, 1) == "1")
                            tmpBuff = belasan[nDigit] + " ";
                        else if (nDigit > 0)
                            tmpBuff = satuan[nDigit] + " ";
                        else
                        {
                            bAllZeros = true;
                            if (i > 1)
                                if (strTemp.Substring(i - 2, 1) != "0")
                                    bAllZeros = false;
                            if (i > 2)
                                if (strTemp.Substring(i - 3, 1) != "0")
                                    bAllZeros = false;
                            tmpBuff = "";
                        }

                        if ((!bAllZeros) && (nPosisi > 1))
                            if ((strTemp.Length == 4) && (strTemp.Substring(0, 1) == "1"))
                                tmpBuff = "Se" + ribuan[Convert.ToInt32(Math.Round(Convert.ToDecimal(nPosisi) / 3))] + " ";
                            else
                                tmpBuff = tmpBuff + ribuan[Convert.ToInt32(Math.Round(Convert.ToDecimal(nPosisi) / 3))] + " ";
                        strHasil = tmpBuff + strHasil;
                        break;
                    case 2:
                        if (nDigit > 0)
                            strHasil = (puluhan[nDigit] + " " + strHasil).Trim();
                        break;
                    case 0:
                        if (nDigit > 0)
                            if (nDigit == 1)
                                strHasil = "Seratus " + strHasil;
                            else
                                strHasil = satuan[nDigit] + " Ratus " + strHasil;
                        break;
                }

            }

            return  strHasil + " sen ";
        }
    }
}
