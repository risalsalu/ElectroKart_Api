using ElectroKart_Api.DTOs;
using ElectroKart_Api.DTOs.Payments;
using ElectroKart_Api.Helpers;
using ElectroKart_Api.Models;
using ElectroKart_Api.Repositories.Orders;
using ElectroKart_Api.Repositories.Payments;
using ElectroKart_Api.Settings;
using Microsoft.Extensions.Options;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectroKart_Api.Services.Payments
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly RazorpaySettings _razorpaySettings;

        public PaymentService(IPaymentRepository paymentRepo, IOrderRepository orderRepo, IOptions<RazorpaySettings> razorpaySettings)
        {
            _paymentRepo = paymentRepo;
            _orderRepo = orderRepo;
            _razorpaySettings = razorpaySettings.Value;
        }

        public async Task<ApiResponse<PaymentResponseDto>> InitiatePaymentAsync(CreatePaymentRequestDto dto, int userId)
        {
            var response = new ApiResponse<PaymentResponseDto>();

            if (string.IsNullOrWhiteSpace(dto.OrderId))
            {
                response.Success = false;
                response.Message = "OrderId cannot be null or empty.";
                return response;
            }

            if (!int.TryParse(dto.OrderId.Replace("order_", ""), out int orderId))
            {
                response.Success = false;
                response.Message = "Invalid order ID format.";
                return response;
            }

            var order = await _orderRepo.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                response.Success = false;
                response.Message = "Order not found.";
                return response;
            }

            var client = new RazorpayClient(_razorpaySettings.KeyId, _razorpaySettings.KeySecret);

            var options = new Dictionary<string, object>
            {
                { "amount", (int)(dto.Amount * 100) },
                { "currency", dto.Currency ?? "INR" },
                { "receipt", $"order_rcptid_{orderId}" },
                { "payment_capture", 1 }
            };

            var razorOrder = client.Order.Create(options);

            var payment = new ElectroKart_Api.Models.Payment
            {
                PaymentId = razorOrder["id"].ToString(),
                OrderId = order.Id,
                OrderReference = razorOrder["receipt"].ToString(),
                Amount = dto.Amount,
                Currency = dto.Currency ?? "INR",
                Description = dto.Description,
                Status = "Created",
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _paymentRepo.CreatePaymentAsync(payment);

            response.Success = true;
            response.Message = "Payment order created successfully.";
            response.Data = new PaymentResponseDto
            {
                PaymentId = razorOrder["id"].ToString(),
                RazorpayOrderId = razorOrder["id"].ToString(),
                OrderId = order.Id,
                OrderReference = razorOrder["receipt"].ToString(),
                Amount = dto.Amount,
                Currency = dto.Currency ?? "INR",
                Status = "Created",
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow
            };

            return response;
        }

        public async Task<ApiResponse<string>> ConfirmPaymentAsync(PaymentConfirmationDto dto)
        {
            var response = new ApiResponse<string>();

            if (string.IsNullOrWhiteSpace(dto.PaymentId))
            {
                response.Success = false;
                response.Message = "PaymentId is required.";
                return response;
            }

            var payment = await _paymentRepo.GetPaymentByPaymentIdAsync(dto.PaymentId);
            if (payment == null)
            {
                response.Success = false;
                response.Message = "Payment not found.";
                return response;
            }

            await _paymentRepo.UpdatePaymentStatusAsync(payment, "Paid");

            var order = await _orderRepo.GetOrderByIdAsync(payment.OrderId);
            if (order != null)
            {
                await _orderRepo.UpdateOrderStatusAsync(order, OrderStatus.Paid);
            }

            response.Success = true;
            response.Message = "Payment confirmed successfully.";
            return response;
        }

        public async Task<ApiResponse<PaymentResponseDto>> GetPaymentByOrderIdAsync(int orderId)
        {
            var response = new ApiResponse<PaymentResponseDto>();
            var payment = await _paymentRepo.GetPaymentByOrderIdAsync(orderId);

            if (payment == null)
            {
                response.Success = false;
                response.Message = "No payment found for this order.";
                return response;
            }

            response.Success = true;
            response.Message = "Payment fetched successfully.";
            response.Data = new PaymentResponseDto
            {
                PaymentId = payment.PaymentId,
                RazorpayOrderId = payment.PaymentId,
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
