using System.ComponentModel.DataAnnotations;

namespace VivesRental.DTO.Order;

public class OrderBulkCreateDto
{
    [Required]
    public List<OrderCreateDto> Orders { get; set; } = new();
}
