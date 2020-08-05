using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRealizationDocumentExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRealizationDocumentExpedition;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.VBRealizationDocumentExpedition
{
    public class VBRealizationDocumentExpeditionDataUtil
    {
        private readonly VBRealizationDocumentExpeditionService Service;
        private FinanceDbContext dbContext;

        public VBRealizationDocumentExpeditionDataUtil(VBRealizationDocumentExpeditionService service, FinanceDbContext financeDbContext)
        {
            Service = service;
            dbContext = financeDbContext;
        }

       

        public VBRealizationDocumentExpeditionModel GetTestData_VBRealizationDocumentExpedition()
        {
            var vbRealization = GetTestData_RealizationVbs();
            var data = new VBRealizationDocumentExpeditionModel(vbRealization.Id, 0, "vbNo", "vbRealizationNo", DateTimeOffset.Now, "vbRequestName", 0, "meter", 0, "divisionName", 0, 0, "IDR", 0, "");
            dbContext.VBRealizationDocumentExpeditions.Add(data);
            dbContext.SaveChanges();
            return data;
        }

        public RealizationVbModel GetTestData_RealizationVbs()
        {
            var data = new RealizationVbModel()
            {
                DivisionName = "DivisionName",
                CurrencyCode = "IDR",
                CurrencyRate = 1,
                Amount = 1,
                AmountNonPO = 1,
                Amount_VB = 1,
                CloseDate = DateTimeOffset.Now,
                DateEstimate = DateTimeOffset.Now,
                DateVB = DateTimeOffset.Now,
                RealizationVbDetail = new List<RealizationVbDetailModel>()
                {
                    new RealizationVbDetailModel()
                    {
                        Remark ="Remark",
                        AmountNonPO=1,
                        CodeProductSPB ="CodeProductSPB",
                        CurrencyCode ="IDR",
                        CurrencyRate =1,
                        CurrencySymbol ="RP",
                        CurrencyId="1",
                        DateNonPO =DateTimeOffset.Now,
                        DateSPB =DateTimeOffset.Now,
                        DivisionSPB ="DivisionSPB"
                    }
                },
                Position = 1
            };
            dbContext.RealizationVbs.Add(data);
            dbContext.SaveChanges();
            return data;

        }



    }
}
