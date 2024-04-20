using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Migrations
{
    public partial class AddColumnVBRealizationExpedition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "VBRealizationDocumentExpeditions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "VBRealizationDocumentExpeditions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TakenBy",
                table: "VBRealizationDocumentExpeditions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "VBRealizationDocumentExpeditions");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "VBRealizationDocumentExpeditions");

            migrationBuilder.DropColumn(
                name: "TakenBy",
                table: "VBRealizationDocumentExpeditions");
        }
    }
}
