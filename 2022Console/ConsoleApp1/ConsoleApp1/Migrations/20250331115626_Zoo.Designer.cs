﻿// <auto-generated />
using System;
using ConsoleApp1;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ConsoleApp1.Migrations
{
    [DbContext(typeof(ZooContext))]
    [Migration("20250331115626_Zoo")]
    partial class Zoo
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.3");

            modelBuilder.Entity("ConsoleApp1.AdultAnimal", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AnimalId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AnimalId")
                        .IsUnique();

                    b.ToTable("AdultAnimals");
                });

            modelBuilder.Entity("ConsoleApp1.Animal", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AviaryId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TypeOfAnimal")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AviaryId");

                    b.ToTable("Animals");
                });

            modelBuilder.Entity("ConsoleApp1.Area", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Adress")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Area_name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Areas");
                });

            modelBuilder.Entity("ConsoleApp1.Aviary", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AreaId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("AviaryName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AreaId");

                    b.ToTable("Aviaries");
                });

            modelBuilder.Entity("ConsoleApp1.BabyAnimal", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AdultAnimalId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AnimalId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AdultAnimalId");

                    b.HasIndex("AnimalId")
                        .IsUnique();

                    b.ToTable("BabyAnimals");
                });

            modelBuilder.Entity("ConsoleApp1.Blog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Blogs");
                });

            modelBuilder.Entity("ConsoleApp1.Day", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Days");
                });

            modelBuilder.Entity("ConsoleApp1.Employer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AreaId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Fio")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AreaId");

                    b.ToTable("Employers");
                });

            modelBuilder.Entity("ConsoleApp1.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AnimalId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("DayId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("EventText")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("EventTime")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsBorn")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDie")
                        .HasColumnType("INTEGER");

                    b.Property<int>("WorkingShiftId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AnimalId");

                    b.HasIndex("DayId");

                    b.HasIndex("WorkingShiftId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("ConsoleApp1.WorkingShift", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("EmployerId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("TimeBegin")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("TimeEnd")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("EmployerId");

                    b.ToTable("WorkingShifts");
                });

            modelBuilder.Entity("ConsoleApp1.AdultAnimal", b =>
                {
                    b.HasOne("ConsoleApp1.Animal", "Animal")
                        .WithOne("AdultAnimal")
                        .HasForeignKey("ConsoleApp1.AdultAnimal", "AnimalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Animal");
                });

            modelBuilder.Entity("ConsoleApp1.Animal", b =>
                {
                    b.HasOne("ConsoleApp1.Aviary", "Aviary")
                        .WithMany("Animals")
                        .HasForeignKey("AviaryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Aviary");
                });

            modelBuilder.Entity("ConsoleApp1.Aviary", b =>
                {
                    b.HasOne("ConsoleApp1.Area", "Area")
                        .WithMany("Aviaries")
                        .HasForeignKey("AreaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Area");
                });

            modelBuilder.Entity("ConsoleApp1.BabyAnimal", b =>
                {
                    b.HasOne("ConsoleApp1.AdultAnimal", "AdultAnimal")
                        .WithMany("BabyAnimals")
                        .HasForeignKey("AdultAnimalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ConsoleApp1.Animal", "Animal")
                        .WithOne("BabyAnimal")
                        .HasForeignKey("ConsoleApp1.BabyAnimal", "AnimalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AdultAnimal");

                    b.Navigation("Animal");
                });

            modelBuilder.Entity("ConsoleApp1.Employer", b =>
                {
                    b.HasOne("ConsoleApp1.Area", "Area")
                        .WithMany("Employers")
                        .HasForeignKey("AreaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Area");
                });

            modelBuilder.Entity("ConsoleApp1.Event", b =>
                {
                    b.HasOne("ConsoleApp1.Animal", "Animal")
                        .WithMany("Events")
                        .HasForeignKey("AnimalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ConsoleApp1.Day", "Day")
                        .WithMany("Events")
                        .HasForeignKey("DayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ConsoleApp1.WorkingShift", "WorkingShift")
                        .WithMany("Events")
                        .HasForeignKey("WorkingShiftId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Animal");

                    b.Navigation("Day");

                    b.Navigation("WorkingShift");
                });

            modelBuilder.Entity("ConsoleApp1.WorkingShift", b =>
                {
                    b.HasOne("ConsoleApp1.Employer", "Employer")
                        .WithMany("Shifts")
                        .HasForeignKey("EmployerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employer");
                });

            modelBuilder.Entity("ConsoleApp1.AdultAnimal", b =>
                {
                    b.Navigation("BabyAnimals");
                });

            modelBuilder.Entity("ConsoleApp1.Animal", b =>
                {
                    b.Navigation("AdultAnimal")
                        .IsRequired();

                    b.Navigation("BabyAnimal")
                        .IsRequired();

                    b.Navigation("Events");
                });

            modelBuilder.Entity("ConsoleApp1.Area", b =>
                {
                    b.Navigation("Aviaries");

                    b.Navigation("Employers");
                });

            modelBuilder.Entity("ConsoleApp1.Aviary", b =>
                {
                    b.Navigation("Animals");
                });

            modelBuilder.Entity("ConsoleApp1.Day", b =>
                {
                    b.Navigation("Events");
                });

            modelBuilder.Entity("ConsoleApp1.Employer", b =>
                {
                    b.Navigation("Shifts");
                });

            modelBuilder.Entity("ConsoleApp1.WorkingShift", b =>
                {
                    b.Navigation("Events");
                });
#pragma warning restore 612, 618
        }
    }
}
