using System.ComponentModel.DataAnnotations;

namespace VivesRental.Models;

public class CreateReservationViewModel
{
    [Required]
    public Guid ArticleId { get; set; }

    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    public DateTime FromDateTime { get; set; }

    [Required]
    public DateTime UntilDateTime { get; set; }
}
