var builder = WebApplication.CreateBuilder(args);

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

app.Run();