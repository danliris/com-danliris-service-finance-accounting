using System;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.PurchasingMemoDetailTextile
{
    public class IndexDto
    {
        public int Id { get; private set; }
        public DateTimeOffset LastModifiedUtc { get; private set; }
        public DateTimeOffset Date { get; private set; }
        public string DivisionName { get; private set; }
        public string CurrencyCode { get; private set; }
        public bool SupplierIsImport { get; private set; }
        public string Remark { get; private set; }
        public string DocumentNo { get; private set; }
    }
}