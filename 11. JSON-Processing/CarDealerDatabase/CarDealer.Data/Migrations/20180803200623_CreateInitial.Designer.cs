﻿// <auto-generated />
using System;
using CarDealer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CarDealer.Data.Migrations
{
    [DbContext(typeof(CarDealerContext))]
    [Migration("20180803200623_CreateInitial")]
    partial class CreateInitial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CarDealer.Models.Car", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Make")
                        .IsRequired();

                    b.Property<string>("Model")
                        .IsRequired();

                    b.Property<long>("TravelledDistance");

                    b.HasKey("Id");

                    b.ToTable("Cars");
                });

            modelBuilder.Entity("CarDealer.Models.Customer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("BirthDate");

                    b.Property<bool>("IsYoungDriver");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("CarDealer.Models.Part", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<decimal>("Price");

                    b.Property<int>("Quantity");

                    b.Property<int>("Supplier_Id");

                    b.HasKey("Id");

                    b.HasIndex("Supplier_Id");

                    b.ToTable("Parts");
                });

            modelBuilder.Entity("CarDealer.Models.PartCar", b =>
                {
                    b.Property<int>("Car_Id");

                    b.Property<int>("Part_Id");

                    b.HasKey("Car_Id", "Part_Id");

                    b.HasIndex("Part_Id");

                    b.ToTable("PartCars");
                });

            modelBuilder.Entity("CarDealer.Models.Sale", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Car_Id");

                    b.Property<int>("Customer_Id");

                    b.Property<decimal>("Discount");

                    b.HasKey("Id");

                    b.HasIndex("Car_Id");

                    b.HasIndex("Customer_Id");

                    b.ToTable("Sales");
                });

            modelBuilder.Entity("CarDealer.Models.Supplier", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsImporter");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Suppliers");
                });

            modelBuilder.Entity("CarDealer.Models.Part", b =>
                {
                    b.HasOne("CarDealer.Models.Supplier", "Supplier")
                        .WithMany("Parts")
                        .HasForeignKey("Supplier_Id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CarDealer.Models.PartCar", b =>
                {
                    b.HasOne("CarDealer.Models.Car", "Car")
                        .WithMany("PartCars")
                        .HasForeignKey("Car_Id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CarDealer.Models.Part", "Part")
                        .WithMany("PartCars")
                        .HasForeignKey("Part_Id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CarDealer.Models.Sale", b =>
                {
                    b.HasOne("CarDealer.Models.Car", "Car")
                        .WithMany("Sales")
                        .HasForeignKey("Car_Id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CarDealer.Models.Customer", "Customer")
                        .WithMany("Sales")
                        .HasForeignKey("Customer_Id")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
