using System.Collections.Generic;
using VivesRental.Domains.Enums;
using ArticleEntity = VivesRental.Domains.EntitiesDB.Article;

namespace VivesRental.Models.Article
{
    public class ArticleIndexViewModel
    {
        public IEnumerable<ArticleEntity> Articles { get; set; } = new List<ArticleEntity>();
        public string? SearchTerm { get; set; }
        public ArticleStatus? StatusFilter { get; set; } 
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
