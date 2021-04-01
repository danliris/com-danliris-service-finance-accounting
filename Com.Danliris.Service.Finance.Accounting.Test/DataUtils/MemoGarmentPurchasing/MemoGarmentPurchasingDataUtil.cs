using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.MemoGarmentPurchasing;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.MemoGarmentPurchasing;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MemoGarmentPurchasing;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.MemoDetailGarmentPurchasing;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.MemoGarmentPurchasing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.MemoGarmentPurchasing
{
    public class MemoGarmentPurchasingDataUtil
    {
        private readonly MemoGarmentPurchasingService _service;

        public MemoGarmentPurchasingDataUtil(MemoGarmentPurchasingService service)
        {
            _service = service;
        }

        public MemoGarmentPurchasingViewModel GetViewModelToValidate(){
            return new MemoGarmentPurchasingViewModel
            {
                Id = 0,
                AccountingBook = new AccountingBookViewModel() { 
                
                },
                Currency = new GarmentCurrencyViewModel() { 
                
                },
                IsPosted =false,
                MemoDate = DateTime.Now,
                MemoNo = "",
                Remarks ="",
                MemoGarmentPurchasingDetails = new List<MemoGarmentPurchasingDetailViewModel>( )
                {
                    new MemoGarmentPurchasingDetailViewModel()
                    {
                        COA = new COAViewModel()
                        {
                            Id = 0,
                            Name ="",
                            No = ""
                        },
                        CreditNominal = 0,
                        DebitNominal = 0
                       
                    }
                }
                 
            };
        }

        public MemoGarmentPurchasingViewModel GetViewModelToCreate(){
            return new MemoGarmentPurchasingViewModel()
            {
                AccountingBook = new AccountingBookViewModel()
                {
                    Code = "12",
                    Id = 1,
                    Type ="PembelianLokal"
                },
                Currency = new GarmentCurrencyViewModel()
                {
                    Id =1,
                    Code = "UT",
                    Rate = 10000
                },
                Id = 1,
                IsPosted = true,
                MemoDate =DateTime.Now,
                MemoNo = "12-345-01",
                Remarks ="UT",
                MemoGarmentPurchasingDetails = new List<MemoGarmentPurchasingDetailViewModel>()
                {
                    new MemoGarmentPurchasingDetailViewModel
                    {
                        COA = new COAViewModel{ 
                        Id = 1,
                        Name = "Asset",
                        No = "12-345-01"
                        }
                    }
                }
            };
        }

        public MemoGarmentPurchasingModel GetModelToCreate()
        {
            return new MemoGarmentPurchasingModel()
            {
                Id = 1,
                MemoNo = "12-345-01",
                MemoDate = DateTime.Now,
                AccountingBookId = 1,
                AccountingBookCode = "UT",
                AccountingBookType = "UnitTest",
                GarmentCurrenciesId =1,
                GarmentCurrenciesCode = "UT",
                GarmentCurrenciesRate = 100000,
                Remarks ="UT",
                IsPosted = false,                
                MemoGarmentPurchasingDetails = new List<MemoGarmentPurchasingDetailModel>()
                {
                     new MemoGarmentPurchasingDetailModel()
                     {
                         Active = true,
                         COAId = 1,
                         COAName = "UnitTest",
                         COANo ="1",
                         CreditNominal =100000,
                         DebitNominal=1000000,
                         Id= 1,
                         MemoId = 1
                     }
                }
                
            };
        }

        public async Task<MemoGarmentPurchasingModel> GetCreatedData()
        {
            var model = GetModelToCreate();
            await _service.CreateAsync(model);
            return await _service.ReadByIdAsync(model.Id);
        }
    }
}
