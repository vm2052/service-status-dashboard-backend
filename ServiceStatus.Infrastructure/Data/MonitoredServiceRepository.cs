using Microsoft.EntityFrameworkCore;
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
            return await _context.MonitoredServices.FindAsync(id);
        }

        public async Task<IEnumerable<MonitoredService>> GetAllAsync()
        {
            return await _context.MonitoredServices.ToListAsync();
        }

        public async Task AddAsync(MonitoredService serviceStatus)
        {
            await _context.MonitoredServices.AddAsync(serviceStatus);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MonitoredService serviceStatus)
        {
            Console.WriteLine(serviceStatus);
            _context.MonitoredServices.Update(serviceStatus);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var service = await GetByIdAsync(id);
            if (service != null)
            {
                _context.MonitoredServices.Remove(service);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(string name)
        {
            return await _context.MonitoredServices.AnyAsync(s => s.Name == name);
        }
    }
}
