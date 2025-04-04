using Microsoft.EntityFrameworkCore;
using GroceryInventory.Core.Models;

namespace GroceryInventory.Data;

public class GroceryInventoryContext : DbContext
{
    public GroceryInventoryContext(DbContextOptions<GroceryInventoryContext> options)
        : base(options)
    {
    }

    public DbSet<GroceryItem> GroceryItems { get; set; } = null!;
    public DbSet<Supplier> Suppliers { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GroceryItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Unit).IsRequired();
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.GroceryItem)
                .WithMany()
                .HasForeignKey(e => e.GroceryItemId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Supplier)
                .WithMany()
                .HasForeignKey(e => e.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}