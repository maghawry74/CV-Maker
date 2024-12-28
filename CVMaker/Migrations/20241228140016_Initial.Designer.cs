﻿// <auto-generated />
using System;
using CVMaker.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CVMaker.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241228140016_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CVMaker.Context.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string[]>("Languages")
                        .IsRequired()
                        .HasColumnType("text[]");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string[]>("Skills")
                        .IsRequired()
                        .HasColumnType("text[]");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CVMaker.Context.Models.User", b =>
                {
                    b.OwnsOne("CVMaker.Context.Models.ContactInfo", "ContactInfo", b1 =>
                        {
                            b1.Property<Guid>("UserId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Address")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Email")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Github")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("JobTitle")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("LinkedIn")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Phone")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("UserId");

                            b1.ToTable("Users");

                            b1.ToJson("ContactInfo");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.OwnsMany("CVMaker.Context.Models.Education", "Education", b1 =>
                        {
                            b1.Property<Guid>("UserId")
                                .HasColumnType("uuid");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            b1.Property<string>("Degree")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<DateTime>("EndDate")
                                .HasColumnType("timestamp with time zone");

                            b1.Property<DateTime>("StartDate")
                                .HasColumnType("timestamp with time zone");

                            b1.Property<string>("University")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("UserId", "Id");

                            b1.ToTable("Users");

                            b1.ToJson("Education");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.OwnsMany("CVMaker.Context.Models.Project", "Projects", b1 =>
                        {
                            b1.Property<Guid>("UserId")
                                .HasColumnType("uuid");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            b1.Property<DateTime>("Date")
                                .HasColumnType("timestamp with time zone");

                            b1.Property<string>("Description")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Title")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("UserId", "Id");

                            b1.ToTable("Users");

                            b1.ToJson("Projects");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.OwnsMany("CVMaker.Context.Models.WorkExperience", "Experiences", b1 =>
                        {
                            b1.Property<Guid>("UserId")
                                .HasColumnType("uuid");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            b1.Property<string>("Company")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Description")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<DateTime?>("EndDate")
                                .HasColumnType("timestamp with time zone");

                            b1.Property<string>("JobTitle")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<DateTime>("StartDate")
                                .HasColumnType("timestamp with time zone");

                            b1.HasKey("UserId", "Id");

                            b1.ToTable("Users");

                            b1.ToJson("Experiences");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.Navigation("ContactInfo")
                        .IsRequired();

                    b.Navigation("Education");

                    b.Navigation("Experiences");

                    b.Navigation("Projects");
                });
#pragma warning restore 612, 618
        }
    }
}