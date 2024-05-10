
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignment.Api.Models;

namespace Assignment.Infrastructure
{
    public class RaidenDBContext : DbContext
    {
        public RaidenDBContext(RaceViewContext options) : base()
        {

        }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<Actions> Actions { get; set; }
        public DbSet<RoleActionPermission> RoleActionPermission { get; set; }
        public DbSet<Organizations> Organizations { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<OrganizationUsers> OrganizationUsers { get; set; }
        public DbSet<Applications> Applications { get; set; }
        public DbSet<ApplicationUsers> ApplicationUsers { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<ProductsApplication> ProductsApplications { get; set; }
        public DbSet<AllowedDomains> AllowedDomains { get; set; }
        public DbSet<GoogleSignIn> GoogleSignIn { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Organizations>()
            .HasIndex(o => o.OrgCode)
            .IsUnique();


            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Application)
                .WithMany()  // Assuming there's a navigation property in Applications pointing back to UserRoles
                .HasForeignKey(ur => ur.ApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the relationship between UserRole and Users
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany()  // Assuming there's a navigation property in Users pointing back to UserRoles
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the relationship between UserRole and Roles
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany()  // Assuming there's a navigation property in Roles pointing back to UserRoles
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<UserRole>()
               .HasOne(ur => ur.Organization)
               .WithMany()  // Assuming there's a navigation property in Organizations pointing back to UserRoles
               .HasForeignKey(ur => ur.OrganizationId)
               .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProductsApplication>()
               .HasOne(ur => ur.Products)
               .WithMany()  // Assuming there's a navigation property in Organizations pointing back to UserRoles
               .HasForeignKey(ur => ur.ProductId)
               .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Roles>().HasData(
                new Roles { RoleId = 1, RoleName = "SuperAdmin", CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new Roles { RoleId = 2, RoleName = "OrgAdmin", CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new Roles { RoleId = 3, RoleName = "AppAdmin", CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now }
            );
            modelBuilder.Entity<Permissions>().HasData(
                new Permissions { PermissionId = 1, PermissionName = "view", CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new Permissions { PermissionId = 2, PermissionName = "create", CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new Permissions { PermissionId = 3, PermissionName = "edit", CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new Permissions { PermissionId = 4, PermissionName = "delete", CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new Permissions { PermissionId = 5, PermissionName = "enable", CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new Permissions { PermissionId = 6, PermissionName = "disable", CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now }
                );
            modelBuilder.Entity<Users>().HasData(
                new Users
                {
                    UserId = 1,
                    Password = Environment.GetEnvironmentVariable("PASSWORD1"),
                    Email = "Spurtree@example.com",
                    FirstName = "Spurtree",
                    IsActive = true
                },
                new Users
                {
                    UserId = 2,
                    Password = Environment.GetEnvironmentVariable("PASSWORD2"),
                    Email = "Abhay@example.com",
                    FirstName = "Abhay",
                    IsActive = true
                },
                new Users
                {
                    UserId = 3,
                    Password = Environment.GetEnvironmentVariable("PASSWORD3"),
                    Email = "Bhoomi@example.com",
                    FirstName = "Bhoomi",
                    IsActive = true
                },
                 new Users
                 {
                     UserId = 4,
                     Password = Environment.GetEnvironmentVariable("PASSWORD4"),
                     Email = "pavan.m@spurtreetech.com",
                     FirstName = "pavan",
                     IsActive = true
                 });
            modelBuilder.Entity<Actions>().HasData(
     new Actions { ActionId = 1, ActionName = "raiden-settings", CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
     new Actions { ActionId = 2, ActionName = "orgsetting", CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
     new Actions { ActionId = 3, ActionName = "appsetting", CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
     new Actions { ActionId = 4, ActionName = "user", CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
     new Actions { ActionId = 5, ActionName = "org-notification-setting", CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
     new Actions { ActionId = 6, ActionName = "app-notification-setting", CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
     new Actions { ActionId = 7, ActionName = "org-auditlog-setting", CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
     new Actions { ActionId = 8, ActionName = "app-auditlog-setting", CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
     new Actions { ActionId = 9, ActionName = "email-event", CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
     new Actions { ActionId = 10, ActionName = "auditlog-event", CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
     new Actions { ActionId = 11, ActionName = "emailtemplate", CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
     new Actions { ActionId = 12, ActionName = "auditlog", CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now });


            modelBuilder.Entity<Organizations>().HasData(
                new Organizations
                {
                    OrganizationId = 1,
                    OrgCode = "CD01",
                    OrganizationName = "Organization A",
                    OrganizationEmail = "orgA@example.com",
                    OrganizationPhone = "1234567890",
                    CreatedTimestamp = DateTime.Now,
                    ModifiedTimestamp = DateTime.Now,
                    IsActive = true
                },
                new Organizations
                {
                    OrganizationId = 2,
                    OrgCode = "AB01",
                    OrganizationName = "Organization B",
                    OrganizationEmail = "orgB@example.com",
                    OrganizationPhone = "9876543210",
                    CreatedTimestamp = DateTime.Now,
                    ModifiedTimestamp = DateTime.Now,
                    IsActive = true
                },
                new Organizations
                {
                    OrganizationId = 3,
                    OrgCode = "DC02",
                    OrganizationName = "Organization C",
                    OrganizationEmail = "orgC@example.com",
                    OrganizationPhone = "5555555555",
                    CreatedTimestamp = DateTime.Now,
                    ModifiedTimestamp = DateTime.Now,
                    IsActive = true
                },
                new Organizations
                {
                    OrganizationId = 8,
                    OrgCode = "STT01",
                    OrganizationName = "Spurtree",
                    OrganizationEmail = "spurtree2023@gmail.com",
                    OrganizationPhone = "5555555555",
                    CreatedTimestamp = DateTime.Now,
                    ModifiedTimestamp = DateTime.Now,
                    IsActive = true
                }
            );

            modelBuilder.Entity<OrganizationUsers>().HasData(
                new OrganizationUsers { OrgUserId = 1, UserId = 1, OrganizationId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now, Orgcode = "CD01", IsActive = true },
                new OrganizationUsers { OrgUserId = 2, UserId = 2, OrganizationId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now, Orgcode = "CD01", IsActive = true },
                new OrganizationUsers { OrgUserId = 3, UserId = 2, OrganizationId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now, Orgcode = "AB01", IsActive = true },
                new OrganizationUsers { OrgUserId = 4, UserId = 4, OrganizationId = 8, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now, Orgcode = "STT01", IsActive = true }
            );

            modelBuilder.Entity<Applications>().HasData(
               new Applications { ApplicationId = 1, ApplicationName = "BIRA", Phone = "07601012345", OrganizationId = 1, AppCode = "BD420", ClientId = "8ff708940da64155a374154518d0191e", ClientSecret = "aWwseuUhomz5zlyJB6HfSqPKCAEoRP2erOGhM2lUNzA=", IsActive = true, FirstName = "Tony", LastName = "Stark" },
               new Applications { ApplicationId = 2, ApplicationName = "JACK DANIALS", Phone = "1800208663", OrganizationId = 3, AppCode = "JD420", ClientId = "8ff708940da64155a374154518d0000e", ClientSecret = "aWwseuUhomz5zlyJB6HfSqPKCAEoRP2erOGhM2lUccc=", IsActive = true, FirstName = "JACK", LastName = "SPARROW" },
               new Applications { ApplicationId = 8, ApplicationName = "CYRAX", Phone = "0987654321", OrganizationId = 8, AppCode = "Cyrax01", ClientId = "23ec653fa43f44f58f1e4ece3b673685", ClientSecret = "A8zvcp1ZkNJk4Sy1dzFkULSvYIGOPCFYdLFA4XbD4Ig=", IsActive = true, FirstName = "Cyrax", LastName = "Service", ApplicationEmail = "stlabmse@gmail.com", Description = "Send notifications" },
               new Applications { ApplicationId = 9, ApplicationName = "KITANA", Phone = "0987654321", OrganizationId = 8, AppCode = "Kitana01", ClientId = "bece4d5ec6bf40bba0396c732c47fd30", ClientSecret = "G9CCFOuRCsOSvQswI4bpgiiht8lIANEtqsQVrEytXQY=", IsActive = true, FirstName = "Kitana", LastName = "Service", ApplicationEmail = "kitana.dev@gmail.com", Description = "Audit Logging" },
               new Applications { ApplicationId = 10, ApplicationName = "RAIDEN", Phone = "9035402732", OrganizationId = 8, AppCode = "Raiden01", ClientId = "4ef085f15f1d488288d22645aeb62ab8", ClientSecret = "mk0BTaz5vuzfuicAgoGZ147GjVPVcDbYKdxpNs4nLpE=", IsActive = true, FirstName = "Raiden", LastName = "Service", ApplicationEmail = "raidendev08@gmail.com", Description = "Centralised authorization and authentication" }
           );
            modelBuilder.Entity<ApplicationUsers>().HasData(
               new ApplicationUsers { AppUserId = 1, UserId = 3, OrganizationId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now, Orgcode = "CD01", ApplicationId = 1, AppCode = "BD420", IsActive = true },
               new ApplicationUsers { AppUserId = 2, UserId = 2, OrganizationId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now, Orgcode = "DC02", ApplicationId = 2, AppCode = "JD420", IsActive = true },
               new ApplicationUsers { AppUserId = 3, UserId = 3, OrganizationId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now, Orgcode = "DC01", ApplicationId = 2, AppCode = "JD420", IsActive = true }
           );
            modelBuilder.Entity<RoleActionPermission>().HasData(
                new RoleActionPermission { OperationId = 1, ActionId = 1, PermissionId = 1, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 2, ActionId = 1, PermissionId = 2, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 3, ActionId = 1, PermissionId = 3, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 4, ActionId = 1, PermissionId = 4, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 5, ActionId = 2, PermissionId = 1, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 6, ActionId = 2, PermissionId = 2, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 7, ActionId = 2, PermissionId = 3, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 8, ActionId = 2, PermissionId = 4, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 9, ActionId = 2, PermissionId = 5, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 10, ActionId = 2, PermissionId = 6, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 11, ActionId = 3, PermissionId = 1, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 12, ActionId = 3, PermissionId = 2, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 13, ActionId = 3, PermissionId = 3, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 14, ActionId = 3, PermissionId = 4, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 15, ActionId = 3, PermissionId = 5, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 16, ActionId = 3, PermissionId = 6, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 17, ActionId = 4, PermissionId = 1, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 18, ActionId = 4, PermissionId = 2, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 19, ActionId = 4, PermissionId = 3, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 20, ActionId = 4, PermissionId = 4, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 21, ActionId = 4, PermissionId = 5, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 22, ActionId = 4, PermissionId = 6, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 23, ActionId = 5, PermissionId = 1, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 24, ActionId = 5, PermissionId = 2, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 25, ActionId = 5, PermissionId = 3, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 26, ActionId = 5, PermissionId = 4, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 27, ActionId = 6, PermissionId = 1, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 28, ActionId = 6, PermissionId = 2, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 29, ActionId = 6, PermissionId = 3, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 30, ActionId = 6, PermissionId = 4, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 31, ActionId = 7, PermissionId = 1, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 32, ActionId = 7, PermissionId = 2, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 33, ActionId = 7, PermissionId = 3, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 34, ActionId = 7, PermissionId = 4, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 35, ActionId = 8, PermissionId = 1, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 36, ActionId = 8, PermissionId = 2, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 37, ActionId = 8, PermissionId = 3, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 38, ActionId = 8, PermissionId = 4, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 39, ActionId = 9, PermissionId = 1, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 40, ActionId = 9, PermissionId = 2, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 41, ActionId = 9, PermissionId = 3, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 42, ActionId = 9, PermissionId = 4, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 43, ActionId = 10, PermissionId = 1, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 44, ActionId = 10, PermissionId = 2, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 45, ActionId = 10, PermissionId = 3, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 46, ActionId = 10, PermissionId = 4, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 47, ActionId = 11, PermissionId = 1, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 48, ActionId = 11, PermissionId = 2, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 49, ActionId = 11, PermissionId = 3, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 50, ActionId = 11, PermissionId = 4, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 51, ActionId = 12, PermissionId = 1, RoleId = 1, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 52, ActionId = 1, PermissionId = 1, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 53, ActionId = 2, PermissionId = 1, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 54, ActionId = 2, PermissionId = 2, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 55, ActionId = 2, PermissionId = 3, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 56, ActionId = 2, PermissionId = 4, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 57, ActionId = 2, PermissionId = 5, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 58, ActionId = 2, PermissionId = 6, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 59, ActionId = 3, PermissionId = 1, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 60, ActionId = 3, PermissionId = 2, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 61, ActionId = 3, PermissionId = 3, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 62, ActionId = 3, PermissionId = 4, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 63, ActionId = 3, PermissionId = 5, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 64, ActionId = 3, PermissionId = 6, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 65, ActionId = 4, PermissionId = 1, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 66, ActionId = 4, PermissionId = 2, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 67, ActionId = 4, PermissionId = 3, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 68, ActionId = 4, PermissionId = 4, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 69, ActionId = 4, PermissionId = 5, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 70, ActionId = 4, PermissionId = 6, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 71, ActionId = 5, PermissionId = 1, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 72, ActionId = 5, PermissionId = 2, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 73, ActionId = 5, PermissionId = 3, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 74, ActionId = 5, PermissionId = 4, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 75, ActionId = 6, PermissionId = 1, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 76, ActionId = 6, PermissionId = 2, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 77, ActionId = 6, PermissionId = 3, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 78, ActionId = 6, PermissionId = 4, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 79, ActionId = 7, PermissionId = 1, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 80, ActionId = 7, PermissionId = 2, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 81, ActionId = 7, PermissionId = 3, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 82, ActionId = 7, PermissionId = 4, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 83, ActionId = 8, PermissionId = 1, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 84, ActionId = 8, PermissionId = 2, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 85, ActionId = 8, PermissionId = 3, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 86, ActionId = 8, PermissionId = 4, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 87, ActionId = 9, PermissionId = 1, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 88, ActionId = 9, PermissionId = 2, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 89, ActionId = 9, PermissionId = 3, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 90, ActionId = 9, PermissionId = 4, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 91, ActionId = 10, PermissionId = 1, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 92, ActionId = 10, PermissionId = 2, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 93, ActionId = 10, PermissionId = 3, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 94, ActionId = 10, PermissionId = 4, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 95, ActionId = 11, PermissionId = 1, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 96, ActionId = 11, PermissionId = 2, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 97, ActionId = 11, PermissionId = 3, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 98, ActionId = 11, PermissionId = 4, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 99, ActionId = 12, PermissionId = 1, RoleId = 2, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 100, ActionId = 3, PermissionId = 1, RoleId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 101, ActionId = 3, PermissionId = 3, RoleId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 102, ActionId = 4, PermissionId = 1, RoleId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 103, ActionId = 4, PermissionId = 2, RoleId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 104, ActionId = 4, PermissionId = 3, RoleId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 105, ActionId = 4, PermissionId = 4, RoleId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 106, ActionId = 6, PermissionId = 1, RoleId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 107, ActionId = 6, PermissionId = 2, RoleId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 108, ActionId = 6, PermissionId = 3, RoleId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 109, ActionId = 6, PermissionId = 4, RoleId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 110, ActionId = 8, PermissionId = 1, RoleId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 111, ActionId = 8, PermissionId = 2, RoleId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 112, ActionId = 8, PermissionId = 3, RoleId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 113, ActionId = 8, PermissionId = 4, RoleId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 114, ActionId = 9, PermissionId = 1, RoleId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 115, ActionId = 9, PermissionId = 2, RoleId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 116, ActionId = 9, PermissionId = 3, RoleId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 117, ActionId = 9, PermissionId = 4, RoleId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 118, ActionId = 10, PermissionId = 1, RoleId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 119, ActionId = 10, PermissionId = 2, RoleId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 120, ActionId = 10, PermissionId = 3, RoleId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 121, ActionId = 10, PermissionId = 4, RoleId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 122, ActionId = 11, PermissionId = 1, RoleId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 123, ActionId = 11, PermissionId = 2, RoleId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 124, ActionId = 11, PermissionId = 3, RoleId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
                new RoleActionPermission { OperationId = 125, ActionId = 11, PermissionId = 4, RoleId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },

                new RoleActionPermission { OperationId = 126, ActionId = 12, PermissionId = 1, RoleId = 3, CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now }

            );

            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { UserRoleId = 1, UserId = 1, RoleId = 1, OrganizationId = null, ApplicationId = null },
                new UserRole { UserRoleId = 2, UserId = 2, RoleId = 2, OrganizationId = 1, ApplicationId = null },
                new UserRole { UserRoleId = 3, UserId = 2, RoleId = 3, OrganizationId = 3, ApplicationId = 2 },
                new UserRole { UserRoleId = 4, UserId = 3, RoleId = 3, OrganizationId = 1, ApplicationId = 1 },
                new UserRole { UserRoleId = 5, UserId = 2, RoleId = 2, OrganizationId = 2, ApplicationId = null },
                new UserRole { UserRoleId = 6, UserId = 3, RoleId = 3, OrganizationId = 2, ApplicationId = null },
                new UserRole { UserRoleId = 7, UserId = 4, RoleId = 2, OrganizationId = 8, ApplicationId = null },
                new UserRole { UserRoleId = 8, UserId = 4, RoleId = 1, OrganizationId = null, ApplicationId = null }
                );
            modelBuilder.Entity<AllowedDomains>().HasData(
               new AllowedDomains { AllowedDomainId = 1, Domain = "spurtreetech.com", OrganizationId = null, ApplicationId = null,IsActive=true },
               new AllowedDomains { AllowedDomainId = 2, Domain = "spurtreetech.in", OrganizationId = null, ApplicationId = null, IsActive=true}
               );
            modelBuilder.Entity<Products>().HasData(
               new Products { ProductId= 1, ProductName= "KITANA", CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now },
               new Products { ProductId= 2, ProductName= "CYRAX", CreatedTimestamp = DateTime.Now, ModifiedTimestamp = DateTime.Now }
               );
            modelBuilder.Entity<ProductsApplication>().HasData(
               new ProductsApplication { ProductAppId = 1, ApplicationId = 1,IsEnabled=true, ProductId=1 },
               new ProductsApplication { ProductAppId = 2, ApplicationId = 1,IsEnabled=true, ProductId=2 },
               new ProductsApplication { ProductAppId = 3, ApplicationId = 2,IsEnabled=true, ProductId=1 },
               new ProductsApplication { ProductAppId = 4, ApplicationId = 2,IsEnabled=true, ProductId=2 },
               new ProductsApplication { ProductAppId = 5, ApplicationId = 8,IsEnabled=true, ProductId=1 },
               new ProductsApplication { ProductAppId = 6, ApplicationId = 8,IsEnabled=true, ProductId=2 },
               new ProductsApplication { ProductAppId = 7, ApplicationId = 9,IsEnabled=true, ProductId=1 },
               new ProductsApplication { ProductAppId = 8, ApplicationId = 9,IsEnabled=true, ProductId=2 },
               new ProductsApplication { ProductAppId = 9, ApplicationId = 10,IsEnabled=true, ProductId=1 },
               new ProductsApplication { ProductAppId = 10, ApplicationId = 10,IsEnabled=true, ProductId=2 }
               );
        }
    }
}
