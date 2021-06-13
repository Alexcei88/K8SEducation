﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using OTUS.HomeWork.EShop.DAL;

namespace OTUS.HomeWork.EShop.Migrations
{
    [DbContext(typeof(OrderContext))]
    [Migration("20210612173303_bucket_domain_reorganization")]
    partial class bucket_domain_reorganization
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasPostgresExtension("uuid-ossp")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.3")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("OTUS.HomeWork.EShop.Domain.Bucket", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.HasKey("UserId")
                        .HasName("pk_buckets");

                    b.ToTable("buckets");
                });

            modelBuilder.Entity("OTUS.HomeWork.EShop.Domain.BucketItem", b =>
                {
                    b.Property<Guid>("BucketId")
                        .HasColumnType("uuid")
                        .HasColumnName("bucket_id");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid")
                        .HasColumnName("product_id");

                    b.Property<Guid?>("BucketUserId")
                        .HasColumnType("uuid")
                        .HasColumnName("bucket_user_id");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer")
                        .HasColumnName("quantity");

                    b.HasKey("BucketId", "ProductId")
                        .HasName("pk_bucket_item");

                    b.HasIndex("BucketUserId")
                        .HasDatabaseName("ix_bucket_item_bucket_user_id");

                    b.ToTable("bucket_item");
                });

            modelBuilder.Entity("OTUS.HomeWork.EShop.Domain.Order", b =>
                {
                    b.Property<Guid>("OrderNumber")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("order_number");

                    b.Property<string>("BillingId")
                        .HasColumnType("text")
                        .HasColumnName("billing_id");

                    b.Property<DateTime>("CreatedOnUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_on_utc");

                    b.Property<string>("DeliveryAddress")
                        .HasColumnType("text")
                        .HasColumnName("delivery_address");

                    b.Property<string>("ErrorDescription")
                        .HasColumnType("text")
                        .HasColumnName("error_description");

                    b.Property<string>("IdempotencyKey")
                        .HasColumnType("text")
                        .HasColumnName("idempotency_key");

                    b.Property<DateTime?>("PaidDateUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("paid_date_utc");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("numeric")
                        .HasColumnName("total_price");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("OrderNumber")
                        .HasName("pk_orders");

                    b.HasIndex("IdempotencyKey")
                        .IsUnique()
                        .HasDatabaseName("ix_orders_idempotency_key");

                    b.ToTable("orders");
                });

            modelBuilder.Entity("OTUS.HomeWork.EShop.Domain.OrderItem", b =>
                {
                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid")
                        .HasColumnName("product_id");

                    b.Property<Guid>("OrderNumberId")
                        .HasColumnType("uuid")
                        .HasColumnName("order_number_id");

                    b.Property<Guid?>("OrderNumber1")
                        .HasColumnType("uuid")
                        .HasColumnName("order_number1");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer")
                        .HasColumnName("quantity");

                    b.HasKey("ProductId", "OrderNumberId")
                        .HasName("pk_order_item");

                    b.HasIndex("OrderNumber1")
                        .HasDatabaseName("ix_order_item_order_number1");

                    b.HasIndex("OrderNumberId")
                        .HasDatabaseName("ix_order_item_order_number_id");

                    b.ToTable("order_item");
                });

            modelBuilder.Entity("OTUS.HomeWork.EShop.Domain.BucketItem", b =>
                {
                    b.HasOne("OTUS.HomeWork.EShop.Domain.Bucket", "Bucket")
                        .WithMany()
                        .HasForeignKey("BucketId")
                        .HasConstraintName("fk_bucket_item_buckets_bucket_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OTUS.HomeWork.EShop.Domain.Bucket", null)
                        .WithMany("Items")
                        .HasForeignKey("BucketUserId")
                        .HasConstraintName("fk_bucket_item_buckets_bucket_user_id");

                    b.Navigation("Bucket");
                });

            modelBuilder.Entity("OTUS.HomeWork.EShop.Domain.OrderItem", b =>
                {
                    b.HasOne("OTUS.HomeWork.EShop.Domain.Order", null)
                        .WithMany("Items")
                        .HasForeignKey("OrderNumber1")
                        .HasConstraintName("fk_order_item_orders_order_number1");

                    b.HasOne("OTUS.HomeWork.EShop.Domain.Order", "Order")
                        .WithMany()
                        .HasForeignKey("OrderNumberId")
                        .HasConstraintName("fk_order_item_orders_order_number")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("OTUS.HomeWork.EShop.Domain.Bucket", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("OTUS.HomeWork.EShop.Domain.Order", b =>
                {
                    b.Navigation("Items");
                });
#pragma warning restore 612, 618
        }
    }
}
