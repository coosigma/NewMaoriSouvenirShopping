using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MaoriSouvenirShopping.Migrations
{
    public partial class RowVersion : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
            name: "RowVersion",
            table: "Souvenir",
            type: "rowversion",
            rowVersion: true,
            nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
             name: "RowVersion",
             table: "Souvenir");
        }
    }
}
