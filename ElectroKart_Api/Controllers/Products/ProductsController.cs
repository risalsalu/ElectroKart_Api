using ElectroKart_Api.DTOs.Products;
using ElectroKart_Api.Models;
using ElectroKart_Api.Services;
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
        private readonly ICloudinaryService _cloudinaryService;

        public ProductsController(IProductService productService, ICloudinaryService cloudinaryService)
        {
            _productService = productService;
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost]
        [Authorize]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto dto, IFormFile? imageFile)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (imageFile != null)
            {
                var upload = await _cloudinaryService.UploadImageAsync(imageFile);
                dto.ImageUrl = upload.SecureUrl.ToString();
                dto.ImagePublicId = upload.PublicId;
            }

            var created = await _productService.CreateProductAsync(dto);
            if (!created.Success) return BadRequest(created.Message);

            var productResp = await _productService.GetProductByIdAsync(created.Data!.Id);
            if (!productResp.Success || productResp.Data == null) return NotFound(productResp.Message);

            return CreatedAtAction(nameof(GetProductById), new { id = productResp.Data.Id }, productResp.Data);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] UpdateProductDto dto, IFormFile? imageFile)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != dto.Id) return BadRequest("Product ID mismatch");

            if (imageFile != null)
            {
                var upload = await _cloudinaryService.UploadImageAsync(imageFile);
                dto.ImageUrl = upload.SecureUrl.ToString();
                dto.ImagePublicId = upload.PublicId;
            }

            var updateResp = await _productService.UpdateProductAsync(dto);
            if (!updateResp.Success) return BadRequest(updateResp.Message);

            return Ok(updateResp.Data);
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var resp = await _productService.DeleteProductAsync(id);
            if (!resp.Success) return BadRequest(resp.Message);

            return Ok(new { message = "Product deleted successfully" });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var resp = await _productService.GetAllProductsAsync();
            if (!resp.Success) return BadRequest(resp.Message);
            return Ok(resp.Data);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var resp = await _productService.GetProductByIdAsync(id);
            if (!resp.Success || resp.Data == null) return NotFound(resp.Message);
            return Ok(resp.Data);
        }

        [HttpGet("category/{categoryId:int}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var resp = await _productService.GetProductsByCategoryIdAsync(categoryId);
            if (!resp.Success) return BadRequest(resp.Message);
            return Ok(resp.Data);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] ProductSearchDto dto)
        {
            var resp = await _productService.SearchProductsAsync(dto);
            if (!resp.Success) return BadRequest(resp.Message);
            return Ok(resp.Data);
        }
    }
}
