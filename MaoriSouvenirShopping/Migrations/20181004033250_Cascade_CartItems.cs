using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MaoriSouvenirShopping.Migrations
{
    public partial class Cascade_CartItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_Souvenir_SouvenirID",
                table: "CartItem");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_Souvenir_SouvenirID",
                table: "CartItem",
                column: "SouvenirID",
                principalTable: "Souvenir",
                principalColumn: "SouvenirID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_Souvenir_SouvenirID",
                table: "CartItem");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_Souvenir_SouvenirID",
                table: "CartItem",
                column: "SouvenirID",
                principalTable: "Souvenir",
                principalColumn: "SouvenirID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
