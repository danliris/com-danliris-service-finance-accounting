using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Migrations
{
    public partial class UpdateVBRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VBCode",
                table: "VbRequests",
                newName: "UnitName");

            migrationBuilder.AddColumn<string>(
                name: "UnitCode",
                table: "VbRequests",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "VbRequests",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitCode",
                table: "VbRequests");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "VbRequests");

            migrationBuilder.RenameColumn(
                name: "UnitName",
                table: "VbRequests",
                newName: "VBCode");
        }
    }
}
