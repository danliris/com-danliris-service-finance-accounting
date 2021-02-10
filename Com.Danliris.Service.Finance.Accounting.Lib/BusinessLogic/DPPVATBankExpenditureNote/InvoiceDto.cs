using Com.Danliris.Service.Finance.Accounting.Lib.Models.DPPVATBankExpenditureNote;
using System;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.DPPVATBankExpenditureNote
{
    public class InvoiceDto
    {
        public InvoiceDto()
        {
                
        }

        public InvoiceDto(DPPVATBankExpenditureNoteDetailModel detail)
        {
            Id = detail.InvoiceId;
            DocumentNo = detail.InvoiceNo;
            Date = detail.InvoiceDate;
            ProductNames = detail.ProductNames;
            Category = new CategoryDto(detail.CategoryId, detail.CategoryName);
            PaymentMethod = detail.PaymentMethod;
            Amount = detail.Amount;
            DeliveryOrdersNo = detail.DeliveryOrdersNo;
            PaymentBills = detail.PaymentBills;
            BillsNo = detail.BillsNo;
        }

        public string DocumentNo { get; set; }
        public DateTimeOffset Date { get; set; }
        public string ProductNames { get; set; }
        public CategoryDto Category { get; set; }
        public string PaymentMethod { get; set; }
        public double Amount { get; set; }
        public string DeliveryOrdersNo { get; private set; }
        public string PaymentBills { get; private set; }
        public string BillsNo { get; private set; }
        public int Id { get; set; }
    }
}