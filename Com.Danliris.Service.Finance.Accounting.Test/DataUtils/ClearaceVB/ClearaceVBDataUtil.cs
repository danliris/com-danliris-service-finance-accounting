using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.ClearaceVB;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.ClearaceVB
{
    public class ClearaceVBDataUtil
    {
        private ClearaceVBService service;

        public ClearaceVBDataUtil(ClearaceVBService service)
        {
            this.service = service;
        }
        public VbRequestModel GetNewData()
        {
            VbRequestModel TestData = new VbRequestModel()
            {
                Id = 1,
                VBNo = "VBNo",
                VBRequestCategory = "VBRequestCategory",
                Date = DateTimeOffset.Now,
                UnitId = 1,
                UnitName = "UnitName",
                CreatedBy = "CreatedBy",
                CompleteDate = DateTimeOffset.Now,
                Complete_Status = true,
            };

            return TestData;
        }

        public virtual async Task<VbRequestModel> GetTestData()
        {
            VbRequestModel model = GetNewData();
            await service.CreateAsync(model);
            return model;
        }
    }
}
