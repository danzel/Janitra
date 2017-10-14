using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Janitra.Data.Migrations
{
    public partial class TestDefinitionReferenceScreenshots : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReferenceScreenshotBottomUrl",
                table: "TestDefinitions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceScreenshotTopUrl",
                table: "TestDefinitions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceScreenshotBottomUrl",
                table: "TestDefinitions");

            migrationBuilder.DropColumn(
                name: "ReferenceScreenshotTopUrl",
                table: "TestDefinitions");
        }
    }
}
