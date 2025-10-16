using ElectroKart_Api.Data;
using ElectroKart_Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ElectroKart_Api.Repositories.Payments
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment?> GetPaymentByPaymentIdAsync(string paymentId)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        }

        public async Task<Payment?> GetPaymentByOrderIdAsync(int orderId)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId.ToString());
        }

        public async Task UpdatePaymentStatusAsync(Payment payment, string status)
        {
            payment.Status = status;
            payment.UpdatedAt = DateTime.UtcNow;
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }
    }
}
