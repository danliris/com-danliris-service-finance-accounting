using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.AutoMapperProfiles.VBWithPORequest
{
    public class VBWithPORequestProfile : Profile
    {
        public VBWithPORequestProfile()
        {
            CreateMap<VbRequestModel, VbWithPORequestViewModel>()

                .ForPath(d => d.Unit.Id, opt => opt.MapFrom(s => s.UnitId))
                .ForPath(d => d.Unit.Code, opt => opt.MapFrom(s => s.UnitCode))    
                .ForPath(d => d.Unit.Name, opt => opt.MapFrom(s => s.UnitName))

                .ReverseMap();
        }
    }
}
