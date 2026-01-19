using ElectroKart_Api.Data;
using ElectroKart_Api.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectroKart_Api.Repositories.GenericRepo
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<ApiResponse<T>> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return ApiResponse<T>.SuccessResponse(entity, $"{typeof(T).Name} added successfully");
        }

        public async Task<ApiResponse<T>> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return ApiResponse<T>.SuccessResponse(entity, $"{typeof(T).Name} updated successfully");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"{typeof(T).Name} not found");

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return ApiResponse<bool>.SuccessResponse(true, $"{typeof(T).Name} deleted successfully");
        }

        public async Task<ApiResponse<T?>> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"{typeof(T).Name} not found");

            return ApiResponse<T?>.SuccessResponse(entity);
        }

        public async Task<ApiResponse<IEnumerable<T>>> GetAllAsync()
        {
            var list = await _dbSet.ToListAsync();
            return ApiResponse<IEnumerable<T>>.SuccessResponse(list);
        }
    }
}
    