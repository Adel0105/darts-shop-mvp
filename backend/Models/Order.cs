namespace backend.Models;

public class Order
{
    public int Id { get; set; }

    public string CustomerName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public decimal Total { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.New;

    public List<OrderItem> Items { get; set; } = new();
}