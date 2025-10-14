using ElectroKart_Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ElectroKart_Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<Wishlist> Wishlists { get; set; } = null!;
        public DbSet<WishlistItem> WishlistItems { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!; // NEW
        public DbSet<OrderItem> OrderItems { get; set; } = null!; // NEW

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Payment entity
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Amount)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(p => p.Currency)
                      .HasMaxLength(10)
                      .IsRequired();

                entity.Property(p => p.Status)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(p => p.PaymentId)
                      .HasMaxLength(100);

                entity.Property(p => p.OrderId)
                      .HasMaxLength(100);

                entity.Property(p => p.CreatedAt)
                      .HasDefaultValueSql("GETDATE()");
            });

            // Configure Order & OrderItem relationships
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.TotalAmount)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();
                entity.HasMany(o => o.Items)
                      .WithOne(i => i.Order)
                      .HasForeignKey(i => i.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(oi => oi.Id);
                entity.Property(oi => oi.Price)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();
            });
        }
    }
}
