﻿namespace ElectroKart_Api.DTOs.Cart
{
    public class CartItemDto
    {
        public int Id { get; set; }      
        public int ProductId { get; set; }  
        public int Quantity { get; set; }  
        public int CartCount { get; set; }   
    }
}
