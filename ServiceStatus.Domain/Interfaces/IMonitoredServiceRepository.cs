using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceStatus.Domain.Interfaces
{
    public interface IMonitoredServiceRepository
    {
        Task<MonitoredService?> GetByIdAsync(Guid id);
        Task<IEnumerable<MonitoredService>> GetAllAsync();
        Task AddAsync(MonitoredService serviceStatus);
        Task UpdateAsync(MonitoredService serviceStatus);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(string name);
    }
}
