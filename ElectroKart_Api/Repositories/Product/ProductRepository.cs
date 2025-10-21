using ElectroKart_Api.Models;
using ElectroKart_Api.DTOs.Products;
using ElectroKart_Api.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            if (product == null) throw new ArgumentNullException(nameof(product));

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> UpdateAsync(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteAsync(Product product)
        {
            if (product == null) return false;

            _context.Products.Remove(product);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Product>> GetAllAsync(int pageNumber = 1, int pageSize = 20)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 20;

            return await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .OrderByDescending(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Product>> GetByCategoryIdAsync(int categoryId, int pageNumber = 1, int pageSize = 20)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 20;

            return await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Category)
                .AsNoTracking()
                .OrderByDescending(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Product>> SearchProductsAsync(ProductSearchDto searchDto, int pageNumber = 1, int pageSize = 20)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchDto.Name))
                query = query.Where(p =>
                    p.Name.Contains(searchDto.Name) ||
                    p.Description.Contains(searchDto.Name));

            if (searchDto.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == searchDto.CategoryId.Value);

            if (searchDto.MinPrice.HasValue)
                query = query.Where(p => p.Price >= searchDto.MinPrice.Value);

            if (searchDto.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= searchDto.MaxPrice.Value);

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 20;

            return await query
                .AsNoTracking()
                .OrderByDescending(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
