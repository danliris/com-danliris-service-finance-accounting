using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Migrations
{
    public partial class UpdateVbRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VbRequestsDetails_VbRequests_Id",
                table: "VbRequestsDetails");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "VbRequestsDetails",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.CreateIndex(
                name: "IX_VbRequestsDetails_VBId",
                table: "VbRequestsDetails",
                column: "VBId");

            migrationBuilder.AddForeignKey(
                name: "FK_VbRequestsDetails_VbRequests_VBId",
                table: "VbRequestsDetails",
                column: "VBId",
                principalTable: "VbRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VbRequestsDetails_VbRequests_VBId",
                table: "VbRequestsDetails");

            migrationBuilder.DropIndex(
                name: "IX_VbRequestsDetails_VBId",
                table: "VbRequestsDetails");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "VbRequestsDetails",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_VbRequestsDetails_VbRequests_Id",
                table: "VbRequestsDetails",
                column: "Id",
                principalTable: "VbRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
