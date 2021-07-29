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
             
            public string buyerAgentCode { get; set; }
            public string buyerAgentName { get; set; }
            public double amount { get; set; }
        }
      
    }
}
