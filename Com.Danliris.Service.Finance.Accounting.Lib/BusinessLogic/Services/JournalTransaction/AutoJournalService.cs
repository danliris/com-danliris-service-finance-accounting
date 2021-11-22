using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.DailyBankTransaction;
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
        private const string UserAgent = "finance-accounting-service";
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

        public async Task<int> AutoJournalVBNonPOClearence(List<int> vbRealizationIds, AccountBankViewModel bank, string referenceNo)
        {
            var dbContext = _serviceProvider.GetService<FinanceDbContext>();

            var vbRealizations = dbContext.VBRealizationDocuments.Where(entity => vbRealizationIds.Contains(entity.Id)).ToList();
            var vbRequestIds = vbRealizations.Select(element => element.VBRequestDocumentId).ToList();
            var _vbRequestDocuments = dbContext.VBRequestDocuments.Where(entity => vbRequestIds.Contains(entity.Id)).ToList();
            var _vbRealizationItems = dbContext.VBRealizationDocumentExpenditureItems.Where(entity => vbRealizationIds.Contains(entity.VBRealizationDocumentId)).ToList();
            var _vbRealizationUnitCosts = dbContext.VBRealizationDocumentUnitCostsItems.Where(entity => vbRealizationIds.Contains(entity.VBRealizationDocumentId) && entity.IsSelected).ToList();

            var units = await _masterCOAService.GetCOAUnits();
            var divisions = await _masterCOAService.GetCOADivisions();
            //var bans = await _masterCOAService.Get

            foreach (var vbRealization in vbRealizations)
            {
                //var bankDocumentNo = DocumentNoGenerator(bank);

                var coaUnit = "00";
                var unit = units.FirstOrDefault(element => vbRealization.SuppliantUnitId == element.Id);
                if (unit != null)
                    coaUnit = unit.COACode;

                var coaDivision = "0";
                var division = divisions.FirstOrDefault(element => vbRealization.SuppliantDivisionId == element.Id);
                if (division != null)
                    coaDivision = division.COACode;

                if (vbRealization.IsInklaring)
                {
                    if (vbRealization.CurrencyCode == "IDR")
                    {
                        var journalTransaction = new JournalTransactionModel()
                        {
                            Date = vbRealization.Date,
                            Description = $"Pajak Clearance {vbRealization.DocumentNo}",
                            ReferenceNo = vbRealization.ReferenceNo,
                            Status = "DRAFT",
                            Items = new List<JournalTransactionItemModel>()
                        };

                        var vbRealizationUnitCosts = _vbRealizationUnitCosts.Where(element => element.VBRealizationDocumentId == vbRealization.Id && element.IsSelected).ToList();

                        foreach (var vbRealizationUnitCost in vbRealizationUnitCosts)
                        {
                            var costCOADivision = "0";
                            var costDivision = divisions.FirstOrDefault(element => element.Id == vbRealizationUnitCost.DivisionId);
                            if (costDivision != null && !string.IsNullOrWhiteSpace(costDivision.COACode))
                                costCOADivision = costDivision.COACode;

                            var costCOAUnit = "00";
                            var costUnit = units.FirstOrDefault(element => element.Id == vbRealizationUnitCost.UnitId);
                            if (costUnit != null && !string.IsNullOrWhiteSpace(costUnit.COACode))
                                costCOAUnit = costUnit.COACode;

                            journalTransaction.Items.Add(new JournalTransactionItemModel()
                            {
                                COA = new COAModel()
                                {
                                    Code = $"1804.00.{costCOADivision}.{costCOAUnit}"
                                },
                                Debit = vbRealizationUnitCost.Amount * (decimal)vbRealization.CurrencyRate
                            });

                            journalTransaction.Items.Add(new JournalTransactionItemModel()
                            {
                                COA = new COAModel()
                                {
                                    Code = $"1503.00.{costCOADivision}.{costCOAUnit}"
                                },
                                Credit = vbRealizationUnitCost.Amount * (decimal)vbRealization.CurrencyRate
                            });
                        }

                        var sumPPn = _vbRealizationItems.Where(element => element.VBRealizationDocumentId == vbRealization.Id).Sum(element => element.PPnAmount);
                        var sumPPh = _vbRealizationItems.Where(element => element.VBRealizationDocumentId == vbRealization.Id).Sum(element => element.PPhAmount);

                        if (sumPPn > 0)
                            journalTransaction.Items.Add(new JournalTransactionItemModel()
                            {
                                COA = new COAModel()
                                {
                                    Code = $"1509.00.{coaDivision}.{coaUnit}"
                                },
                                Debit = sumPPn * (decimal)vbRealization.CurrencyRate
                            });

                        if (sumPPh > 0)
                            journalTransaction.Items.Add(new JournalTransactionItemModel()
                            {
                                COA = new COAModel()
                                {
                                    Code = $"3330.00.{coaDivision}.{coaUnit}"
                                },
                                Credit = sumPPh * (decimal)vbRealization.CurrencyRate
                            });

                        await _journalTransactionService.CreateAsync(journalTransaction);

                        var vbRequestDocument = _vbRequestDocuments.FirstOrDefault(element => element.Id == vbRealization.VBRequestDocumentId);

                        if (vbRequestDocument != null)
                        {
                            var difference = vbRequestDocument.Amount - vbRealization.Amount;
                            if (difference > 0)
                            {
                                if (!string.IsNullOrWhiteSpace(vbRealization.ReferenceNo) && vbRealization.ReferenceNo.Length == 15)
                                {
                                    //var bankCode = vbRealization.ReferenceNo.Substring(4, 6);

                                    //bankCod = GetAccountBankCOA(bank.);
                                    if (bank != null)
                                    {
                                        var diffReferenceNo = await GetDocumentNo("M", bank.BankCode, vbRealization.CreatedBy, vbRealization.Date.DateTime);
                                        var modelDifference = new JournalTransactionModel()
                                        {
                                            Date = vbRealization.Date,
                                            Description = "Sisa Clearence VB Inklaring",
                                            ReferenceNo = diffReferenceNo,
                                            Status = "POSTED",
                                            Items = new List<JournalTransactionItemModel>()
                                        };

                                        modelDifference.Items.Add(new JournalTransactionItemModel()
                                        {
                                            COA = new COAModel()
                                            {
                                                Code = bank.AccountCOA
                                            },
                                            Debit = difference * (decimal)vbRealization.CurrencyRate
                                        });

                                        modelDifference.Items.Add(new JournalTransactionItemModel()
                                        {
                                            COA = new COAModel()
                                            {
                                                Code = $"1503.00.{coaDivision}.{coaUnit}"
                                            },
                                            Credit = difference * (decimal)vbRealization.CurrencyRate
                                        });

                                        await _journalTransactionService.CreateAsync(modelDifference);

                                        var dailyBankTransaction = new DailyBankTransactionModel()
                                        {
                                            AccountBankAccountName = bank.AccountName,
                                            AccountBankAccountNumber = bank.AccountNumber,
                                            AccountBankCode = bank.BankCode,
                                            AccountBankCurrencyCode = bank.Currency.Code,
                                            AccountBankCurrencyId = bank.Id,
                                            AccountBankCurrencySymbol = vbRealization.CurrencyCode,
                                            AccountBankId = bank.Id,
                                            AccountBankName = bank.BankName,
                                            Date = vbRealization.Date,
                                            Nominal = (decimal)difference * (decimal)vbRealization.CurrencyRate,
                                            CurrencyRate = (decimal)vbRealization.CurrencyRate,
                                            ReferenceNo = diffReferenceNo,
                                            ReferenceType = "Sisa Clearance VB Inklaring",
                                            Remark = vbRealization.CurrencyCode != "IDR" ? $"Pembayaran atas {bank.Currency.Code} dengan nominal {string.Format("{0:n}", vbRealization.Amount)} dan kurs {vbRealization.CurrencyCode}" : "",
                                            SourceType = "OPERASIONAL",
                                            SupplierCode = vbRealization.SuppliantUnitCode,
                                            SupplierId = vbRealization.SuppliantUnitId,
                                            SupplierName = vbRealization.SuppliantUnitName,
                                            Status = "IN",
                                            IsPosted = true
                                        };

                                        //if (vbRealizationDocument.IsInklaring)
                                        //    dailyBankTransaction.ReferenceType = "Clearence VB Inklaring";
                                        //else if (vbRealizationDocument.Type == 2)
                                        //    dailyBankTransaction.ReferenceType = "Clearence VB Non PO";
                                        //else
                                        //    dailyBankTransaction.ReferenceType = "Clearence VB With PO";

                                        if (bank.Currency.Code != "IDR")
                                            dailyBankTransaction.NominalValas = difference;

                                        dailyBankTransaction.FlagForCreate(_identityService.Username, UserAgent);
                                        _dbContext.DailyBankTransactions.Add(dailyBankTransaction);
                                        _dbContext.SaveChanges();
                                    }
                                }

                            }
                        }
                    }

                }
                else
                {
                    var journalTransaction = new JournalTransactionModel()
                    {
                        Date = vbRealization.Date,
                        Description = "Clearance VB",
                        ReferenceNo = vbRealization.ReferenceNo,
                        Status = "DRAFT",
                        Items = new List<JournalTransactionItemModel>()
                    };

                    if (vbRealization.CurrencyCode == "IDR")
                    {
                        var vbRealizationUnitCosts = _vbRealizationUnitCosts.Where(element => element.VBRealizationDocumentId == vbRealization.Id && element.IsSelected).ToList();

                        foreach (var vbRealizationUnitCost in vbRealizationUnitCosts)
                        {
                            var costCOADivision = "0";
                            var costDivision = divisions.FirstOrDefault(element => element.Id == vbRealizationUnitCost.DivisionId);
                            if (costDivision != null && !string.IsNullOrWhiteSpace(costDivision.COACode))
                                costCOADivision = costDivision.COACode;

                            var costCOAUnit = "00";
                            var costUnit = units.FirstOrDefault(element => element.Id == vbRealizationUnitCost.UnitId);
                            if (costUnit != null && !string.IsNullOrWhiteSpace(costUnit.COACode))
                                costCOAUnit = costUnit.COACode;

                            journalTransaction.Items.Add(new JournalTransactionItemModel()
                            {
                                COA = new COAModel()
                                {
                                    Code = $"1011.00.{costCOADivision}.{costCOAUnit}"
                                },
                                Credit = vbRealizationUnitCost.Amount * (decimal)vbRealization.CurrencyRate
                            });
                        }

                        var sumPPn = _vbRealizationItems.Where(element => element.VBRealizationDocumentId == vbRealization.Id).Sum(element =>
                        {
                            var result = (decimal)0;

                            if (element.UseVat)
                                result = element.Amount * (decimal)0.1;

                            return result;
                        });

                        var sumPPh = _vbRealizationItems.Where(element => element.VBRealizationDocumentId == vbRealization.Id).Sum(element =>
                        {
                            var result = (decimal)0;

                            if (element.UseIncomeTax)
                                result = element.Amount * ((decimal)element.IncomeTaxRate / 100);

                            return result;
                        });

                        if (sumPPn > 0)
                            journalTransaction.Items.Add(new JournalTransactionItemModel()
                            {
                                COA = new COAModel()
                                {
                                    Code = $"1509.00.{coaDivision}.{coaUnit}"
                                },
                                Debit = sumPPn * (decimal)vbRealization.CurrencyRate
                            });

                        if (sumPPh > 0)
                            journalTransaction.Items.Add(new JournalTransactionItemModel()
                            {
                                COA = new COAModel()
                                {
                                    Code = $"3330.00.{coaDivision}.{coaUnit}"
                                },
                                Credit = sumPPh * (decimal)vbRealization.CurrencyRate
                            });

                        await _journalTransactionService.CreateAsync(journalTransaction);
                    }
                    else
                    {
                        var currency = await GetBICurrency(vbRealization.CurrencyCode, vbRealization.Date);
                        var currencyRate = currency != null ? (decimal)currency.Rate.GetValueOrDefault() : (decimal)vbRealization.CurrencyRate;

                        journalTransaction.Items.Add(new JournalTransactionItemModel()
                        {
                            COA = new COAModel()
                            {
                                Code = $"{bank.AccountCOA}"
                            },
                            Credit = vbRealization.Amount * (decimal)currencyRate
                        });

                        await _journalTransactionService.CreateAsync(journalTransaction);
                    }
                }

            }

            return vbRealizations.Count;
        }

        private async Task<GarmentCurrency> GetBICurrency(string codeCurrency, DateTimeOffset date)
        {
            string stringDate = date.ToString("yyyy/MM/dd HH:mm:ss");
            string queryString = $"code={codeCurrency}&stringDate={stringDate}";

            var http = _serviceProvider.GetService<IHttpClientService>();
            var response = await http.GetAsync(APIEndpoint.Core + $"master/bi-currencies/single-by-code-date?{queryString}");

            var responseString = await response.Content.ReadAsStringAsync();
            var jsonSerializationSetting = new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Ignore };

            var result = JsonConvert.DeserializeObject<APIDefaultResponse<GarmentCurrency>>(responseString, jsonSerializationSetting);

            return result.data;
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

        public async Task<string> GetDocumentNo(string type, string bankCode, string username, DateTime date)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var http = _serviceProvider.GetService<IHttpClientService>();
            var uri = APIEndpoint.Purchasing + $"bank-expenditure-notes/bank-document-no-date?type={type}&bankCode={bankCode}&username={username}&date={date}";
            var response = await http.GetAsync(uri);

            var result = new BaseResponse<string>();

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<BaseResponse<string>>(responseContent, jsonSerializerSettings);
            }

            return result.data;
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
                Credit = journalTransactionModel.Items.Sum(item => item.Debit)
            };
            journalTransactionModel.Items.Add(creditItem);

            return await _journalTransactionService.CreateAsync(journalTransactionModel);
        }
    }
}