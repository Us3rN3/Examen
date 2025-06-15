using System.ComponentModel.DataAnnotations;

namespace VivesRental.Models.Reservation;

public class CreateReservationViewModel
{
    [Required(ErrorMessage = "Klant is verplicht")]
    public Guid? CustomerId { get; set; }

    [Required(ErrorMessage = "Product is verplicht")]
    public Guid? ProductId { get; set; }

    [Required(ErrorMessage = "Startdatum is verplicht")]
    public DateTime FromDateTime { get; set; }

    [Required(ErrorMessage = "Einddatum is verplicht")]
    public DateTime UntilDateTime { get; set; }

}