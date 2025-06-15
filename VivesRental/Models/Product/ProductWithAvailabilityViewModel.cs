namespace VivesRental.Models.Product;

public class ProductWithAvailabilityViewModel
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public int AvailableCount { get; set; }
}
