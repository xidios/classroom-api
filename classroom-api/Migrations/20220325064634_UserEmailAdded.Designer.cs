﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using classroom_api.Services;

#nullable disable

namespace classroom_api.Migrations
{
    [DbContext(typeof(ClassroomapiContext))]
    [Migration("20220325064634_UserEmailAdded")]
    partial class UserEmailAdded
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("classroom_api.Models.CourseModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CourseState")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("DescriptionHeading")
                        .HasColumnType("text");

                    b.Property<string>("GoogleId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Section")
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("SubdivisionModelId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("SubdivisionModelId");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("classroom_api.Models.EmailModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsMain")
                        .HasColumnType("boolean");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("EmailModel");
                });

            modelBuilder.Entity("classroom_api.Models.InvitationModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CourseId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("CourseModelId")
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("GoogleInvitationId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CourseModelId");

                    b.ToTable("Invitations");
                });

            modelBuilder.Entity("classroom_api.Models.PermissionModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("classroom_api.Models.RoleModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("classroom_api.Models.StudentModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AccountId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Students");
                });

            modelBuilder.Entity("classroom_api.Models.SubdivisionModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Subdivisions");
                });

            modelBuilder.Entity("classroom_api.Models.UserModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CourseModelStudentModel", b =>
                {
                    b.Property<Guid>("CoursesId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("StudentsId")
                        .HasColumnType("uuid");

                    b.HasKey("CoursesId", "StudentsId");

                    b.HasIndex("StudentsId");

                    b.ToTable("CourseModelStudentModel");
                });

            modelBuilder.Entity("PermissionModelRoleModel", b =>
                {
                    b.Property<Guid>("PermissionsId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("RolesId")
                        .HasColumnType("uuid");

                    b.HasKey("PermissionsId", "RolesId");

                    b.HasIndex("RolesId");

                    b.ToTable("PermissionModelRoleModel");
                });

            modelBuilder.Entity("RoleModelUserModel", b =>
                {
                    b.Property<Guid>("RolesId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UsersId")
                        .HasColumnType("uuid");

                    b.HasKey("RolesId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("RoleModelUserModel");
                });

            modelBuilder.Entity("SubdivisionModelUserModel", b =>
                {
                    b.Property<Guid>("ModeratorsId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("SubdivisionsId")
                        .HasColumnType("uuid");

                    b.HasKey("ModeratorsId", "SubdivisionsId");

                    b.HasIndex("SubdivisionsId");

                    b.ToTable("SubdivisionModelUserModel");
                });

            modelBuilder.Entity("classroom_api.Models.CourseModel", b =>
                {
                    b.HasOne("classroom_api.Models.SubdivisionModel", null)
                        .WithMany("Courses")
                        .HasForeignKey("SubdivisionModelId");
                });

            modelBuilder.Entity("classroom_api.Models.EmailModel", b =>
                {
                    b.HasOne("classroom_api.Models.UserModel", "User")
                        .WithMany("Emails")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("classroom_api.Models.InvitationModel", b =>
                {
                    b.HasOne("classroom_api.Models.CourseModel", null)
                        .WithMany("Invations")
                        .HasForeignKey("CourseModelId");
                });

            modelBuilder.Entity("CourseModelStudentModel", b =>
                {
                    b.HasOne("classroom_api.Models.CourseModel", null)
                        .WithMany()
                        .HasForeignKey("CoursesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("classroom_api.Models.StudentModel", null)
                        .WithMany()
                        .HasForeignKey("StudentsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PermissionModelRoleModel", b =>
                {
                    b.HasOne("classroom_api.Models.PermissionModel", null)
                        .WithMany()
                        .HasForeignKey("PermissionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("classroom_api.Models.RoleModel", null)
                        .WithMany()
                        .HasForeignKey("RolesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RoleModelUserModel", b =>
                {
                    b.HasOne("classroom_api.Models.RoleModel", null)
                        .WithMany()
                        .HasForeignKey("RolesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("classroom_api.Models.UserModel", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SubdivisionModelUserModel", b =>
                {
                    b.HasOne("classroom_api.Models.UserModel", null)
                        .WithMany()
                        .HasForeignKey("ModeratorsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("classroom_api.Models.SubdivisionModel", null)
                        .WithMany()
                        .HasForeignKey("SubdivisionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("classroom_api.Models.CourseModel", b =>
                {
                    b.Navigation("Invations");
                });

            modelBuilder.Entity("classroom_api.Models.SubdivisionModel", b =>
                {
                    b.Navigation("Courses");
                });

            modelBuilder.Entity("classroom_api.Models.UserModel", b =>
                {
                    b.Navigation("Emails");
                });
#pragma warning restore 612, 618
        }
    }
}
