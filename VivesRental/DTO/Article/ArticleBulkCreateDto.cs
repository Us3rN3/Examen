using System;
using System.ComponentModel.DataAnnotations;

namespace VivesRental.DTO.Article;

public class ArticleBulkCreateDto
{
    [Required]
    public Guid ProductId { get; set; }

    [Range(1, 1000)]
    public int Amount { get; set; }

    public int Status { get; set; } = 0; // standaard beschikbaar
}
