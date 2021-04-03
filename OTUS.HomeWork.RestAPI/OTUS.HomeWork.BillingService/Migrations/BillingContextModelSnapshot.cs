﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using OTUS.HomeWork.BillingService.DAL;

namespace OTUS.HomeWork.BillingService.Migrations
{
    [DbContext(typeof(BillingContext))]
    partial class BillingContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasPostgresExtension("uuid-ossp")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.3")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("OTUS.HomeWork.BillingService.Domain.Payment", b =>
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

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_payments_user_id");

                    b.ToTable("payments");
                });

            modelBuilder.Entity("OTUS.HomeWork.BillingService.Domain.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<decimal>("Balance")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric")
                        .HasDefaultValue(0.0m)
                        .HasColumnName("balance");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.ToTable("users");
                });

            modelBuilder.Entity("OTUS.HomeWork.BillingService.Domain.Payment", b =>
                {
                    b.HasOne("OTUS.HomeWork.BillingService.Domain.User", "User")
                        .WithMany("Payments")
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_payments_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("OTUS.HomeWork.BillingService.Domain.User", b =>
                {
                    b.Navigation("Payments");
                });
#pragma warning restore 612, 618
        }
    }
}
