using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Kunde> Kunden { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Sensor> Sensoren { get; set; }

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
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Sensor>(entity =>
        {
            entity.ToTable("sensoren", "securearch");
            entity.HasKey(e => e.sensor_id);
            entity.Property(e => e.sensor_name).IsRequired();
            entity.Property(e => e.created_at).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(s => s.Kunde)
                .WithMany(k => k.Sensoren)
                .HasForeignKey(s => s.kunden_id)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}