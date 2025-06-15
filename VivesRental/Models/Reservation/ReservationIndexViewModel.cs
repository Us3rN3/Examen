using VivesRental.Domains.EntitiesDB;

namespace VivesRental.Models.Reservation
{
    public class ReservationIndexViewModel
    {
        public List<ArticleReservation> Reservations { get; set; } = new();
        public string? Search { get; set; }

        public int PageNumber { get; set; }
        public int TotalPages { get; set; }

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }

}
