using System.ComponentModel.DataAnnotations;

namespace VivesRental.DTO.Customer;

public class CustomerUpdateDto
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;
}
