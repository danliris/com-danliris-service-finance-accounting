using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentPurchasingPphBankExpenditureNote;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.GarmentPurchasingPphBankExpenditureNoteViewModels;

namespace Com.Danliris.Service.Finance.Accounting.Lib.AutoMapperProfiles.GarmentPurchasingExpenditureNote
{
    public class GarmentPurchasingExpenditureNoteProfile : Profile
    {
        public GarmentPurchasingExpenditureNoteProfile()
        {
            CreateMap<FormInsert,GarmentPurchasingPphBankExpenditureNoteModel>()
                .ForPath(d=> d.)

                string InvoiceOutNumber { get; set; }
                DateTimeOffset InvoiceOutDate { get; set; }
                DateTimeOffset DueDateStart { get; set; }
                DateTimeOffset DueDateEnd { get; set; }
                int IncomeTaxId { get; set; }
                string IncomeTaxName { get; set; }
                double IncomeTaxRate { get; set; }
                string AccountBankCOA { get; set; }
                string AccountBankName { get; set; }
                string AccountBankNumber { get; set; }
                string BankAddress { get; set; }
                string BankCode { get; set; }
                string BankName { get; set; }
                string BankCode1 { get; set; }
                string BankCurrencyCode { get; set; }
                int BankCurrencyId { get; set; }
                string BankSwiftCode { get; set; }
                bool IsPosted { get; set; }
    }
    }
}
