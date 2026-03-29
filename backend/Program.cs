using backend.Data;
using backend.Dtos;
using Microsoft.EntityFrameworkCore;
using backend.Models;
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
// Add services to the container.
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.


app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");

app.MapGet("/api/health", () =>
{
    return Results.Ok(new
    {
        status = "ok",
        timestamp = DateTime.UtcNow,
        app = "darts-shop-api"
    });
});

app.MapGet("/api/categories", async (AppDbContext db) =>
{
    var categories = await db.Categories
        .AsNoTracking()
        .OrderBy(c => c.Name)
        .Select(c => new CategoryDto(c.Id, c.Name))
        .ToListAsync();

    return Results.Ok(categories);
});


app.MapGet("/api/products", async (AppDbContext db) =>
{
    var products = await db.Products
        .AsNoTracking()
        .Include(p => p.Category)
        .OrderBy(p => p.Name)
        .Select(p => new ProductDto(
            p.Id,
            p.Name,
            p.Description,
            p.Price,
            p.ImageUrl,
            p.Stock,
            p.CategoryId,
            p.Category.Name
        ))
        .ToListAsync();

    return Results.Ok(products);
});

app.MapGet("/api/products/{id:int}", async (int id, AppDbContext db) =>
{
    var product = await db.Products
        .AsNoTracking()
        .Include(p => p.Category)
        .Where(p => p.Id == id)
        .Select(p => new ProductDto(
            p.Id,
            p.Name,
            p.Description,
            p.Price,
            p.ImageUrl,
            p.Stock,
            p.CategoryId,
            p.Category.Name
        ))
        .FirstOrDefaultAsync();

    return product is null ? Results.NotFound() : Results.Ok(product);
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await SeedData.EnsureSeededAsync(db);
}

app.MapPost("/api/orders", async (CreateOrderDto dto, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(dto.CustomerName) ||
        string.IsNullOrWhiteSpace(dto.Phone) ||
        string.IsNullOrWhiteSpace(dto.Address) ||
        string.IsNullOrWhiteSpace(dto.City))
    {
        return Results.BadRequest(new { message = "Sva polja su obavezna." });
    }

    if (dto.Items is null || dto.Items.Count == 0)
    {
        return Results.BadRequest(new { message = "Narudžba mora imati barem jednu stavku." });
    }

    // isti ProductId više puta u payloadu -> zbroji kolièine
    var lines = dto.Items
        .GroupBy(x => x.ProductId)
        .Select(g => (ProductId: g.Key, Quantity: g.Sum(x => x.Quantity)))
        .ToList();

    foreach (var line in lines)
    {
        if (line.Quantity < 1)
            return Results.BadRequest(new { message = "Neispravna kolièina." });
    }

    await using var tx = await db.Database.BeginTransactionAsync();

    var order = new Order
    {
        CustomerName = dto.CustomerName.Trim(),
        Phone = dto.Phone.Trim(),
        Address = dto.Address.Trim(),
        City = dto.City.Trim(),
        CreatedAt = DateTime.UtcNow,
        Status = OrderStatus.New,
        Total = 0m
    };

    decimal total = 0m;

    foreach (var line in lines)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == line.ProductId);
        if (product is null)
        {
            await tx.RollbackAsync();
            return Results.BadRequest(new { message = $"Nepoznat proizvod ID {line.ProductId}." });
        }

        if (product.Stock < line.Quantity)
        {
            await tx.RollbackAsync();
            return Results.BadRequest(new { message = $"Nema dovoljno zaliha za: {product.Name}." });
        }

        var unitPrice = product.Price;
        total += unitPrice * line.Quantity;

        order.Items.Add(new OrderItem
        {
            ProductId = product.Id,
            Quantity = line.Quantity,
            UnitPrice = unitPrice
        });

        product.Stock -= line.Quantity;
    }

    order.Total = total;

    db.Orders.Add(order);
    await db.SaveChangesAsync();
    await tx.CommitAsync();

    return Results.Ok(new
    {
        order.Id,
        order.Total,
        order.CreatedAt,
        order.Status
    });
});

app.Run();