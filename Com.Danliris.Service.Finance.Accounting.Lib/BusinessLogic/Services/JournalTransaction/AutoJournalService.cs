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
        private readonly IMasterCOAService _masterCOAService;

        public AutoJournalService(FinanceDbContext dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _serviceProvider = serviceProvider;
            _journalTransactionService = serviceProvider.GetService<IJournalTransactionService>();
            _identityService = serviceProvider.GetService<IIdentityService>();
            _masterCOAService = serviceProvider.GetService<IMasterCOAService>();
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
            var vbRealizationUnitCosts = dbContext.VBRealizationDocumentUnitCostsItems.Where(entity => vbRealizationIds.Contains(entity.VBRealizationDocumentId) && entity.IsSelected).ToList();

            var units = await _masterCOAService.GetCOAUnits();
            var divisions = await _masterCOAService.GetCOADivisions();

            foreach (var vbRealization in vbRealizations)
            {
                var bankDocumentNo = DocumentNoGenerator(bank);

                var coaUnit = "00";
                var unit = units.FirstOrDefault(element => vbRealization.SuppliantUnitId == element.Id);
                if (unit != null)
                    coaUnit = unit.COACode;

                var coaDivision = "0";
                var division = divisions.FirstOrDefault(element => vbRealization.SuppliantDivisionId == element.Id);
                if (division != null)
                    coaDivision = division.COACode;


                //var vbRequest = vbRequests.FirstOrDefault(element => element.Id == vbRealization.VBRequestDocumentId);
                var selectedVbRealizationItems = vbRealizationItems.Where(entity => entity.VBRealizationDocumentId == vbRealization.Id).ToList();
                var selectedVbRealizationUnitCosts = vbRealizationUnitCosts.Where(entity => entity.VBRealizationDocumentId == vbRealization.Id).ToList();
                if (vbRealization.IsInklaring)
                {
                    var vbRequest = vbRequests.FirstOrDefault(element => element.Id == vbRealization.VBRequestDocumentId);
                    if (vbRealization.CurrencyCode == "IDR")
                    {
                        var modelInklaring = new JournalTransactionModel()
                        {
                            Date = vbRealization.Date,
                            Description = "Clearance VB Inklaring",
                            ReferenceNo = bankDocumentNo,
                            Status = "DRAFT",
                            Items = new List<JournalTransactionItemModel>()
                        };

                        foreach (var vbRealizationUnitCost in selectedVbRealizationUnitCosts)
                        {
                            var costCOADivision = "0";
                            var costDivision = divisions.FirstOrDefault(element => element.Id == vbRealizationUnitCost.DivisionId);
                            if (costDivision != null && !string.IsNullOrWhiteSpace(costDivision.COACode))
                                costCOADivision = costDivision.COACode;

                            var costCOAUnit = "00";
                            var costUnit = units.FirstOrDefault(element => element.Id == vbRealizationUnitCost.UnitId);
                            if (costUnit != null && !string.IsNullOrWhiteSpace(costUnit.COACode))
                                costCOAUnit = costUnit.COACode;

                            modelInklaring.Items.Add(new JournalTransactionItemModel()
                            {
                                COA = new COAModel()
                                {
                                    Code = $"1804.00.{costCOADivision}.{costCOAUnit}"
                                },
                                Debit = vbRealizationUnitCost.Amount
                            });

                            modelInklaring.Items.Add(new JournalTransactionItemModel()
                            {
                                COA = new COAModel()
                                {
                                    Code = $"1503.00.{costCOADivision}.{costCOAUnit}"
                                },
                                Credit = vbRealizationUnitCost.Amount
                            });
                        }

                        var sumPPn = selectedVbRealizationItems.Sum(element => element.PPnAmount);
                        var sumPPh = selectedVbRealizationItems.Sum(element => element.PPhAmount);


                        if (sumPPn > 0)
                            modelInklaring.Items.Add(new JournalTransactionItemModel()
                            {
                                COA = new COAModel()
                                {
                                    Code = $"1509.00.{coaDivision}.{coaUnit}"
                                },
                                Debit = sumPPn
                            });

                        if (sumPPh > 0)
                            modelInklaring.Items.Add(new JournalTransactionItemModel()
                            {
                                COA = new COAModel()
                                {
                                    Code = $"3330.00.{coaDivision}.{coaUnit}"
                                },
                                Credit = sumPPh
                            });

                        await _journalTransactionService.CreateAsync(modelInklaring);

                        if (vbRequest != null)
                        {
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
                                        Code = $"1503.00.{coaDivision}.{coaUnit}"
                                    },
                                    Credit = difference
                                });

                                await _journalTransactionService.CreateAsync(modelDifference);
                            }
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
                        foreach (var vbRealizationUnitCost in selectedVbRealizationUnitCosts)
                        {
                            var costCOADivision = "0";
                            var costDivision = divisions.FirstOrDefault(element => element.Id == vbRealizationUnitCost.DivisionId);
                            if (costDivision != null && !string.IsNullOrWhiteSpace(costDivision.COACode))
                                costCOADivision = costDivision.COACode;

                            var costCOAUnit = "00";
                            var costUnit = units.FirstOrDefault(element => element.Id == vbRealizationUnitCost.UnitId);
                            if (costUnit != null && !string.IsNullOrWhiteSpace(costUnit.COACode))
                                costCOAUnit = costUnit.COACode;

                            model.Items.Add(new JournalTransactionItemModel()
                            {
                                COA = new COAModel()
                                {
                                    Code = $"1011.00.{costCOADivision}.{costCOAUnit}"
                                },
                                Credit = vbRealizationUnitCost.Amount
                            });
                        }

                        var sumPPn = selectedVbRealizationUnitCosts.Sum(element =>
                        {
                            var result = (decimal)0;

                            if (element.UseVat)
                                result = element.Amount * (decimal)0.1;

                            return result;
                        });

                        var sumPPh = selectedVbRealizationUnitCosts.Sum(element =>
                        {
                            var result = (decimal)0;

                            if (element.UseIncomeTax)
                                result = element.Amount * ((decimal)element.IncomeTaxRate / 100);

                            return result;
                        });

                        if (sumPPn > 0)
                        {
                            model.Items.Add(new JournalTransactionItemModel()
                            {
                                COA = new COAModel()
                                {
                                    Code = $"1509.00.{coaDivision}.{coaUnit}"
                                },
                                Debit = sumPPn
                            });
                        }

                        if (sumPPh > 0)
                        {
                            model.Items.Add(new JournalTransactionItemModel()
                            {
                                COA = new COAModel()
                                {
                                    Code = $"3330.00.{coaDivision}.{coaUnit}"
                                },
                                Credit = sumPPh
                            });
                        }
                    }
                    else
                    {
                        model.Items.Add(new JournalTransactionItemModel()
                        {
                            COA = new COAModel()
                            {
                                Code = $"1012.00.{coaDivision}.{coaUnit}"
                            },
                            Credit = vbRealization.Amount
                        });
                    }

                    //if (model.Items.Any(element => element.COA.Code.Contains("9999")))
                    //    model.Status = "DRAFT";

                    await _journalTransactionService.CreateAsync(model);
                }

                #region old auto journal
                //else
                //{
                //    var model = new JournalTransactionModel()
                //    {
                //        Date = vbRealization.Date,
                //        Description = "Clearance VB",
                //        ReferenceNo = vbRealization.DocumentNo,
                //        Status = "DRAFT",
                //        Items = new List<JournalTransactionItemModel>()
                //    };

                //    //model.Items.Add(new JournalTransactionItemModel()
                //    //{
                //    //    COA = new COAModel()
                //    //    {
                //    //        Code = $"9999.00.0.00"
                //    //    },
                //    //    Debit = vbRealization.Amount
                //    //});

                //    if (vbRealization.CurrencyCode == "IDR")
                //    {
                //        model.Items.Add(new JournalTransactionItemModel()
                //        {
                //            COA = new COAModel()
                //            {
                //                Code = $"1011.00.{coaDivision}.{coaUnit}"
                //            },
                //            Credit = vbRealization.Amount
                //        });
                //    }
                //    else
                //    {
                //        model.Items.Add(new JournalTransactionItemModel()
                //        {
                //            COA = new COAModel()
                //            {
                //                Code = $"1012.00.{coaDivision}.{coaUnit}"
                //            },
                //            Credit = vbRealization.Amount
                //        });
                //    }

                //    //if (model.Items.Any(element => element.COA.Code.Contains("9999")))
                //    //    model.Status = "DRAFT";

                //    await _journalTransactionService.CreateAsync(model);

                //}
                #endregion

            }

            return vbRealizations.Count;
        }

        public async Task<int> AutoJournalInklaring(List<int> vbRequestIds, AccountBankViewModel bank)
        {
            var dbContext = _serviceProvider.GetService<FinanceDbContext>();
            var vbRequests = dbContext.VBRequestDocuments.Where(entity => vbRequestIds.Contains(entity.Id)).ToList();

            var units = await _masterCOAService.GetCOAUnits();
            var divisions = await _masterCOAService.GetCOAUnits();

            foreach (var vbRequest in vbRequests)
            {
                var bankDocumentNo = vbRequest.BankDocumentNo;

                var coaUnit = "00";
                var unit = units.FirstOrDefault(element => vbRequest.SuppliantUnitId == element.Id);
                if (unit != null)
                    coaUnit = unit.COACode;

                var coaDivision = "0";
                var division = divisions.FirstOrDefault(element => vbRequest.SuppliantDivisionId == element.Id);
                if (division != null)
                    coaDivision = division.COACode;

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
                            Code = $"1503.00.{coaDivision}.{coaUnit}"
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

        public string DocumentNoGenerator(AccountBankViewModel bank)
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
            _dbContext.OthersExpenditureProofDocuments.Add(model);
            _dbContext.SaveChanges();

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

        public async Task<int> AutoJournalFromOthersExpenditureProof(OthersExpenditureProofDocumentModel model, List<OthersExpenditureProofDocumentItemModel> items)
        {
            var journalTransactionModel = new JournalTransactionModel()
            {
                Date = model.Date,
                Description = "Auto Journal Bukti Pengeluaran Bank Lain - Lain",
                ReferenceNo = model.DocumentNo,
                Status = "POSTED",
                Items = new List<JournalTransactionItemModel>()
            };

            journalTransactionModel.Items = items.Select(item => new JournalTransactionItemModel()
            {
                COA = new COAModel()
                {
                    Id = item.COAId,
                },
                Debit = item.Debit * (decimal)model.CurrencyRate
            }).ToList();

            var accountBankCOA = await GetAccountBankCOA(model.AccountBankId);
            var creditItem = new JournalTransactionItemModel()
            {
                COA = new COAModel() { Code = accountBankCOA },
                Credit = items.Sum(item => item.Debit)
            };
            journalTransactionModel.Items.Add(creditItem);

            return await _journalTransactionService.CreateAsync(journalTransactionModel);
        }
    }
}