using System;
using System.Collections.Generic;

namespace VivesRental.Domains.EntitiesDB;

public partial class Order
{
    public Guid Id { get; set; }

    public Guid? CustomerId { get; set; }

    public string CustomerFirstName { get; set; } = null!;

    public string CustomerLastName { get; set; } = null!;

    public string CustomerEmail { get; set; } = null!;

    public string CustomerPhoneNumber { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
}
