namespace VivesRental.DTO.Reservation
{
    public class ArticleReservationDto
    {
        public Guid Id { get; set; }
        public Guid ArticleId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime FromDateTime { get; set; }
        public DateTime UntilDateTime { get; set; }

        public string? ArticleName { get; set; }

        // Gebruik volledige naam i.p.v. alleen voornaam
        public string? CustomerFullName { get; set; }
    }

}
