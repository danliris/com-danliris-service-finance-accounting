using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentFinance.MemorialDetail;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.GarmentFinance.MemorialDetail;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.AutoMapperProfiles.GarmentFinance.MemorialDetail
{
    public class GarmentFinanceMemorialDetailProfile : Profile
    {
        public GarmentFinanceMemorialDetailProfile()
        {
            CreateMap<GarmentFinanceMemorialDetailModel, GarmentFinanceMemorialDetailViewModel>()
                .ReverseMap();

            CreateMap<GarmentFinanceMemorialDetailItemModel, GarmentFinanceMemorialDetailItemViewModel>()
                .ForPath(d => d.Buyer.Id, opt => opt.MapFrom(s => s.BuyerId))
                .ForPath(d => d.Buyer.Code, opt => opt.MapFrom(s => s.BuyerCode))
                .ForPath(d => d.Buyer.Name, opt => opt.MapFrom(s => s.BuyerName))

                .ForPath(d => d.Currency.Id, opt => opt.MapFrom(s => s.CurrencyId))
                .ForPath(d => d.Currency.Code, opt => opt.MapFrom(s => s.CurrencyCode))
                .ForPath(d => d.Currency.Rate, opt => opt.MapFrom(s => s.CurrencyRate))
                .ReverseMap();

            CreateMap<GarmentFinanceMemorialDetailOtherItemModel, GarmentFinanceMemorialDetailOtherItemViewModel>()
                .ForPath(d => d.Currency.Id, opt => opt.MapFrom(s => s.CurrencyId))
                .ForPath(d => d.Currency.Code, opt => opt.MapFrom(s => s.CurrencyCode))
                .ForPath(d => d.Currency.Rate, opt => opt.MapFrom(s => s.CurrencyRate))

                .ForPath(d => d.Account.Id, opt => opt.MapFrom(s => s.ChartOfAccountId))
                .ForPath(d => d.Account.Code, opt => opt.MapFrom(s => s.ChartOfAccountCode))
                .ForPath(d => d.Account.Name, opt => opt.MapFrom(s => s.ChartOfAccountName))
                .ReverseMap();
        }
    }
}
