using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.BudgetCashflow;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.BudgetCashflow
{
    public class BudgetCashflowDataUtil
    {
        private readonly BudgetCashflowService _service;
        public BudgetCashflowDataUtil(BudgetCashflowService service)
        {
            _service = service;
        }

        public CashflowUnitFormDto GetNewData_CashflowUnitFormDto()
        {
            return new CashflowUnitFormDto()
            {
                CashflowSubCategoryId = 1,
                Date = DateTimeOffset.Now,
                DivisionId = 1,
                Items = new List<CashflowUnitItemFormDto>()
                {
                    new CashflowUnitItemFormDto()
                    {
                        CurrencyId=1,
                        CurrencyNominal=1,
                        IsIDR=true,
                        Nominal=1,
                        Total=1
                    }
                }
            };
        }

        public CashBalanceFormDto GetNewData_CashBalanceFormDto()
        {
            return new CashBalanceFormDto()
            {
                Date = DateTimeOffset.Now,
                DivisionId = 1,
                Items = new List<CashflowUnitItemFormDto>()
                {
                    new CashflowUnitItemFormDto()
                    {
                        CurrencyId=1,
                        CurrencyNominal=1,
                        IsIDR=false,
                        Nominal=1,
                        Total=1
                    }
                }
            };
        }

        public CashflowSubCategoryFormDto GetNewData_CashflowSubCategoryFormDto()
        {
            return new CashflowSubCategoryFormDto()
            {
                CashflowCategoryId = 1,
                IsImport = false,
                IsReadOnly = false,
                LayoutOrder = 1,
                Name = "",
                PurchasingCategoryIds = new List<int>()
                {
                    1
                },
                ReportType = ReportType.DebtAndDispositionSummary,

            };
        }


        public CashflowCategoryFormDto GetNewData_CashflowCategoryFormDto()
        {
            return new CashflowCategoryFormDto()
            {
                CashflowTypeId = 1,
                IsLabelOnly = false,
                LayoutOrder = 1,
                Name = "Name",
                Type = CashType.In

            };
        }


        public CashflowTypeFormDto GetNewData_CashflowTypeFormDto()
        {
            return new CashflowTypeFormDto()
            {
                LayoutOrder = 1,
                Name = "Name"


            };
        }


    }



}



