using ElectroKart_Api.Data;
using ElectroKart_Api.Helpers;
using ElectroKart_Api.Repositories.GenericRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectroKart_Api.Repositories
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
            try
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
                return ApiResponse<T>.SuccessResponse(entity, $"{typeof(T).Name} added successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<T>.FailureResponse($"{typeof(T).Name} add failed", ex.Message);
            }
        }

        public async Task<ApiResponse<T>> UpdateAsync(T entity)
        {
            try
            {
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
                return ApiResponse<T>.SuccessResponse(entity, $"{typeof(T).Name} updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<T>.FailureResponse($"{typeof(T).Name} update failed", ex.Message);
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null)
                    return ApiResponse<bool>.FailureResponse($"{typeof(T).Name} not found");

                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
                return ApiResponse<bool>.SuccessResponse(true, $"{typeof(T).Name} deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResponse($"{typeof(T).Name} delete failed", ex.Message);
            }
        }

        public async Task<ApiResponse<T?>> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null)
                    return ApiResponse<T?>.FailureResponse($"{typeof(T).Name} not found");

                return ApiResponse<T?>.SuccessResponse(entity);
            }
            catch (Exception ex)
            {
                return ApiResponse<T?>.FailureResponse($"{typeof(T).Name} fetch failed", ex.Message);
            }
        }

        public async Task<ApiResponse<IEnumerable<T>>> GetAllAsync()
        {
            try
            {
                var list = await _dbSet.ToListAsync();
                return ApiResponse<IEnumerable<T>>.SuccessResponse(list);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<T>>.FailureResponse($"{typeof(T).Name} fetch failed", ex.Message);
            }
        }
    }
}
