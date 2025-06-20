using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Net;
using System.Text.Json;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Kunde> Kunden { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Sensor> Sensoren { get; set; }

    public DbSet<IpResult> IpResults { get; set; }
    public DbSet<Temperatur> Temperaturen { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("securearch");

        modelBuilder.Entity<Kunde>(entity =>
        {
            entity.ToTable("kunden", "securearch");
            entity.HasKey(e => e.kunden_id);
            entity.Property(e => e.kunden_name).IsRequired();
            entity.Property(e => e.created_at).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users", "securearch");
            entity.HasKey(e => e.id);
            entity.HasIndex(e => e.email).IsUnique();
            entity.Property(e => e.firstname).IsRequired();
            entity.Property(e => e.lastname).IsRequired();
            entity.Property(e => e.role).HasDefaultValue("Kunde");
            entity.Property(e => e.created).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(u => u.Kunde)
                .WithMany(k => k.Users)
                .HasForeignKey(u => u.kunden_id)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Sensor>(static entity =>
        {
            entity.ToTable("sensoren", "securearch");
            entity.HasKey(e => e.sensor_id);
            entity.Property(e => e.sensor_name).IsRequired();
            entity.Property(e => e.created_at).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(s => s.Kunde)
                .WithMany(k => k.Sensoren)
                .HasForeignKey(s => s.kunden_id)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.ip_addresses)
            .HasColumnType("jsonb")
            .HasConversion(
           v => JsonSerializer.Serialize(v, default(JsonSerializerOptions)),
           v => JsonSerializer.Deserialize<List<string>>(v, default(JsonSerializerOptions))!);
        });

        modelBuilder.Entity<IpResult>(entity =>
        {
            entity.ToTable("ip_results", "securearch");
            entity.HasKey(e => e.id);
            entity.Property(e => e.ip_address).IsRequired();
            entity.Property(e => e.status).IsRequired();
            entity.Property(e => e.timestamp).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Sensor)
                  .WithMany(s => s.IpResults)
                  .HasForeignKey(e => e.sensor_id)
                  .OnDelete(DeleteBehavior.Cascade);


        });

        modelBuilder.Entity<IpResult>()
        .ToTable("ip_results")
        .HasKey(i => i.id);


        modelBuilder.Entity<Temperatur>()
            .ToTable("temperatur")
            .HasOne(t => t.Sensor)
            .WithMany(s => s.Temperaturen)
            .HasForeignKey(t => t.sensor_id);

    }
}
