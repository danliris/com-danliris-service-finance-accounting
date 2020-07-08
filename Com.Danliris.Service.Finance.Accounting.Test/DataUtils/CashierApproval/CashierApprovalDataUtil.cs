using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.CashierApproval;
using Com.Danliris.Service.Finance.Accounting.Lib.Helpers;
using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CashierApproval;
using Com.Danliris.Service.Finance.Accounting.WebApi.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.CashierApproval
{
    public class CashierApprovalDataUtil
    {
        private readonly CashierApprovalService Service;

        public CashierApprovalDataUtil(CashierApprovalService service)
        {
            Service = service;
        }

        public VbRequestModel GetNewData()
        {
            VbRequestModel TestData = new VbRequestModel()
            {
                VBNo = "VBNo",
                Date = DateTimeOffset.UtcNow,
                DateEstimate = DateTimeOffset.UtcNow,
                UnitId = 1,
                UnitCode = "UnitCode",
                UnitName = "UnitName",
                CurrencyId = 1,
                CurrencyCode = "CurrencyCode",
                CurrencyRate = 123,
                CurrencySymbol = "CurrencySymbol",
                Amount = 123,
                Usage = "Usage",
                UnitLoad = "UnitLoad",
                //Status_Post = true,
                Apporve_Status = true,
                Complete_Status = true,
                VBRequestCategory = "VBRequestCategory",
                CreatedBy = "CreatedBy"
            };

            return TestData;
        }

        public CashierApprovalViewModel GetDataToValidate()
        {
            CashierApprovalViewModel TestData = new CashierApprovalViewModel()
            {
                VBRequestCategory = "VBRequestCategory",
                CashierApproval = new List<CashierApprovalItemViewModel>{
                        new CashierApprovalItemViewModel{
                            Id = 10,
                            VBNo = "VBNo"
                        }
                    }
            };

            return TestData;
        }

        public async Task<VbRequestModel> GetTestDataById()
        {
            VbRequestModel model = GetNewData();
            await Service.CreateAsync(model);
            return await Service.ReadByIdAsync(model.Id);
        }
    }
}
