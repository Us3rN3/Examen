using ProductEntity = VivesRental.Domains.EntitiesDB.Product;

namespace VivesRental.Models.Product
{
    public class ProductIndexViewModel
    {
        public List<ProductEntity> Products { get; set; } = new();
        public string? SearchTerm { get; set; }

        public int PageNumber { get; set; }
        public int TotalPages { get; set; }

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }

}
