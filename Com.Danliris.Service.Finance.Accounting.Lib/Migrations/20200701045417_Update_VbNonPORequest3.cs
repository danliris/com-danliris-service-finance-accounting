using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Migrations
{
    public partial class Update_VbNonPORequest3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DetailOthers",
                table: "VbRequests");

            migrationBuilder.DropColumn(
                name: "Finishing",
                table: "VbRequests");

            migrationBuilder.DropColumn(
                name: "Konfeksi1A",
                table: "VbRequests");

            migrationBuilder.DropColumn(
                name: "Konfeksi1B",
                table: "VbRequests");

            migrationBuilder.DropColumn(
                name: "Konfeksi2A",
                table: "VbRequests");

            migrationBuilder.DropColumn(
                name: "Konfeksi2B",
                table: "VbRequests");

            migrationBuilder.DropColumn(
                name: "Konfeksi2C",
                table: "VbRequests");

            migrationBuilder.DropColumn(
                name: "Others",
                table: "VbRequests");

            migrationBuilder.DropColumn(
                name: "Printing",
                table: "VbRequests");

            migrationBuilder.DropColumn(
                name: "Spinning1",
                table: "VbRequests");

            migrationBuilder.DropColumn(
                name: "Spinning2",
                table: "VbRequests");

            migrationBuilder.DropColumn(
                name: "Spinning3",
                table: "VbRequests");

            migrationBuilder.DropColumn(
                name: "Umum",
                table: "VbRequests");

            migrationBuilder.DropColumn(
                name: "Weaving1",
                table: "VbRequests");

            migrationBuilder.DropColumn(
                name: "Weaving2",
                table: "VbRequests");

            migrationBuilder.CreateTable(
                name: "VbRequestDetailModel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeletedUtc = table.Column<DateTime>(nullable: false),
                    DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    VBNo = table.Column<string>(maxLength: 64, nullable: true),
                    POId = table.Column<int>(nullable: false),
                    PONo = table.Column<string>(maxLength: 64, nullable: true),
                    UnitId = table.Column<int>(nullable: false),
                    UnitName = table.Column<string>(maxLength: 64, nullable: true),
                    DetailOthers = table.Column<string>(maxLength: 64, nullable: true),
                    VBId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VbRequestDetailModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VbRequestDetailModel_VbRequests_Id",
                        column: x => x.Id,
                        principalTable: "VbRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VbRequestDetailModel");

            migrationBuilder.AddColumn<string>(
                name: "DetailOthers",
                table: "VbRequests",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Finishing",
                table: "VbRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Konfeksi1A",
                table: "VbRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Konfeksi1B",
                table: "VbRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Konfeksi2A",
                table: "VbRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Konfeksi2B",
                table: "VbRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Konfeksi2C",
                table: "VbRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Others",
                table: "VbRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Printing",
                table: "VbRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Spinning1",
                table: "VbRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Spinning2",
                table: "VbRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Spinning3",
                table: "VbRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Umum",
                table: "VbRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Weaving1",
                table: "VbRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Weaving2",
                table: "VbRequests",
                nullable: false,
                defaultValue: false);
        }
    }
}
