using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.JournalTransaction;

namespace Com.Danliris.Service.Finance.Accounting.Lib.AutoMapperProfiles.JournalTransaction
{
    public class JournalTransactionProfile : Profile
    {
        public JournalTransactionProfile()
        {
            CreateMap<JournalTransactionModel, JournalTransactionViewModel>()
                .ReverseMap();

            CreateMap<JournalTransactionItemModel, JournalTransactionItemViewModel>()
                .ForPath(d => d.COA.Id, opt => opt.MapFrom(s => s.COAId))
                .ReverseMap();
        }
    }
}
