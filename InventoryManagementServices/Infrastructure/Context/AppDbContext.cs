using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<UnitOfMeasures> UnitOfMeasures { get; set; }
        public DbSet<Images> Images { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<HsnCode> HsnCodes { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrdersItem { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<PurchaseReturn> PurchaseReturns { get; set; }
        public DbSet<PurchaseReturnInvoice> PurchaseReturnInvoices { get; set; }
        public DbSet<PurchaseReturnItem> PurchaseReturnItems { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        public DbSet<Sales> Sales { get; set; }
        public DbSet<SaleItems> SaleItems { get; set; }
        public DbSet<SalesInvoice> SalesInvoices { get; set; }
        public DbSet<CreditCustomers> CreditCustomers { get; set; }
        public DbSet<CashCustomers> CashCustomers { get; set; }
        public DbSet<SalesReturn> SalesReturn { get; set; }
        public DbSet<SalesReturnItems> SalesReturnItems { get; set; }
        public DbSet<SalesReturnInvoice> SalesReturnInvoice { get; set; }



        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Product>()
        .HasOne(p => p.Category)
        .WithMany(c => c.Products)
        .HasForeignKey(p => p.ProductCategoryId)
        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(p => p.Supplier)
                .WithMany(s=>s.PurchasesOrder)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Cascade);
           

            modelBuilder.Entity<Product>()
                .HasOne(p => p.HsnCode)
                .WithMany(h => h.Products)
                .HasForeignKey(p => p.HsnCodeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasOne(p => p.UnitOfMeasures)
                    .WithMany(u => u.Products)
                    .HasForeignKey(p => p.UnitOfMeasuresId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.SaleItems)
                .WithOne(si => si.Product)
                .HasForeignKey<SaleItems>(si => si.ProductId);

                entity.HasOne(p => p.SalesReturnItems)
                .WithOne(sri => sri.Products)
                .HasForeignKey<SalesReturnItems>(sri => sri.ProductId);
            });
                

            modelBuilder.Entity<Images>()
                .HasOne(i => i.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PurchaseOrder>()
        .HasMany(p => p.PurchaseOrderItems)
        .WithOne(i => i.PurchaseOrder)
        .HasForeignKey(i => i.PurchaseOrderId)
        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PurchaseOrderItem>()
          .HasOne(poi => poi.Product)
          .WithMany(p => p.PurchaseOrderItems)
          .HasForeignKey(poi => poi.ProductId)
          .OnDelete(DeleteBehavior.Restrict);

            //        modelBuilder.Entity<Images>()
            //.Property(i => i.ImageData)
            //.HasColumnType("varbinary(max)");

            //    }


            modelBuilder.Entity<PurchaseItem>()
                .HasOne(pi => pi.Purchase)
                .WithMany(pitm => pitm.PurchaseItems)
                .HasForeignKey(pitm=>pitm.PurchaseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Purchase>(entity => { 
                
                entity.HasOne(p => p.PurchaseInvoice)
                .WithOne(pi => pi.Purchase)
                .HasForeignKey<Purchase>(p => p.PurchaseInvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Supplier)
                .WithMany(s=>s.Purchases)
                .HasForeignKey(p => p.SupplierId);

                //entity.HasOne(p => p.Document)
                //.WithOne()
                //.HasForeignKey<Purchase>(p => p.DocumentId)
                //.IsRequired(false); 

                entity.HasOne(p => p.PurchaseOrder)
                .WithMany(po => po.Purchases)
                .HasForeignKey(p => p.PurchaseOrderId);
              
            
            });
            modelBuilder.Entity<PurchaseItem>()
                .HasOne(pi=>pi.Product)
                .WithMany(p=>p.PurchaseItemItems)
                .HasForeignKey(pi=>pi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseInvoice>(entity =>
            {
                entity.Property(p => p.PaymentMode)
                      .HasConversion<string>();

                entity.Property(p => p.GstType)
                      .HasConversion<string>();
            });

            modelBuilder.Entity<PurchaseReturnInvoice>(entity =>
            {

                entity.Property(p => p.GstType)
                      .HasConversion<string>();
            });

            modelBuilder.Entity<PurchaseReturn>(entity =>
            {
                //entity.ToTable("PurchaseReturns");

                //entity.HasKey(e => e.Id);

                //entity.Property(e => e.Reason)
                //      .HasMaxLength(255)
                //      .IsRequired();

                //entity.Property(e => e.Notes)
                //      .HasMaxLength(500);

                //entity.Property(e => e.CreatedAt)
                //      .HasColumnType("datetime");

                entity.HasOne(e => e.Purchase)
                      .WithMany(p=>p.PurchaseReturns) 
                      .HasForeignKey(e => e.OriginalPurchaseId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.PurchaseReturnInvoice)
                      .WithOne(pri => pri.PurchaseReturn)
                      .HasForeignKey<PurchaseReturn>(e => e.PurchaseReturnInvoiceId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Items)
                      .WithOne(i => i.PurchaseReturn)
                      .HasForeignKey(i => i.PurchaseReturnId);


                entity.HasOne(pr => pr.Supplier)
                .WithMany(s => s.PurchaseReturns)
                .HasForeignKey(pr => pr.SupplierId);

                

            });

            modelBuilder.Entity<PurchaseReturnInvoice>(entity =>
            {
                //entity.ToTable("PurchaseReturnInvoices");

                //entity.HasKey(e => e.Id);

                //entity.Property(e => e.InvoiceNumber)
                //      .IsRequired()
                //      .HasMaxLength(50);

                //entity.HasIndex(e => e.InvoiceNumber)
                //      .IsUnique();

                //entity.Property(e => e.SubTotal).HasColumnType("decimal(18,2)");
                //entity.Property(e => e.TaxAmount).HasColumnType("decimal(18,2)");
                //entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                //entity.Property(e => e.CGST).HasColumnType("decimal(18,2)");
                //entity.Property(e => e.SGST).HasColumnType("decimal(18,2)");
                //entity.Property(e => e.IGST).HasColumnType("decimal(18,2)");
                //entity.Property(e => e.UGST).HasColumnType("decimal(18,2)");

                //entity.Property(e => e.Narration)
                //      .HasMaxLength(500);

                entity.Property(e => e.GstType)
                      .HasConversion<string>(); // stores enum as int

                entity.Property(e => e.ReturnDate)
                      .HasColumnType("datetime");

                entity.Property(e => e.CreatedAt)
                      .HasColumnType("datetime");

                entity.HasOne(e => e.PurchaseInvoice)
                      .WithMany(pi => pi.PurchaseReturnInvoices) 
                      .HasForeignKey(e => e.OriginalPurchaseInvoiceId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PurchaseReturnItem>(entity =>
            {
                //entity.ToTable("PurchaseReturnItems");

                //entity.HasKey(e => e.id);

                //entity.Property(e => e.ReturnedQuantity)
                //      .IsRequired();

                //entity.Property(e => e.ReturnPrice)
                //      .HasColumnType("decimal(18,2)");

                //entity.Property(e => e.TotalAmount)
                //      .HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.Product)
                      .WithMany(p=>p.PurchaseReturnItems) 
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.PurchaseItem)
                      .WithMany() 
                      .HasForeignKey(e => e.OriginalPurchaseItemId)
                      .OnDelete(DeleteBehavior.Restrict);
            });







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
                .WithMany(um=>um.SaleItems)
                .HasForeignKey(si => si.UnitId);
            });

            modelBuilder.Entity<SalesReturnItems>(entity =>
            {
                entity.HasOne(si => si.UnitOfMeasures)
                .WithMany(um=>um.SalesReturnItems)
                .HasForeignKey(sri => sri.UnitId);
            });

        }

    }
}