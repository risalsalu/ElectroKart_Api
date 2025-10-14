    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    namespace ElectroKart_Api.DTOs.Orders
    {
        public class CreateOrderDto
        {
            public int UserId { get; set; }

            [Required]
            public List<OrderItemDto> Items { get; set; } = new();
        }
    }
