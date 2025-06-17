using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class IdentityDbContext : DbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<OrganizationRole> OrganizationRoles { get; set; }
        public DbSet<OrganizationRolePermission> OrganizationRolePermissions { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserOrganizationRole> UserOrganizationRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Role
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasMany(e => e.OrganizationRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(e => e.RoleId);
            });

            // Permission
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasMany(p => p.OrganizationRolePermissions)
                      .WithOne(orp => orp.Permission)
                      .HasForeignKey(orp => orp.PermissionId);
            });

            // OrganizationRole
            modelBuilder.Entity<OrganizationRole>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.OrganizationId)
                    .IsRequired();

                entity.HasOne(e => e.Role)
                    .WithMany(r => r.OrganizationRoles)
                    .HasForeignKey(e => e.RoleId);

                entity.HasMany(e => e.OrganizationRolePermissions)
                    .WithOne(e => e.OrganizationRole)
                    .HasForeignKey(e => e.OrganizationRoleId);

                entity.HasMany(e => e.UserOrganizationRoles)
                    .WithOne(e => e.OrganizationRole)
                    .HasForeignKey(e => e.OrganizationRoleId);
            });

            // OrganizationRolePermissions
            modelBuilder.Entity<OrganizationRolePermission>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.OrganizationRoleId).IsRequired();
                entity.Property(e => e.PermissionId).IsRequired();

                entity.HasOne(e => e.OrganizationRole)
                    .WithMany(or => or.OrganizationRolePermissions)
                    .HasForeignKey(e => e.OrganizationRoleId);

                entity.HasOne(e => e.Permission)
                    .WithMany(p => p.OrganizationRolePermissions)
                    .HasForeignKey(e => e.PermissionId);
            });

            // UserOrganizationRole
            modelBuilder.Entity<UserOrganizationRole>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.OrganizationRoleId).IsRequired();

                entity.HasOne(e => e.User)
                    .WithMany(u=>u.userOrganizationRole).HasForeignKey(x=>x.UserId); // Add navigation in User class if needed


                entity.HasOne(e => e.OrganizationRole)
                    .WithMany(or => or.UserOrganizationRoles)
                    .HasForeignKey(e => e.OrganizationRoleId);
            });
        }


    }
}
