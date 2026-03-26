using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

	public DbSet<Category> Categories => Set<Category>();
	public DbSet<Product> Products => Set<Product>();

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
	}
}