using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Migrations
{
    public partial class add_column_posted_in_VbRealizationDocumentExpeditionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CleranceDate",
                table: "VBRealizationDocumentExpeditions",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPosted",
                table: "VBRealizationDocumentExpeditions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PostedBy",
                table: "VBRealizationDocumentExpeditions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CleranceDate",
                table: "VBRealizationDocumentExpeditions");

            migrationBuilder.DropColumn(
                name: "IsPosted",
                table: "VBRealizationDocumentExpeditions");

            migrationBuilder.DropColumn(
                name: "PostedBy",
                table: "VBRealizationDocumentExpeditions");
        }
    }
}
