using Com.Danliris.Service.Finance.Accounting.Lib.Models.CreditorAccount;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MasterCOA;
using Com.Moonlay.Data.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib
{
    public class FinanceDbContext : StandardDbContext
    {
        public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options)
        {
        }

        public DbSet<COAModel> ChartsOfAccounts { get; set; }

        public DbSet<CreditorAccountModel> CreditorAccounts { get; set; }

        public DbSet<DailyBankTransactionModel> DailyBankTransactions { get; set; }
        public DbSet<BankTransactionMonthlyBalanceModel> BankTransactionMonthlyBalances { get; set; }

        public DbSet<JournalTransactionModel> JournalTransactions { get; set; }
        public DbSet<JournalTransactionItemModel> JournalTransactionItems { get; set; }

    }
}
