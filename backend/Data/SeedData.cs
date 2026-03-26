using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public static class SeedData
{
    public static async Task EnsureSeededAsync(AppDbContext db)
    {
        // Ako veæ postoje podaci, ne seedaj ponovo
        if (await db.Categories.AnyAsync() || await db.Products.AnyAsync())
            return;

        var categories = new List<Category>
        {
            new() { Name = "Darts" },
            new() { Name = "Flights" },
            new() { Name = "Shafts" },
            new() { Name = "Boards" },
            new() { Name = "Accessories" }
        };

        db.Categories.AddRange(categories);
        await db.SaveChangesAsync();

        var dartsCategory = categories.First(c => c.Name == "Darts");
        var flightsCategory = categories.First(c => c.Name == "Flights");
        var shaftsCategory = categories.First(c => c.Name == "Shafts");
        var boardsCategory = categories.First(c => c.Name == "Boards");
        var accessoriesCategory = categories.First(c => c.Name == "Accessories");

        var products = new List<Product>
        {
            new() { Name = "Steel Darts 22g", Description = "Balanced steel tip darts for precision throws.", Price = 59.90m, ImageUrl = "https://picsum.photos/seed/dart1/600/400", Stock = 25, CategoryId = dartsCategory.Id },
            new() { Name = "Steel Darts 24g", Description = "Heavier darts for stable, controlled flights.", Price = 64.90m, ImageUrl = "https://picsum.photos/seed/dart2/600/400", Stock = 20, CategoryId = dartsCategory.Id },
            new() { Name = "Soft Tip Darts 18g", Description = "Soft tip darts for electronic dartboards.", Price = 49.90m, ImageUrl = "https://picsum.photos/seed/dart3/600/400", Stock = 30, CategoryId = dartsCategory.Id },
            new() { Name = "Pro Steel Darts 23g", Description = "Tournament-style barrel grip and control.", Price = 89.90m, ImageUrl = "https://picsum.photos/seed/dart4/600/400", Stock = 15, CategoryId = dartsCategory.Id },
            new() { Name = "Beginner Darts Set", Description = "Entry-level darts set with case and spare parts.", Price = 39.90m, ImageUrl = "https://picsum.photos/seed/dart5/600/400", Stock = 40, CategoryId = dartsCategory.Id },

            new() { Name = "Standard Flights - Black", Description = "Durable black standard flights (set of 3).", Price = 4.90m, ImageUrl = "https://picsum.photos/seed/flight1/600/400", Stock = 120, CategoryId = flightsCategory.Id },
            new() { Name = "Standard Flights - Red", Description = "Durable red standard flights (set of 3).", Price = 4.90m, ImageUrl = "https://picsum.photos/seed/flight2/600/400", Stock = 110, CategoryId = flightsCategory.Id },
            new() { Name = "Slim Flights - Blue", Description = "Slim flights for faster throw profile.", Price = 5.50m, ImageUrl = "https://picsum.photos/seed/flight3/600/400", Stock = 90, CategoryId = flightsCategory.Id },
            new() { Name = "Pear Flights - White", Description = "Pear-shaped flights for tighter grouping.", Price = 5.20m, ImageUrl = "https://picsum.photos/seed/flight4/600/400", Stock = 80, CategoryId = flightsCategory.Id },
            new() { Name = "Flight Protectors", Description = "Metal protectors to increase flight durability.", Price = 3.90m, ImageUrl = "https://picsum.photos/seed/flight5/600/400", Stock = 150, CategoryId = flightsCategory.Id },

            new() { Name = "Nylon Shafts - Short", Description = "Lightweight nylon shafts (short).", Price = 3.50m, ImageUrl = "https://picsum.photos/seed/shaft1/600/400", Stock = 100, CategoryId = shaftsCategory.Id },
            new() { Name = "Nylon Shafts - Medium", Description = "Lightweight nylon shafts (medium).", Price = 3.50m, ImageUrl = "https://picsum.photos/seed/shaft2/600/400", Stock = 100, CategoryId = shaftsCategory.Id },
            new() { Name = "Nylon Shafts - Long", Description = "Lightweight nylon shafts (long).", Price = 3.50m, ImageUrl = "https://picsum.photos/seed/shaft3/600/400", Stock = 100, CategoryId = shaftsCategory.Id },
            new() { Name = "Aluminium Shafts - Medium", Description = "Strong aluminium shafts for durability.", Price = 6.90m, ImageUrl = "https://picsum.photos/seed/shaft4/600/400", Stock = 75, CategoryId = shaftsCategory.Id },
            new() { Name = "Carbon Shafts - Medium", Description = "Premium carbon shafts for pro players.", Price = 12.90m, ImageUrl = "https://picsum.photos/seed/shaft5/600/400", Stock = 40, CategoryId = shaftsCategory.Id },

            new() { Name = "Bristle Dartboard", Description = "Competition-style bristle dartboard.", Price = 79.90m, ImageUrl = "https://picsum.photos/seed/board1/600/400", Stock = 18, CategoryId = boardsCategory.Id },
            new() { Name = "Starter Dartboard", Description = "Affordable dartboard for home practice.", Price = 39.90m, ImageUrl = "https://picsum.photos/seed/board2/600/400", Stock = 28, CategoryId = boardsCategory.Id },
            new() { Name = "Dartboard Surround", Description = "Wall protection ring around the dartboard.", Price = 49.90m, ImageUrl = "https://picsum.photos/seed/board3/600/400", Stock = 22, CategoryId = boardsCategory.Id },
            new() { Name = "Dartboard Cabinet", Description = "Wooden cabinet for stylish board setup.", Price = 99.90m, ImageUrl = "https://picsum.photos/seed/board4/600/400", Stock = 10, CategoryId = boardsCategory.Id },

            new() { Name = "Dart Mat", Description = "Floor mat with throw line markings.", Price = 34.90m, ImageUrl = "https://picsum.photos/seed/acc1/600/400", Stock = 35, CategoryId = accessoriesCategory.Id },
            new() { Name = "Point Sharpener", Description = "Tool for sharpening steel dart points.", Price = 9.90m, ImageUrl = "https://picsum.photos/seed/acc2/600/400", Stock = 60, CategoryId = accessoriesCategory.Id },
            new() { Name = "Darts Case", Description = "Hard shell case for darts and spare parts.", Price = 14.90m, ImageUrl = "https://picsum.photos/seed/acc3/600/400", Stock = 70, CategoryId = accessoriesCategory.Id },
            new() { Name = "Checkout Scorebook", Description = "Printed scorebook for match tracking.", Price = 6.90m, ImageUrl = "https://picsum.photos/seed/acc4/600/400", Stock = 85, CategoryId = accessoriesCategory.Id }
        };

        db.Products.AddRange(products);
        await db.SaveChangesAsync();
    }
}