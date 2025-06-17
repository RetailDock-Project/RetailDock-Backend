using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly AppDbContext _context;

        public ProductCategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductCategory> AddCategory(ProductCategory productCategory)
        {
            await _context.ProductCategories.AddAsync(productCategory);
            await _context.SaveChangesAsync();
            return productCategory;
        }

        public async Task<List<ProductCategory>> GetAllCategory(Guid OrganaiztionId)
        {
            return await _context.ProductCategories
                .Where(x=>!x.IsDeleted && x.OrgnaisationId==OrganaiztionId)
                .ToListAsync();
        }

        public async Task<ProductCategory> GetCategoryById(int id)
        {
            return await _context.ProductCategories.FirstOrDefaultAsync(x=>x.ProductCategoryId==id && !x.IsDeleted);
        }

        public async Task<ProductCategory> UpdateCategory(ProductCategory productCategory)
        {
            _context.ProductCategories.Update(productCategory);
            await _context.SaveChangesAsync();
            return productCategory;
        }
        //public async Task<bool> DeleteCategory(int id)
        //{
        //    var category = await _context.ProductCategories.FindAsync(id);
        //    if (category == null)
        //        return false;

        //    _context.ProductCategories.Remove(category);
        //    await _context.SaveChangesAsync();
        //    return true;
        //}

        public async Task<ProductCategory> DeleteCategory(ProductCategory category)
        {
            var existingCategory = await _context.ProductCategories
                .FirstOrDefaultAsync(x => x.ProductCategoryId == category.ProductCategoryId);

            if (existingCategory == null)
                return null;

            existingCategory.IsDeleted = true;
            _context.ProductCategories.Update(existingCategory);
            await _context.SaveChangesAsync();

            return existingCategory;
        }

    }
}
