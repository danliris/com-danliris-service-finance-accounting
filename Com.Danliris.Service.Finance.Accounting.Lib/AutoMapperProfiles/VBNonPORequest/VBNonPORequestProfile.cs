using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.AutoMapperProfiles.VBNonPORequest
{
    public class VBNonPORequestProfile: Profile
    {

        public VBNonPORequestProfile()
        {
            CreateMap<VbRequestModel, VbNonPORequestViewModel>()

                .ForPath(d => d.Currency.Id, opt => opt.MapFrom(s => s.CurrencyId))
                .ForPath(d => d.Currency.Code, opt => opt.MapFrom(s => s.CurrencyCode))
                .ForPath(d => d.Currency.Rate, opt => opt.MapFrom(s => s.CurrencyRate))
                .ForPath(d => d.Currency.Symbol, opt => opt.MapFrom(s => s.CurrencySymbol))

                .ReverseMap();
        }
    }
}
