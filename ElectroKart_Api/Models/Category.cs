﻿using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ElectroKart_Api.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public ICollection<Product>? Products { get; set; }
    }
}
