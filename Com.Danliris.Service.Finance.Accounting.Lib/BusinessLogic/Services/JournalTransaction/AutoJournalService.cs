using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MasterCOA;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.OthersExpenditureProofDocumentViewModels;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.JournalTransaction
{
    public class AutoJournalService : IAutoJournalService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IJournalTransactionService _journalTransactionService;

        public AutoJournalService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _journalTransactionService = serviceProvider.GetService<IJournalTransactionService>();
        }

        private async Task<string> GetAccountBankCOA(int accountBankId)
        {
            var http = _serviceProvider.GetService<IHttpClientService>();

            var response = await http.GetAsync(APIEndpoint.Core + $"master/account-banks/{accountBankId}");

            var responseString = await response.Content.ReadAsStringAsync();
            var jsonSerializationSetting = new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Ignore };

            var result = JsonConvert.DeserializeObject<APIDefaultResponse<AccountBank>>(responseString, jsonSerializationSetting);

            return result.data.AccountCOA;
        }

        public async Task<int> AutoJournalFromOthersExpenditureProof(OthersExpenditureProofDocumentCreateUpdateViewModel viewModel, string documentNo)
        {
            var model = new JournalTransactionModel()
            {
                Date = viewModel.Date.GetValueOrDefault(),
                Description = "Auto Journal Bukti Pengeluaran Bank Lain - Lain",
                ReferenceNo = documentNo,
                Status = "POSTED",
                Items = new List<JournalTransactionItemModel>()
            };

            model.Items = viewModel.Items.Select(item => new JournalTransactionItemModel()
            {
                COA = new COAModel()
                {
                    Id = item.COAId.GetValueOrDefault(),
                },
                Debit = item.Debit.GetValueOrDefault()
            }).ToList();

            var accountBankCOA = await GetAccountBankCOA(viewModel.AccountBankId.GetValueOrDefault());
            var creditItem = new JournalTransactionItemModel()
            {
                COA = new COAModel() { Code = accountBankCOA },
                Credit = viewModel.Items.Sum(item => item.Debit.GetValueOrDefault())
            };
            model.Items.Add(creditItem);

            return await _journalTransactionService.CreateAsync(model);
        }

        public Task<int> AutoJournalReverseFromOthersExpenditureProof(string documentNo)
        {
            return _journalTransactionService.ReverseJournalTransactionByReferenceNo(documentNo);
        }

        public async Task<int> AutoJournalVBNonPOClearence(List<int> vbRealizationIds)
        {
            var dbContext = _serviceProvider.GetService<FinanceDbContext>();

            var vbRealizations = dbContext.VBRealizationDocuments.Where(entity => vbRealizationIds.Contains(entity.Id)).ToList();

            foreach (var vbRealization in vbRealizations)
            {
                var model = new JournalTransactionModel()
                {
                    Date = vbRealization.Date,
                    Description = "Auto Journal Clearance VB",
                    ReferenceNo = vbRealization.DocumentNo,
                    Status = "POSTED",
                    Items = new List<JournalTransactionItemModel>()
                };

                model.Items.Add(new JournalTransactionItemModel()
                {
                    COA = new COAModel()
                    {
                        Code = $"9999.00.0.00"
                    },
                    Debit = vbRealization.Amount
                });

                if (vbRealization.CurrencyCode == "IDR")
                {
                    model.Items.Add(new JournalTransactionItemModel()
                    {
                        COA = new COAModel()
                        {
                            Code = $"1011.00.0.00"
                        },
                        Credit = vbRealization.Amount
                    });
                }
                else
                {
                    model.Items.Add(new JournalTransactionItemModel()
                    {
                        COA = new COAModel()
                        {
                            Code = $"1012.00.0.00"
                        },
                        Credit = vbRealization.Amount
                    });
                }

                if (model.Items.Any(element => element.COA.Code.Contains("9999")))
                    model.Status = "DRAFT";

                await _journalTransactionService.CreateAsync(model);
            }

            return vbRealizations.Count;
        }
    }
}