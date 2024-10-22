﻿// <auto-generated />
using System;
using HotelService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HotelService.Migrations
{
    [DbContext(typeof(HotelDbContext))]
    [Migration("20241020144801_AddCommunicationInfoBackToHotel")]
    partial class AddCommunicationInfoBackToHotel
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.30")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("HotelService.Models.CommunicationInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("HotelId")
                        .HasColumnType("char(36)");

                    b.Property<string>("InfoDetails")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("InfoType")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("HotelId");

                    b.ToTable("CommunicationInfo");
                });

            modelBuilder.Entity("HotelService.Models.Hotel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("CompanyTitle")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("ContactPersonFirstName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("ContactPersonLastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Hotels");
                });

            modelBuilder.Entity("HotelService.Models.CommunicationInfo", b =>
                {
                    b.HasOne("HotelService.Models.Hotel", "Hotel")
                        .WithMany("CommunicationInfos")
                        .HasForeignKey("HotelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Hotel");
                });

            modelBuilder.Entity("HotelService.Models.Hotel", b =>
                {
                    b.Navigation("CommunicationInfos");
                });
#pragma warning restore 612, 618
        }
    }
}
