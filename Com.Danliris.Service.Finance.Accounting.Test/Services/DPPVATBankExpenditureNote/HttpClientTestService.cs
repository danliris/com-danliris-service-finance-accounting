using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.DPPVATBankExpenditureNote
{
    public class HttpClientTestService : IHttpClientService
    {
        public  Task<HttpResponseMessage> GetAsync(string url)
        {
            if (url.Contains("bank-expenditure-notes/bank-document-no") && url.Contains("type"))
            {
               
                var defaultresponse = new BaseResponse<string>()
                {
                   apiVersion="v1",
                  data = "1",
                  message="",
                  statusCode=200
                };
                var result = new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(defaultresponse))
                };

                return Task.FromResult(result);
            }
            else
            {
                var datalist = new[] { new { DocumentNo = "DocumentNo", BankName = "BankName", BGCheckNumber = "CheckNumber" } };
                var json = new
                {
                    apiVersion = "1.0.0",
                    data = datalist,
                    message = "Ok",
                    statusCode = "200"
                };
                var jsonString = JsonConvert.SerializeObject(json);
                var jsonContent = new StringContent(jsonString);
                return Task.Run(() => new HttpResponseMessage() { Content = jsonContent });
            }
           
        }

        public Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> PutAsync(string url, HttpContent content)
        {
            return Task.Run(() => new HttpResponseMessage());
        }
    }
}
