using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Migrations
{
    public partial class AddSalesReceipId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesReceiptDetails_SalesReceipts_SalesReceiptModelId",
                table: "SalesReceiptDetails");

            migrationBuilder.DropIndex(
                name: "IX_SalesReceiptDetails_SalesReceiptModelId",
                table: "SalesReceiptDetails");

            migrationBuilder.DropColumn(
                name: "SalesReceiptModelId",
                table: "SalesReceiptDetails");

            migrationBuilder.AddColumn<int>(
                name: "SalesReceiptId",
                table: "SalesReceiptDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SalesReceiptDetails_SalesReceiptId",
                table: "SalesReceiptDetails",
                column: "SalesReceiptId");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesReceiptDetails_SalesReceipts_SalesReceiptId",
                table: "SalesReceiptDetails",
                column: "SalesReceiptId",
                principalTable: "SalesReceipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesReceiptDetails_SalesReceipts_SalesReceiptId",
                table: "SalesReceiptDetails");

            migrationBuilder.DropIndex(
                name: "IX_SalesReceiptDetails_SalesReceiptId",
                table: "SalesReceiptDetails");

            migrationBuilder.DropColumn(
                name: "SalesReceiptId",
                table: "SalesReceiptDetails");

            migrationBuilder.AddColumn<int>(
                name: "SalesReceiptModelId",
                table: "SalesReceiptDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesReceiptDetails_SalesReceiptModelId",
                table: "SalesReceiptDetails",
                column: "SalesReceiptModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesReceiptDetails_SalesReceipts_SalesReceiptModelId",
                table: "SalesReceiptDetails",
                column: "SalesReceiptModelId",
                principalTable: "SalesReceipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
