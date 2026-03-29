namespace backend.Dtos;

public record CreateOrderItemDto(int ProductId, int Quantity);

public record CreateOrderDto(
	string CustomerName,
	string Phone,
	string Address,
	string City,
	List<CreateOrderItemDto> Items
);