using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.AppDbContext
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options) { }
        public DbSet<OrganizationDetails> OrganizationDetail { get; set; }
        public DbSet<Subscriptions> Subscription {  get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrganizationDetails>()
        .HasOne(o => o.Subscriptions)
        .WithOne(s => s.OrganizationDetails)
        .HasForeignKey<Subscriptions>(s => s.OrganizationId);
        }
    }
}
