using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> AddProduct(Product product);
        Task<Product> UpdateProduct(Product product);
        Task<Product> GetProductById(Guid id);
        Task<List<Product>> GetAllProducts(Guid OrganizationId);
        Task<bool> DeleteProduct(Guid id);
        Task<List<Product>>GetLowStock(Guid OrganizationId);
        Task<bool> ProductExistsAsync(string productName, string productCode);
        Task<List<Product>> GetProductByCategory(int CategoryId,Guid OrganizationId);
        Task<List<Product>> SearchProducts(Guid organizationId, int? categoryId, string searchTerm);
        Task<ProductStatisticsDto> GetProductStatisticsAsync(Guid organizationId);
    }
}
