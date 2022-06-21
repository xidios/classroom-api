using System;
using System.Collections.Generic;
using classroom_api.Models;
using Google.Apis.Classroom.v1.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace classroom_api.Services
{
    public partial class ClassroomapiContext : DbContext
    {
        public ClassroomapiContext()
        {
        }

        public ClassroomapiContext(DbContextOptions<ClassroomapiContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<InvitationModel>()
               .HasOne(p => p.Course)
               .WithMany(b => b.InvitationsOnCourse)
               .HasForeignKey(p => p.CourseId);


            modelBuilder.Entity<InvitationModel>()
                .HasOne(p => p.CourseUser)
                .WithMany(b => b.InvitationsToUser)
                .HasForeignKey(p => p.CourseUserId);
        }
        public DbSet<CourseModel> Courses { get; set; }
        public DbSet<CourseUserModel> CourseUsers { get; set; }
        public DbSet<InvitationModel> Invitations { get; set; }
        public DbSet<SubdivisionModel> Subdivisions { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<RoleModel> Roles { get; set; }
        public DbSet<PermissionModel> Permissions { get; set; }
        public DbSet<EmailModel> Emails { get; set; }
    }
}
