using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using SecurityArch.Models;

namespace SecurityArch;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("securearch");

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("kunden"); // Tabelle im Schema
            entity.HasKey(c => c.id);
        });
    }
}