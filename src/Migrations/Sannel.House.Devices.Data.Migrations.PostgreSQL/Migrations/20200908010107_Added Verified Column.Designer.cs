﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Sannel.House.Devices.Data;

namespace Sannel.House.Devices.Data.Migrations.PostgreSQL.Migrations
{
    [DbContext(typeof(DevicesDbContext))]
    [Migration("20200908010107_Added Verified Column")]
    partial class AddedVerifiedColumn
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Sannel.House.Devices.Models.AlternateDeviceId", b =>
                {
                    b.Property<int>("AlternateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTimeOffset>("DateCreated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("DeviceId")
                        .HasColumnType("integer");

                    b.Property<long?>("MacAddress")
                        .HasColumnType("bigint");

                    b.Property<string>("Manufacture")
                        .HasColumnType("text");

                    b.Property<string>("ManufactureId")
                        .HasColumnType("text");

                    b.Property<Guid?>("Uuid")
                        .HasColumnType("uuid");

                    b.HasKey("AlternateId");

                    b.HasIndex("DeviceId");

                    b.HasIndex("MacAddress")
                        .IsUnique();

                    b.HasIndex("Uuid")
                        .IsUnique();

                    b.HasIndex("Manufacture", "ManufactureId")
                        .IsUnique();

                    b.ToTable("AlternateDeviceIds");
                });

            modelBuilder.Entity("Sannel.House.Devices.Models.Device", b =>
                {
                    b.Property<int>("DeviceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTimeOffset>("DateCreated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("character varying(2000)")
                        .HasMaxLength(2000);

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("integer");

                    b.Property<bool>("IsReadOnly")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<bool>("Verified")
                        .HasColumnType("boolean");

                    b.HasKey("DeviceId");

                    b.HasIndex("DisplayOrder");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("Sannel.House.Devices.Models.AlternateDeviceId", b =>
                {
                    b.HasOne("Sannel.House.Devices.Models.Device", "Device")
                        .WithMany()
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
