using Com.Danliris.Service.Finance.Accounting.Lib.Models.BudgetCashflow;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.CreditorAccount;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.DPPVATBankExpenditureNote;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentInvoicePayment;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentPurchasingExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.LockTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MasterCOA;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.Memo;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.OthersExpenditureProofDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.PaymentDispositionNote;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.PurchasingDispositionExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.SalesReceipt;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.VbNonPORequest;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRealizationDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRealizationDocumentExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRequestDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentPurchasingPphBankExpenditureNote;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentInvoicePurchasingDisposition;
using Com.Moonlay.Data.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentDebtBalance;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentDispositionExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MemoGarmentPurchasing;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.AccountingBook;

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
        public DbSet<JournalTransactionNumber> JournalTransactionNumbers { get; set; }

        public DbSet<LockTransactionModel> LockTransactions { get; set; }

        public DbSet<PurchasingDispositionExpeditionModel> PurchasingDispositionExpeditions { get; set; }
        public DbSet<PurchasingDispositionExpeditionItemModel> PurchasingDispositionExpeditionItems { get; set; }

        public DbSet<PaymentDispositionNoteModel> PaymentDispositionNotes { get; set; }
        public DbSet<PaymentDispositionNoteItemModel> PaymentDispositionNoteItems { get; set; }
        public DbSet<PaymentDispositionNoteDetailModel> PaymentDispositionNoteDetails { get; set; }

        public DbSet<OthersExpenditureProofDocumentModel> OthersExpenditureProofDocuments { get; set; }
        public DbSet<OthersExpenditureProofDocumentItemModel> OthersExpenditureProofDocumentItems { get; set; }

        public DbSet<SalesReceiptModel> SalesReceipts { get; set; }
        public DbSet<SalesReceiptDetailModel> SalesReceiptDetails { get; set; }

        public DbSet<MemoModel> Memos { get; set; }
        public DbSet<MemoItemModel> MemoItems { get; set; }

        public DbSet<DownPaymentModel> DownPayments { get; set; }

        public DbSet<VbRequestModel> VbRequests { get; set; }
        public DbSet<VbRequestDetailModel> VbRequestsDetails { get; set; }

        public DbSet<RealizationVbModel> RealizationVbs { get; set; }
        public DbSet<RealizationVbDetailModel> RealizationVbDetails { get; set; }

        public DbSet<VBRealizationDocumentExpeditionModel> VBRealizationDocumentExpeditions { get; set; }

        public DbSet<VBRequestDocumentModel> VBRequestDocuments { get; set; }
        public DbSet<VBRequestDocumentItemModel> VBRequestDocumentItems { get; set; }
        public DbSet<VBRequestDocumentEPODetailModel> VBRequestDocumentEPODetails { get; set; }

        public DbSet<GarmentInvoicePaymentModel> GarmentInvoicePayments { get; set; }
        public DbSet<GarmentInvoicePaymentItemModel> GarmentInvoicePaymentItems { get; set; }
        
        public DbSet<VBRealizationDocumentModel> VBRealizationDocuments { get; set; }
        public DbSet<VBRealizationDocumentExpenditureItemModel> VBRealizationDocumentExpenditureItems { get; set; }
        public DbSet<VBRealizationDocumentUnitCostsItemModel> VBRealizationDocumentUnitCostsItems { get; set; }

        public DbSet<GarmentPurchasingExpeditionModel> GarmentPurchasingExpeditions { get; set; }
        public DbSet<GarmentDispositionExpeditionModel> GarmentDispositionExpeditions { get; set; }

        public DbSet<BudgetCashflowTypeModel> BudgetCashflowTypes { get; set; }
        public DbSet<BudgetCashflowCategoryModel> BudgetCashflowCategories { get; set; }
        public DbSet<BudgetCashflowSubCategoryModel> BudgetCashflowSubCategories { get; set; }
        public DbSet<BudgetCashflowUnitModel> BudgetCashflowUnits { get; set; }
        public DbSet<InitialCashBalanceModel> InitialCashBalances { get; set; }
        public DbSet<RealCashBalanceModel> RealCashBalances { get; set; }
        
        public DbSet<DPPVATBankExpenditureNoteModel> DPPVATBankExpenditureNotes { get; set; }
        public DbSet<DPPVATBankExpenditureNoteItemModel> DPPVATBankExpenditureNoteItems { get; set; }
        public DbSet<DPPVATBankExpenditureNoteDetailModel> DPPVATBankExpenditureNoteDetails { get; set; }


        public DbSet<GarmentPurchasingPphBankExpenditureNoteModel> GarmentPurchasingPphBankExpenditureNotes { get; set; }
        public DbSet<GarmentPurchasingPphBankExpenditureNoteItemModel> GarmentPurchasingPphBankExpenditureNoteItems { get; set; }
        public DbSet<GarmentPurchasingPphBankExpenditureNoteInvoiceModel> GarmentPurchasingPphBankExpenditureNoteInvoices { get; set; }

        public DbSet<GarmentInvoicePurchasingDispositionModel> GarmentInvoicePurchasingDispositions { get; set; }
        public DbSet<GarmentInvoicePurchasingDispositionItemModel> GarmentInvoicePurchasingDispositionItems { get; set; }
        public DbSet<GarmentDebtBalanceModel> GarmentDebtBalances { get; set; }

        public DbSet<MemoGarmentPurchasingModel> MemoGarmentPurchasings { get; set; }
        public DbSet<MemoDetailGarmentPurchasing> MemoDetailGarmentPurchasings { get; set; }
        public DbSet<MemoDetailGarmentPurchasingDetail> MemoDetailGarmentPurchasingDetails { get; set; }

        public DbSet<AccountingBookModel> AccountingBooks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JournalTransactionItemModel>().Property(x => x.Debit).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<JournalTransactionItemModel>().Property(x => x.Credit).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<COAModel>().Property(x => x.Balance).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<VBRealizationDocumentModel>().Property(x => x.VBRequestDocumentAmount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<VBRealizationDocumentExpenditureItemModel>().Property(x => x.Amount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<VBRealizationDocumentUnitCostsItemModel>().Property(x => x.Amount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<VBRequestDocumentModel>().Property(x => x.Amount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<VBRealizationDocumentModel>().Property(x => x.Amount).HasColumnType("decimal(18,2)");
            //AccountingBook
            modelBuilder.Entity<AccountingBookModel>().HasKey(x => x.Id);
            modelBuilder.Entity<AccountingBookModel>()
                .HasIndex(x => x.Code)
                .IsUnique();

            modelBuilder.Entity<AccountingBookModel>()
                .HasIndex(x => x.AccountingBookType)
                .IsUnique();
            base.OnModelCreating(modelBuilder);
        }
    }
}
