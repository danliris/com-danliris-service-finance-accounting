using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Migrations
{
    public partial class AddColumnBankCasNoBankExpenditureAndInvoicePurchasingDispositionAndPaymentDispoNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankCashNo",
                table: "PaymentDispositionNotes",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankCashNo",
                table: "GarmentInvoicePurchasingDispositions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankCashNo",
                table: "DPPVATBankExpenditureNotes",
                maxLength: 32,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankCashNo",
                table: "PaymentDispositionNotes");

            migrationBuilder.DropColumn(
                name: "BankCashNo",
                table: "GarmentInvoicePurchasingDispositions");

            migrationBuilder.DropColumn(
                name: "BankCashNo",
                table: "DPPVATBankExpenditureNotes");
        }
    }
}
