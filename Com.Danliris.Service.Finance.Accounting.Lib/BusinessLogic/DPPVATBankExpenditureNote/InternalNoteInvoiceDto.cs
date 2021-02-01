using Com.Danliris.Service.Finance.Accounting.Lib.Models.DPPVATBankExpenditureNote;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.DPPVATBankExpenditureNote
{
    public class InternalNoteInvoiceDto
    {
        public InternalNoteInvoiceDto(DPPVATBankExpenditureNoteDetailModel detail)
        {
            Id = detail.Id;
            Invoice = new InvoiceDto(detail);
        }

        public int Id { get; set; }
        public InvoiceDto Invoice { get; set; }
    }
}