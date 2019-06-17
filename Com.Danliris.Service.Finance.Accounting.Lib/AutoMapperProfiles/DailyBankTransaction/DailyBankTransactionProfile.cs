using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.DailyBankTransaction;

namespace Com.Danliris.Service.Finance.Accounting.Lib.AutoMapperProfiles.DailyBankTransaction
{
    public class DailyBankTransactionProfile : Profile
    {
        public DailyBankTransactionProfile()
        {
            CreateMap<DailyBankTransactionModel, DailyBankTransactionViewModel>()
                .ForPath(d => d.Date, opt => opt.MapFrom(s => s.Date))
                /* Bank */
                .ForPath(d => d.Bank.Id, opt => opt.MapFrom(s => s.AccountBankId))
                .ForPath(d => d.Bank.BankCode, opt => opt.MapFrom(s => s.AccountBankCode))
                .ForPath(d => d.Bank.AccountName, opt => opt.MapFrom(s => s.AccountBankAccountName))
                .ForPath(d => d.Bank.AccountNumber, opt => opt.MapFrom(s => s.AccountBankAccountNumber))
                .ForPath(d => d.Bank.BankName, opt => opt.MapFrom(s => s.AccountBankName))
                .ForPath(d => d.Bank.Currency.Id, opt => opt.MapFrom(s => s.AccountBankCurrencyId))
                .ForPath(d => d.Bank.Currency.Code, opt => opt.MapFrom(s => s.AccountBankCurrencyCode))
                .ForPath(d => d.Bank.Currency.Symbol, opt => opt.MapFrom(s => s.AccountBankCurrencySymbol))

                /* Supplier */
                .ForPath(d => d.Supplier._id, opt => opt.MapFrom(s => s.SupplierId))
                .ForPath(d => d.Supplier.code, opt => opt.MapFrom(s => s.SupplierCode))
                .ForPath(d => d.Supplier.name, opt => opt.MapFrom(s => s.SupplierName))

                /* Buyer */
                .ForPath(d => d.Buyer.Id, opt => opt.MapFrom(s => s.BuyerId))
                .ForPath(d => d.Buyer.Code, opt => opt.MapFrom(s => s.BuyerCode))
                .ForPath(d => d.Buyer.Name, opt => opt.MapFrom(s => s.BuyerName))

                .ReverseMap();
        }
    }
}
