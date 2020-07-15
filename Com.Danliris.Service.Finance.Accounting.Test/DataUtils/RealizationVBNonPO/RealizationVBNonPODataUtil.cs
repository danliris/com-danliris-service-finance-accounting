using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.RealizationVBNonPO;
using Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.RealizationVBNonPO;
using System;
using System.Collections.Generic;
using System.Text;

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
                VBRealizeCategory = "VBRealizeCategory",
                VerifiedDate = DateTimeOffset.Now,
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

                    }
                }
            };
        }

        public RealizationVbNonPOViewModel GetNewViewModel()
        {
            return new RealizationVbNonPOViewModel()
            {
                VBRealizationNo = "VBRealizationNo",
                Date = DateTimeOffset.Now,
                numberVB = new DetailRequestNonPO()
                {
                    Amount = 123,
                    CreateBy = "CreateBy",
                    CurrencyCode = "IDR",
                    CurrencyRate = 123,
                    Date = DateTimeOffset.Now,
                    DateEstimate = DateTimeOffset.Now,
                    UnitCode = "UnitCode",
                    UnitId = 1,
                    UnitLoad = "UnitLoad",
                    UnitName = "UnitName",
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
                        isGetPPn = true
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
                    CreateBy = "CreateBy",
                    CurrencyCode = "IDR",
                    CurrencyRate = 123,
                    Date = DateTimeOffset.Now,
                    DateEstimate = DateTimeOffset.Now,
                    UnitCode = "UnitCode",
                    UnitId = 1,
                    UnitLoad = "UnitLoad",
                    UnitName = "UnitName",
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
    }
}
