﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TopLearn.DataLayer.Migrations
{
    public partial class Mig_update_discount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Percent",
                table: "Discounts",
                newName: "DiscountPercent");

            migrationBuilder.RenameColumn(
                name: "DiscountCount",
                table: "Discounts",
                newName: "DiscountCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiscountPercent",
                table: "Discounts",
                newName: "Percent");

            migrationBuilder.RenameColumn(
                name: "DiscountCode",
                table: "Discounts",
                newName: "DiscountCount");
        }
    }
}
