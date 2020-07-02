using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Migrations
{
    public partial class VbNonPoRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VbRequests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
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
                    Date = table.Column<DateTimeOffset>(nullable: false),
                    VBCode = table.Column<string>(maxLength: 64, nullable: true),
                    CurrencyId = table.Column<int>(nullable: false),
                    CurrencyCode = table.Column<string>(maxLength: 64, nullable: true),
                    CurrencyRate = table.Column<decimal>(nullable: false),
                    CurrencySymbol = table.Column<string>(maxLength: 64, nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    Usage = table.Column<string>(maxLength: 255, nullable: true),
                    Spinning1 = table.Column<bool>(nullable: false),
                    Spinning2 = table.Column<bool>(nullable: false),
                    Spinning3 = table.Column<bool>(nullable: false),
                    Weaving1 = table.Column<bool>(nullable: false),
                    Weaving2 = table.Column<bool>(nullable: false),
                    Finishing = table.Column<bool>(nullable: false),
                    Printing = table.Column<bool>(nullable: false),
                    Konfeksi1A = table.Column<bool>(nullable: false),
                    Konfeksi1B = table.Column<bool>(nullable: false),
                    Konfeksi2A = table.Column<bool>(nullable: false),
                    Konfeksi2B = table.Column<bool>(nullable: false),
                    Konfeksi2C = table.Column<bool>(nullable: false),
                    Umum = table.Column<bool>(nullable: false),
                    Others = table.Column<bool>(nullable: false),
                    DetailOthers = table.Column<string>(maxLength: 255, nullable: true),
                    UnitLoad = table.Column<string>(maxLength: 255, nullable: true),
                    Stauts_Post = table.Column<string>(maxLength: 255, nullable: true),
                    Apporve_Status = table.Column<string>(maxLength: 255, nullable: true),
                    Complete_Status = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VbRequests", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VbRequests");
        }
    }
}
