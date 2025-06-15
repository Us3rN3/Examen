using OrderEntity = VivesRental.Domains.EntitiesDB.Order;

namespace VivesRental.Models.Order
{
    public class OrderOverviewViewModel
    {
        public List<IGrouping<CustomerGroupKey, OrderEntity>> GroupedOrders { get; set; } = new();
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public string SearchTerm { get; set; }
    }
}
