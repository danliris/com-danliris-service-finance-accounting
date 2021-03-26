using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.GarmentInvoicePurchasingDisposition
{
    public class GarmentInvoicePurchasingDispositionItemViewModel
    {
        public int purchasingDispositionExpeditionId { get; set; }
        public DivisionViewModel division { get; set; }
        public string proformaNo { get; set; }
        public double totalPaid { get; set; }
        public string dispositionId { get; set; }
        public DateTimeOffset dispositionDate { get; set; }
        public DateTimeOffset paymentDueDate { get; set; }
        public string dispositionNo { get; set; }
        public double dpp { get; set; }
        public double vatValue { get; set; }
        public double incomeTaxValue { get; set; }
        public CategoryViewModel category { get; set; }
        public double payToSupplier { get; set; }
    }
}
