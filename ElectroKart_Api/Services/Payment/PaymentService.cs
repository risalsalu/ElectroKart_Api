using ElectroKart_Api.DTOs.Payments;
using ElectroKart_Api.Models;
using ElectroKart_Api.Repositories.Orders;
using ElectroKart_Api.Repositories.Payments;
using System;
using System.Threading.Tasks;

namespace ElectroKart_Api.Services.Payments
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;

        public PaymentService(IPaymentRepository paymentRepository, IOrderRepository orderRepository)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
        }

        public async Task<PaymentResponseDto> InitiatePaymentAsync(CreatePaymentRequestDto dto, int userId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(dto.OrderId);
            if (order == null)
                throw new Exception("Order not found.");

            var payment = new Payment
            {
                PaymentId = Guid.NewGuid().ToString(),
                OrderId = order.Id, // int matches Orders.Id
                OrderReference = $"order_{order.Id}", // string for gateway
                Amount = dto.Amount,
                Currency = dto.Currency,
                Status = "Pending",
                Description = dto.Description,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _paymentRepository.CreatePaymentAsync(payment);

            return new PaymentResponseDto
            {
                PaymentId = created.PaymentId,
                OrderId = created.OrderId.ToString(),
                OrderReference = created.OrderReference,
                Amount = created.Amount,
                Currency = created.Currency,
                Status = created.Status,
                CreatedAt = created.CreatedAt
            };
        }

        public async Task<bool> ConfirmPaymentAsync(PaymentConfirmationDto dto)
        {
            var payment = await _paymentRepository.GetPaymentByPaymentIdAsync(dto.PaymentId);
            if (payment == null) return false;

            bool isValid = true; // validate signature if needed
            if (!isValid)
            {
                await _paymentRepository.UpdatePaymentStatusAsync(payment, "Failed");
                return false;
            }

            await _paymentRepository.UpdatePaymentStatusAsync(payment, "Success");

            var order = await _orderRepository.GetOrderByIdAsync(payment.OrderId);
            if (order != null)
            {
                order.Status = OrderStatus.Paid;
                await _orderRepository.UpdateOrderStatusAsync(order, OrderStatus.Paid);
            }

            return true;
        }
    }
}
