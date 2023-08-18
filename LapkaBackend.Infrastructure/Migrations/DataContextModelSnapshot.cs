﻿// <auto-generated />
using System;
using LapkaBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LapkaBackend.Infrastructure.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("LapkaBackend.Domain.Entities.Animal", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsArchival")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSterilized")
                        .HasColumnType("bit");

                    b.Property<bool>("IsVisible")
                        .HasColumnType("bit");

                    b.Property<string>("Marking")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Months")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhotoName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ShelterId")
                        .IsRequired()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Species")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Weight")
                        .HasColumnType("decimal(5, 2)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ShelterId");

                    b.ToTable("Animals", (string)null);
                });

            modelBuilder.Entity("LapkaBackend.Domain.Entities.AnimalCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AnimalCategories");
                });

            modelBuilder.Entity("LapkaBackend.Domain.Entities.AnimalView", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AnimalId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("UserId")
                        .IsRequired()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ViewDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AnimalId");

                    b.HasIndex("UserId");

                    b.ToTable("AnimalViews");
                });

            modelBuilder.Entity("LapkaBackend.Domain.Entities.ChatMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("RoomId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RoomId");

                    b.HasIndex("UserId");

                    b.ToTable("ChatMessages", (string)null);
                });

            modelBuilder.Entity("LapkaBackend.Domain.Entities.ChatRoom", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("User1Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("User2Id")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("User1Id");

                    b.HasIndex("User2Id");

                    b.ToTable("ChatRooms", (string)null);
                });

            modelBuilder.Entity("LapkaBackend.Domain.Entities.FileBlob", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BlobName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ParentEntityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UploadName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Blobs");
                });

            modelBuilder.Entity("LapkaBackend.Domain.Entities.Photo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AnimalId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsProfilePhoto")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("AnimalId");

                    b.ToTable("Photos");
                });

            modelBuilder.Entity("LapkaBackend.Domain.Entities.Reaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AnimalId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("NameOfReaction")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("AnimalId");

                    b.HasIndex("UserId");

                    b.ToTable("Reactions");
                });

            modelBuilder.Entity("LapkaBackend.Domain.Entities.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("LapkaBackend.Domain.Entities.Shelter", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Krs")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<float>("Latitude")
                        .HasMaxLength(255)
                        .HasColumnType("real");

                    b.Property<float>("Longitude")
                        .HasMaxLength(255)
                        .HasColumnType("real");

                    b.Property<string>("Nip")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("OrganizationName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("ZipCode")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("Shelters", (string)null);
                });

            modelBuilder.Entity("LapkaBackend.Domain.Entities.ShelterVolunteering", b =>
                {
                    b.Property<Guid>("ShelterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BankAccountNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DailyHelpDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DonationDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDailyHelpActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDonationActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsTakingDogsOutActive")
                        .HasColumnType("bit");

                    b.Property<string>("TakingDogsOutDesctiption")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ShelterId");

                    b.ToTable("SheltersVolunteering", (string)null);
                });

            modelBuilder.Entity("LapkaBackend.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LoginProvider")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProfilePicture")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RefreshToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("RoleId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<Guid?>("ShelterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("SoftDeleteAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("VerificationToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("VerifiedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("ShelterId");

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("LapkaBackend.Domain.Entities.Animal", b =>
                {
                    b.HasOne("LapkaBackend.Domain.Entities.AnimalCategory", "AnimalCategory")
                        .WithMany("Animals")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LapkaBackend.Domain.Entities.Shelter", "Shelter")
                        .WithMany("Animals")
                        .HasForeignKey("ShelterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AnimalCategory");

                    b.Navigation("Shelter");
                });

            modelBuilder.Entity("LapkaBackend.Domain.Entities.AnimalView", b =>
                {
                    b.HasOne("LapkaBackend.Domain.Entities.Animal", "Animal")
                        .WithMany("AnimalViews")
                        .HasForeignKey("AnimalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LapkaBackend.Domain.Entities.User", "User")
                        .WithMany("AnimalViews")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Animal");

                    b.Navigation("User");
                });

            modelBuilder.Entity("LapkaBackend.Domain.Entities.ChatMessage", b =>
                {
                    b.HasOne("LapkaBackend.Domain.Entities.ChatRoom", "Room")
                        .WithMany("Messages")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LapkaBackend.Domain.Entities.User", "User")
                        .WithMany("Messages")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Room");

                    b.Navigation("User");
                });

            modelBuilder.Entity("LapkaBackend.Domain.Entities.ChatRoom", b =>
                {
                    b.HasOne("LapkaBackend.Domain.Entities.User", "User1")
                        .WithMany("ChatRoomsAsUser1")
                        .HasForeignKey("User1Id")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("LapkaBackend.Domain.Entities.User", "User2")
                        .WithMany("ChatRoomsAsUser2")
                        .HasForeignKey("User2Id")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("User1");

                    b.Navigation("User2");
                });

            modelBuilder.Entity("LapkaBackend.Domain.Entities.Photo", b =>
                {
                    b.HasOne("LapkaBackend.Domain.Entities.Animal", "Animal")
                        .WithMany("Photos")
                        .HasForeignKey("AnimalId");

                    b.Navigation("Animal");
                });

            modelBuilder.Entity("LapkaBackend.Domain.Entities.Reaction", b =>
                {
                    b.HasOne("LapkaBackend.Domain.Entities.Animal", "Animal")
                        .WithMany("Reactions")
                        .HasForeignKey("AnimalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LapkaBackend.Domain.Entities.User", "User")
                        .WithMany("Reactions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Animal");

                    b.Navigation("User");
                });

            modelBuilder.Entity("LapkaBackend.Domain.Entities.ShelterVolunteering", b =>
                {
                    b.HasOne("LapkaBackend.Domain.Entities.Shelter", "Shelter")
                        .WithOne("ShelterVolunteering")
                        .HasForeignKey("LapkaBackend.Domain.Entities.ShelterVolunteering", "ShelterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Shelter");
                });

            modelBuilder.Entity("LapkaBackend.Domain.Entities.User", b =>
                {
                    b.HasOne("LapkaBackend.Domain.Entities.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LapkaBackend.Domain.Entities.Shelter", "Shelter")
                        .WithMany()
                        .HasForeignKey("ShelterId");

                    b.Navigation("Role");

                    b.Navigation("Shelter");
                });

            modelBuilder.Entity("LapkaBackend.Domain.Entities.Animal", b =>
                {
                    b.Navigation("AnimalViews");

                    b.Navigation("Photos");

                    b.Navigation("Reactions");
                });

            modelBuilder.Entity("LapkaBackend.Domain.Entities.AnimalCategory", b =>
                {
                    b.Navigation("Animals");
                });

            modelBuilder.Entity("LapkaBackend.Domain.Entities.ChatRoom", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("LapkaBackend.Domain.Entities.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("LapkaBackend.Domain.Entities.Shelter", b =>
                {
                    b.Navigation("Animals");

                    b.Navigation("ShelterVolunteering");
                });

            modelBuilder.Entity("LapkaBackend.Domain.Entities.User", b =>
                {
                    b.Navigation("AnimalViews");

                    b.Navigation("ChatRoomsAsUser1");

                    b.Navigation("ChatRoomsAsUser2");

                    b.Navigation("Messages");

                    b.Navigation("Reactions");
                });
#pragma warning restore 612, 618
        }
    }
}
