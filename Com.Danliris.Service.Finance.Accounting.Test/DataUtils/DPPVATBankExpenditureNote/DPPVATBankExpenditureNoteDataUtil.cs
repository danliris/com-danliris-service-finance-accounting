using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.DPPVATBankExpenditureNote;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.DPPVATBankExpenditureNote;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.DPPVATBankExpenditureNote
{
  public  class DPPVATBankExpenditureNoteDataUtil
    {
        private readonly DPPVATBankExpenditureNoteService Service;

        public DPPVATBankExpenditureNoteDataUtil(DPPVATBankExpenditureNoteService service)
        {
            Service = service;
        }

        public FormDto GetNewForm()
        {
            return new FormDto
            {
                Amount=1,
                Bank=new AccountBankDto()
                {
                    AccountNumber= "AccountNumber",
                    BankCode= "BankCode",
                    BankName= "BankName",
                    Currency=new CurrencyDto()
                    {
                        Code= "IDR",
                        Rate=1
                    }
                },
                BGCheckNo= "BGCheckNo",
                Currency=new CurrencyDto()
                {
                    Code= "Code",
                    Rate=1
                },
                Date=DateTimeOffset.Now,
                
                Items=new List<FormItemDto>()
                {
                    new FormItemDto()
                    { 
                        OutstandingAmount=1,
                        Select=true,
                        InternalNote=new InternalNoteDto()
                        {
                            Id=1,
                            DocumentNo="1",
                            Date=DateTimeOffset.Now,
                            Currency=new CurrencyDto()
                            {
                                Code="Code",
                                Rate=1
                            },
                            DPP=1,
                            DueDate=DateTimeOffset.Now.AddDays(1),
                            IncomeTaxAmount=1,
                            Supplier=new SupplierDto()
                            {
                                Id=1,
                                Code="Code"
                            },
                            TotalAmount=1,
                            VATAmount=1,
                            Items=new List<InternalNoteInvoiceDto>()
                            {
                                new InternalNoteInvoiceDto()
                                {
                                    Id=1,
                                    Invoice=new InvoiceDto()
                                    {
                                        Id=1,
                                        BillsNo="1",
                                        Amount=1,
                                        Category=new CategoryDto(1,"CategoryName"),
                                        Date=DateTimeOffset.Now,
                                        DeliveryOrdersNo="1",
                                        DocumentNo="1",
                                        PaymentBills="PaymentBills",
                                        PaymentMethod="PaymentMethod",
                                        ProductNames="ProductNames"
                                    },
                                    SelectInvoice=true,
                                    
                                }
                            }
                        }
                    }

                },
                Supplier=new SupplierDto()
                {
                    Code="Code",
                    IsImport=false,
                    Name="Name"
                }
            };
        }

        public async Task<DPPVATBankExpenditureNoteDto> GetTestData()
        {
            FormDto form = GetNewForm();
           var id = await Service.Create(form);
            return  Service.Read(id);
        }
    }
}
