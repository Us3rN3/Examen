namespace VivesRental.DTO.Order
{
    public class OrderLineDto
    {
        public Guid Id { get; set; }
        public Guid ArticleId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductDescription { get; set; }
        public DateTime RentedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? ReturnedAt { get; set; }
    }
}
