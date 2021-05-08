﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using OTUS.HomeWork.PaymentGatewayService.DAL;

namespace OTUS.HomeWork.BillingService.Migrations
{
    [DbContext(typeof(PaymentContext))]
    [Migration("20210504185241_adrefund")]
    partial class adrefund
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.3")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("OTUS.HomeWork.PaymentGatewayService.Domain.Payment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric")
                        .HasColumnName("amount");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date");

                    b.Property<string>("IdempotanceKey")
                        .HasColumnType("text")
                        .HasColumnName("idempotance_key");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_payments");

                    b.HasIndex("IdempotanceKey")
                        .IsUnique()
                        .HasDatabaseName("ix_payments_idempotance_key");

                    b.ToTable("payments");
                });

            modelBuilder.Entity("OTUS.HomeWork.PaymentGatewayService.Domain.Refund", b =>
                {
                    b.Property<Guid>("BillingId")
                        .HasColumnType("uuid")
                        .HasColumnName("billing_id");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("BillingId")
                        .HasName("pk_refunds");

                    b.ToTable("refunds");
                });

            modelBuilder.Entity("OTUS.HomeWork.PaymentGatewayService.Domain.Refund", b =>
                {
                    b.HasOne("OTUS.HomeWork.PaymentGatewayService.Domain.Payment", "Payment")
                        .WithOne("Refund")
                        .HasForeignKey("OTUS.HomeWork.PaymentGatewayService.Domain.Refund", "BillingId")
                        .HasConstraintName("fk_refunds_payments_billing_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Payment");
                });

            modelBuilder.Entity("OTUS.HomeWork.PaymentGatewayService.Domain.Payment", b =>
                {
                    b.Navigation("Refund");
                });
#pragma warning restore 612, 618
        }
    }
}
