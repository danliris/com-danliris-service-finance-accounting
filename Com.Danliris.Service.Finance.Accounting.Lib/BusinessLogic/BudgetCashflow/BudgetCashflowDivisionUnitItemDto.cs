using Com.Danliris.Service.Finance.Accounting.Lib.Models.BudgetCashflow;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.BudgetCashflow
{
    public class BudgetCashflowDivisionUnitItemDto
    {
        public BudgetCashflowDivisionUnitItemDto(BudgetCashflowUnitModel cashflowUnit, UnitDto divisionUnit)
        {
            CashflowUnit = cashflowUnit;
            Unit = divisionUnit;
        }

        public BudgetCashflowDivisionUnitItemDto(DivisionDto division, double divisionCurrencyNominal, double divisionNominal, double divisionActual)
        {
            Division = division;
            DivisionCurrencyNominal = divisionCurrencyNominal;
            DivisionNominal = divisionNominal;
            DivisionActual = divisionActual;
        }

        public BudgetCashflowUnitModel CashflowUnit { get; private set; }
        public UnitDto Unit { get; private set; }
        public DivisionDto Division { get; private set; }
        public double DivisionCurrencyNominal { get; private set; }
        public double DivisionNominal { get; private set; }
        public double DivisionActual { get; private set; }
    }
}