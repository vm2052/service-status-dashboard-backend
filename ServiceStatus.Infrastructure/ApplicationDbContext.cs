using Microsoft.EntityFrameworkCore;
using ServiceStatus.Domain;
using ServiceStatus.Domain.Entities;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<MonitoredService> MonitoredServices { get; set; }
    public DbSet<ServiceLatency> ServiceLatencies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
         modelBuilder.Entity<MonitoredService>(entity =>
        {
            entity.ToTable("services", "public");
            entity.HasKey(e => e.Id).HasName("pk_services");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Url).HasColumnName("url").IsRequired().HasMaxLength(500);
            entity.Property(e => e.LastChecked).HasColumnName("last_checked").IsRequired();
            entity.Property(e => e.IsHealthy).HasColumnName("is_healthy").IsRequired();
            entity.Property(e => e.CheckIntervalSeconds).HasColumnName("check_interval_seconds").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.LastErrorMessage).HasColumnName("last_error_message").HasMaxLength(1000);

            entity.HasIndex(e => e.Name).IsUnique();
        });

        // New ServiceLatency mapping
        modelBuilder.Entity<ServiceLatency>(entity =>
        {
            entity.ToTable("servicelatency", "public");
            entity.HasKey(e => e.Id).HasName("pk_service_latency");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ServiceId).HasColumnName("serviceid").IsRequired();
            entity.Property(e => e.LatencyMs).HasColumnName("latencyms").IsRequired();
            entity.Property(e => e.Timestamp).HasColumnName("timestamp").IsRequired();

            // Proper foreign key mapping with navigation
            entity.HasOne(e => e.Service)
                .WithMany(s => s.Latencies)
                .HasForeignKey(e => e.ServiceId)
                .HasConstraintName("fk_service_latency_service")
                .OnDelete(DeleteBehavior.Cascade);
        });

    base.OnModelCreating(modelBuilder);

    }
}