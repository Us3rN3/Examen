using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VivesRental.Models.Article
{
    public class ArticleBulkCreateViewModel
    {
        [Required]
        [Display(Name = "Product")]
        public Guid ProductId { get; set; }

        [Required]
        [Range(1, 1000)]
        [Display(Name = "Aantal Artikelen")]
        public int Amount { get; set; }

        public IEnumerable<SelectListItem>? Products { get; set; }
    }
}