using ElectroKart_Api.Data;
using ElectroKart_Api.Models;
using Microsoft.EntityFrameworkCore;
using ElectroKart_Api.DTOs.Products;

namespace ElectroKart_Api.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Product>> GetByCategoryIdAsync(int categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<List<Product>> SearchProductsAsync(ProductSearchDto searchDto)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchDto.Query))
            {
                query = query.Where(p => p.Name.Contains(searchDto.Query) ||
                                         p.Description.Contains(searchDto.Query));
            }

            if (searchDto.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == searchDto.CategoryId.Value);
            }

            if (searchDto.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= searchDto.MinPrice.Value);
            }

            if (searchDto.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= searchDto.MaxPrice.Value);
            }

            return await query.ToListAsync();
        }
    }
}
