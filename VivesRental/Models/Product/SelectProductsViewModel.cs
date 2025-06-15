namespace VivesRental.Models.Product
{
    public class SelectProductsViewModel
    {
        public List<ProductWithAvailabilityViewModel> Products { get; set; } = new();
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int TotalPages { get; set; }
        public Guid CustomerId { get; set; }
    }

}
