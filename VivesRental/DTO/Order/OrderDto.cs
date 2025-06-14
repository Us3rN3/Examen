namespace VivesRental.DTO.Order;

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid? CustomerId { get; set; }
    public string CustomerFirstName { get; set; } = string.Empty;
    public string CustomerLastName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhoneNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<OrderLineDto>? OrderLines { get; set; }
}
