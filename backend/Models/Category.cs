namespace backend.Models;

public class Category
{
	public int Id { get; set; }

	public string Name { get; set; } = string.Empty;

	// Navigation: Category ima više proizvoda
	public List<Product> Products { get; set; } = new();
}