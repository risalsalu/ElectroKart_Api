﻿namespace ElectroKart_Api.DTOs.Payment
{
    public class VerifyPaymentDto
    {
        public string OrderId { get; set; } = string.Empty;
        public string PaymentId { get; set; } = string.Empty;
        public string Signature { get; set; } = string.Empty;
    }
}
