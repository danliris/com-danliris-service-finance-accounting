using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseClass;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.DailyBankTransaction
{
    public class DailyBankTransactionViewModel : BaseViewModel, IValidatableObject
    {
        public AccountBankViewModel Bank { get; set; }
        public string Code { get; set; }
        public NewBuyerViewModel Buyer { get; set; }
        public DateTimeOffset? Date { get; set; }
        public decimal? Nominal { get; set; }
        public string ReferenceNo { get; set; }
        public string ReferenceType { get; set; }
        public string Remark { get; set; }
        public string SourceType { get; set; }
        public string Status { get; set; }
        public NewSupplierViewModel Supplier { get; set; }
        public string Receiver { get; set; }
        public decimal? AfterNominal { get; set; }
        public AccountBankViewModel OutputBank { get; set; }
        public decimal? BeforeNominal { get; set; }
        public decimal? NominalOut { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Bank == null || Bank.Id <= 0)
            {
                yield return new ValidationResult("Bank harus diisi", new List<string> { "Bank" });
            }

            if (Date == null)
            {
                yield return new ValidationResult("Tanggal harus diisi", new List<string> { "Date" });
            }
            else if (Date > DateTimeOffset.Now)
            {
                yield return new ValidationResult("Tanggal harus lebih kecil dari atau sama dengan tanggal sekarang", new List<string> { "Date" });
            }

            //if (string.IsNullOrWhiteSpace(ReferenceNo))
            //{
            //    yield return new ValidationResult("No. Referensi harus diisi", new List<string> { "ReferenceNo" });
            //}

            if (string.IsNullOrWhiteSpace(ReferenceType))
            {
                yield return new ValidationResult("Jenis Referensi harus diisi", new List<string> { "ReferenceType" });
            }

            if (string.IsNullOrWhiteSpace(Status) || (!Status.ToUpper().Equals("IN") && !Status.ToUpper().Equals("OUT")))
            {
                yield return new ValidationResult("Status harus diisi", new List<string> { "Status" });
            }
            else
            {
                switch (Status.ToUpper())
                {
                    case "IN":
                        if (!string.IsNullOrWhiteSpace(SourceType) && (SourceType.ToUpper().Equals("OPERASIONAL") || SourceType.ToUpper().Equals("INVESTASI") || SourceType.ToUpper().Equals("PENDANAAN")))
                        {
                            if (Buyer == null || Buyer.Id <= 0)
                                if (SourceType.ToUpper().Equals("OPERASIONAL"))
                                {
                                    yield return new ValidationResult("Buyer harus diisi", new List<string> { "Buyer" });
                                }
                                else
                                {
                                    yield return new ValidationResult("Dari harus diisi", new List<string> { "Buyer" });
                                }
                        }
                        break;
                    case "OUT":
                        if (!string.IsNullOrWhiteSpace(SourceType) && (SourceType.ToUpper().Equals("OPERASIONAL") || SourceType.ToUpper().Equals("INVESTASI") || SourceType.ToUpper().Equals("PENDANAAN")))
                        {
                            if (Supplier == null || Supplier._id <= 0)
                                if (SourceType.ToUpper().Equals("OPERASIONAL"))
                                {
                                    yield return new ValidationResult("Supplier harus diisi", new List<string> { "Supplier" });
                                }
                                else if (SourceType.ToUpper().Equals("INVESTASI"))
                                {
                                    yield return new ValidationResult("Ke harus diisi", new List<string> { "Supplier" });
                                }
                        }
                        if (OutputBank == null || OutputBank.Id <= 0)
                        {
                            yield return new ValidationResult("Bank tujuan harus diisi", new List<string> { "OutputBank" });
                        }
                        else
                        {
                            if(OutputBank.Id == Bank.Id)
                            {
                                yield return new ValidationResult("Bank tujuan tidak boleh sama dengan Bank", new List<string> { "OutputBank", "Bank" });
                            }
                        }
                        break;
                }
            }

            if (string.IsNullOrWhiteSpace(SourceType) || (!SourceType.ToUpper().Equals("OPERASIONAL") && !SourceType.ToUpper().Equals("INVESTASI") && !SourceType.ToUpper().Equals("PENDANAAN")))
            {
                yield return new ValidationResult("Jenis Sumber harus diisi", new List<string> { "SourceType" });
            }

            if (Nominal <= 0)
            {
                yield return new ValidationResult("Nominal harus lebih besar dari 0", new List<string> { "Nominal" });
            }
        }
    }
}
