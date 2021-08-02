using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentFinance.Memorial;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentFinance.MemorialDetail;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentFinance.MemorialDetail;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.GarmentFinance.Memorial;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.GarmentFinance.MemorialDetail
{
    public class GarmentFinanceMemorialDetailDataUtil
    {
        private readonly GarmentFinanceMemorialDetailService Service;
        private readonly GarmentFinanceMemorialDataUtil MemorialDataUtil;

        public GarmentFinanceMemorialDetailDataUtil(GarmentFinanceMemorialDetailService service, GarmentFinanceMemorialDataUtil memorialDataUtil)
        {
            Service = service;
            MemorialDataUtil = memorialDataUtil;
        }

        public GarmentFinanceMemorialDetailModel GetNewData()
        {
            var memorial = Task.Run(() => MemorialDataUtil.GetTestData()).Result;
            return new GarmentFinanceMemorialDetailModel
            {
                MemorialNo = memorial.MemorialNo,
                MemorialId = memorial.Id,
                MemorialDate = DateTimeOffset.Now,
                Items = new List<GarmentFinanceMemorialDetailItemModel>
                {
                    new GarmentFinanceMemorialDetailItemModel()
                    {
                        BuyerId = 1,
                        BuyerCode = "code",
                        BuyerName = "name",
                        Amount = 1,
                        CurrencyId = 1,
                        CurrencyCode="code",
                        CurrencyRate=1,
                        InvoiceId=1,
                        InvoiceNo="no",

                    },
                }
            };
        }

        public async Task<GarmentFinanceMemorialDetailModel> GetTestData()
        {
            GarmentFinanceMemorialDetailModel model = GetNewData();
            await Service.CreateAsync(model);
            return await Service.ReadByIdAsync(model.Id);
        }
    }
}
