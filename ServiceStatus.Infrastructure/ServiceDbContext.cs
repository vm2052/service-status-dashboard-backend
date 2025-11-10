using Microsoft.EntityFrameworkCore;
using ServiceStatus.Domain;

public class ServiceDbContext(DbContextOptions<DbContext> options) : DbContext(options)
{
    public DbSet<MonitoredService> MonitoredServices { get; set; }
}