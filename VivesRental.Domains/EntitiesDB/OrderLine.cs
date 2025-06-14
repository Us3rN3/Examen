using System;
using System.Collections.Generic;

namespace VivesRental.Domains.EntitiesDB;

public partial class OrderLine
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public Guid? ArticleId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? ProductDescription { get; set; }

    public DateTime RentedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime? ReturnedAt { get; set; }

    public virtual Article? Article { get; set; }

    public virtual Order Order { get; set; } = null!;
}
