using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TopLearn.DataLayer.Migrations
{
    public partial class Mig_order_discount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UsedDisCount",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsedDisCount",
                table: "Orders");
        }
    }
}
