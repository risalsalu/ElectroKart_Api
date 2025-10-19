using ElectroKart_Api.DTOs;
using ElectroKart_Api.DTOs.Payments;
using ElectroKart_Api.Helpers;
using ElectroKart_Api.Models;
using ElectroKart_Api.Repositories.Orders;
using ElectroKart_Api.Repositories.Payments;
using System;
using System.Threading.Tasks;

namespace ElectroKart_Api.Services.Payments
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IOrderRepository _orderRepo;

        public PaymentService(IPaymentRepository paymentRepo, IOrderRepository orderRepo)
        {
            _paymentRepo = paymentRepo;
            _orderRepo = orderRepo;
        }

        public async Task<ApiResponse<PaymentResponseDto>> InitiatePaymentAsync(CreatePaymentRequestDto dto, int userId)
        {
            var response = new ApiResponse<PaymentResponseDto>();

            var order = await _orderRepo.GetOrderByIdAsync(dto.OrderId);
            if (order == null)
            {
                response.Success = false;
                response.Message = "Order not found";
                return response;
            }

            var payment = new Payment
            {
                PaymentId = Guid.NewGuid().ToString(),
                OrderId = order.Id,
                OrderReference = $"ORDER_REF_{order.Id}_{DateTime.UtcNow.Ticks}",
                Amount = dto.Amount,
                Currency = dto.Currency,
                Description = dto.Description,
                Status = "Pending",
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _paymentRepo.CreatePaymentAsync(payment);

            response.Success = true;
            response.Message = "Payment initiated successfully";
            response.Data = new PaymentResponseDto
            {
                PaymentId = created.PaymentId,
                OrderId = created.OrderId,
                OrderReference = created.OrderReference,
                Amount = created.Amount,
                Currency = created.Currency,
                Status = created.Status,
                Description = created.Description,
                CreatedAt = created.CreatedAt
            };

            return response;
        }

        public async Task<ApiResponse<string>> ConfirmPaymentAsync(PaymentConfirmationDto dto)
        {
            var response = new ApiResponse<string>();

            var payment = await _paymentRepo.GetPaymentByPaymentIdAsync(dto.PaymentId);
            if (payment == null)
            {
                response.Success = false;
                response.Message = "Payment not found";
                return response;
            }

            await _paymentRepo.UpdatePaymentStatusAsync(payment, "Success");

            var order = await _orderRepo.GetOrderByIdAsync(payment.OrderId);
            if (order != null)
            {
                order.Status = OrderStatus.Paid;
                await _orderRepo.UpdateOrderStatusAsync(order, OrderStatus.Paid);
            }

            response.Success = true;
            response.Message = "Payment confirmed successfully";
            response.Data = null;

            return response;
        }

        public async Task<ApiResponse<PaymentResponseDto>> GetPaymentByOrderIdAsync(int orderId)
        {
            var response = new ApiResponse<PaymentResponseDto>();

            var payment = await _paymentRepo.GetPaymentByOrderIdAsync(orderId);
            if (payment == null)
            {
                response.Success = false;
                response.Message = "No payment found for this order";
                return response;
            }

            response.Success = true;
            response.Message = "Payment fetched successfully";
            response.Data = new PaymentResponseDto
            {
                PaymentId = payment.PaymentId,
                OrderId = payment.OrderId,
                OrderReference = payment.OrderReference,
                Amount = payment.Amount,
                Currency = payment.Currency,
                Status = payment.Status,
                Description = payment.Description,
                CreatedAt = payment.CreatedAt
            };

            return response;
        }
    }
}
