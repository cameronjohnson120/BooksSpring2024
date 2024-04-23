﻿// <auto-generated />
using BooksSpring2024.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BooksSpring2024.Migrations
{
    [DbContext(typeof(BooksDBContext))]
    [Migration("20240111173747_creatingCategoriesTableAndSeedingData")]
    partial class creatingCategoriesTableAndSeedingData
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BooksSpring2024.Models.Category", b =>
                {
                    b.Property<int>("CategoryID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryID"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CategoryID");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            CategoryID = 1,
                            Description = "This is the description for the travel category",
                            Name = "Travel"
                        },
                        new
                        {
                            CategoryID = 2,
                            Description = "This is the description for the fiction category",
                            Name = "Fiction"
                        },
                        new
                        {
                            CategoryID = 3,
                            Description = "This is the description for the technology category",
                            Name = "Technology"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
