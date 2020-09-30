using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MasterCOA;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.OthersExpenditureProofDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.NewIntegrationViewModel;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.OthersExpenditureProofDocumentViewModels;
using Com.Moonlay.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.JournalTransaction
{
    public class AutoJournalService : IAutoJournalService
    {
        private readonly FinanceDbContext _dbContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly IJournalTransactionService _journalTransactionService;
        private readonly IIdentityService _identityService;

        public AutoJournalService(FinanceDbContext dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _serviceProvider = serviceProvider;
            _journalTransactionService = serviceProvider.GetService<IJournalTransactionService>();
            _identityService = serviceProvider.GetService<IIdentityService>();
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

        public async Task<int> AutoJournalVBNonPOClearence(List<int> vbRealizationIds, AccountBankViewModel bank)
        {
            var dbContext = _serviceProvider.GetService<FinanceDbContext>();

            var vbRealizations = dbContext.VBRealizationDocuments.Where(entity => vbRealizationIds.Contains(entity.Id)).ToList();
            var vbRequestIds = vbRealizations.Select(element => element.VBRequestDocumentId).ToList();
            var vbRequests = dbContext.VBRequestDocuments.Where(entity => vbRequestIds.Contains(entity.Id)).ToList();
            var vbRealizationItems = dbContext.VBRealizationDocumentExpenditureItems.Where(entity => vbRealizationIds.Contains(entity.VBRealizationDocumentId)).ToList();


            foreach (var vbRealization in vbRealizations)
            {
                var bankDocumentNo = DocumentNoGenerator(bank);

                if (vbRealization.VBRequestDocumentId > 0)
                {
                    var vbRequest = vbRequests.FirstOrDefault(element => element.Id == vbRealization.VBRequestDocumentId);
                    var selectedVbRealizationItems = vbRealizationItems.Where(entity => entity.VBRealizationDocumentId == vbRealization.Id).ToList();
                    if (vbRequest.IsInklaring)
                    {
                        if (vbRequest.CurrencyCode == "IDR")
                        {
                            var modelInklaring = new JournalTransactionModel()
                            {
                                Date = vbRealization.Date,
                                Description = "Clearance VB Inklaring",
                                ReferenceNo = bankDocumentNo,
                                Status = "POSTED",
                                Items = new List<JournalTransactionItemModel>()
                            };

                            foreach (var vbRealizationItem in selectedVbRealizationItems)

                            await _journalTransactionService.CreateAsync(modelInklaring);

                            var difference = vbRequest.Amount - vbRealization.Amount;
                            if (difference > 0)
                            {
                                var modelDifference = new JournalTransactionModel()
                                {
                                    Date = vbRequest.Date,
                                    Description = "Clearence VB Inklaring",
                                    ReferenceNo = bankDocumentNo,
                                    Status = "POSTED",
                                    Items = new List<JournalTransactionItemModel>()
                                };

                                modelDifference.Items.Add(new JournalTransactionItemModel()
                                {
                                    COA = new COAModel()
                                    {
                                        Code = bank.AccountCOA
                                    },
                                    Debit = difference
                                });

                                modelDifference.Items.Add(new JournalTransactionItemModel()
                                {
                                    COA = new COAModel()
                                    {
                                        Code = $"1503.00.0.00"
                                    },
                                    Credit = difference
                                });

                                await _journalTransactionService.CreateAsync(modelDifference);
                            }
                        }
                    }
                    else
                    {
                        var model = new JournalTransactionModel()
                        {
                            Date = vbRealization.Date,
                            Description = "Clearance VB",
                            ReferenceNo = bankDocumentNo,
                            Status = "DRAFT",
                            Items = new List<JournalTransactionItemModel>()
                        };

                        //model.Items.Add(new JournalTransactionItemModel()
                        //{
                        //    COA = new COAModel()
                        //    {
                        //        Code = $"9999.00.0.00"
                        //    },
                        //    Debit = vbRealization.Amount
                        //});

                        if (vbRealization.CurrencyCode == "IDR")
                        {
                            foreach (var vbRealizationItem in selectedVbRealizationItems)
                            {
                                var ppn = vbRealizationItem.UseVat ? vbRealizationItem.Amount * (decimal)0.1 : 0;
                                var pph = vbRealizationItem.UseIncomeTax ? vbRealizationItem.Amount * (decimal)vbRealizationItem.IncomeTaxRate : 0;

                                if (ppn > 0) 
                                {
                                    model.Items.Add(new JournalTransactionItemModel()
                                    {
                                        COA = new COAModel()
                                        {
                                            Code = $"1509.00.0.00"
                                        },
                                        Debit = ppn
                                    });
                                }

                                if (pph > 0)
                                {
                                    model.Items.Add(new JournalTransactionItemModel()
                                    {
                                        COA = new COAModel()
                                        {
                                            Code = $"3330.00.0.00"
                                        },
                                        Credit = pph
                                    });
                                }

                                model.Items.Add(new JournalTransactionItemModel()
                                {
                                    COA = new COAModel()
                                    {
                                        Code = $"1011.00.0.00"
                                    },
                                    Credit = vbRealizationItem.Amount
                                });
                            }
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

                        //if (model.Items.Any(element => element.COA.Code.Contains("9999")))
                        //    model.Status = "DRAFT";

                        await _journalTransactionService.CreateAsync(model);
                    }
                }
                else
                {
                    var model = new JournalTransactionModel()
                    {
                        Date = vbRealization.Date,
                        Description = "Clearance VB",
                        ReferenceNo = vbRealization.DocumentNo,
                        Status = "DRAFT",
                        Items = new List<JournalTransactionItemModel>()
                    };

                    //model.Items.Add(new JournalTransactionItemModel()
                    //{
                    //    COA = new COAModel()
                    //    {
                    //        Code = $"9999.00.0.00"
                    //    },
                    //    Debit = vbRealization.Amount
                    //});

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

                    //if (model.Items.Any(element => element.COA.Code.Contains("9999")))
                    //    model.Status = "DRAFT";

                    await _journalTransactionService.CreateAsync(model);
                }
            }

            return vbRealizations.Count;
        }

        public async Task<int> AutoJournalInklaring(List<int> vbRequestIds, AccountBankViewModel bank)
        {
            var dbContext = _serviceProvider.GetService<FinanceDbContext>();
            var vbRequests = dbContext.VBRequestDocuments.Where(entity => vbRequestIds.Contains(entity.Id)).ToList();

            foreach (var vbRequest in vbRequests)
            {
                var bankDocumentNo = DocumentNoGenerator(bank);
                if (vbRequest.IsInklaring && vbRequest.CurrencyCode == "IDR")
                {
                    var modelInklaring = new JournalTransactionModel()
                    {
                        Date = vbRequest.Date,
                        Description = "Approval VB Inklaring",
                        ReferenceNo = bankDocumentNo,
                        Status = "DRAFT",
                        Items = new List<JournalTransactionItemModel>()
                    };

                    modelInklaring.Items.Add(new JournalTransactionItemModel()
                    {
                        COA = new COAModel()
                        {
                            Code = $"1503.00.0.00"
                        },
                        Debit = vbRequest.Amount
                    });

                    modelInklaring.Items.Add(new JournalTransactionItemModel()
                    {
                        COA = new COAModel()
                        {
                            Code = bank.AccountCOA
                        },
                        Credit = vbRequest.Amount
                    });

                    //modelInklaring.Items.Add(new JournalTransactionItemModel()
                    //{
                    //    COA = new COAModel()
                    //    {
                    //        Code = $"9999.00.0.00"
                    //    },
                    //    Credit = vbRequest.Amount
                    //});

                    //if (modelInklaring.Items.Any(element => element.COA.Code.Contains("9999")))
                    //modelInklaring.Status = "DRAFT";

                    await _journalTransactionService.CreateAsync(modelInklaring);
                }
            }
            return vbRequests.Count;
        }

        private string DocumentNoGenerator(AccountBankViewModel bank)
        {
            var latestDocumentNo = _dbContext.OthersExpenditureProofDocuments.IgnoreQueryFilters().Where(document => document.DocumentNo.Contains(bank.BankCode)).OrderByDescending(document => document.Id).Select(document => new { document.DocumentNo, document.CreatedUtc }).FirstOrDefault();

            var now = DateTimeOffset.Now;
            var result = "";
            if (latestDocumentNo == null)
            {
                result = $"{now:yy}{now:MM}{bank.BankCode}K0001";
            }
            else
            {
                if (latestDocumentNo.CreatedUtc.Month != now.Month)
                    result = $"{now:yy}{now:MM}{bank.BankCode}K0001";
                else
                {
                    var numberString = latestDocumentNo.DocumentNo.Split("K").ToList()[1];
                    var number = int.Parse(numberString) + 1;
                    result = $"{now:yy}{now:MM}{bank.BankCode}K{number.ToString().PadLeft(4, '0')}";
                }
            }

            var model = new OthersExpenditureProofDocumentModel();
            model.DocumentNo = result;
            EntityExtension.FlagForCreate(model, _identityService.Username, "finance-accounting-service");
            EntityExtension.FlagForDelete(model, _identityService.Username, "finance-accounting-service");

            return result;
        }

        public async Task<int> AutoJournalVBNonPOClearence(List<int> vbRealizationIds)
        {
            var dbContext = _serviceProvider.GetService<FinanceDbContext>();

            var vbRealizations = dbContext.VBRealizationDocuments.Where(entity => vbRealizationIds.Contains(entity.Id)).ToList();
            var vbRequestIds = vbRealizations.Select(element => element.VBRequestDocumentId).ToList();
            var vbRequests = dbContext.VBRequestDocuments.Where(entity => vbRequestIds.Contains(entity.Id)).ToList();

            foreach (var vbRealization in vbRealizations)
            {


                if (vbRealization.VBRequestDocumentId > 0)
                {
                    var vbRequest = vbRequests.FirstOrDefault(element => element.Id == vbRealization.VBRequestDocumentId);
                    if (vbRequest.IsInklaring)
                    {
                        if (vbRequest.CurrencyCode == "IDR")
                        {
                            var modelInklaring = new JournalTransactionModel()
                            {
                                Date = vbRealization.Date,
                                Description = "Clearance VB Inklaring",
                                ReferenceNo = vbRealization.DocumentNo,
                                Status = "POSTED",
                                Items = new List<JournalTransactionItemModel>()
                            };

                            modelInklaring.Items.Add(new JournalTransactionItemModel()
                            {
                                COA = new COAModel()
                                {
                                    Code = $"1804.00.0.00"
                                },
                                Debit = vbRealization.Amount
                            });

                            modelInklaring.Items.Add(new JournalTransactionItemModel()
                            {
                                COA = new COAModel()
                                {
                                    Code = $"1503.00.0.00"
                                },
                                Credit = vbRealization.Amount
                            });

                            if (modelInklaring.Items.Any(element => element.COA.Code.Contains("9999")))
                                modelInklaring.Status = "DRAFT";

                            await _journalTransactionService.CreateAsync(modelInklaring);
                        }
                    }
                    else
                    {
                        var model = new JournalTransactionModel()
                        {
                            Date = vbRealization.Date,
                            Description = "Clearance VB",
                            ReferenceNo = vbRealization.DocumentNo,
                            Status = "DRAFT",
                            Items = new List<JournalTransactionItemModel>()
                        };

                        //model.Items.Add(new JournalTransactionItemModel()
                        //{
                        //    COA = new COAModel()
                        //    {
                        //        Code = $"9999.00.0.00"
                        //    },
                        //    Debit = vbRealization.Amount
                        //});

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

                        //if (model.Items.Any(element => element.COA.Code.Contains("9999")))
                        //    model.Status = "DRAFT";

                        await _journalTransactionService.CreateAsync(model);
                    }
                }
                else
                {
                    var model = new JournalTransactionModel()
                    {
                        Date = vbRealization.Date,
                        Description = "Clearance VB",
                        ReferenceNo = vbRealization.DocumentNo,
                        Status = "DRAFT",
                        Items = new List<JournalTransactionItemModel>()
                    };

                    //model.Items.Add(new JournalTransactionItemModel()
                    //{
                    //    COA = new COAModel()
                    //    {
                    //        Code = $"9999.00.0.00"
                    //    },
                    //    Debit = vbRealization.Amount
                    //});

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

                    //if (model.Items.Any(element => element.COA.Code.Contains("9999")))
                    //    model.Status = "DRAFT";

                    await _journalTransactionService.CreateAsync(model);
                }
            }

            return vbRealizations.Count;
        }

        public async Task<int> AutoJournalInklaring(List<int> vbRequestIds)
        {
            var dbContext = _serviceProvider.GetService<FinanceDbContext>();
            var vbRequests = dbContext.VBRequestDocuments.Where(entity => vbRequestIds.Contains(entity.Id)).ToList();

            foreach (var vbRequest in vbRequests)
            {
                if (vbRequest.IsInklaring && vbRequest.CurrencyCode == "IDR")
                {
                    var modelInklaring = new JournalTransactionModel()
                    {
                        Date = vbRequest.Date,
                        Description = "Approval VB Inklaring",
                        ReferenceNo = vbRequest.DocumentNo,
                        Status = "DRAFT",
                        Items = new List<JournalTransactionItemModel>()
                    };

                    modelInklaring.Items.Add(new JournalTransactionItemModel()
                    {
                        COA = new COAModel()
                        {
                            Code = $"1503.00.0.00"
                        },
                        Debit = vbRequest.Amount
                    });

                    //modelInklaring.Items.Add(new JournalTransactionItemModel()
                    //{
                    //    COA = new COAModel()
                    //    {
                    //        Code = $"9999.00.0.00"
                    //    },
                    //    Credit = vbRequest.Amount
                    //});

                    //if (modelInklaring.Items.Any(element => element.COA.Code.Contains("9999")))
                    //modelInklaring.Status = "DRAFT";

                    await _journalTransactionService.CreateAsync(modelInklaring);
                }
            }
            return vbRequests.Count;
        }
    }
}