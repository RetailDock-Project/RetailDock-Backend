using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IProductCategoryRepository
    {
        Task<ProductCategory> AddCategory(ProductCategory productCategory);
        Task<List<ProductCategory>> GetAllCategory(Guid OrganaiztionId);

        Task<ProductCategory> GetCategoryById(int id);
        Task<ProductCategory> UpdateCategory(ProductCategory productCategory);
        Task <ProductCategory>DeleteCategory(ProductCategory category);
    }
}
