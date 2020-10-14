using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.DailyBankTransaction
{
    public class DailyBankTransactionIHttpService : IHttpClientService
    {
        public static string Token;

        public Task<HttpResponseMessage> PutAsync(string url, HttpContent content)
        {
            return Task.Run(() => new HttpResponseMessage());
        }

        public Task<HttpResponseMessage> GetAsync(string url)
        {
            if (url.Contains("master/account-banks/"))
            {
                var defaultresponse = new APIDefaultResponse<AccountBank>()
                {
                    data = new AccountBank()
                    {
                        Id = 1,
                        AccountCOA = "AccountCOA",
                        AccountName = "AccountName",
                        Currency = new Currency()
                        {
                            Id = 1,
                            Code = "Code",
                            Symbol = "Symbol"
                        },
                        AccountNumber = "AccountNumber",
                        BankCode = "BankCode",
                        BankName = "BankName"
                    }
                };
                var result = new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(defaultresponse))
                };

                return Task.FromResult(result);
            }
            else
            {
                return Task.Run(() => new HttpResponseMessage());
            }
        }

        public Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            return Task.Run(() => new HttpResponseMessage());
        }
    }
}
