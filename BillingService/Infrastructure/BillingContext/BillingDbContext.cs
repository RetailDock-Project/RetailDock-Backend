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
    public class BillingDbContext : DbContext
    {
        //private readonly Guid _currentOrgId;
        public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options)
        {

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

            modelBuilder.Entity<Product>()
        .HasOne(p => p.Category)
        .WithMany(c => c.Products)
        .HasForeignKey(p => p.ProductCategoryId)
        .OnDelete(DeleteBehavior.Cascade);




            modelBuilder.Entity<Product>()
                .HasOne(p => p.HsnCode)
                .WithMany(h => h.Products)
                .HasForeignKey(p => p.HsnCodeId)
                .OnDelete(DeleteBehavior.Cascade);

          



            //        modelBuilder.Entity<Images>()
            //.Property(i => i.ImageData)
            //.HasColumnType("varbinary(max)");

            //    }









            modelBuilder.Entity<SalesReturn>()
       .HasOne(sr => sr.Sales)
       .WithOne(s => s.SalesReturn)
       .HasForeignKey<SalesReturn>(sr => sr.SaleId);
            modelBuilder.Entity<SalesReturn>().HasMany(sr => sr.SalesReturnItems).WithOne(ri => ri.SalesReturn).HasForeignKey(sr => sr.ReturnId);
            modelBuilder.Entity<Sales>().HasOne(s => s.Invoices).WithOne(i => i.Sales).HasForeignKey<Sales>(s => s.InvoiceId);
            modelBuilder.Entity<Sales>().HasOne(s => s.CreditCustomers).WithMany(cr => cr.Sales).HasForeignKey(s => s.DebtorsId);
            modelBuilder.Entity<Sales>().HasOne(s => s.CashCustomers).WithMany(cs => cs.Sales).HasForeignKey(s => s.CashCustomerId);
            modelBuilder.Entity<Sales>().HasMany(s => s.SaleItems).WithOne(si => si.Sales).HasForeignKey(s => s.SaleId);
            modelBuilder.Entity<SalesReturn>().HasOne(sr => sr.ReturnInvoice).WithOne(sri => sri.SalesReturn).HasForeignKey<SalesReturn>(sr => sr.ReturnInvoiceId);
            modelBuilder.Entity<Sales>()
       .Property(s => s.PaymentType)
       .HasConversion<string>();

            // Store GST_Type enum as string
            modelBuilder.Entity<Sales>()
                .Property(s => s.GST_Type)
                .HasConversion<string>();
            modelBuilder.Entity<SalesReturnInvoice>()
                .Property(sri => sri.PaymentMode)
                .HasConversion<string>();


            //modelBuilder.Entity<PurchaseOrder>(entity=>

            //{ 

            //    entity.has
            //}) 


            modelBuilder.Entity<SaleItems>(entity =>
            {
                entity.HasOne(si => si.UnitOfMeasures)
                .WithMany(um => um.SaleItems)
                .HasForeignKey(si => si.UnitId);

                entity.HasOne(si => si.Products)
                .WithMany(p => p.SaleItems)
                .HasForeignKey(si => si.ProductId);
            });

            modelBuilder.Entity<SalesReturnItems>(entity =>
            {
                entity.HasOne(si => si.UnitOfMeasures)
                .WithMany(um => um.SalesReturnItems)
                .HasForeignKey(sri => sri.UnitId);

                entity.HasOne(sri => sri.Products)
                .WithMany(p => p.SalesReturnItems)
                .HasForeignKey(si => si.ProductId);
            });


        }

    }


}
