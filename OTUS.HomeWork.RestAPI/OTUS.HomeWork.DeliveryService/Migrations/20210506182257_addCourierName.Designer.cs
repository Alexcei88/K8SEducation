﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using OTUS.HomeWork.DeliveryService.DAL;

namespace OTUS.HomeWork.DeliveryService.Migrations
{
    [DbContext(typeof(DeliveryContext))]
    [Migration("20210506182257_addCourierName")]
    partial class addCourierName
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasPostgresExtension("uuid-ossp")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.3")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("OTUS.HomeWork.DeliveryService.Domain.Delivery", b =>
                {
                    b.Property<string>("OrderNumber")
                        .HasColumnType("text")
                        .HasColumnName("order_number");

                    b.Property<string>("CourierName")
                        .HasColumnType("text")
                        .HasColumnName("courier_name");

                    b.HasKey("OrderNumber")
                        .HasName("pk_delivery");

                    b.ToTable("delivery");
                });

            modelBuilder.Entity("OTUS.HomeWork.DeliveryService.Domain.DeliveryLocation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("CurrentAddress")
                        .HasColumnType("text")
                        .HasColumnName("current_address");

                    b.Property<string>("DeliveryAddress")
                        .HasColumnType("text")
                        .HasColumnName("delivery_address");

                    b.Property<DateTime>("EstimatedDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("estimated_date");

                    b.Property<string>("OrderNumber")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("order_number");

                    b.Property<DateTime>("ShipmentDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("shipment_date");

                    b.HasKey("Id")
                        .HasName("pk_location");

                    b.HasIndex("OrderNumber")
                        .IsUnique()
                        .HasDatabaseName("ix_location_order_number");

                    b.ToTable("location");
                });

            modelBuilder.Entity("OTUS.HomeWork.DeliveryService.Domain.DeliveryProduct", b =>
                {
                    b.Property<string>("OrderNumber")
                        .HasColumnType("text")
                        .HasColumnName("order_number");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid")
                        .HasColumnName("product_id");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<double>("Space")
                        .HasColumnType("double precision")
                        .HasColumnName("space");

                    b.Property<double>("Weight")
                        .HasColumnType("double precision")
                        .HasColumnName("weight");

                    b.HasKey("OrderNumber", "ProductId")
                        .HasName("pk_delivery_product");

                    b.ToTable("delivery_product");
                });

            modelBuilder.Entity("OTUS.HomeWork.DeliveryService.Domain.DeliveryLocation", b =>
                {
                    b.HasOne("OTUS.HomeWork.DeliveryService.Domain.Delivery", "Delivery")
                        .WithOne("Location")
                        .HasForeignKey("OTUS.HomeWork.DeliveryService.Domain.DeliveryLocation", "OrderNumber")
                        .HasConstraintName("fk_location_delivery_order_number")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Delivery");
                });

            modelBuilder.Entity("OTUS.HomeWork.DeliveryService.Domain.DeliveryProduct", b =>
                {
                    b.HasOne("OTUS.HomeWork.DeliveryService.Domain.Delivery", "Delivery")
                        .WithMany("Products")
                        .HasForeignKey("OrderNumber")
                        .HasConstraintName("fk_delivery_product_delivery_order_number")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Delivery");
                });

            modelBuilder.Entity("OTUS.HomeWork.DeliveryService.Domain.Delivery", b =>
                {
                    b.Navigation("Location");

                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
