using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SampleMVCApp.Infra.Migrations
{
    public partial class UpdateVisibleColumnNameToMenu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Visisble",
                table: "Menus",
                newName: "Visible");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Visible",
                table: "Menus",
                newName: "Visisble");
        }
    }
}
