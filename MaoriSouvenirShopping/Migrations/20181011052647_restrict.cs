using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MaoriSouvenirShopping.Migrations
{
    public partial class restrict : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropForeignKey(
                name: "FK_Souvenir_Category_CategoryID",
                table: "Souvenir");

            migrationBuilder.DropForeignKey(
                name: "FK_Souvenir_Supplier_SupplierID",
                table: "Souvenir");

            migrationBuilder.AddForeignKey(
                name: "FK_Souvenir_Category_CategoryID",
                table: "Souvenir",
                column: "CategoryID",
                principalTable: "Category",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Souvenir_Supplier_SupplierID",
                table: "Souvenir",
                column: "SupplierID",
                principalTable: "Supplier",
                principalColumn: "SupplierID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
