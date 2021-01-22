using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentPurchasingExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentPurchasingExpedition;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.GarmentPurchasingExpedition
{
    public class GarmentPurchasingExpeditionDataUtil
    {
        private readonly GarmentPurchasingExpeditionService _service;
        public GarmentPurchasingExpeditionDataUtil(GarmentPurchasingExpeditionService service)
        {
            _service = service;
        }


        public SendToVerificationAccountingForm GetNewData_SendToVerificationAccountingForm()
        {
            return new SendToVerificationAccountingForm
            {
                Items = new List<FormItemDto>()
               {
                   new FormItemDto()
                   {
                       InternalNote=new InternalNoteDto()
                       {
                           AmountDPP=1,
                          CorrectionAmount=1,
                          CurrencyCode="CurrencyCode",
                          CurrencyId=1,
                          Date=DateTimeOffset.Now,
                          DocumentNo="DocumentNo",
                          DueDate=DateTimeOffset.Now.AddDays(1),
                          IncomeTax=1,
                          InvoicesNo="InvoicesNo",
                          PaymentDueDays=1,
                          PaymentMethod="",
                          PaymentType="",
                          SupplierId=1,
                          SupplierName="SupplierName",
                          TotalPaid=1,
                          VAT=1,
                       Id=1
                       }
                   }
               }
            };
        }

       

        public async Task<IndexDto> GetTestData()
        {
            var form = GetNewData_SendToVerificationAccountingForm();
            await _service.SendToAccounting(form);
            return _service.GetById(1);
        }
    }
}
