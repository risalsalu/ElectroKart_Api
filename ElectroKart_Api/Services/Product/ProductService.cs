using ElectroKart_Api.DTOs.Products;
using ElectroKart_Api.Helpers;
using ElectroKart_Api.Models;
using ElectroKart_Api.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroKart_Api.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly ICloudinaryService _cloudinaryService;

        public ProductService(IProductRepository productRepo, ICloudinaryService cloudinaryService)
        {
            _productRepo = productRepo;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<ApiResponse<string>> CreateProductAsync(CreateProductDto dto)
        {
            if (dto.Image != null)
            {
                var upload = await _cloudinaryService.UploadImageAsync(dto.Image);
                dto.ImageUrl = upload.SecureUrl.ToString();
                dto.ImagePublicId = upload.PublicId;
            }

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl,
                ImagePublicId = dto.ImagePublicId,
                CategoryId = dto.CategoryId,
                IsActive = true
            };

            await _productRepo.CreateAsync(product);
            return ApiResponse<string>.SuccessResponse("Product added successfully.");
        }

        public async Task<ApiResponse<string>> UpdateProductAsync(UpdateProductDto dto)
        {
            var existing = await _productRepo.GetByIdAsync(dto.Id);
            if (existing == null)
                return ApiResponse<string>.FailureResponse("Product not found");

            if (dto.Image != null)
            {
                if (!string.IsNullOrEmpty(existing.ImagePublicId))
                    await _cloudinaryService.DeleteImageAsync(existing.ImagePublicId);

                var upload = await _cloudinaryService.UploadImageAsync(dto.Image);
                existing.ImageUrl = upload.SecureUrl.ToString();
                existing.ImagePublicId = upload.PublicId;
            }

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Price = dto.Price;
            existing.CategoryId = dto.CategoryId;
            existing.IsActive = dto.IsActive;

            await _productRepo.UpdateAsync(existing);
            return ApiResponse<string>.SuccessResponse("Product updated successfully.");
        }

        public async Task<ApiResponse<bool>> DeleteProductAsync(int id)
        {
            var existing = await _productRepo.GetByIdAsync(id);
            if (existing == null)
                return ApiResponse<bool>.FailureResponse("Product not found");

            if (!string.IsNullOrEmpty(existing.ImagePublicId))
                await _cloudinaryService.DeleteImageAsync(existing.ImagePublicId);

            var deleted = await _productRepo.DeleteAsync(existing);
            return ApiResponse<bool>.SuccessResponse(deleted);
        }

        public async Task<ApiResponse<ProductDto?>> GetProductByIdAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null)
                return ApiResponse<ProductDto?>.FailureResponse("Product not found");

            return ApiResponse<ProductDto?>.SuccessResponse(MapToDto(product));
        }

        public async Task<ApiResponse<IEnumerable<ProductDto>>> GetAllProductsAsync()
        {
            var products = await _productRepo.GetAllAsync();
            return ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(products.Select(MapToDto));
        }

        public async Task<ApiResponse<IEnumerable<ProductDto>>> GetProductsByCategoryIdAsync(int categoryId)
        {
            var products = await _productRepo.GetByCategoryIdAsync(categoryId);
            return ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(products.Select(MapToDto));
        }

        public async Task<ApiResponse<IEnumerable<ProductDto>>> SearchProductsAsync(ProductSearchDto dto)
        {
            var products = await _productRepo.SearchProductsAsync(dto);
            return ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(products.Select(MapToDto));
        }

        private ProductDto MapToDto(Product p)
        {
            return new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name ?? "N/A",
                ImageUrl = p.ImageUrl,
                IsActive = p.IsActive
            };
        }
    }
}
