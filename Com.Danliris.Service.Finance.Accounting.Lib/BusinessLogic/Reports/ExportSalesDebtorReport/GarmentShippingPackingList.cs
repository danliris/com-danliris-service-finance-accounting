using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Reports.ExportSalesDebtorReport
{
    public class GarmentShippingPackingList
    {
        public IList<ShippingPackingDto> data { get; set; }
        public GarmentShippingPackingList()
        {
            data = new List<ShippingPackingDto>();
        }


        public class ShippingPackingDto
        {
            public int shippingInvoiceId { get; set; }
            public int invoiceNo { get; set; }
            public DateTimeOffset truckingDate { get; set; }
            public string buyerId { get; set; }
            public string buyerName { get; set; }
            public double amount { get; set; }
        }
      
    }
}
