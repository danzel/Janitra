using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Janitra.Data.Migrations
{
    public partial class RomMovies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roms",
                columns: table => new
                {
                    RomId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AddedByUserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RomFileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RomSha256 = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    RomType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roms", x => x.RomId);
                    table.ForeignKey(
                        name: "FK_Roms_Users_AddedByUserId",
                        column: x => x.AddedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RomMovies",
                columns: table => new
                {
                    RomMovieId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActivelyTesting = table.Column<bool>(type: "bit", nullable: false),
                    AddedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    AddedByUserId = table.Column<int>(type: "int", nullable: false),
                    CitraRegionValue = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Length = table.Column<TimeSpan>(type: "time", nullable: false),
                    MovieSha256 = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    MovieUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RomId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RomMovies", x => x.RomMovieId);
                    table.ForeignKey(
                        name: "FK_RomMovies_Users_AddedByUserId",
                        column: x => x.AddedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RomMovies_Roms_RomId",
                        column: x => x.RomId,
                        principalTable: "Roms",
                        principalColumn: "RomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RomMovieResults",
                columns: table => new
                {
                    RomMovieResultId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CitraBuildId = table.Column<int>(type: "int", nullable: false),
                    ExecutionResult = table.Column<int>(type: "int", nullable: false),
                    JanitraBotId = table.Column<int>(type: "int", nullable: false),
                    LogUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReportedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RomMovieId = table.Column<int>(type: "int", nullable: false),
                    TimeTaken = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RomMovieResults", x => x.RomMovieResultId);
                    table.ForeignKey(
                        name: "FK_RomMovieResults_CitraBuilds_CitraBuildId",
                        column: x => x.CitraBuildId,
                        principalTable: "CitraBuilds",
                        principalColumn: "CitraBuildId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RomMovieResults_JanitraBots_JanitraBotId",
                        column: x => x.JanitraBotId,
                        principalTable: "JanitraBots",
                        principalColumn: "JanitraBotId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RomMovieResults_RomMovies_RomMovieId",
                        column: x => x.RomMovieId,
                        principalTable: "RomMovies",
                        principalColumn: "RomMovieId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RomMovieResultScreenshot",
                columns: table => new
                {
                    RomMovieResultScreenshotId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BottomImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FrameNumber = table.Column<int>(type: "int", nullable: false),
                    RomMovieResultId = table.Column<int>(type: "int", nullable: false),
                    TopImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RomMovieResultScreenshot", x => x.RomMovieResultScreenshotId);
                    table.ForeignKey(
                        name: "FK_RomMovieResultScreenshot_RomMovieResults_RomMovieResultId",
                        column: x => x.RomMovieResultId,
                        principalTable: "RomMovieResults",
                        principalColumn: "RomMovieResultId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RomMovieResults_CitraBuildId",
                table: "RomMovieResults",
                column: "CitraBuildId");

            migrationBuilder.CreateIndex(
                name: "IX_RomMovieResults_JanitraBotId",
                table: "RomMovieResults",
                column: "JanitraBotId");

            migrationBuilder.CreateIndex(
                name: "IX_RomMovieResults_RomMovieId",
                table: "RomMovieResults",
                column: "RomMovieId");

            migrationBuilder.CreateIndex(
                name: "IX_RomMovieResultScreenshot_RomMovieResultId",
                table: "RomMovieResultScreenshot",
                column: "RomMovieResultId");

            migrationBuilder.CreateIndex(
                name: "IX_RomMovies_AddedByUserId",
                table: "RomMovies",
                column: "AddedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RomMovies_RomId",
                table: "RomMovies",
                column: "RomId");

            migrationBuilder.CreateIndex(
                name: "IX_Roms_AddedByUserId",
                table: "Roms",
                column: "AddedByUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RomMovieResultScreenshot");

            migrationBuilder.DropTable(
                name: "RomMovieResults");

            migrationBuilder.DropTable(
                name: "RomMovies");

            migrationBuilder.DropTable(
                name: "Roms");
        }
    }
}
