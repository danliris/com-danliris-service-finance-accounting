using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Migrations
{
    public partial class UpdateVBRequest2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Conversion",
                table: "VbRequestsDetails",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DealQuantity",
                table: "VbRequestsDetails",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "DealUOMId",
                table: "VbRequestsDetails",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DealUOMUnit",
                table: "VbRequestsDetails",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultQuantity",
                table: "VbRequestsDetails",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "DefaultUOMId",
                table: "VbRequestsDetails",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefaultUOMUnit",
                table: "VbRequestsDetails",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "VbRequestsDetails",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ProductCode",
                table: "VbRequestsDetails",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductId",
                table: "VbRequestsDetails",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "VbRequestsDetails",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductRemark",
                table: "VbRequestsDetails",
                maxLength: 64,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Conversion",
                table: "VbRequestsDetails");

            migrationBuilder.DropColumn(
                name: "DealQuantity",
                table: "VbRequestsDetails");

            migrationBuilder.DropColumn(
                name: "DealUOMId",
                table: "VbRequestsDetails");

            migrationBuilder.DropColumn(
                name: "DealUOMUnit",
                table: "VbRequestsDetails");

            migrationBuilder.DropColumn(
                name: "DefaultQuantity",
                table: "VbRequestsDetails");

            migrationBuilder.DropColumn(
                name: "DefaultUOMId",
                table: "VbRequestsDetails");

            migrationBuilder.DropColumn(
                name: "DefaultUOMUnit",
                table: "VbRequestsDetails");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "VbRequestsDetails");

            migrationBuilder.DropColumn(
                name: "ProductCode",
                table: "VbRequestsDetails");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "VbRequestsDetails");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "VbRequestsDetails");

            migrationBuilder.DropColumn(
                name: "ProductRemark",
                table: "VbRequestsDetails");
        }
    }
}
