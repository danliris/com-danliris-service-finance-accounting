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



        public VBRequestDocumentModel GetTestData_VBRequestDocumentNonPO()
        {

            var testData = GetNewData_VBRequestDocumentNonPO();
            _dbContext.VBRequestDocuments.Add(testData);
            _dbContext.SaveChanges();
            return testData;
        }

        public VBRequestDocumentModel GetNewData_VBRequestDocumentNonPO()
        {
            VBRequestDocumentModel TestData = new VBRequestDocumentModel("documentNo", DateTimeOffset.Now, DateTimeOffset.Now, 1, "suppliantUnitCode", "suppliantUnitName", 1, "suppliantDivisionCode", "suppliantDivisionName", 1, "IDR", "Rp", "currencyDescription", 1, "Purpose", 1, true, true, true,VBType.NonPO);

            return TestData;
        }


        public VBRequestDocumentModel GetTestData_VBRequestDocumentWithPO()
        {

            var testData = GetNewData_VBRequestDocumentWithPO();
            _dbContext.VBRequestDocuments.Add(testData);
            _dbContext.SaveChanges();
            return testData;
        }

        public VBRequestDocumentModel GetNewData_VBRequestDocumentWithPO()
        {
            VBRequestDocumentModel TestData = new VBRequestDocumentModel("documentNo", DateTimeOffset.Now, DateTimeOffset.Now, 1, "suppliantUnitCode", "suppliantUnitName", 1, "suppliantDivisionCode", "suppliantDivisionName", 1, "IDR", "Rp", "currencyDescription", 1, "Purpose", 1, false, false, false, VBType.WithPO);

            return TestData;
        }

        public VBRequestDocumentItemModel GetNewData_VBRequestDocumentItemModel()
        {
            var vBRequestDocument = GetTestData_VBRequestDocumentNonPO();
            VBRequestDocumentItemModel TestData = new VBRequestDocumentItemModel(vBRequestDocument.Id, 1, "unitName", "unitCode", 1, "divisionName", "divisionCode", true, 1);
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

                           Items=new List<PurchaseOrderExternalItem>()
                           {
                               new PurchaseOrderExternalItem()
                               {
                                   IncomeTax=new IncomeTaxDto()
                                   {
                                       Name="IncomeTax",
                                       Rate=1
                                   },
                                   Unit=new UnitDto()
                                   {
                                       Code="Code",
                                       Name="Name",
                                       Division=new DivisionDto()
                                       {
                                           Code="Code",
                                           Name="Name",
                                       }
                                   },
                                   UseIncomeTax=true,
                                   IncomeTaxBy="IncomeTaxBy",
                                   Conversion=1,
                                   DealQuantity=1,
                                   DealUOM=new UnitOfMeasurement()
                                   {
                                       Unit="meter"
                                   },
                                   DefaultQuantity=1,
                                   Price=1,
                                   Product=new Product()
                                   {
                                       Name="ProdukName",
                                       UOM=new UnitOfMeasurement()
                                       {
                                           Unit="meter"
                                       },
                                       Code="code"
                                   },
                                   UseVat=true
                               }
                           },

                       }
                    }
                }
            };
        }

        public VBRequestDocumentEPODetailModel GetNewData_VBRequestDocumentEPODetailModel()
        {
            var vBRequestDocumentDataTest = GetTestData_VBRequestDocumentNonPO();

            VBRequestDocumentEPODetailModel newData = new VBRequestDocumentEPODetailModel(vBRequestDocumentDataTest.Id, 1, "epoNo", "remark");
            
            return newData;
        }

        public VBRequestDocumentEPODetailModel GetTestData_VBRequestDocumentEPODetail()
        {
            var VBRequestDocumentEPODetailDataTest = GetNewData_VBRequestDocumentEPODetailModel();

            _dbContext.VBRequestDocumentEPODetails.Add(VBRequestDocumentEPODetailDataTest);
            _dbContext.SaveChanges();

            return VBRequestDocumentEPODetailDataTest;
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
