using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using VivesRental.Domains.EntitiesDB;

namespace VivesRental.Domains.DataDB;

public partial class RentalDbContext : DbContext
{
    public RentalDbContext()
    {
    }

    public RentalDbContext(DbContextOptions<RentalDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Article> Articles { get; set; }

    public virtual DbSet<ArticleReservation> ArticleReservations { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderLine> OrderLines { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-VE0MPARM\\SQLEXPRESS; Database=VivesRentalDB; Trusted_Connection=True; TrustServerCertificate=True; MultipleActiveResultSets=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Article__3214EC0792A00C7D");

            entity.ToTable("Article");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Product).WithMany(p => p.Articles)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Article__Product__398D8EEE");
        });

        modelBuilder.Entity<ArticleReservation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ArticleR__3214EC07E6CB49CE");

            entity.ToTable("ArticleReservation");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Article).WithMany(p => p.ArticleReservations)
                .HasForeignKey(d => d.ArticleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ArticleRe__Artic__44FF419A");

            entity.HasOne(d => d.Customer).WithMany(p => p.ArticleReservations)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ArticleRe__Custo__45F365D3");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3214EC072318EAB4");

            entity.ToTable("Customer");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order__3214EC0789D3F5BE");

            entity.ToTable("Order");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Order__CustomerI__3E52440B");
        });

        modelBuilder.Entity<OrderLine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderLin__3214EC0709399773");

            entity.ToTable("OrderLine");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Article).WithMany(p => p.OrderLines)
                .HasForeignKey(d => d.ArticleId)
                .HasConstraintName("FK__OrderLine__Artic__4222D4EF");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderLines)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderLine__Order__412EB0B6");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Product__3214EC07E57E2193");

            entity.ToTable("Product");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
