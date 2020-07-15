using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.RealizationVBNonPO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.AutoMapperProfiles.RealizationVBNonPO
{
    public class RealizationVbNonPOProfile : Profile
    {
        public RealizationVbNonPOProfile()
        {
            CreateMap<RealizationVbModel, RealizationVbNonPOViewModel>()
                .ForPath(d => d.numberVB.VBNo, opt => opt.MapFrom(s => s.VBNo))
                .ForPath(d => d.numberVB.DateEstimate, opt => opt.MapFrom(s => s.DateEstimate))
                .ForPath(d => d.numberVB.CreateBy, opt => opt.MapFrom(s => s.RequestVbName))
                .ForPath(d => d.numberVB.UnitCode, opt => opt.MapFrom(s => s.UnitCode))
                .ForPath(d => d.numberVB.UnitName, opt => opt.MapFrom(s => s.UnitName))
                .ForPath(d => d.numberVB.Date, opt => opt.MapFrom(s => s.DateVB))
                .ForPath(d => d.numberVB.UnitLoad, opt => opt.MapFrom(s => s.UnitLoad))
                .ForPath(d => d.numberVB.Amount, opt => opt.MapFrom(s => s.AmountNonPO))
                .ForPath(d => d.numberVB.CurrencyCode, opt => opt.MapFrom(s => s.CurrencyCodeNonPO))
                .ForPath(d => d.numberVB.CurrencyRate, opt => opt.MapFrom(s => s.CurrencyRateNonPO))
                .ForPath(d => d.numberVB.UnitLoad, opt => opt.MapFrom(s => s.UnitLoad))
                .ForPath(d => d.numberVB.VBRequestCategory, opt => opt.MapFrom(s => s.VBRealizeCategory))
            .ReverseMap();

            CreateMap<RealizationVbDetailModel, VbNonPORequestDetailViewModel>()
                
            .ReverseMap();
        }
    }
}
