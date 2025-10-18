using ElectroKart_Api.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectroKart_Api.Repositories.GenericRepo
{
    public interface IGenericRepository<T> where T : class
    {
        Task<ApiResponse<T>> AddAsync(T entity);
        Task<ApiResponse<T>> UpdateAsync(T entity);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        Task<ApiResponse<T?>> GetByIdAsync(int id);
        Task<ApiResponse<IEnumerable<T>>> GetAllAsync();
    }
}
