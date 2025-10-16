using ElectroKart_Api.DTOs.Payments;
using System.Threading.Tasks;

namespace ElectroKart_Api.Services.Payments
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> InitiatePaymentAsync(CreatePaymentRequestDto dto, int userId);
        Task<bool> ConfirmPaymentAsync(PaymentConfirmationDto dto);
    }
}
