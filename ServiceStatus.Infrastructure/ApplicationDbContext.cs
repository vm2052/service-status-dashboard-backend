using Microsoft.EntityFrameworkCore;
using ServiceStatus.Domain;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<MonitoredService> MonitoredServices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MonitoredService>(entity =>
        {
            entity.ToTable("ServiceStatuses","public");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Url).IsRequired().HasMaxLength(500);
            entity.Property(e => e.LastChecked).IsRequired();
            entity.Property(e => e.IsHealthy).IsRequired();
            entity.Property(e => e.CheckIntervalSeconds).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.LastErrorMessage).HasMaxLength(1000);

            entity.HasIndex(e => e.Name).IsUnique();
        });
    }
}