using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Migrations
{
    public partial class UpdateVBRequest3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Status_Post",
                table: "VbRequests",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Complete_Status",
                table: "VbRequests",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Apporve_Status",
                table: "VbRequests",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 255,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status_Post",
                table: "VbRequests",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<string>(
                name: "Complete_Status",
                table: "VbRequests",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<string>(
                name: "Apporve_Status",
                table: "VbRequests",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(bool));
        }
    }
}
