using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.VBVerification;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.VbVerification
{
    public class VbVerificationDataUtil
    {
        private readonly VbVerificationService _service;

        public VbVerificationDataUtil(VbVerificationService service)
        {
            _service = service;
        }

        public VbVerificationViewModel GetVbVerificationViewModel()
        {
            return new VbVerificationViewModel()
            {
                numberVB = new NumberVBData()
                {
                    Id = 1,
                    Amount_Realization = 1,
                    Amount_Request = 1,
                    Currency = "USD",
                    DateEstimate = DateTimeOffset.UtcNow,
                    DateRealization = DateTimeOffset.UtcNow,
                    DateVB = DateTimeOffset.UtcNow,
                    Diff = 1,
                    RequestVbName = "RequestVbName",
                    UnitName = "UnitName",
                    Usage = "Usage",
                    VBNo = "VBNo",
                    VBNoRealize = "VBNoRealize",
                    VBRealizeCategory = "VBRealizeCategory",
                    Status_ReqReal = "Status_ReqReal",
                    Amount_Vat = 1,
                    DetailItems = new List<VbVerificationDetailViewModel>()
                    {
                        new VbVerificationDetailViewModel()
                        {
                            Date = DateTimeOffset.UtcNow,
                            Remark = "Remark",
                            Amount = 1,
                            isGetPPn = true,
                            Total = 1
                        }
                    }
                },
                Reason = "Reason",
                Remark = "Remark",
                VerifyDate = DateTimeOffset.UtcNow,
                isNotVeridied = true,
                isVerified = true

            };
        }

        public VbVerificationViewModel GetVbVerificationViewModelNotVerified()
        {
            return new VbVerificationViewModel()
            {
                numberVB = new NumberVBData()
                {
                    Id = 1,
                    Amount_Realization = 1,
                    Amount_Request = 1,
                    Currency = "USD",
                    DateEstimate = DateTimeOffset.UtcNow,
                    DateRealization = DateTimeOffset.UtcNow,
                    DateVB = DateTimeOffset.UtcNow,
                    Diff = 1,
                    RequestVbName = "RequestVbName",
                    UnitName = "UnitName",
                    Usage = "Usage",
                    VBNo = "VBNo",
                    VBNoRealize = "VBNoRealize",
                    VBRealizeCategory = "VBRealizeCategory",
                    Status_ReqReal = "Status_ReqReal",
                    Amount_Vat = 1,
                    DetailItems = new List<VbVerificationDetailViewModel>()
                    {
                        new VbVerificationDetailViewModel()
                        {
                            Date = DateTimeOffset.UtcNow,
                            Remark = "Remark",
                            Amount = 1,
                            isGetPPn = true,
                            Total = 1
                        }
                    }
                },
                Reason = "",
                Remark = "Remark",
                VerifyDate = DateTimeOffset.UtcNow,
                isNotVeridied = true,
                isVerified = false
            };
        }

        public VbVerificationViewModel GetViewModelToValidate2()
        {
            return new VbVerificationViewModel()
            {
                numberVB = new NumberVBData()
                {
                    Id = 1,
                    Amount_Realization = 1,
                    Amount_Request = 1,
                    Currency = "USD",
                    DateEstimate = DateTimeOffset.UtcNow,
                    DateRealization = DateTimeOffset.UtcNow,
                    DateVB = DateTimeOffset.UtcNow,
                    Diff = 1,
                    RequestVbName = "RequestVbName",
                    UnitName = "UnitName",
                    Usage = "Usage",
                    VBNo = "VBNo",
                    VBNoRealize = "VBNoRealize",
                    VBRealizeCategory = "VBRealizeCategory",
                    Status_ReqReal = "Status_ReqReal",
                    Amount_Vat = 1,
                    DetailItems = new List<VbVerificationDetailViewModel>()
                    {
                        new VbVerificationDetailViewModel()
                        {
                            Date = DateTimeOffset.UtcNow,
                            Remark = "Remark",
                            Amount = 1,
                            isGetPPn = true,
                            Total = 1
                        }
                    }
                },
                Reason = "",
                Remark = "Remark",
                VerifyDate = DateTimeOffset.UtcNow,
                isNotVeridied = true,
                isVerified = true

            };
        }
    }
}
