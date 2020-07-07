using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VbWIthPORequest;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.VbWithPORequest
{
    public class VbWithPORequestDataUtil
    {
        private readonly VbWithPORequestService _service;

        public VbWithPORequestDataUtil(VbWithPORequestService service)
        {
            _service = service;
        }

        public VbWithPORequestViewModel GetViewModelToValidate()
        {
            return new VbWithPORequestViewModel()
            {
                VBNo = "VBNo",
                Date = DateTimeOffset.UtcNow,
                DateEstimate = DateTimeOffset.UtcNow,
                Unit = new Unit()
                {
                    Id = 0,
                    Code = "Code",
                    Name = "Name",
                },
                Items = new List<VbWithPORequestDetailViewModel>()
                {
                    new VbWithPORequestDetailViewModel()
                    {
                        no = "no",
                        unit = new Unit()
                        {
                            Id = 0,
                            Code = "Code",
                            Name = "Name",
                        },

                        Details = new List<VbWithPORequestDetailItemsViewModel>()
                        {
                            new VbWithPORequestDetailItemsViewModel()
                            {
                                Conversion = 123,
                                dealQuantity = 123,
                                dealUom  = new dealUom()
                                {
                                    _id = "id",
                                    unit = "unit"
                                },
                                defaultQuantity = 123,
                                defaultUom = new defaultUom()
                                {
                                    _id="id",
                                    unit = "unit"
                                },
                                priceBeforeTax = 123,
                                product = new Product_VB()
                                {
                                    _id = "id",
                                    code = "code",
                                    name = "name"
                                },
                                productRemark = "productRemark"
                            }
                        }
                    }
                }
            };
        }

        public VbWithPORequestViewModel GetViewModel()
        {
            return new VbWithPORequestViewModel()
            {
                VBNo = "VBNo",
                Date = DateTimeOffset.UtcNow,
                DateEstimate = DateTimeOffset.UtcNow,
                Unit = new Unit()
                {
                    Id = 1,
                    Code = "Code",
                    Name = "Name",
                },
                Items = new List<VbWithPORequestDetailViewModel>()
                {
                    new VbWithPORequestDetailViewModel()
                    {
                        no = "no",
                        unit = new Unit()
                        {
                            Id = 1,
                            Code = "Code",
                            Name = "Name",
                        },

                        Details = new List<VbWithPORequestDetailItemsViewModel>()
                        {
                            new VbWithPORequestDetailItemsViewModel()
                            {
                                Conversion = 123,
                                dealQuantity = 123,
                                dealUom  = new dealUom()
                                {
                                    _id = "id",
                                    unit = "unit"
                                },
                                defaultQuantity = 123,
                                defaultUom = new defaultUom()
                                {
                                    _id="id",
                                    unit = "unit"
                                },
                                priceBeforeTax = 123,
                                product = new Product_VB()
                                {
                                    _id = "id",
                                    code = "code",
                                    name = "name"
                                },
                                productRemark = "productRemark"
                            }
                        }
                    }
                }
            };
        }

        public VbRequestModel GetVbRequestModelToCreate()
        {
            return new VbRequestModel()
            {

                VBNo = "VBNo",
                Date = DateTimeOffset.UtcNow,
                DateEstimate = DateTimeOffset.UtcNow,
                UnitId = 1,
                UnitCode = "UnitCode",
                UnitName = "UnitName",
                CurrencyId = 1,
                CurrencyCode = "CurrencyCode",
                CurrencyRate = 123,
                CurrencySymbol = "CurrencySymbol",
                Amount = 123,
                Usage = "Usage",
                UnitLoad = "UnitLoad",
                //Status_Post = true,
                Apporve_Status = true,
                Complete_Status = true,
                VBRequestCategory = "VBRequestCategory",
                CreatedBy = "CreatedBy"

            };
        }

        public async Task<VbWithPORequestViewModel> GetCreatedData()
        {
            var viewmodel = GetViewModel();
            var model = GetVbRequestModelToCreate();
            await _service.CreateAsync(model, viewmodel);
            return await _service.ReadByIdAsync2(model.Id);
        }

        public async Task<VbRequestModel> GetTestData()
        {
            VbRequestModel vbRequest = GetVbRequestModelToCreate();
            var viewmodel = GetViewModel();
            await _service.CreateAsync(vbRequest, viewmodel);

            return vbRequest;
        }
    }
}
