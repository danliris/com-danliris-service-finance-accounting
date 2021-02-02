using Com.Danliris.Service.Finance.Accounting.Lib.Models.DPPVATBankExpenditureNote;
using System;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.DPPVATBankExpenditureNote
{
    public class InvoiceDto
    {
        public InvoiceDto(DPPVATBankExpenditureNoteDetailModel detail)
        {
            Id = detail.InvoiceId;
            DocumentNo = detail.InvoiceNo;
            Date = detail.InvoiceDate;
            ProductNames = detail.ProductNames;
            Category = new CategoryDto(detail.CategoryId, detail.CategoryName);
            Amount = detail.Amount;
        }

        public string DocumentNo { get; set; }
        public DateTimeOffset Date { get; set; }
        public string ProductNames { get; set; }
        public CategoryDto Category { get; set; }
        public double Amount { get; set; }
        public int Id { get; set; }
    }
}