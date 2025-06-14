using System.ComponentModel.DataAnnotations;

namespace VivesRental.DTO.Product;

public class ProductUpdateDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? Manufacturer { get; set; }
    public string? Publisher { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "RentalExpiresAfterDays moet positief zijn")]
    public int RentalExpiresAfterDays { get; set; }
}
