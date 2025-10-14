    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace ElectroKart_Api.Models
    {
        public class Order
        {
            [Key]
            public int Id { get; set; }

            [Required]
            public int UserId { get; set; }
            [ForeignKey("UserId")]
            public User? User { get; set; }

            [Required]
            public decimal TotalAmount { get; set; }

            [Required]
            [MaxLength(50)]
            public string Status { get; set; } = "Pending"; // Pending, Completed, Cancelled, etc.

            public DateTime CreatedAt { get; set; } = DateTime.Now;

            public List<OrderItem> Items { get; set; } = new();
        }
    }
