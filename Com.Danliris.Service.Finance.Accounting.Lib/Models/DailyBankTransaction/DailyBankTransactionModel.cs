using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.DailyBankTransaction
{
    public class DailyBankTransactionModel : StandardEntity, IValidatableObject
    {
        //Bank
        //[MaxLength(50)]
        public int AccountBankId { get; set; }
        [MaxLength(25)]
        public string AccountBankCode { get; set; }
        [MaxLength(100)]
        public string AccountBankName { get; set; }
        [MaxLength(100)]
        public string AccountBankAccountName { get; set; }
        [MaxLength(100)]
        public string AccountBankAccountNumber { get; set; }
        [MaxLength(100)]
        public string AccountBankCurrencyCode { get; set; }
        [MaxLength(50)]
        public int AccountBankCurrencyId { get; set; }
        [MaxLength(100)]
        public string AccountBankCurrencySymbol { get; set; }
        [MaxLength(25)]
        public string Code { get; set; }
        //Buyer
        [MaxLength(25)]
        public string BuyerCode { get; set; }
        //[MaxLength(50)]
        public int BuyerId { get; set; }
        [MaxLength(150)]
        public string BuyerName { get; set; }
        public DateTimeOffset Date { get; set; }
        public double Nominal { get; set; }
        [MaxLength(50)]
        public string ReferenceNo { get; set; }
        [MaxLength(50)]
        public string ReferenceType { get; set; }
        [MaxLength(500)]
        public string Remark { get; set; }
        [MaxLength(50)]
        public string SourceType { get; set; }
        [MaxLength(50)]
        public string Status { get; set; }
        //Supplier
        //[MaxLength(50)]
        public int SupplierId { get; set; }
        [MaxLength(100)]
        public string SupplierCode { get; set; }
        [MaxLength(250)]
        public string SupplierName { get; set; }
        public double AfterNominal { get; set; }
        public double BeforeNominal { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
