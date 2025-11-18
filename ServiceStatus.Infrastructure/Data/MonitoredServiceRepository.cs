using ServiceStatus.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceStatus.Infrastructure.Data
{
    public class MonitoredServiceRepository : IMonitoredServiceRepository
    {
        private readonly ApplicationDbContext _context;

        public MonitoredServiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MonitoredService?> GetByIdAsync(Guid id)
        {
            return await _context.ServiceStatuses.FindAsync(id);
        }

        public async Task<IEnumerable<MonitoredService>> GetAllAsync()
        {
            return await _context.ServiceStatuses.ToListAsync();
        }

        public async Task AddAsync(MonitoredService serviceStatus)
        {
            await _context.ServiceStatuses.AddAsync(serviceStatus);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MonitoredService serviceStatus)
        {
            _context.ServiceStatuses.Update(serviceStatus);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var service = await GetByIdAsync(id);
            if (service != null)
            {
                _context.ServiceStatuses.Remove(service);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(string name)
        {
            return await _context.ServiceStatuses.AnyAsync(s => s.Name == name);
        }
    }
}
