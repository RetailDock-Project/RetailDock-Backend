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
        public DbSet<OrganizationRole> OrganizationRoles { get; set; }
        public DbSet<OrganizationRolePermission> OrganizationRolePermissions { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserOrganizationRole> UserOrganizationRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);

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

                entity.HasQueryFilter(r => !r.IsDeleted);

                entity.Property(e => e.OrganizationId)
                    .IsRequired();

                entity.HasMany(e => e.UserOrganizationRoles)
                    .WithOne(e => e.OrganizationRole)
                    .HasForeignKey(e => e.OrganizationRoleId);
            });

            // OrganizationRolePermissions
            modelBuilder.Entity<OrganizationRolePermission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasQueryFilter(p => !p.IsDeleted);

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

                // ✅ One-to-one: User → UserOrganizationRole
                entity.HasOne(e => e.User)
                    .WithOne(u => u.UserOrganizationRole)
                    .HasForeignKey<UserOrganizationRole>(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // ✅ Unique constraint on UserId to enforce 1-to-1 at DB level
                entity.HasIndex(uor => uor.UserId).IsUnique();

                // ✅ Relationship to OrganizationRole (many-to-one is fine here)
                entity.HasOne(e => e.OrganizationRole)
                    .WithMany(or => or.UserOrganizationRoles)
                    .HasForeignKey(e => e.OrganizationRoleId);
            });

        }


    }
}
