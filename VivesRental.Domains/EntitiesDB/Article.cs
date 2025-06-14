using System;
using System.Collections.Generic;
using VivesRental.Domains.Enums;
namespace VivesRental.Domains.EntitiesDB;

public partial class Article
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public ArticleStatus Status { get; set; }

    public virtual ICollection<ArticleReservation> ArticleReservations { get; set; } = new List<ArticleReservation>();

    public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

    public virtual Product? Product { get; set; }
}
