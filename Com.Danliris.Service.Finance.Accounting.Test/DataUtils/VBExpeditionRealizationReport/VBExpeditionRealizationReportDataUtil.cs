using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.VBExpeditionRealizationReport;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.VBExpeditionRealizationReport
{
    public class VBExpeditionRealizationReportDataUtil
    {
        private VBExpeditionRealizationReportService service;

        public VBExpeditionRealizationReportDataUtil(VBExpeditionRealizationReportService service)
        {
            this.service = service;
        }

        public VbRequestModel GetNewData()
        {
            VbRequestModel TestData = new VbRequestModel()
            {
                Realization_Status = true,
                VBNo = "VBNo",
                Date = DateTimeOffset.UtcNow,
                DateEstimate = DateTimeOffset.UtcNow,
                UnitId = 1,
                UnitName = "UnitName",
                CreatedBy = "CreatedBy",
                Usage = "Usage",
                Amount = 1000,
            };

            return TestData;
        }

        public async Task<VbRequestModel> GetTestData_ById()
        {
            VbRequestModel model = GetNewData();
            await service.CreateAsync(model);
            return await service.ReadByIdAsync(model.Id);
        }
    }
}
