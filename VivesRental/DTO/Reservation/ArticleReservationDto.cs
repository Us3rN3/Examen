namespace VivesRental.DTO.Reservation
{
    public class ArticleReservationDto
    {
        public Guid Id { get; set; }
        public Guid ArticleId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime FromDateTime { get; set; }
        public DateTime UntilDateTime { get; set; }

        // Optioneel: naam van artikel en klant als read-only info (voor response)
        public string? ArticleName { get; set; }
        public string? CustomerName { get; set; }
    }
}
