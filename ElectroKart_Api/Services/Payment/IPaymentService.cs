using ElectroKart_Api.DTOs.Payment;
using System.Threading.Tasks;

namespace ElectroKart_Api.Services.Payment
{
    public interface IPaymentService
    {
        Task<object> CreateOrderAsync(PaymentRequestDto request);
        Task<bool> VerifyPaymentAsync(VerifyPaymentDto request);
    }
}
