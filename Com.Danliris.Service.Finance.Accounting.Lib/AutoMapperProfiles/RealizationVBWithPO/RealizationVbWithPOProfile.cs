using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.RealizationVBWIthPO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.AutoMapperProfiles.RealizationVBWithPO
{
    public class RealizationVBWithPOProfile : Profile
    {
        public RealizationVBWithPOProfile()
        {
            CreateMap<RealizationVbModel, RealizationVbWithPOViewModel>()

            .ReverseMap();
        }
    }
}
