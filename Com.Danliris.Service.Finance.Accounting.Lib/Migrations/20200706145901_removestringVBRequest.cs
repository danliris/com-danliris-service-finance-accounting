using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Migrations
{
    public partial class removestringVBRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Apporve_Status",
                table: "VbRequests");

            migrationBuilder.DropColumn(
                name: "Complete_Status",
                table: "VbRequests");

            migrationBuilder.DropColumn(
                name: "Status_Post",
                table: "VbRequests");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Apporve_Status",
                table: "VbRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Complete_Status",
                table: "VbRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Status_Post",
                table: "VbRequests",
                nullable: false,
                defaultValue: false);
        }
    }
}
