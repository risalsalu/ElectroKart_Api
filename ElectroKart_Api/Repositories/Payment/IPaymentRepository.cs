using ElectroKart_Api.Models;
using System.Threading.Tasks;

namespace ElectroKart_Api.Repositories
{
    public interface IPaymentRepository
    {
        Task AddPaymentAsync(Payment payment);
        Task<Payment?> GetByOrderIdAsync(string orderId);
        Task UpdateStatusAsync(string orderId, string status, string? paymentId = null, string? signature = null);
    }
}
