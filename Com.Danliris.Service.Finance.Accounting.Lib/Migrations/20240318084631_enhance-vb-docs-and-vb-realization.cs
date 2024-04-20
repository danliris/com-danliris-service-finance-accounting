using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Migrations
{
    public partial class enhancevbdocsandvbrealization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "VBRequestDocuments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "VBRequestDocuments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TakenBy",
                table: "VBRequestDocuments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "VBRealizationDocuments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "VBRealizationDocuments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TakenBy",
                table: "VBRealizationDocuments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "VBRequestDocuments");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "VBRequestDocuments");

            migrationBuilder.DropColumn(
                name: "TakenBy",
                table: "VBRequestDocuments");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "VBRealizationDocuments");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "VBRealizationDocuments");

            migrationBuilder.DropColumn(
                name: "TakenBy",
                table: "VBRealizationDocuments");
        }
    }
}
