using System;
using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MemoGarmentPurchasing;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.MemoDetailGarmentPurchasing;

namespace Com.Danliris.Service.Finance.Accounting.Lib.AutoMapperProfiles.MemoDetailGarmentPurchasing
{
    public class MemoDetailGarmentPurchasingProfile: Profile
    {
        public MemoDetailGarmentPurchasingProfile()
        {
            CreateMap<MemoDetailGarmentPurchasingModel, MemoDetailGarmentPurchasingViewModel>()
                .ForPath(d => d.MemoId, opt => opt.MapFrom(s => s.MemoId))
                .ForPath(d => d.Remarks, opt => opt.MapFrom(s => s.Remarks))
                .ForPath(d => d.IsPosted, opt => opt.MapFrom(s => s.IsPosted))

                .ReverseMap();

            CreateMap<MemoDetailGarmentPurchasingDetailModel, MemoDetailGarmentPurchasingDetailViewModel>()
                .ForPath(d => d.GarmentDeliveryOrderId, opt => opt.MapFrom(s => s.GarmentDeliveryOrderId))
                .ForPath(d => d.GarmentDeliveryOrderNo, opt => opt.MapFrom(s => s.GarmentDeliveryOrderNo))
                .ForPath(d => d.RemarksDetail, opt => opt.MapFrom(s => s.RemarksDetail))
                .ForPath(d => d.PaymentRate, opt => opt.MapFrom(s => s.PaymentRate))
                .ForPath(d => d.PurchasingRate, opt => opt.MapFrom(s => s.PurchasingRate))
                .ForPath(d => d.MemoAmount, opt => opt.MapFrom(s => s.MemoAmount))
                .ForPath(d => d.MemoIdrAmount, opt => opt.MapFrom(s => s.MemoIdrAmount))

                .ReverseMap();
        }
    }
}
