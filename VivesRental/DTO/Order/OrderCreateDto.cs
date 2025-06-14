using System.ComponentModel.DataAnnotations;

namespace VivesRental.DTO.Order;

public class OrderCreateDto
{
    public Guid? CustomerId { get; set; }

    [Required]
    public string CustomerFirstName { get; set; } = string.Empty;

    [Required]
    public string CustomerLastName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string CustomerEmail { get; set; } = string.Empty;

    public string CustomerPhoneNumber { get; set; } = string.Empty;

    public List<Guid> ProductIds { get; set; } = new(); // Wat gekozen wordt uit UI

    public DateTime ExpiresAt { get; set; }
}
