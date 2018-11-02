using System.Collections.Generic;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CreditorAccount
{
    public class CreditorAccountUnitPaymentOrderPostedViewModel
    {
        public string InvoiceNo { get; set; }

        public List<CreditorAccountPostedViewModel> CreditorAccounts { get; set; }
    }
}
