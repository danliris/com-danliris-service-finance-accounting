using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Migrations
{
    public partial class UpdateVBRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status_Post",
                table: "VbRequests");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateEstimate",
                table: "VbRequests",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateEstimate",
                table: "VbRequests");

            migrationBuilder.AddColumn<bool>(
                name: "Status_Post",
                table: "VbRequests",
                nullable: false,
                defaultValue: false);
        }
    }
}
