using System;
using System.Collections.Generic;

namespace VivesRental.Domains.EntitiesDB;

public partial class ArticleReservation
{
    public Guid Id { get; set; }

    public Guid ArticleId { get; set; }

    public Guid CustomerId { get; set; }

    public DateTime FromDateTime { get; set; }

    public DateTime UntilDateTime { get; set; }

    public virtual Article Article { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;
}
