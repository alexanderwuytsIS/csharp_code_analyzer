﻿// <auto-generated />
using System;
using CodeAnalyzerService.Backend.DAL.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CodeAnalyzerService.Backend.Migrations
{
    [DbContext(typeof(CodeAnalyzerServiceDbContext))]
    partial class CodeAnalyzerServiceDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.5");

            modelBuilder.Entity("CodeAnalyzerService.Backend.DAL.EF.Entities.Analysis", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<int>("FK_ProjectAnalysis_Analysis")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("FK_ProjectAnalysis_Analysis");

                    b.ToTable("Analyses");
                });

            modelBuilder.Entity("CodeAnalyzerService.Backend.DAL.EF.Entities.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ProjectName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ProjectName")
                        .IsUnique();

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("CodeAnalyzerService.Backend.DAL.EF.Entities.Rule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("DefaultSeverity")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsEnabledByDefault")
                        .HasColumnType("INTEGER");

                    b.Property<string>("RuleName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("RuleName")
                        .IsUnique();

                    b.ToTable("Rules");
                });

            modelBuilder.Entity("CodeAnalyzerService.Backend.DAL.EF.Entities.RuleViolation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("FK_Analysis_RuleViolation")
                        .HasColumnType("INTEGER");

                    b.Property<int>("FK_RuleViolation_Rule")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PluginId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Severity")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TargetLanguage")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("FK_Analysis_RuleViolation");

                    b.HasIndex("FK_RuleViolation_Rule");

                    b.ToTable("RuleViolations");
                });

            modelBuilder.Entity("CodeAnalyzerService.Backend.DAL.EF.Entities.Analysis", b =>
                {
                    b.HasOne("CodeAnalyzerService.Backend.DAL.EF.Entities.Project", "Project")
                        .WithMany("Analyses")
                        .HasForeignKey("FK_ProjectAnalysis_Analysis")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("CodeAnalyzerService.Backend.DAL.EF.Entities.RuleViolation", b =>
                {
                    b.HasOne("CodeAnalyzerService.Backend.DAL.EF.Entities.Analysis", "Analysis")
                        .WithMany("RuleViolations")
                        .HasForeignKey("FK_Analysis_RuleViolation")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CodeAnalyzerService.Backend.DAL.EF.Entities.Rule", "Rule")
                        .WithMany("RuleViolations")
                        .HasForeignKey("FK_RuleViolation_Rule")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("CodeAnalyzerService.Backend.DAL.EF.Entities.Location", "Location", b1 =>
                        {
                            b1.Property<int>("RuleViolationId")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("EndLine")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("FileExtension")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<int>("Id")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Path")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<int>("StartLine")
                                .HasColumnType("INTEGER");

                            b1.HasKey("RuleViolationId");

                            b1.ToTable("RuleViolations");

                            b1.WithOwner()
                                .HasForeignKey("RuleViolationId");
                        });

                    b.Navigation("Analysis");

                    b.Navigation("Location")
                        .IsRequired();

                    b.Navigation("Rule");
                });

            modelBuilder.Entity("CodeAnalyzerService.Backend.DAL.EF.Entities.Analysis", b =>
                {
                    b.Navigation("RuleViolations");
                });

            modelBuilder.Entity("CodeAnalyzerService.Backend.DAL.EF.Entities.Project", b =>
                {
                    b.Navigation("Analyses");
                });

            modelBuilder.Entity("CodeAnalyzerService.Backend.DAL.EF.Entities.Rule", b =>
                {
                    b.Navigation("RuleViolations");
                });
#pragma warning restore 612, 618
        }
    }
}
