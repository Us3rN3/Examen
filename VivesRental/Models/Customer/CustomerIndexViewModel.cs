using System.Collections.Generic;
using VivesRental.Domains.EntitiesDB;

namespace VivesRental.Models
{
    public class CustomerIndexViewModel
    {
        public IEnumerable<Customer> Customers { get; set; } = new List<Customer>();
        public string? SearchTerm { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
