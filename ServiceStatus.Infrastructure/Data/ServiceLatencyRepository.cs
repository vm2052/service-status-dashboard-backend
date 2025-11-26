using System;
using Microsoft.EntityFrameworkCore;
using ServiceStatus.Domain.Entities;
using ServiceStatus.Domain.Interfaces;

namespace ServiceStatus.Infrastructure.Data;

public class ServiceLatencyRepository : IServiceLatencyRepository
{
    private readonly ApplicationDbContext _db;

    public ServiceLatencyRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(ServiceLatency latency)
    {
        _db.ServiceLatencies.Add(latency);
        await _db.SaveChangesAsync();
    }
public async Task<List<(int LatencyMs, DateTime Timestamp)>> GetLastNByServiceId(Guid serviceId, int n)
{
    var results = await _db.ServiceLatencies
        .Where(l => l.ServiceId == serviceId)
        .OrderByDescending(l => l.Timestamp)
        .Take(n)
        .OrderBy(l => l.Timestamp) // chronological order
        .Select(l => new { l.LatencyMs, l.Timestamp }) // Use anonymous type
        .ToListAsync();

    // Convert to tuple list after the query
    return results.Select(x => (x.LatencyMs, x.Timestamp)).ToList();
}
}
