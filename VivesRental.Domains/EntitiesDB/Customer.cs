using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VivesRental.Domains.EntitiesDB;

public partial class Customer
{
    public Guid Id { get; set; }

    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public virtual ICollection<ArticleReservation> ArticleReservations { get; set; } = new List<ArticleReservation>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
