using System;
using System.Collections.Generic;

namespace VivesRental.Domains.EntitiesDB;

public partial class Product
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Manufacturer { get; set; }

    public string? Publisher { get; set; }

    public int RentalExpiresAfterDays { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}
