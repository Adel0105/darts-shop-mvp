namespace backend.Dtos;

public record ProductDto(
    int Id,
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    int Stock,
    int CategoryId,
    string CategoryName
);