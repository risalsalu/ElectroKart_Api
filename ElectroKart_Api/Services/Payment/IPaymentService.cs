using ElectroKart_Api.DTOs;
using ElectroKart_Api.DTOs.Payments;
using ElectroKart_Api.Helpers;
using System.Threading.Tasks;

namespace ElectroKart_Api.Services.Payments
{
    public interface IPaymentService
    {
        Task<ApiResponse<PaymentResponseDto>> InitiatePaymentAsync(CreatePaymentRequestDto dto, int userId);
        Task<ApiResponse<string>> ConfirmPaymentAsync(PaymentConfirmationDto dto);
        Task<ApiResponse<PaymentResponseDto>> GetPaymentByOrderIdAsync(int orderId);
    }
}
