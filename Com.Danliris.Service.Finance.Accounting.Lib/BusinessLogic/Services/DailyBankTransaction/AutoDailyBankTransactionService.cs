using System;
using System.Linq;
using System.Threading.Tasks;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.OthersExpenditureProofDocumentViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.DailyBankTransaction
{
    public class AutoDailyBankTransactionService : IAutoDailyBankTransactionService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDailyBankTransactionService _dailyBankTransactionService;

        public AutoDailyBankTransactionService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _dailyBankTransactionService = serviceProvider.GetService<IDailyBankTransactionService>();
        }

        public Task<int> AutoCreate(OthersExpenditureProofDocumentCreateUpdateViewModel viewModel, string documentNo)
        {
            // var items = viewModel.Items.Select(item => new ).ToList();
            var model = new DailyBankTransactionModel()
            {
                AccountBankId = viewModel.AccountBankId.GetValueOrDefault(),
                Date = viewModel.Date.GetValueOrDefault(),
                Nominal = viewModel.Items.Sum(item => item.Debit.GetValueOrDefault()),
                ReferenceNo = documentNo,
                ReferenceType = "Dokument Pengeluaran Bank Lain - Lain",
                SourceType = "Lain - Lain"
            };
            return _dailyBankTransactionService.CreateAsync(model);
            throw new System.NotImplementedException();
        }

        public Task<int> AutoDelete(string documentNo)
        {
            return _dailyBankTransactionService.DeleteByReferenceNoAsync(documentNo);
        }
    }
}