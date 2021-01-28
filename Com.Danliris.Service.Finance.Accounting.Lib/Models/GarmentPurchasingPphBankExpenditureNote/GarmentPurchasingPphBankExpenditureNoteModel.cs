using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentPurchasingPphBankExpenditureNote
{
    public class GarmentPurchasingPphBankExpenditureNoteModel : StandardEntity
    {
        /// <summary>
        /// construct all property class
        /// </summary>
        /// <param name="invoiceOutNumber"></param>
        /// <param name="dueDateStart"></param>
        /// <param name="dueDateEnd"></param>
        /// <param name="incomeTaxId"></param>
        /// <param name="incomeTaxName"></param>
        /// <param name="incomeTaxRate"></param>
        /// <param name="accountBankCOA"></param>
        /// <param name="accountBankName"></param>
        /// <param name="accountBankNumber"></param>
        /// <param name="bankAddress"></param>
        /// <param name="bankCode"></param>
        /// <param name="bankName"></param>
        /// <param name="bankCode1"></param>
        /// <param name="bankCurrencyCode"></param>
        /// <param name="bankCurrencyId"></param>
        /// <param name="bankSwiftCode"></param>
        public GarmentPurchasingPphBankExpenditureNoteModel(string invoiceOutNumber, DateTimeOffset dueDateStart, DateTimeOffset dueDateEnd, int incomeTaxId, string incomeTaxName, string incomeTaxRate, string accountBankCOA, string accountBankName, string accountBankNumber, string bankAddress, string bankCode, string bankName, string bankCode1, string bankCurrencyCode, int bankCurrencyId, string bankSwiftCode)
        {
            InvoiceOutNumber = invoiceOutNumber;
            DueDateStart = dueDateStart;
            DueDateEnd = dueDateEnd;
            IncomeTaxId = incomeTaxId;
            IncomeTaxName = incomeTaxName;
            IncomeTaxRate = incomeTaxRate;
            AccountBankCOA = accountBankCOA;
            AccountBankName = accountBankName;
            AccountBankNumber = accountBankNumber;
            BankAddress = bankAddress;
            BankCode = bankCode;
            BankName = bankName;
            BankCode1 = bankCode1;
            BankCurrencyCode = bankCurrencyCode;
            BankCurrencyId = bankCurrencyId;
            BankSwiftCode = bankSwiftCode;
        }

        public GarmentPurchasingPphBankExpenditureNoteModel()
        {

        }

        /// <summary>
        /// Nomor Bukti Keluar
        /// </summary>
        public string InvoiceOutNumber { get; set; }
        public DateTimeOffset DueDateStart { get; set; }
        public DateTimeOffset DueDateEnd { get; set; }
        public int IncomeTaxId { get; set; }
        public string IncomeTaxName { get; set; }
        public string IncomeTaxRate { get; set; }
        public string AccountBankCOA { get; set; }
        public string AccountBankName { get; set; }
        public string AccountBankNumber { get; set; }
        public string BankAddress { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string BankCode1 { get; set; }
        public string BankCurrencyCode { get; set; }
        public int BankCurrencyId { get; set; }
        public string BankSwiftCode { get; set; }
    }
}
