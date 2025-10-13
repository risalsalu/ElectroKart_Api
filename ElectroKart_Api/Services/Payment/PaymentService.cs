using ElectroKart_Api.DTOs.Payment;
using ElectroKart_Api.Models;
using ElectroKart_Api.Repositories;
using Microsoft.Extensions.Configuration;
using Razorpay.Api;
using System.Security.Cryptography;
using System.Text;

namespace ElectroKart_Api.Services.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repo;
        private readonly string _key;
        private readonly string _secret;

        public PaymentService(IConfiguration config, IPaymentRepository repo)
        {
            _repo = repo;
            _key = config["RazorpaySettings:KeyId"]!;       // ✅ Updated to match your Program.cs config section
            _secret = config["RazorpaySettings:KeySecret"]!;
        }

        public async Task<object> CreateOrderAsync(PaymentRequestDto request)
        {
            try
            {
                var client = new RazorpayClient(_key, _secret);

                var options = new Dictionary<string, object>
                {
                    { "amount", request.Amount * 100 }, // Razorpay expects amount in paise
                    { "currency", "INR" },
                    { "receipt", Guid.NewGuid().ToString() }
                };

                var order = client.Order.Create(options);

                // ✅ Explicitly reference model class to avoid namespace conflict
                var payment = new ElectroKart_Api.Models.Payment
                {
                    OrderId = order["id"].ToString(),
                    Amount = request.Amount,
                    Currency = "INR",
                    Status = "Created",
                    CreatedAt = DateTime.UtcNow
                };

                await _repo.AddPaymentAsync(payment);

                return new
                {
                    orderId = order["id"].ToString(),
                    key = _key,
                    amount = request.Amount * 100,
                    currency = "INR"
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error while creating Razorpay order", ex);
            }
        }

        public async Task<bool> VerifyPaymentAsync(VerifyPaymentDto request)
        {
            try
            {
                string signatureData = $"{request.OrderId}|{request.PaymentId}";
                string generatedSignature = CalculateSignature(signatureData, _secret);

                if (generatedSignature == request.Signature)
                {
                    await _repo.UpdateStatusAsync(request.OrderId, "Success", request.PaymentId, request.Signature);
                    return true;
                }

                await _repo.UpdateStatusAsync(request.OrderId, "Failed", request.PaymentId, request.Signature);
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Error verifying payment signature", ex);
            }
        }

        private string CalculateSignature(string data, string secret)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
