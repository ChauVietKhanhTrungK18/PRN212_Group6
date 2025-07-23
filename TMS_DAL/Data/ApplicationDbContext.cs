using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMS_DAL.Model;

namespace TMS_DAL.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public ApplicationDbContext() { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ProjectRole> ProjectRoles { get; set; }
        public DbSet<Attachment> Attachments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var config = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();
                var connectionString = config.GetConnectionString("DefaultConnectionStringDB");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Connection string is not initialized.");
                }
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.UserId);
            modelBuilder.Entity<Role>().HasKey(r => r.RoleId);
            modelBuilder.Entity<Project>().HasKey(p => p.ProjectId);
            modelBuilder.Entity<ProjectTask>().HasKey(t => t.TaskId);
            modelBuilder.Entity<Notification>().HasKey(n => n.NotificationId);
            modelBuilder.Entity<ProjectRole>().HasKey(pr => pr.ProjectRoleId);
            modelBuilder.Entity<Attachment>().HasKey(a => a.AttachmentId);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Username = "admin",
                    FullName = "Admin User",
                    Email = "admin@example.com",
                    PasswordHash = "0192023a7bbd73250516f069df18b500c33910a2d4d4074ec5b0d1b0e5b0c1c6",
                    DateCreated = DateTime.Now,
                    IsAdmin = true
                },
                new User
                {
                    UserId = 2,
                    Username = "user",
                    FullName = "Normal User",
                    Email = "user@example.com",
                    PasswordHash = "6ad14ba9986e3615423dfca256d04e3f735357c3558b8b4319c49a23ea8bc99c", 
                    DateCreated = DateTime.Now,
                    IsAdmin = false
                }
            );

            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Project Manager", Description = "Manages the project and team members" },
                new Role { RoleId = 2, RoleName = "Member", Description = "Project team member" },
                new Role { RoleId = 3, RoleName = "Viewer", Description = "Can view project information only" }
            );

            modelBuilder.Entity<ProjectRole>()
                .HasOne(pr => pr.Project)
                .WithMany(p => p.ProjectRoles)
                .HasForeignKey(pr => pr.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectRole>()
                .HasOne(pr => pr.User)
                .WithMany(u => u.ProjectRoles)
                .HasForeignKey(pr => pr.UserId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<ProjectRole>()
                .HasOne(pr => pr.Role)
                .WithMany(r => r.ProjectRoles)
                .HasForeignKey(pr => pr.RoleId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<ProjectTask>()
                .HasOne(t => t.Project)
                .WithMany(p => p.ProjectTasks)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<ProjectTask>()
                .HasOne(t => t.AssignedUser)
                .WithMany(u => u.ProjectTasks)
                .HasForeignKey(t => t.AssignedTo)
                .OnDelete(DeleteBehavior.Restrict); 

            base.OnModelCreating(modelBuilder);
        }
    }

  }
