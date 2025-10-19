using ElectroKart_Api.DTOs.Products;
using ElectroKart_Api.Helpers;
using ElectroKart_Api.Models;
using ElectroKart_Api.Services.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroKart_Api.Controllers.Products
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        [Authorize]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _productService.CreateProductAsync(dto);
            if (!result.Success) return BadRequest(new { Success = false, Message = result.Message });

            return Ok(new { Success = true, Message = result.Data });
        }

        [HttpPut("{id:int}")]
        [Authorize]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] UpdateProductDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != dto.Id) return BadRequest(new { Success = false, Message = "Product ID mismatch" });

            var result = await _productService.UpdateProductAsync(dto);
            if (!result.Success) return BadRequest(new { Success = false, Message = result.Message });

            return Ok(new { Success = true, Message = result.Data });
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result.Success) return BadRequest(new { Success = false, Message = result.Message });

            return Ok(new { Success = true, Message = "Product deleted successfully" });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            if (!result.Success || result.Data == null) return NotFound(new { Success = false, Message = result.Message });
            return Ok(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = await _productService.GetAllProductsAsync();
            if (!result.Success) return BadRequest(new { Success = false, Message = result.Message });
            return Ok(result.Data);
        }

        [HttpGet("category/{categoryId:int}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var result = await _productService.GetProductsByCategoryIdAsync(categoryId);
            if (!result.Success) return BadRequest(new { Success = false, Message = result.Message });
            return Ok(result.Data);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] ProductSearchDto dto)
        {
            var result = await _productService.SearchProductsAsync(dto);
            if (!result.Success) return BadRequest(new { Success = false, Message = result.Message });
            return Ok(result.Data);
        }
    }
}
