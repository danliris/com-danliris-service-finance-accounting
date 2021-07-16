using System;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.PurchasingMemoDetailTextile
{
    public class UnitReceiptNoteDto
    {
        public int Id { get; set; }
        public string UnitReceiptNoteNo { get; set; }
        public DateTimeOffset UnitReceiptNoteDate { get; set; }
    }
}