using backend.Data;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using backend.Dtos;
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
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "darts-shop-api";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "darts-shop-client";
var expiresMinutes = int.TryParse(builder.Configuration["Jwt:ExpiresMinutes"], out var m) ? m : 240;

var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,

        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,

        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.


app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/api/health", () =>
{
    return Results.Ok(new
    {
        status = "ok",
        timestamp = DateTime.UtcNow,
        app = "darts-shop-api"
    });
});

app.MapPost("/api/auth/login", (AdminLoginDto dto) =>
{
    // MVP: hardkodirani admin (kasnije može u Admin kontrolu/DB).
    const string adminUsername = "admin";
    const string adminPassword = "admin123"; 

    if (dto is null || string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
        return Results.Unauthorized();

    if (!string.Equals(dto.Username, adminUsername, StringComparison.Ordinal) ||
        !string.Equals(dto.Password, adminPassword, StringComparison.Ordinal))
    {
        return Results.Unauthorized();
    }

    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, adminUsername),
        new Claim(ClaimTypes.Role, "Admin")
    };

    var tokenHandler = new JwtSecurityTokenHandler();
    var securityToken = new JwtSecurityToken(
        issuer: jwtIssuer,
        audience: jwtAudience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
        signingCredentials: new SigningCredentials(
            new SymmetricSecurityKey(keyBytes),
            SecurityAlgorithms.HmacSha256
        )
    );

    var token = tokenHandler.WriteToken(securityToken);

    return Results.Ok(new { token });
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