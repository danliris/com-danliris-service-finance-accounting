using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentInvoicePurchasingDisposition
{
    public class GarmentInvoicePurchasingDispositionItemModel:StandardEntity
    {
        public int DispositionId { get; private set; }
        public string DispositionNo { get; private set; }
        public DateTimeOffset DispositionDate { get; private set; }
        public DateTimeOffset DipositionDueDate { get; private set; }
        public string ProformaNo { get; private set; }
        public string SupplierName { get; private set; }
        public string Category { get; private set; }
        public double VATAmount { get; private set; }
        public double TotalAmount { get; private set; }
        public double TotalPaid { get; set; }
        /// <summary>
        /// Sum of Total Paid With same Disposistion ID
        /// </summary>
        public double TotalPaidBefore { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public double CurrencyRate { get; set; }
        public int GarmentInvoicePurchasingDisposistionId { get; set; }
        [ForeignKey("GarmentInvoicePurchasingDisposistionId")]
        public virtual GarmentInvoicePurchasingDispositionModel GarmentInvoicePurchasingDisposition { get; set; }
    }
}
