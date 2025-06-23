using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entites;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.BillingContext
{
    public class BillingDbContext:DbContext
    {
        //private readonly Guid _currentOrgId;
        public BillingDbContext(DbContextOptions <BillingDbContext> options) :base(options){

            //_currentOrgId = new Guid(httpContextAccessor.HttpContext.User.FindFirst("OrgId").Value);
        }

        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<UnitOfMeasures> UnitOfMeasures { get; set; }
 
        public DbSet<Product> Products { get; set; }
        public DbSet<HsnCode> HsnCodes { get; set; }

        public DbSet<Sales> Sales { get; set; }
        public DbSet<SaleItems> SaleItems { get; set; }
        public DbSet<SalesInvoice> SalesInvoices { get; set; }
        public DbSet<CreditCustomers> CreditCustomers { get; set; }
        public DbSet<CashCustomers> CashCustomers { get; set; }
        public DbSet<SalesReturn> SalesReturn { get; set; }
        public DbSet<SalesReturnItems> SalesReturnItems { get; set; }
        public DbSet<SalesReturnInvoice> SalesReturnInvoice { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                modelBuilder.Entity(entity.ClrType).ToTable(entity.GetTableName(), t => t.ExcludeFromMigrations());
            }
        }


    }
}
