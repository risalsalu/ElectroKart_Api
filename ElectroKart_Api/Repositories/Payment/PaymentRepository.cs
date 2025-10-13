using ElectroKart_Api.Data;
using ElectroKart_Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ElectroKart_Api.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;
        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddPaymentAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<Payment?> GetByOrderIdAsync(string orderId)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
        }

        public async Task UpdateStatusAsync(string orderId, string status, string? paymentId = null, string? signature = null)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
            if (payment != null)
            {
                payment.Status = status;
                payment.PaymentId = paymentId;
                payment.Signature = signature;
                await _context.SaveChangesAsync();
            }
        }
    }
}
