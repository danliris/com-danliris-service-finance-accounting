using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentFinance.BankCashReceipt
{
    public class BankCashReceiptModel : StandardEntity
    {
        [MaxLength(256)]
        public string ReceiptNo { get; set; }
        public DateTimeOffset ReceiptDate { get; set; }

        public int BankAccountId { get; set; }
        [MaxLength(64)]
        public string BankAccountNumber { get; set; }
        [MaxLength(256)]
        public string BankAccountName { get; set; }
        [MaxLength(256)]
        public string BankName { get; private set; }
        [MaxLength(32)]
        public string BankAccountingCode { get; set; }
        [MaxLength(32)]
        public string BankCurrencyCode { get; set; }
        public int BankCurrencyId { get; set; }
        public double BankCurrencyRate { get; set; }

        public int DebitCoaId { get; set; }
        [MaxLength(32)]
        public string DebitCoaCode { get; set; }
        [MaxLength(256)]
        public string DebitCoaName { get; set; }

        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public decimal CurrencyRate { get; set; }

        [MaxLength(32)]
        public string NumberingCode { get; set; }
        [MaxLength(256)]
        public string IncomeType { get; set; }
        [MaxLength(1024)]
        public string Remarks { get; set; }

        public decimal Amount { get; set; }
        public virtual ICollection<BankCashReceiptItemModel> Items { get; set; }
    }
}
