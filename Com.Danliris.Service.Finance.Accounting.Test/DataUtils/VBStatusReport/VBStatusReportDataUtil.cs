using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.VBStatusReport;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.VbNonPORequest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.VBStatusReport
{
    public class VBStatusReportDataUtil
    {
        private VBStatusReportService service;

        public VBStatusReportDataUtil(VBStatusReportService service)
        {
            this.service = service;
        }

        public VbRequestModel GetNewData_Realisasi()
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
        public VbRequestModel GetNewData_Outstanding()
        {
            VbRequestModel TestData = new VbRequestModel()
            {
                Realization_Status = false,
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

        public async Task<VbRequestModel> GetTestData_Realisasi_ById()
        {
            VbRequestModel model = GetNewData_Realisasi();
            await service.CreateAsync(model);
            return await service.ReadByIdAsync(model.Id);
        }
        public async Task<VbRequestModel> GetTestData_Outstanding_ById()
        {
            VbRequestModel model = GetNewData_Outstanding();
            await service.CreateAsync(model);
            return await service.ReadByIdAsync(model.Id);
        }
    }
}
