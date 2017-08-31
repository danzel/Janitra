﻿// <auto-generated />
using Janitra.Data;
using Janitra.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace Janitra.Data.Migrations
{
    [DbContext(typeof(JanitraContext))]
    [Migration("20170831091637_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Janitra.Data.Models.CitraBuild", b =>
                {
                    b.Property<int>("CitraBuildId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("ActivelyTesting");

                    b.Property<int>("AddedByUserId");

                    b.Property<DateTimeOffset>("DateAdded");

                    b.Property<string>("GitHash");

                    b.Property<string>("LinuxUrl");

                    b.Property<string>("OsxUrl");

                    b.Property<string>("WindowsUrl");

                    b.HasKey("CitraBuildId");

                    b.HasIndex("AddedByUserId");

                    b.HasIndex("GitHash")
                        .IsUnique()
                        .HasFilter("[GitHash] IS NOT NULL");

                    b.ToTable("CitraBuilds");
                });

            modelBuilder.Entity("Janitra.Data.Models.JanitraBot", b =>
                {
                    b.Property<int>("JanitraBotId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AccessKey")
                        .IsRequired();

                    b.Property<int>("AddedByUserId");

                    b.Property<string>("HardwareDetails")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("Os");

                    b.HasKey("JanitraBotId");

                    b.HasIndex("AddedByUserId");

                    b.ToTable("JanitraBots");
                });

            modelBuilder.Entity("Janitra.Data.Models.TestDefinition", b =>
                {
                    b.Property<int>("TestDefinitionId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("ActivelyTesting");

                    b.Property<DateTimeOffset>("AddedAt");

                    b.Property<int>("AddedByUserId");

                    b.Property<string>("MovieSha256")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<string>("MovieUrl")
                        .IsRequired();

                    b.Property<string>("Notes")
                        .IsRequired();

                    b.Property<string>("TestName")
                        .IsRequired();

                    b.Property<int>("TestRomId");

                    b.HasKey("TestDefinitionId");

                    b.HasIndex("AddedByUserId");

                    b.HasIndex("TestName")
                        .IsUnique();

                    b.HasIndex("TestRomId");

                    b.ToTable("TestDefinitions");
                });

            modelBuilder.Entity("Janitra.Data.Models.TestResult", b =>
                {
                    b.Property<int>("TestResultId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CitraBuildId");

                    b.Property<int>("JanitraBotId");

                    b.Property<string>("LogUrl")
                        .IsRequired();

                    b.Property<DateTimeOffset>("ReportedAt");

                    b.Property<string>("ScreenshotUrl")
                        .IsRequired();

                    b.Property<int>("TestDefinitionId");

                    b.Property<int>("TestResultType");

                    b.HasKey("TestResultId");

                    b.HasIndex("CitraBuildId");

                    b.HasIndex("JanitraBotId");

                    b.HasIndex("TestDefinitionId");

                    b.ToTable("TestResults");
                });

            modelBuilder.Entity("Janitra.Data.Models.TestRom", b =>
                {
                    b.Property<int>("TestRomId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AddedByUserId");

                    b.Property<string>("CodeUrl");

                    b.Property<string>("FileName")
                        .IsRequired();

                    b.Property<string>("ReadableName")
                        .IsRequired();

                    b.Property<string>("RomSha256")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<int>("RomType");

                    b.Property<string>("RomUrl");

                    b.HasKey("TestRomId");

                    b.HasIndex("AddedByUserId");

                    b.ToTable("TestRoms");
                });

            modelBuilder.Entity("Janitra.Data.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("OAuthId")
                        .IsRequired();

                    b.Property<string>("OAuthName")
                        .IsRequired();

                    b.Property<string>("OAuthProvider")
                        .IsRequired();

                    b.Property<int>("UserLevel");

                    b.HasKey("UserId");

                    b.HasIndex("OAuthProvider", "OAuthId")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Janitra.Data.Models.CitraBuild", b =>
                {
                    b.HasOne("Janitra.Data.Models.User", "AddedByUser")
                        .WithMany("AddedCitraBuilds")
                        .HasForeignKey("AddedByUserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Janitra.Data.Models.JanitraBot", b =>
                {
                    b.HasOne("Janitra.Data.Models.User", "AddedByUser")
                        .WithMany("AddedJanitraBots")
                        .HasForeignKey("AddedByUserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Janitra.Data.Models.TestDefinition", b =>
                {
                    b.HasOne("Janitra.Data.Models.User", "AddedByUser")
                        .WithMany("AddedTestDefinitions")
                        .HasForeignKey("AddedByUserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Janitra.Data.Models.TestRom", "TestRom")
                        .WithMany("UsedByTestDefinitions")
                        .HasForeignKey("TestRomId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Janitra.Data.Models.TestResult", b =>
                {
                    b.HasOne("Janitra.Data.Models.CitraBuild", "CitraBuild")
                        .WithMany("TestResults")
                        .HasForeignKey("CitraBuildId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Janitra.Data.Models.JanitraBot", "JanitraBot")
                        .WithMany("TestResults")
                        .HasForeignKey("JanitraBotId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Janitra.Data.Models.TestDefinition", "TestDefinition")
                        .WithMany("TestResults")
                        .HasForeignKey("TestDefinitionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Janitra.Data.Models.TestRom", b =>
                {
                    b.HasOne("Janitra.Data.Models.User", "AddedByUser")
                        .WithMany("AddedTestRoms")
                        .HasForeignKey("AddedByUserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
