using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public static class SeedData
{
    /// <summary>Images are served by the Angular app from /public/products (path: /products/...).</summary>
    private static readonly Dictionary<string, string> ProductImageByName = new()
    {
        ["Steel Darts 22g"] = "/products/steel-darts-22g.jpg",
        ["Steel Darts 24g"] = "/products/steel-darts-24g.jpg",
        ["Soft Tip Darts 18g"] = "/products/soft-tip-darts-18g.jpg",
        ["Pro Steel Darts 23g"] = "/products/pro-steel-darts-23g.jpg",
        ["Beginner Darts Set"] = "/products/beginner-darts-set.jpg",
        ["Standard Flights - Black"] = "/products/flights-black.jpg",
        ["Standard Flights - Red"] = "/products/flights-red.jpg",
        ["Slim Flights - Blue"] = "/products/flights-slim-blue.jpg",
        ["Pear Flights - White"] = "/products/flights-pear-white.jpg",
        ["Flight Protectors"] = "/products/flight-protectors.jpg",
        ["Nylon Shafts - Short"] = "/products/shafts-nylon-short.jpg",
        ["Nylon Shafts - Medium"] = "/products/shafts-nylon-medium.jpg",
        ["Nylon Shafts - Long"] = "/products/shafts-nylon-long.jpg",
        ["Aluminium Shafts - Medium"] = "/products/shafts-aluminium-medium.jpg",
        ["Carbon Shafts - Medium"] = "/products/shafts-carbon-medium.jpg",
        ["Bristle Dartboard"] = "/products/bristle-dartboard.jpg",
        ["Starter Dartboard"] = "/products/starter-dartboard.jpg",
        ["Dartboard Surround"] = "/products/dartboard-surround.jpg",
        ["Dartboard Cabinet"] = "/products/dartboard-cabinet.jpg",
        ["Dart Mat"] = "/products/dart-mat.jpg",
        ["Point Sharpener"] = "/products/point-sharpener.jpg",
        ["Darts Case"] = "/products/darts-case.jpg",
        ["Checkout Scorebook"] = "/products/checkout-scorebook.jpg",
    };

    /// <summary>Point existing DB rows from picsum placeholders to local /products images.</summary>
    public static async Task SyncProductImagesAsync(AppDbContext db)
    {
        var products = await db.Products.ToListAsync();
        var changed = false;
        foreach (var p in products)
        {
            if (!ProductImageByName.TryGetValue(p.Name, out var path))
                continue;
            if (p.ImageUrl.Contains("picsum.photos", StringComparison.OrdinalIgnoreCase))
            {
                p.ImageUrl = path;
                changed = true;
            }
        }

        if (changed)
            await db.SaveChangesAsync();
    }

    public static async Task EnsureSeededAsync(AppDbContext db)
    {
        // Ako veù postoje podaci, ne seedaj ponovo
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
            new() { Name = "Steel Darts 22g", Description = "Balanced steel tip darts for precision throws.", Price = 59.90m, ImageUrl = ProductImageByName["Steel Darts 22g"], Stock = 25, CategoryId = dartsCategory.Id },
            new() { Name = "Steel Darts 24g", Description = "Heavier darts for stable, controlled flights.", Price = 64.90m, ImageUrl = ProductImageByName["Steel Darts 24g"], Stock = 20, CategoryId = dartsCategory.Id },
            new() { Name = "Soft Tip Darts 18g", Description = "Soft tip darts for electronic dartboards.", Price = 49.90m, ImageUrl = ProductImageByName["Soft Tip Darts 18g"], Stock = 30, CategoryId = dartsCategory.Id },
            new() { Name = "Pro Steel Darts 23g", Description = "Tournament-style barrel grip and control.", Price = 89.90m, ImageUrl = ProductImageByName["Pro Steel Darts 23g"], Stock = 15, CategoryId = dartsCategory.Id },
            new() { Name = "Beginner Darts Set", Description = "Entry-level darts set with case and spare parts.", Price = 39.90m, ImageUrl = ProductImageByName["Beginner Darts Set"], Stock = 40, CategoryId = dartsCategory.Id },

            new() { Name = "Standard Flights - Black", Description = "Durable black standard flights (set of 3).", Price = 4.90m, ImageUrl = ProductImageByName["Standard Flights - Black"], Stock = 120, CategoryId = flightsCategory.Id },
            new() { Name = "Standard Flights - Red", Description = "Durable red standard flights (set of 3).", Price = 4.90m, ImageUrl = ProductImageByName["Standard Flights - Red"], Stock = 110, CategoryId = flightsCategory.Id },
            new() { Name = "Slim Flights - Blue", Description = "Slim flights for faster throw profile.", Price = 5.50m, ImageUrl = ProductImageByName["Slim Flights - Blue"], Stock = 90, CategoryId = flightsCategory.Id },
            new() { Name = "Pear Flights - White", Description = "Pear-shaped flights for tighter grouping.", Price = 5.20m, ImageUrl = ProductImageByName["Pear Flights - White"], Stock = 80, CategoryId = flightsCategory.Id },
            new() { Name = "Flight Protectors", Description = "Metal protectors to increase flight durability.", Price = 3.90m, ImageUrl = ProductImageByName["Flight Protectors"], Stock = 150, CategoryId = flightsCategory.Id },

            new() { Name = "Nylon Shafts - Short", Description = "Lightweight nylon shafts (short).", Price = 3.50m, ImageUrl = ProductImageByName["Nylon Shafts - Short"], Stock = 100, CategoryId = shaftsCategory.Id },
            new() { Name = "Nylon Shafts - Medium", Description = "Lightweight nylon shafts (medium).", Price = 3.50m, ImageUrl = ProductImageByName["Nylon Shafts - Medium"], Stock = 100, CategoryId = shaftsCategory.Id },
            new() { Name = "Nylon Shafts - Long", Description = "Lightweight nylon shafts (long).", Price = 3.50m, ImageUrl = ProductImageByName["Nylon Shafts - Long"], Stock = 100, CategoryId = shaftsCategory.Id },
            new() { Name = "Aluminium Shafts - Medium", Description = "Strong aluminium shafts for durability.", Price = 6.90m, ImageUrl = ProductImageByName["Aluminium Shafts - Medium"], Stock = 75, CategoryId = shaftsCategory.Id },
            new() { Name = "Carbon Shafts - Medium", Description = "Premium carbon shafts for pro players.", Price = 12.90m, ImageUrl = ProductImageByName["Carbon Shafts - Medium"], Stock = 40, CategoryId = shaftsCategory.Id },

            new() { Name = "Bristle Dartboard", Description = "Competition-style bristle dartboard.", Price = 79.90m, ImageUrl = ProductImageByName["Bristle Dartboard"], Stock = 18, CategoryId = boardsCategory.Id },
            new() { Name = "Starter Dartboard", Description = "Affordable dartboard for home practice.", Price = 39.90m, ImageUrl = ProductImageByName["Starter Dartboard"], Stock = 28, CategoryId = boardsCategory.Id },
            new() { Name = "Dartboard Surround", Description = "Wall protection ring around the dartboard.", Price = 49.90m, ImageUrl = ProductImageByName["Dartboard Surround"], Stock = 22, CategoryId = boardsCategory.Id },
            new() { Name = "Dartboard Cabinet", Description = "Wooden cabinet for stylish board setup.", Price = 99.90m, ImageUrl = ProductImageByName["Dartboard Cabinet"], Stock = 10, CategoryId = boardsCategory.Id },

            new() { Name = "Dart Mat", Description = "Floor mat with throw line markings.", Price = 34.90m, ImageUrl = ProductImageByName["Dart Mat"], Stock = 35, CategoryId = accessoriesCategory.Id },
            new() { Name = "Point Sharpener", Description = "Tool for sharpening steel dart points.", Price = 9.90m, ImageUrl = ProductImageByName["Point Sharpener"], Stock = 60, CategoryId = accessoriesCategory.Id },
            new() { Name = "Darts Case", Description = "Hard shell case for darts and spare parts.", Price = 14.90m, ImageUrl = ProductImageByName["Darts Case"], Stock = 70, CategoryId = accessoriesCategory.Id },
            new() { Name = "Checkout Scorebook", Description = "Printed scorebook for match tracking.", Price = 6.90m, ImageUrl = ProductImageByName["Checkout Scorebook"], Stock = 85, CategoryId = accessoriesCategory.Id }
        };

        db.Products.AddRange(products);
        await db.SaveChangesAsync();
    }
}