using System;
using ServiceStatus.Domain.Entities;

namespace ServiceStatus.Domain.Interfaces;

public interface IServiceLatencyRepository
{ 
    Task AddAsync(ServiceLatency latency);
    Task<List<(int LatencyMs, DateTime Timestamp)>> GetLastNByServiceId(Guid serviceId, int n);

}
