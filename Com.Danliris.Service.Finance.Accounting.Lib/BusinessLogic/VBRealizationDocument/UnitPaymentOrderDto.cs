using iTextSharp.text;
using System.Collections.Generic;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRealizationDocument
{
    public class UnitPaymentOrderDto
    {
        public int? Id { get; set; }
        public string No { get; set; }
        public List<UnitPaymentOrderItemDto> Items { get; set; }
    }
}