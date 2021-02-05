using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.ClearaceVB;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.ClearaceVB;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRequestDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.ClearaceVB;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.ClearaceVB
{
    public class ClearaceVBDataUtil
    {
        private ClearaceVBService service;

        public ClearaceVBDataUtil(ClearaceVBService service)
        {
            this.service = service;
        }
        public VBRequestDocumentModel GetNewData()
        {
            VBRequestDocumentModel TestData = new VBRequestDocumentModel()
            {
                Id = 1,
                
                CreatedBy = "CreatedBy",
            };

            return TestData;
        }

        public virtual async Task<VBRequestDocumentModel> GetTestData()
        {
            VBRequestDocumentModel model = GetNewData();
            await service.CreateAsync(model);
            return model;
        }

        public ClearenceFormDto GetNewData_ClearenceFormDto()
        {
            return new ClearenceFormDto()
            {
                Bank=new AccountBankViewModel()
                {
                    AccountCOA= "AccountCOA",
                    AccountName= "AccountName",
                    AccountNumber= "AccountNumber",
                    BankCode= "BankCode",
                    BankName= "BankName",
                    Code= "Code",
                    Currency=new CurrencyViewModel()
                    {
                        Code="Code",
                        Description= "Description",
                        Rate=1,
                        Symbol="IDR"
                    },
                    

                },
                ListIds=new List<ClearencePostId>()
                {
                    new ClearencePostId()
                    {
                        VBRealizationId=1,
                        VBRequestId=1,
                    }
                },
                
            };
        }

        public ClearaceVBViewModel GetNewData_ClearaceVBViewModel()
        {
            return new ClearaceVBViewModel()
            {
                Appliciant= "Appliciant",
                ClearanceDate=DateTimeOffset.Now,
                CurrencyCode="IDR",
                DiffAmount=1,
                DiffStatus="",
                IsPosted=false,
                RealDate=DateTimeOffset.Now,
                RealNo= "RealNo",
                RqstDate=DateTimeOffset.Now,
                RqstNo= "RqstNo",
                Status="",
                Unit=new Unit()
                {
                    Code="Code"
                },
                VBCategory= Lib.BusinessLogic.VBRequestDocument.VBType.NonPO,
                VerDate=DateTimeOffset.Now
            };
        }
    }
}
