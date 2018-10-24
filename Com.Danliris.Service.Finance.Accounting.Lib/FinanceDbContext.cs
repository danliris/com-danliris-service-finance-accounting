using Com.Danliris.Service.Finance.Accounting.Lib.Models.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MasterCOA;
using Com.Moonlay.Data.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Com.Danliris.Service.Finance.Accounting.Lib
{
    public class FinanceDbContext : StandardDbContext
    {
        public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options)
        {
        }

        //Master
        public DbSet<COAModel> ChartsOfAccounts { get; set; }

        //Manager
        public DbSet<DailyBankTransactionModel> DailyBankTransactions { get; set; }
        public DbSet<BankTransactionMonthlyBalance> BankTransactionMonthlyBalances { get; set; }
    }
}
