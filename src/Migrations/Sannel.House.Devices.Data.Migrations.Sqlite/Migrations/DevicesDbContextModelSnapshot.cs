﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sannel.House.Devices.Data;

namespace Sannel.House.Devices.Data.Migrations.Sqlite.Migrations
{
    [DbContext(typeof(DevicesDbContext))]
    partial class DevicesDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028");

            modelBuilder.Entity("Sannel.House.Devices.Models.AlternateDeviceId", b =>
                {
                    b.Property<int>("AlternateId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<int>("DeviceId");

                    b.Property<long?>("MacAddress");

                    b.Property<string>("Manufacture");

                    b.Property<string>("ManufactureId");

                    b.Property<Guid?>("Uuid");

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
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(2000);

                    b.Property<int>("DisplayOrder");

                    b.Property<bool>("IsReadOnly");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.HasKey("DeviceId");

                    b.HasIndex("DisplayOrder");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("Sannel.House.Devices.Models.AlternateDeviceId", b =>
                {
                    b.HasOne("Sannel.House.Devices.Models.Device", "Device")
                        .WithMany()
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
