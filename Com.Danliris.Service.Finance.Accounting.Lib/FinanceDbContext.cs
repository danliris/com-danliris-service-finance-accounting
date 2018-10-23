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
    }
}
