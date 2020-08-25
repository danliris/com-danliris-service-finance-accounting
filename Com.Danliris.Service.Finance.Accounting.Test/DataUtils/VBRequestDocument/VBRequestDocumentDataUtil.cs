using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRequestDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRequestDocument;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.VBRequestDocument
{
    public class VBRequestDocumentDataUtil
    {
        private readonly VBRequestDocumentService _service;
        private FinanceDbContext _dbContext;
        public VBRequestDocumentDataUtil(VBRequestDocumentService service, FinanceDbContext dbContext)
        {
            _service = service;
            _dbContext = dbContext;
        }

       

        public VBRequestDocumentModel GetTestData_VBRequestDocument()
        {

            var testData = GetNewData_VBRequestDocument();
            _dbContext.VBRequestDocuments.Add(testData);
            _dbContext.SaveChanges();
            return testData;
        }

        public VBRequestDocumentModel GetNewData_VBRequestDocument()
        {
            VBRequestDocumentModel TestData = new VBRequestDocumentModel("documentNo", DateTimeOffset.Now, DateTimeOffset.Now, 1, "suppliantUnitCode", "suppliantUnitName", 1, "suppliantDivisionCode", "suppliantDivisionName", 1, "IDR", "Rp", "currencyDescription", 1, "Purpose", 1, true, true, true, new VBType());

            return TestData;
        }


        public VBRequestDocumentItemModel GetNewData_VBRequestDocumentItemModel()
        {
            var vBRequestDocument = GetTestData_VBRequestDocument();
            VBRequestDocumentItemModel TestData = new VBRequestDocumentItemModel(vBRequestDocument.Id,1,"unitName","unitCode",1,"divisionName","divisionCode",1,"epoNo",true,1,"incomeTaxName",1,"incomeTaxBy",1,true,1);

            return TestData;
        }

        public VBRequestDocumentItemModel GetTestData_VBRequestDocumentItem()
        {

            var testData = GetNewData_VBRequestDocumentItemModel();
            _dbContext.VBRequestDocumentItems.Add(testData);
            _dbContext.SaveChanges();
            return testData;
        }

        public VBRequestDocumentNonPOFormDto GetNewData_VBRequestDocumentNonPOFormDto()
        {
            VBRequestDocumentNonPOFormDto form = new VBRequestDocumentNonPOFormDto()
            {
                Amount = 1,
                Currency = new CurrencyDto()
                {
                    Code = "IDR",
                    Description = "Description",
                    Rate = 1,
                    Symbol = "Rp"
                },
                Date = DateTimeOffset.Now,
                Purpose = "Purpose",
                RealizationEstimationDate = DateTimeOffset.Now,
                SuppliantUnit = new UnitDto()
                {
                    Code = "Code",
                    Division = new DivisionDto()
                    {
                        Code = "Code",
                        Name = "GARMENT"
                    },
                    Name = "Meter",
                    VBDocumentLayoutOrder = 1
                },
                Items = new List<VBRequestDocumentNonPOItemFormDto>()
                {
                    new VBRequestDocumentNonPOItemFormDto()
                    {
                        IsSelected=true,
                        Unit=new UnitDto()
                        {
                            Code="Code",
                            Division = new DivisionDto()
                            {
                                Code = "Code",
                                Name = "Name",
                                
                            },
                            Name = "Name",
                            VBDocumentLayoutOrder = 1
                        }
                    }
                }
            };
            return form;
        }

        public VBRequestDocumentWithPOFormDto GetNewData_VBRequestDocumentWithPOFormDto()
        {
            return new VBRequestDocumentWithPOFormDto()
            {
                Amount = 1,
                Currency = new CurrencyDto()
                {
                    Code = "IDR",
                    Description = "Description",
                    Rate = 1,
                    Symbol = "Rp"
                },
                Date = DateTimeOffset.Now,
                Purpose = "Purpose",
                RealizationEstimationDate = DateTimeOffset.Now,
                SuppliantUnit = new UnitDto()
                {
                    Code = "Code",
                    Division = new DivisionDto()
                    {
                        Code = "Code",
                        Name = "GARMENT"
                    },
                    Name = "Meter",
                    VBDocumentLayoutOrder = 1
                },
                
                Items = new List<VBRequestDocumentWithPOItemFormDto>()
                {
                    new VBRequestDocumentWithPOItemFormDto()
                    {
                       
                       PurchaseOrderExternal=new PurchaseOrderExternal()
                       {
                           incomeTax=new IncomeTaxDto()
                           {
                               name="name",
                               rate=1
                           },
                           
                           incomeTaxBy="incomeTaxBy",
                           no="1",
                           unit=new OldUnitDto()
                           {
                               
                               code="code",
                               division =new OldDivisionDto()
                               {
                                   code="code",
                                   name="name"
                               },
                               name="name"
                           },
                           useIncomeTax=true,
                           useVat=true,
                           Details=new List<PurchaseOrderExternalItem>()
                           {
                               new PurchaseOrderExternalItem()
                               {
                                   
                                   Conversion=1,
                                   DealQuantity=1,
                                   DealUOM=new UnitOfMeasurement()
                                   {
                                       unit="meter" 
                                   },
                                   DefaultQuantity=1,
                                   Price=1,
                                   Product=new Product()
                                   {
                                       name="ProdukName",
                                       uom=new UnitOfMeasurement()
                                       {
                                           unit="meter"
                                       },
                                       code="code"
                                   },
                                   UseVat=true
                               }
                           },
                           
                       }
                    }
                }
            };
        }
      //  VBRequestDocumentItems

        //public async Task<VBRequestDocumentModel> GetTestData()
        //{
        //    VBRequestDocumentModel model = GetNewData();
        //    await Service.CreateNonPO()
        //    return await Service.ReadByIdAsync(model.Id);
        //}
    }
}
