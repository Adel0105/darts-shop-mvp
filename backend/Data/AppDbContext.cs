using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

	public DbSet<Category> Categories => Set<Category>();
	public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Category>(entity =>
		{
			entity.Property(x => x.Name)
				  .HasMaxLength(100)
				  .IsRequired();
		});

		modelBuilder.Entity<Product>(entity =>
		{
			entity.Property(x => x.Name)
				  .HasMaxLength(200)
				  .IsRequired();

			entity.Property(x => x.Price)
				  .HasColumnType("decimal(18,2)");

			entity.Property(x => x.Stock)
				  .HasDefaultValue(0);

			entity.HasOne(x => x.Category)
				  .WithMany(x => x.Products)
				  .HasForeignKey(x => x.CategoryId)
				  .OnDelete(DeleteBehavior.Restrict);
		});
        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(x => x.CustomerName).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Phone).HasMaxLength(40).IsRequired();
            entity.Property(x => x.Address).HasMaxLength(200).IsRequired();
            entity.Property(x => x.City).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Total).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Status).HasConversion<int>();
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");

            entity.HasOne(x => x.Order)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}