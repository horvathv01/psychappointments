﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PsychAppointments_API.DAL;

#nullable disable

namespace PsychAppointments_API.Migrations
{
    [DbContext(typeof(PsychAppointmentContext))]
    partial class PsychAppointmentContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ClientPsychologist", b =>
                {
                    b.Property<long>("ClientsId")
                        .HasColumnType("bigint");

                    b.Property<long>("PsychologistsId")
                        .HasColumnType("bigint");

                    b.HasKey("ClientsId", "PsychologistsId");

                    b.HasIndex("PsychologistsId");

                    b.ToTable("PsychologistClients", (string)null);
                });

            modelBuilder.Entity("LocationManager", b =>
                {
                    b.Property<long>("LocationsId")
                        .HasColumnType("bigint");

                    b.Property<long>("ManagersId")
                        .HasColumnType("bigint");

                    b.HasKey("LocationsId", "ManagersId");

                    b.HasIndex("ManagersId");

                    b.ToTable("LocationManagers", (string)null);
                });

            modelBuilder.Entity("LocationPsychologist", b =>
                {
                    b.Property<long>("LocationId")
                        .HasColumnType("bigint");

                    b.Property<long>("PsychologistsId")
                        .HasColumnType("bigint");

                    b.HasKey("LocationId", "PsychologistsId");

                    b.HasIndex("PsychologistsId");

                    b.ToTable("LocationPsychologists", (string)null);
                });

            modelBuilder.Entity("PsychAppointments_API.Models.Address", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Rest")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Zip")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("PsychAppointments_API.Models.Location", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("AddressId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("PsychAppointments_API.Models.Session", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<bool>("Blank")
                        .HasColumnType("boolean");

                    b.Property<long>("ClientId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("End")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Frequency")
                        .HasColumnType("integer");

                    b.Property<long>("LocationId")
                        .HasColumnType("bigint");

                    b.Property<long?>("PartnerPsychologistId")
                        .HasColumnType("bigint");

                    b.Property<int>("Price")
                        .HasColumnType("integer");

                    b.Property<long>("PsychologistId")
                        .HasColumnType("bigint");

                    b.Property<long>("SlotId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("Start")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("LocationId");

                    b.HasIndex("PartnerPsychologistId");

                    b.HasIndex("PsychologistId");

                    b.HasIndex("SlotId");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("PsychAppointments_API.Models.Slot", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("LocationId")
                        .HasColumnType("bigint");

                    b.Property<long>("PsychologistId")
                        .HasColumnType("bigint");

                    b.Property<int>("Rest")
                        .HasColumnType("integer");

                    b.Property<int>("SessionLength")
                        .HasColumnType("integer");

                    b.Property<DateTime>("SlotEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("SlotStart")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("Weekly")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.HasIndex("PsychologistId");

                    b.ToTable("Slots");
                });

            modelBuilder.Entity("PsychAppointments_API.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("AddressId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("RegisteredById")
                        .HasColumnType("bigint");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.HasIndex("RegisteredById");

                    b.ToTable("User");

                    b.HasDiscriminator<string>("Discriminator").HasValue("User");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("PsychAppointments_API.Models.Admin", b =>
                {
                    b.HasBaseType("PsychAppointments_API.Models.User");

                    b.HasDiscriminator().HasValue("Admin");
                });

            modelBuilder.Entity("PsychAppointments_API.Models.Client", b =>
                {
                    b.HasBaseType("PsychAppointments_API.Models.User");

                    b.HasDiscriminator().HasValue("Client");
                });

            modelBuilder.Entity("PsychAppointments_API.Models.Manager", b =>
                {
                    b.HasBaseType("PsychAppointments_API.Models.User");

                    b.HasDiscriminator().HasValue("Manager");
                });

            modelBuilder.Entity("PsychAppointments_API.Models.Psychologist", b =>
                {
                    b.HasBaseType("PsychAppointments_API.Models.User");

                    b.HasDiscriminator().HasValue("Psychologist");
                });

            modelBuilder.Entity("ClientPsychologist", b =>
                {
                    b.HasOne("PsychAppointments_API.Models.Client", null)
                        .WithMany()
                        .HasForeignKey("ClientsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PsychAppointments_API.Models.Psychologist", null)
                        .WithMany()
                        .HasForeignKey("PsychologistsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("LocationManager", b =>
                {
                    b.HasOne("PsychAppointments_API.Models.Location", null)
                        .WithMany()
                        .HasForeignKey("LocationsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PsychAppointments_API.Models.Manager", null)
                        .WithMany()
                        .HasForeignKey("ManagersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("LocationPsychologist", b =>
                {
                    b.HasOne("PsychAppointments_API.Models.Location", null)
                        .WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PsychAppointments_API.Models.Psychologist", null)
                        .WithMany()
                        .HasForeignKey("PsychologistsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PsychAppointments_API.Models.Location", b =>
                {
                    b.HasOne("PsychAppointments_API.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Address");
                });

            modelBuilder.Entity("PsychAppointments_API.Models.Session", b =>
                {
                    b.HasOne("PsychAppointments_API.Models.Client", "Client")
                        .WithMany("Sessions")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PsychAppointments_API.Models.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PsychAppointments_API.Models.Psychologist", "PartnerPsychologist")
                        .WithMany()
                        .HasForeignKey("PartnerPsychologistId");

                    b.HasOne("PsychAppointments_API.Models.Psychologist", "Psychologist")
                        .WithMany("Sessions")
                        .HasForeignKey("PsychologistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PsychAppointments_API.Models.Slot", "Slot")
                        .WithMany("Sessions")
                        .HasForeignKey("SlotId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");

                    b.Navigation("Location");

                    b.Navigation("PartnerPsychologist");

                    b.Navigation("Psychologist");

                    b.Navigation("Slot");
                });

            modelBuilder.Entity("PsychAppointments_API.Models.Slot", b =>
                {
                    b.HasOne("PsychAppointments_API.Models.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PsychAppointments_API.Models.Psychologist", "Psychologist")
                        .WithMany("Slots")
                        .HasForeignKey("PsychologistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Location");

                    b.Navigation("Psychologist");
                });

            modelBuilder.Entity("PsychAppointments_API.Models.User", b =>
                {
                    b.HasOne("PsychAppointments_API.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PsychAppointments_API.Models.User", "RegisteredBy")
                        .WithMany()
                        .HasForeignKey("RegisteredById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Address");

                    b.Navigation("RegisteredBy");
                });

            modelBuilder.Entity("PsychAppointments_API.Models.Slot", b =>
                {
                    b.Navigation("Sessions");
                });

            modelBuilder.Entity("PsychAppointments_API.Models.Client", b =>
                {
                    b.Navigation("Sessions");
                });

            modelBuilder.Entity("PsychAppointments_API.Models.Psychologist", b =>
                {
                    b.Navigation("Sessions");

                    b.Navigation("Slots");
                });
#pragma warning restore 612, 618
        }
    }
}
