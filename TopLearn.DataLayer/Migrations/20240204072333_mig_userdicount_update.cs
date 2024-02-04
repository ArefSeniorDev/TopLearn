using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TopLearn.DataLayer.Migrations
{
    public partial class mig_userdicount_update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDiscountCode_Discounts_DiscountId",
                table: "UserDiscountCode");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDiscountCode_Users_UserId",
                table: "UserDiscountCode");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDiscountCode",
                table: "UserDiscountCode");

            migrationBuilder.RenameTable(
                name: "UserDiscountCode",
                newName: "UserDiscountCodes");

            migrationBuilder.RenameIndex(
                name: "IX_UserDiscountCode_UserId",
                table: "UserDiscountCodes",
                newName: "IX_UserDiscountCodes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserDiscountCode_DiscountId",
                table: "UserDiscountCodes",
                newName: "IX_UserDiscountCodes_DiscountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDiscountCodes",
                table: "UserDiscountCodes",
                column: "UD_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDiscountCodes_Discounts_DiscountId",
                table: "UserDiscountCodes",
                column: "DiscountId",
                principalTable: "Discounts",
                principalColumn: "DiscountId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDiscountCodes_Users_UserId",
                table: "UserDiscountCodes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDiscountCodes_Discounts_DiscountId",
                table: "UserDiscountCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDiscountCodes_Users_UserId",
                table: "UserDiscountCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDiscountCodes",
                table: "UserDiscountCodes");

            migrationBuilder.RenameTable(
                name: "UserDiscountCodes",
                newName: "UserDiscountCode");

            migrationBuilder.RenameIndex(
                name: "IX_UserDiscountCodes_UserId",
                table: "UserDiscountCode",
                newName: "IX_UserDiscountCode_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserDiscountCodes_DiscountId",
                table: "UserDiscountCode",
                newName: "IX_UserDiscountCode_DiscountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDiscountCode",
                table: "UserDiscountCode",
                column: "UD_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDiscountCode_Discounts_DiscountId",
                table: "UserDiscountCode",
                column: "DiscountId",
                principalTable: "Discounts",
                principalColumn: "DiscountId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDiscountCode_Users_UserId",
                table: "UserDiscountCode",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
