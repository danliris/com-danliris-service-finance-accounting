namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.BudgetCashflow
{
    public class UnitItemDto
    {
        public UnitItemDto(double total, double nominal, double currencyNominal)
        {
            Total = total;
            Nominal = nominal;
            CurrencyNominal = currencyNominal;
        }

        public double Total { get; private set; }
        public double Nominal { get; private set; }
        public double CurrencyNominal { get; private set; }
    }
}