using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public interface IProductCategoryServices
    {
        Task<Responses<string>> AddproductCategory(ProductCategoryDto productCategoryDto);
        Task<Responses<List<GetProductCategoryDto>>> GetAllCategoryProducts(Guid OrganaiztionId);

        Task<Responses<ProductCategoryDto>> GetCategoryById(int id);
        Task<Responses<string>> UpdateProductCategory(int id,ProductCategoryDto productCategoryDto);
        Task<Responses<string>> DeleteProductCategory(int id);

    }
    public class ProductCategoryServices : IProductCategoryServices
    {

        private readonly IProductCategoryRepository _repository;
        private readonly IMapper _mapper;

        public ProductCategoryServices(IProductCategoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<Responses<string>> AddproductCategory(ProductCategoryDto productCategoryDto)
        {
            try
            {
                var category = _mapper.Map<ProductCategory>(productCategoryDto);
                await _repository.AddCategory(category);
                return new Responses<string> { Message = "Product Category Added", StatusCode = 201 };
            }
            catch (Exception ex)
            {
                return new Responses<string> { Message = $"Error in adding: {ex.Message}", StatusCode = 400 };

            }

        }


        public async Task<Responses<List<GetProductCategoryDto>>> GetAllCategoryProducts(Guid OrganizationId)
        {
            try
            {
                var category = await _repository.GetAllCategory(OrganizationId);
                var data = _mapper.Map<List<GetProductCategoryDto>>(category);
                return new Responses<List<GetProductCategoryDto>> { Message = "Catogories Fetched", StatusCode = 200, Data = data };
            }
            catch (Exception ex)
            {
                return new Responses<List<GetProductCategoryDto>> { StatusCode = 200, Message = $"Error fetching categories: {ex.Message}" };
            }
        }

        public async Task<Responses<ProductCategoryDto>> GetCategoryById(int id)
        {
            try
            {
                var category = await _repository.GetCategoryById(id);
                if (category == null)
                    return new Responses<ProductCategoryDto> { Message = "Category not found", StatusCode = 404 };

                var dto = _mapper.Map<ProductCategoryDto>(category);
                return new Responses<ProductCategoryDto> { Data = dto, Message = "Category fetched", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new Responses<ProductCategoryDto> { Message = $"Error: {ex.Message}", StatusCode = 500 };
            }
        }

        public async Task<Responses<string>> UpdateProductCategory(int id,ProductCategoryDto productCategoryDto)
        {
            try
            {
                var productCategory = await _repository.GetCategoryById(id);
                if(productCategory == null)
                {
                    return new Responses<string> { Message = "Category not found", StatusCode = 400 };
                }
                productCategory.ProductCategoryName = productCategoryDto.ProductCategoryName;
                var result = await _repository.UpdateCategory(productCategory);
                
                return new Responses<string> { Message = "Category updated", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new Responses<string> { Message = $"Error updating category: {ex.Message}", StatusCode = 500 };
            }
        }

        public async Task<Responses<string>> DeleteProductCategory(int id)
        {
            try
            {
                var category = await _repository.GetCategoryById(id);
                if (category == null)
                    return new Responses<string> { Message = "Category not found", StatusCode = 404 };

                
                category.IsDeleted = true;
                await _repository.UpdateCategory(category);

                return new Responses<string> { Message = "Category deleted (soft)", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new Responses<string> { Message = $"Error deleting category: {ex.Message}", StatusCode = 500 };
            }
        }

    }
}
