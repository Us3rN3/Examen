using System.ComponentModel.DataAnnotations;

namespace VivesRental.DTO.Reservation
{
    public class ArticleReservationUpdateDto
    {
        public Guid ArticleId { get; set; }
        public Guid CustomerId { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime FromDateTime { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime UntilDateTime { get; set; }
    }
}
