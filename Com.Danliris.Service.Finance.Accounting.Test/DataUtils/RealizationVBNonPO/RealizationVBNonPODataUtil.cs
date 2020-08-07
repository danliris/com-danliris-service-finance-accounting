using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.RealizationVBNonPO;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.VbNonPORequest;
using Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.RealizationVBNonPO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.RealizationVBNonPO
{
    public class RealizationVBNonPODataUtil
    {
        private readonly RealizationVbNonPOService Service;

        public RealizationVBNonPODataUtil(RealizationVbNonPOService service)
        {
            Service = service;
        }

        public RealizationVbModel GetNewData()
        {
            return new RealizationVbModel()
            {
                DateEstimate = DateTimeOffset.Now,
                CloseDate = DateTimeOffset.Now,
                Date = DateTimeOffset.Now,
                isClosed = true,
                isNotVeridied = false,
                isVerified = true,
                RequestVbName = "RequestVbName",
                UnitCode = "UnitCode",
                UnitName = "UnitName",
                VBNo = "VBNo",
                VBNoRealize = "VBNoRealize",
                VBRealizeCategory = "NONPO",
                VerifiedDate = DateTimeOffset.Now,
                TypeWithOrWithoutVB = "Supplier",
                RealizationVbDetail = new List<RealizationVbDetailModel>()
                {
                    new RealizationVbDetailModel()
                    {
                        CodeProductSPB ="CodeProductSPB",
                        NameProductSPB ="NameProductSPB",
                        NoPOSPB ="NoPOSPB",
                        NoSPB="NoSPB",
                        PriceTotalSPB=1,
                        RealizationVbDetail =new RealizationVbModel()
                        {
                            DateEstimate = DateTimeOffset.Now,
                            CloseDate = DateTimeOffset.Now,
                            Date = DateTimeOffset.Now,
                            isClosed = true,
                            isNotVeridied = false,
                            isVerified = true,
                            RequestVbName = "RequestVbName",
                            UnitCode = "UnitCode",
                            UnitName = "UnitName",
                            VBNo = "VBNo",
                            VBNoRealize = "VBNoRealize",
                            VBRealizeCategory = "VBRealizeCategory",
                            VerifiedDate = DateTimeOffset.Now,
                        },
                        DivisionSPB ="DivisionSPB",
                        VBRealizationId =1,
                        IdProductSPB ="IdProductSPB",
                        DateSPB =DateTimeOffset.Now,
                        IncomeTaxId = "1",
                        IncomeTaxName = "IncomeTaxName",
                        IncomeTaxBy = "IncomeTaxBy",
                        IncomeTaxRate = "1"

                    }
                }
            };
        }

        public RealizationVbNonPOViewModel GetNewViewModel()
        {
            return new RealizationVbNonPOViewModel()
            {
                IsDeleted = false,
                Active = true,
                CreatedUtc = DateTime.Now,
                CreatedBy = "CreatedBy",
                CreatedAgent = "CreatedAgent",
                LastModifiedUtc = DateTime.Now,
                LastModifiedBy = "LastModifiedBy",
                LastModifiedAgent = "LastModifiedAgent",
                VBRealizationNo = "VBRealizationNo",
                Date = DateTimeOffset.Now,

                TypeVBNonPO = "Tanpa Nomor VB",
                AmountVB = 100,
                Spinning1 = true,
                Spinning2 = true,
                Spinning3 = true,
                Weaving1 = true,
                Weaving2 = true,
                Finishing = true,
                Printing = true,
                Konfeksi1A = true,
                Konfeksi1B = true,
                Konfeksi2A = true,
                Konfeksi2B = true,
                Konfeksi2C = true,
                Umum = true,
                Others = true,
                DetailOthers = "DetailOthers",

                AmountSpinning1 = 123,
                AmountSpinning2 = 123,
                AmountSpinning3 = 123,
                AmountWeaving1 = 123,
                AmountWeaving2 = 123,
                AmountFinishing = 123,
                AmountPrinting = 123,
                AmountKonfeksi1A = 123,
                AmountKonfeksi1B = 123,
                AmountKonfeksi2A = 123,
                AmountKonfeksi2B = 123,
                AmountKonfeksi2C = 123,
                AmountUmum = 123,
                AmountOthers = 123,
                DateEstimateVB = DateTimeOffset.Now,
                Unit = new Unit()
                {
                    Code = "code",
                    Name = "name"
                },
                DateVB = DateTimeOffset.Now,
                Currency = new CurrencyVBRequest()
                {
                    Code = "code",
                    Rate = 1,
                    Description = "des",
                    Symbol = "Rp"
                },
                Division = new Division()
                {
                    Id = 1,
                    Name = "name"
                },

                numberVB = new DetailRequestNonPO()
                {
                    Id = 1,
                    Amount = 123,
                    CreatedBy = "CreateBy",
                    CurrencyCode = "IDR",
                    CurrencyRate = 123,
                    CurrencyDescription = "CurrencyDescription",
                    CurrencySymbol = "CurrencySymbol",
                    Date = DateTimeOffset.Now,
                    DateEstimate = DateTimeOffset.Now,
                    UnitCode = "UnitCode",
                    UnitId = 1,
                    UnitLoad = "UnitLoad",
                    UnitName = "UnitName",
                    UnitDivisionId = 1,
                    UnitDivisionName = "UnitDivisionName",
                    VBNo = "VBNo",
                    VBRequestCategory = "NONPO"

                },
                Items = new List<VbNonPORequestDetailViewModel>()
                {
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now,
                        Remark = "Remark",
                        Amount = 123,
                        isGetPPn = true,
                        isGetPPh = true,
                        incomeTax = new IncomeTaxNew()
                        {
                            _id = "1",
                            name = "name",
                            rate = "1"
                        },
                        IncomeTaxBy = "Supplier"
                    },
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now,
                        Remark = "Remark",
                        Amount = 123,
                        isGetPPn = true,
                        isGetPPh = true,
                        incomeTax = new IncomeTaxNew()
                        {
                            _id = "1",
                            name = "name",
                            rate = "1"
                        },
                        IncomeTaxBy = "Danliris"
                    }
                }
            };
        }

        public RealizationVbNonPOViewModel GetNewViewModelNew()
        {
            return new RealizationVbNonPOViewModel()
            {
                IsDeleted = false,
                Active = true,
                CreatedUtc = DateTime.Now,
                CreatedBy = "CreatedBy",
                CreatedAgent = "CreatedAgent",
                LastModifiedUtc = DateTime.Now,
                LastModifiedBy = "LastModifiedBy",
                LastModifiedAgent = "LastModifiedAgent",
                VBRealizationNo = "VBRealizationNo",
                Date = DateTimeOffset.Now,
                TypeVBNonPO = "Dengan Nomor VB",
                numberVB = new DetailRequestNonPO()
                {
                    Amount = 123,
                    CreatedBy = "CreateBy",
                    CurrencyCode = "IDR",
                    CurrencyRate = 1,
                    CurrencyDescription = "CurrencyDescription",
                    CurrencySymbol = "CurrencySymbol",
                    Date = DateTimeOffset.Now,
                    DateEstimate = DateTimeOffset.Now,
                    UnitCode = "UnitCode",
                    UnitId = 1,
                    UnitLoad = "UnitLoad",
                    UnitName = "UnitName",
                    UnitDivisionId = 1,
                    UnitDivisionName = "UnitDivisionName",
                    VBNo = "VBNo",
                    VBRequestCategory = "NONPO"

                },
                Items = new List<VbNonPORequestDetailViewModel>()
                {
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now,
                        Remark = "Remark",
                        Amount = 123,
                        isGetPPn = false,
                        incomeTax = new IncomeTaxNew()
                        {
                            _id = "1",
                            name = "name",
                            rate = "1"
                        },
                        IncomeTaxBy = "IncomeTaxBy"
                    }
                }
            };
        }

        public RealizationVbNonPOViewModel GetNewViewModelNew1()
        {
            return new RealizationVbNonPOViewModel()
            {
                //Id = 1,
                IsDeleted = false,
                Active = true,
                CreatedUtc = DateTime.Now,
                CreatedBy = "CreatedBy",
                CreatedAgent = "CreatedAgent",
                LastModifiedUtc = DateTime.Now,
                LastModifiedBy = "LastModifiedBy",
                LastModifiedAgent = "LastModifiedAgent",
                VBRealizationNo = "VBRealizationNo",
                Date = DateTimeOffset.Now,
                TypeVBNonPO = "Supplier",
                numberVB = new DetailRequestNonPO()
                {
                    Amount = 123,
                    CreatedBy = "CreateBy",
                    CurrencyCode = "IDR",
                    CurrencyRate = 1,
                    CurrencyDescription = "CurrencyDescription",
                    CurrencySymbol = "CurrencySymbol",
                    Date = DateTimeOffset.Now,
                    DateEstimate = DateTimeOffset.Now,
                    UnitCode = "UnitCode",
                    UnitId = 1,
                    UnitLoad = "UnitLoad",
                    UnitName = "UnitName",
                    UnitDivisionId = 1,
                    UnitDivisionName = "UnitDivisionName",
                    VBNo = "VBNo",
                    VBRequestCategory = "NONPO"

                },
                Items = new List<VbNonPORequestDetailViewModel>()
                {
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now,
                        Remark = "Remark",
                        Amount = 123,
                        isGetPPn = false,
                        incomeTax = new IncomeTaxNew()
                        {
                            _id = "1",
                            name = "name",
                            rate = "1"
                        },
                        IncomeTaxBy = "IncomeTaxBy"
                    }
                }
            };
        }

        public RealizationVbNonPOViewModel GetNewViewModel2()
        {
            return new RealizationVbNonPOViewModel()
            {
                VBRealizationNo = "VBRealizationNo",
                Date = DateTimeOffset.Now,
                TypeVBNonPO = "Dengan Nomor VB",
                numberVB = new DetailRequestNonPO()
                {
                    Amount = 123,
                    CreatedBy = "CreateBy",
                    CurrencyCode = "IDR",
                    CurrencyRate = 123,
                    CurrencyDescription = "CurrencyDescription",
                    CurrencySymbol = "CurrencySymbol",
                    Date = DateTimeOffset.Now,
                    DateEstimate = DateTimeOffset.Now,
                    UnitCode = "UnitCode",
                    UnitId = 1,
                    UnitLoad = "UnitLoad",
                    UnitName = "UnitName",
                    UnitDivisionId = 1,
                    UnitDivisionName = "UnitDivisionName",
                    VBNo = "VBNo",
                    VBRequestCategory = "NONPO"

                },
                Items = new List<VbNonPORequestDetailViewModel>()
                {
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now,
                        Remark = "Remark",
                        Amount = 123,
                        isGetPPn = false
                    }
                }
            };
        }

        public RealizationVbNonPOViewModel GetNewViewModel3()
        {
            return new RealizationVbNonPOViewModel()
            {
                VBRealizationNo = "VBRealizationNo",
                Date = DateTimeOffset.Now,
                numberVB = new DetailRequestNonPO()
                {
                    Amount = 123,
                    CreatedBy = "CreateBy",
                    CurrencyCode = "USD",
                    CurrencyRate = 123,
                    CurrencyDescription = "CurrencyDescription",
                    CurrencySymbol = "CurrencySymbol",
                    Date = DateTimeOffset.Now,
                    DateEstimate = DateTimeOffset.Now,
                    UnitCode = "UnitCode",
                    UnitId = 1,
                    UnitLoad = "UnitLoad",
                    UnitName = "UnitName",
                    UnitDivisionId = 1,
                    UnitDivisionName = "UnitDivisionName",
                    VBNo = "VBNo",
                    VBRequestCategory = "NONPO"

                },
                Items = new List<VbNonPORequestDetailViewModel>()
                {
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now,
                        Remark = "Remark",
                        Amount = 123,
                        isGetPPn = false
                    }
                }
            };
        }

        public RealizationVbNonPOViewModel GetNewViewModel4()
        {
            return new RealizationVbNonPOViewModel()
            {
                VBRealizationNo = "VBRealizationNo",
                Date = DateTimeOffset.Now,
                TypeVBNonPO = "Tanpa Nomor VB",
                AmountVB = 100,
                Spinning1 = true,
                Spinning2 = true,
                Spinning3 = true,
                Weaving1 = true,
                Weaving2 = true,
                Finishing = true,
                Printing = true,
                Konfeksi1A = true,
                Konfeksi1B = true,
                Konfeksi2A = true,
                Konfeksi2B = true,
                Konfeksi2C = true,
                Umum = true,
                Others = true,
                DetailOthers = "DetailOthers",

                AmountSpinning1 = 123,
                AmountSpinning2 = 123,
                AmountSpinning3 = 123,
                AmountWeaving1 = 123,
                AmountWeaving2 = 123,
                AmountFinishing = 123,
                AmountPrinting = 123,
                AmountKonfeksi1A = 123,
                AmountKonfeksi1B = 123,
                AmountKonfeksi2A = 123,
                AmountKonfeksi2B = 123,
                AmountKonfeksi2C = 123,
                AmountUmum = 123,
                AmountOthers = 123,
                DateEstimateVB = DateTimeOffset.Now,
                Unit = new Unit()
                {
                    Code = "code",
                    Name = "name"
                },
                DateVB = DateTimeOffset.Now,
                Currency = new CurrencyVBRequest()
                {
                    Code = "code",
                    Rate = 1,
                    Description = "des",
                    Symbol = "Rp"
                },
                Division = new Division()
                {
                    Id = 1,
                    Name = "name"
                },

                numberVB = new DetailRequestNonPO()
                {
                    Amount = 0,
                    CreatedBy = "CreateBy",
                    CurrencyCode = "USD",
                    CurrencyRate = 123,
                    CurrencyDescription = "CurrencyDescription",
                    CurrencySymbol = "CurrencySymbol",
                    Date = DateTimeOffset.Now,
                    DateEstimate = DateTimeOffset.Now,
                    UnitCode = "UnitCode",
                    UnitId = 1,
                    UnitLoad = "UnitLoad",
                    UnitName = "UnitName",
                    UnitDivisionId = 1,
                    UnitDivisionName = "UnitDivisionName",
                    VBNo = "VBNo",
                    VBRequestCategory = "NONPO"

                },
                Items = new List<VbNonPORequestDetailViewModel>()
                {
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now,
                        Remark = "Remark",
                        Amount = 123,
                        isGetPPn = false,
                        isGetPPh = true,
                        incomeTax = new IncomeTaxNew()
                        {
                            _id = "2",
                            name = "name",
                            rate = "1"
                        },
                        IncomeTaxBy = "Dan Liris"
                    }
                }
            };
        }

        public RealizationVbNonPOViewModel GetNewViewModel5()
        {
            return new RealizationVbNonPOViewModel()
            {
                VBRealizationNo = "VBRealizationNo",
                Date = DateTimeOffset.Now,
                TypeVBNonPO = "Supplier",
                numberVB = new DetailRequestNonPO()
                {
                    Amount = 123,
                    CreatedBy = "CreateBy",
                    CurrencyCode = "USD",
                    CurrencyRate = 123,
                    CurrencyDescription = "CurrencyDescription",
                    CurrencySymbol = "CurrencySymbol",
                    Date = DateTimeOffset.Now,
                    DateEstimate = DateTimeOffset.Now,
                    UnitCode = "UnitCode",
                    UnitId = 1,
                    UnitLoad = "UnitLoad",
                    UnitName = "UnitName",
                    UnitDivisionId = 1,
                    UnitDivisionName = "UnitDivisionName",
                    VBNo = "VBNo",
                    VBRequestCategory = "NONPO"

                },
                Items = new List<VbNonPORequestDetailViewModel>()
                {
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now,
                        Remark = "Remark",
                        Amount = 0,
                        isGetPPn = false,
                        incomeTax = new IncomeTaxNew()
                        {
                            _id = "1",
                            name = "name",
                            rate = "1"
                        },
                        IncomeTaxBy = "IncomeTaxBy"
                    }
                }
            };
        }

        public RealizationVbNonPOViewModel GetNewViewModel6()
        {
            return new RealizationVbNonPOViewModel()
            {

                VBRealizationNo = "VBRealizationNo",
                Date = DateTimeOffset.Now,
                TypeVBNonPO = "Supplier",
                numberVB = new DetailRequestNonPO()
                {
                    Amount = 123,
                    CreatedBy = "CreateBy",
                    CurrencyCode = "USD",
                    CurrencyRate = 123,
                    CurrencyDescription = "CurrencyDescription",
                    CurrencySymbol = "CurrencySymbol",
                    Date = DateTimeOffset.Now,
                    DateEstimate = DateTimeOffset.Now,
                    UnitCode = "UnitCode",
                    UnitId = 1,
                    UnitLoad = "UnitLoad",
                    UnitName = "UnitName",
                    UnitDivisionId = 1,
                    UnitDivisionName = "UnitDivisionName",
                    VBNo = "VBNo",
                    VBRequestCategory = "NONPO"

                },
                Items = new List<VbNonPORequestDetailViewModel>()
                {
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now,
                        Remark = "Remark",
                        Amount = 0,
                        isGetPPn = false,
                        isGetPPh = true,
                        incomeTax = new IncomeTaxNew()
                        {
                            _id = "1",
                            name = "name",
                            rate = "1"
                        },
                        IncomeTaxBy = "IncomeTaxBy"
                    }
                }
            };
        }

        public RealizationVbNonPOViewModel GetNewViewModelFalse()
        {
            return new RealizationVbNonPOViewModel()
            {
                VBRealizationNo = "VBRealizationNo",
                Date = DateTimeOffset.Now,
                numberVB = new DetailRequestNonPO()
                {
                    Amount = 123,
                    CreatedBy = "CreateBy",
                    CurrencyCode = "IDR",
                    CurrencyRate = 123,
                    CurrencyDescription = "CurrencyDescription",
                    CurrencySymbol = "CurrencySymbol",
                    Date = DateTimeOffset.Now,
                    DateEstimate = DateTimeOffset.Now,
                    UnitCode = "UnitCode",
                    UnitId = 1,
                    UnitLoad = "UnitLoad",
                    UnitName = "UnitName",
                    UnitDivisionId = 1,
                    UnitDivisionName = "UnitDivisionName",
                    VBNo = "VBNo",
                    VBRequestCategory = "NONPO"

                },
                Items = new List<VbNonPORequestDetailViewModel>()
                {
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now.AddDays(5),
                        Remark = "",
                        Amount = -1,
                        isGetPPn = false
                    },
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = null,
                        Remark = "",
                        Amount = -1,
                        isGetPPn = false
                    },
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now.AddDays(-5),
                        Remark = "",
                        Amount = -1,
                        isGetPPn = false
                    },
                }
            };
        }

        public RealizationVbNonPOViewModel GetNewViewModelFalse2()
        {
            return new RealizationVbNonPOViewModel()
            {
                VBRealizationNo = "VBRealizationNo",
                Date = DateTimeOffset.Now,
                numberVB = new DetailRequestNonPO()
                {
                    Amount = 1,
                    Date = DateTimeOffset.Now,
                },
                Items = new List<VbNonPORequestDetailViewModel>()
                {
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now.AddDays(5),
                        Remark = "",
                        Amount = -1,
                        isGetPPn = false,
                        isGetPPh = true,
                        incomeTax = new IncomeTaxNew()
                        {
                            _id = "1",
                            name = "name",
                            rate = "1"
                        },
                        IncomeTaxBy = "income"
                    },
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = null,
                        Remark = "",
                        Amount = -1,
                        isGetPPn = false,
                        isGetPPh = true,
                        IncomeTaxBy = ""
                    },
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now.AddDays(-5),
                        Remark = "",
                        Amount = -1,
                        isGetPPn = false
                    },
                }
            };
        }

        public RealizationVbNonPOViewModel GetNewViewModelFalse3()
        {
            return new RealizationVbNonPOViewModel()
            {
                VBRealizationNo = "VBRealizationNo",
                Items = new List<VbNonPORequestDetailViewModel>()
                {
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now.AddDays(5),
                        Remark = "",
                        Amount = -1,
                        isGetPPn = false,
                        isGetPPh = true,
                        incomeTax = new IncomeTaxNew()
                        {
                            _id = "1",
                            name = "name",
                            rate = "1"
                        },
                        IncomeTaxBy = "income"
                    },
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = null,
                        Remark = "",
                        Amount = -1,
                        isGetPPn = false,
                        isGetPPh = true,
                        IncomeTaxBy = ""
                    },
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now.AddDays(-5),
                        Remark = "",
                        Amount = -1,
                        isGetPPn = false
                    },
                }
            };
        }

        public RealizationVbNonPOViewModel GetNewViewModelFalse4()
        {
            return new RealizationVbNonPOViewModel()
            {
                VBRealizationNo = "VBRealizationNo",
                Date = DateTimeOffset.Now,
                Items = new List<VbNonPORequestDetailViewModel>()
                {
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now.AddDays(5),
                        Remark = "",
                        Amount = -1,
                        isGetPPn = false,
                        isGetPPh = true,
                        incomeTax = new IncomeTaxNew()
                        {
                            _id = "1",
                            name = "name",
                            rate = "1"
                        },
                        IncomeTaxBy = "income"
                    },
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = null,
                        Remark = "",
                        Amount = -1,
                        isGetPPn = false,
                        isGetPPh = true,
                        IncomeTaxBy = ""
                    },
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now.AddDays(-5),
                        Remark = "",
                        Amount = -1,
                        isGetPPn = false
                    },
                }
            };
        }

        public RealizationVbNonPOViewModel GetNewViewModelFalse5()
        {
            return new RealizationVbNonPOViewModel()
            {
                VBRealizationNo = "VBRealizationNo",
                Date = DateTimeOffset.Now.AddDays(10),
                numberVB = new DetailRequestNonPO()
                {
                    Date = DateTimeOffset.Now.AddDays(-10),
                },
                Items = new List<VbNonPORequestDetailViewModel>()
                {
                    new VbNonPORequestDetailViewModel()
                    {
                        DateDetail = DateTimeOffset.Now,
                        Remark = "",
                        Amount = -1,
                        isGetPPn = false,
                        isGetPPh = true,
                        incomeTax = new IncomeTaxNew()
                        {
                            _id = "1",
                            name = "name",
                            rate = "1"
                        },
                        IncomeTaxBy = "income"
                    }
                }
            };
        }

        public VbRequestModel GetDataRequestVB()
        {
            return new VbRequestModel()
            {
                VBNo = "VBNo",
                Realization_Status = false,
                IsDeleted = false
            };
        }

        public RealizationVbModel GetDataRealizationVB()
        {
            return new RealizationVbModel()
            {
                VBNo = "VBNo",
                isVerified = true,
                IsDeleted = false,
            };
        }

        public async Task<RealizationVbNonPOViewModel> GetCreatedData()
        {
            var model = GetNewData();
            var viewmodel = GetNewViewModel();
            await Service.CreateAsync(model, viewmodel);
            return await Service.ReadByIdAsync2(model.Id);
        }

        public async Task<RealizationVbNonPOViewModel> GetCreatedData2()
        {
            var model = GetNewData();
            var viewmodel = GetNewViewModelNew();
            await Service.CreateAsync(model, viewmodel);
            return await Service.ReadByIdAsync2(model.Id);
        }
    }
}
