using ElectroKart_Api.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Payment
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string PaymentId { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string OrderId { get; set; } = string.Empty;


    [Required, MaxLength(100)]
    public string OrderReference { get; set; } = string.Empty;

    [Required, Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required, MaxLength(10)]
    public string Currency { get; set; } = "INR";

    [Required, MaxLength(50)]
    public string Status { get; set; } = "Pending";

    public int UserId { get; set; }
    public User? User { get; set; }

    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
