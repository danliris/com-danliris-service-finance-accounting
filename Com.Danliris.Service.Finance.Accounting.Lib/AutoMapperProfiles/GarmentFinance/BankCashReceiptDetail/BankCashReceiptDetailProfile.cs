using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentFinance.BankCashReceiptDetail;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.GarmentFinance.BankCashReceiptDetail;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.AutoMapperProfiles.GarmentFinance.BankCashReceiptDetail
{
    public class BankCashReceiptDetailProfile : Profile
    {
        public BankCashReceiptDetailProfile()
        {
            CreateMap<BankCashReceiptDetailModel, BankCashReceiptDetailViewModel>()
                .ReverseMap();

            CreateMap<BankCashReceiptDetailItemModel, BankCashReceiptDetailItemViewModel>()
                .ReverseMap();
        }
    }
}
