using Microsoft.EntityFrameworkCore;
using OrderProcessing.Core.Entities;

namespace OrderProcessing.DataAccess;

public class OrderProcessingDbContext : DbContext
{
    public OrderProcessingDbContext(DbContextOptions<OrderProcessingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Order entity
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.CustomerName)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(e => e.CustomerEmail)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            
            entity.Property(e => e.TotalAmount)
                .HasPrecision(18, 2);
            
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            
            entity.Property(e => e.UpdatedAt);

            // Configure relationship
            entity.HasMany(e => e.Items)
                .WithOne(e => e.Order)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add index for common queries
            entity.HasIndex(e => e.CustomerEmail);
            entity.HasIndex(e => e.CreatedAt);
        });

        // Configure OrderItem entity
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItems");
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.ProductName)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(e => e.Quantity)
                .IsRequired();
            
            entity.Property(e => e.UnitPrice)
                .HasPrecision(18, 2);
            
            entity.Property(e => e.TotalPrice)
                .HasPrecision(18, 2);
        });
    }
}
