using System;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.PurchasingMemoDetailTextile
{
    public class AutoCompleteDto
    {
        public AutoCompleteDto()
        {

        }

        public AutoCompleteDto(int id, string documentNo, DateTimeOffset date, string currencyCode, int currencyId, double currencyRate)
        {
            Id = id;
            DocumentNo = documentNo;
            Date = date;
            Currency = new CurrencyDto(currencyId, currencyCode, currencyRate);
        }

        public int Id { get; set; }
        public string DocumentNo { get; set; }
        public DateTimeOffset Date { get; set; }
        public CurrencyDto Currency { get; set; }
    }
}