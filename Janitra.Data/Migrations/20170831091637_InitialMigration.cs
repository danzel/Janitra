using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Janitra.Data.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OAuthId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OAuthName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OAuthProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "CitraBuilds",
                columns: table => new
                {
                    CitraBuildId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActivelyTesting = table.Column<bool>(type: "bit", nullable: false),
                    AddedByUserId = table.Column<int>(type: "int", nullable: false),
                    DateAdded = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    GitHash = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LinuxUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OsxUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WindowsUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitraBuilds", x => x.CitraBuildId);
                    table.ForeignKey(
                        name: "FK_CitraBuilds_Users_AddedByUserId",
                        column: x => x.AddedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JanitraBots",
                columns: table => new
                {
                    JanitraBotId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddedByUserId = table.Column<int>(type: "int", nullable: false),
                    HardwareDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Os = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JanitraBots", x => x.JanitraBotId);
                    table.ForeignKey(
                        name: "FK_JanitraBots_Users_AddedByUserId",
                        column: x => x.AddedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestRoms",
                columns: table => new
                {
                    TestRomId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AddedByUserId = table.Column<int>(type: "int", nullable: false),
                    CodeUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReadableName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RomSha256 = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    RomType = table.Column<int>(type: "int", nullable: false),
                    RomUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRoms", x => x.TestRomId);
                    table.ForeignKey(
                        name: "FK_TestRoms_Users_AddedByUserId",
                        column: x => x.AddedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestDefinitions",
                columns: table => new
                {
                    TestDefinitionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActivelyTesting = table.Column<bool>(type: "bit", nullable: false),
                    AddedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    AddedByUserId = table.Column<int>(type: "int", nullable: false),
                    MovieSha256 = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    MovieUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TestName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TestRomId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestDefinitions", x => x.TestDefinitionId);
                    table.ForeignKey(
                        name: "FK_TestDefinitions_Users_AddedByUserId",
                        column: x => x.AddedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestDefinitions_TestRoms_TestRomId",
                        column: x => x.TestRomId,
                        principalTable: "TestRoms",
                        principalColumn: "TestRomId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestResults",
                columns: table => new
                {
                    TestResultId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CitraBuildId = table.Column<int>(type: "int", nullable: false),
                    JanitraBotId = table.Column<int>(type: "int", nullable: false),
                    LogUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReportedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ScreenshotUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TestDefinitionId = table.Column<int>(type: "int", nullable: false),
                    TestResultType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResults", x => x.TestResultId);
                    table.ForeignKey(
                        name: "FK_TestResults_CitraBuilds_CitraBuildId",
                        column: x => x.CitraBuildId,
                        principalTable: "CitraBuilds",
                        principalColumn: "CitraBuildId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestResults_JanitraBots_JanitraBotId",
                        column: x => x.JanitraBotId,
                        principalTable: "JanitraBots",
                        principalColumn: "JanitraBotId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestResults_TestDefinitions_TestDefinitionId",
                        column: x => x.TestDefinitionId,
                        principalTable: "TestDefinitions",
                        principalColumn: "TestDefinitionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CitraBuilds_AddedByUserId",
                table: "CitraBuilds",
                column: "AddedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CitraBuilds_GitHash",
                table: "CitraBuilds",
                column: "GitHash",
                unique: true,
                filter: "[GitHash] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_JanitraBots_AddedByUserId",
                table: "JanitraBots",
                column: "AddedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TestDefinitions_AddedByUserId",
                table: "TestDefinitions",
                column: "AddedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TestDefinitions_TestName",
                table: "TestDefinitions",
                column: "TestName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestDefinitions_TestRomId",
                table: "TestDefinitions",
                column: "TestRomId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_CitraBuildId",
                table: "TestResults",
                column: "CitraBuildId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_JanitraBotId",
                table: "TestResults",
                column: "JanitraBotId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_TestDefinitionId",
                table: "TestResults",
                column: "TestDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRoms_AddedByUserId",
                table: "TestRoms",
                column: "AddedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_OAuthProvider_OAuthId",
                table: "Users",
                columns: new[] { "OAuthProvider", "OAuthId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestResults");

            migrationBuilder.DropTable(
                name: "CitraBuilds");

            migrationBuilder.DropTable(
                name: "JanitraBots");

            migrationBuilder.DropTable(
                name: "TestDefinitions");

            migrationBuilder.DropTable(
                name: "TestRoms");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
