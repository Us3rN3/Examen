using System.Collections.Generic;
using VivesRental.Domains.EntitiesDB;

namespace VivesRental.Models
{
    public class ArticleIndexViewModel
    {
        public IEnumerable<Article> Articles { get; set; } = new List<Article>();
        public string? SearchTerm { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
