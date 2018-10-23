using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.Master;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MasterCOA;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.MasterCOA;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.Masters.COADataUtils
{
    public class COADataUtil
    {
        private readonly COAService Service;

        public COADataUtil(COAService service)
        {
            Service = service;
        }

        public COAModel GetNewData()
        {
            return new COAModel()
            {
                CashAccount = "CashAccount",
                Name = "Name",
                Code = "Code",
                Nature = "Nature",
                Path = "/",
                ReportType = "ReportType"
                
            };
        }

        public COAViewModel GetNewViewModel()
        {
            return new COAViewModel()
            {
                CashAccount = "CashAccount",
                Name = "Name",
                Code = "Code",
                Nature = "Nature",
                Path = "/",
                ReportType = "ReportType"

            };
        }

        public async Task<COAModel> GetTestData()
        {
            COAModel model = GetNewData();
            await Service.CreateAsync(model);
            return await Service.ReadByIdAsync(model.Id);
        }
    }
}
