using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _appDbContext;
        public ProductRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<Product> AddProduct(Product product)
        {


            if (product.Images != null && product.Images.Count > 0)
            {
                Console.WriteLine("Images:");
                foreach (var img in product.Images)
                {
                    Console.WriteLine($"\t{img.ToString()}");
                }
            }
            else
            {
                Console.WriteLine("Images: null or empty");
            }

            await _appDbContext.Products.AddAsync(product);
            await _appDbContext.SaveChangesAsync();
            return product;
        }
        public async Task<bool> ProductExistsAsync(string productName, string productCode)
        {
            return await _appDbContext.Products
                .AnyAsync(p => p.ProductName == productName || p.ProductCode == productCode);
        }
        public async Task<List<Product>> GetAllProducts(Guid OrganizationId)
        {
            return await _appDbContext.Products
                  .Include(p => p.Images)
                  .Include(p => p.UnitOfMeasures)
                  .Include(p => p.Category)
                  .Include(p => p.HsnCode)
                  .Where(p => !p.IsDeleted && p.OrgnaisationId == OrganizationId)
                  .ToListAsync();
        }

        public async Task<Product> GetProductById(Guid id)
        {
            return await _appDbContext.Products
                .Include(p => p.UnitOfMeasures)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.HsnCode)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }
        public async Task<Product> UpdateProduct(Product product)
        {
            //var exists = await _appDbContext.Products.AnyAsync(p => p.Id == product.Id);
            //if (!exists) return null;

            _appDbContext.Products.Update(product);
            await _appDbContext.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteProduct(Guid id)
        {
            var product = await _appDbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) return false;

            product.IsDeleted = true;
            _appDbContext.Products.Update(product);
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        public async Task<List<Product>> GetLowStock(Guid OrganizationId)
        {
            var products = await _appDbContext.Products.Where(x => x.OrgnaisationId == OrganizationId && x.Stock <= x.ReOrderLevel).ToListAsync();
            return products;
        }
        public async Task<List<Product>> GetProductByCategory(int categoryId, Guid organizationId)
        {
            var categoryItems = await _appDbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.UnitOfMeasures)
                .Where(p => p.ProductCategoryId == categoryId && p.OrgnaisationId == organizationId)
                .ToListAsync();

            return categoryItems;
        }
        public async Task<List<Product>> SearchProducts(Guid organizationId, int? categoryId, string searchTerm)
        {
            searchTerm = searchTerm?.Trim().ToLower(); 

            var query = _appDbContext.Products
                .Where(p => p.OrgnaisationId == organizationId &&
                            (string.IsNullOrEmpty(searchTerm) ||
                             p.ProductName.ToLower().Contains(searchTerm) ||  
                             p.ProductCode.ToLower().Contains(searchTerm)));

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.ProductCategoryId == categoryId.Value);
            }

            return await query.ToListAsync();
        }
        public async Task<ProductStatisticsDto> GetProductStatisticsAsync(Guid organizationId)
        {
            var products = await _appDbContext.Products
                .Where(p => !p.IsDeleted && p.OrgnaisationId == organizationId)
                .ToListAsync();

            return new ProductStatisticsDto
            {
                TotalProducts = products.Count,
                LowStockItems = products.Count(p => p.Stock <= p.ReOrderLevel),
                InventoryValue = products.Sum(p => p.Stock * p.CostPrice),  
               
            };
        }

        public async Task<bool> ProductStockUpdate(ProductStockUpdateDto updateData) {
            var product = await _appDbContext.Products.FirstOrDefaultAsync(p => p.Id == updateData.ProductId && p.OrgnaisationId == updateData.OrgId);
            if (product == null) {
                return false;
            }
            product.LastStockUpdate = DateTime.UtcNow;
            if (updateData.Increase) {
                product.Stock += updateData.Quantity;
            }
            product.Stock -= updateData.Quantity;
            return true;

        }
    }
    }
