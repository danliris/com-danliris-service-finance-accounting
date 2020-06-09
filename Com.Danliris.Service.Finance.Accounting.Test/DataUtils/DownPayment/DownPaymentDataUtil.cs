using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.DownPayment;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.DownPayment
{
    public class DownPaymentDataUtil
    {
        private readonly DownPaymentService Service;

        public DownPaymentDataUtil(DownPaymentService service)
        {
            Service = service;
        }

        public DownPaymentModel GetNewData()
        {
            DownPaymentModel TestData = new DownPaymentModel()
            {

            };

            return TestData;
        }
    }
}
