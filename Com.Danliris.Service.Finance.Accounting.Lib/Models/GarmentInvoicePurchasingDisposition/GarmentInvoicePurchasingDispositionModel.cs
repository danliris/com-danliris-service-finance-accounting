using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentInvoicePurchasingDisposition
{
    public class GarmentInvoicePurchasingDispositionModel: StandardEntity
    {
        public string InvoiceNo { get; private set; }
        public DateTimeOffset InvoiceDate { get; private set; }
        public int BankId { get; private set; }
        public string BankName { get; private set; }
        public string BankCode { get; private set; }
        public int BankAccount { get; private set; }
        public int CurrencyId { get; private set; }
        public string CurrencyCode { get; private set; }
        public double CurrencyRate { get; private set; }
        public DateTimeOffset CurrencyDate { get; private set; }
        public int SupplierId { get; private set; }
        public string SupplierName { get; private set; }
        public string SupplierCode { get; private set; }
        public bool IsImportSupplier { get; private set; }
        public string ChequeNo { get; private set; }
        public string PaymentType { get; private set; }
        public virtual List<GarmentInvoicePurchasingDispositionItemModel> Items { get; set; }

    }
}
