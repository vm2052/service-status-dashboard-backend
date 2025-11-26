using System;

namespace ServiceStatus.Domain.Entities;

public class ServiceLatency
{
    public Guid Id { get; set; }
    public Guid ServiceId { get; set; }
    public int LatencyMs { get; set; }
    public DateTime Timestamp { get; set; }

    public MonitoredService Service { get; set; } = null!;
}
