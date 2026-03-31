namespace backend.Dtos;

public record AdminOrderItemDto(
    int ProductId,
    int Quantity,
    decimal UnitPrice
);

public record AdminOrderDto(
    int Id,
    string CustomerName,
    string Phone,
    string Address,
    string City,
    DateTime CreatedAt,
    decimal Total,
    string Status,
    List<AdminOrderItemDto> Items
);