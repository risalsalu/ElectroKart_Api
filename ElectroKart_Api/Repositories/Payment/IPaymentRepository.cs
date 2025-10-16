using ElectroKart_Api.Models;
using System.Threading.Tasks;

namespace ElectroKart_Api.Repositories.Payments
{
    public interface IPaymentRepository
    {
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task<Payment?> GetPaymentByPaymentIdAsync(string paymentId);
        Task<Payment?> GetPaymentByOrderIdAsync(int orderId);
        Task UpdatePaymentStatusAsync(Payment payment, string status);
    }
}
