using ServiceStatus.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceStatus.Application.Interfaces
{
    public interface IMonitoredServiceService
    {
        Task<MonitoredServiceDTO?> GetServiceStatusAsync(Guid id);
        Task<IEnumerable<MonitoredServiceDTO>> GetAllServicesAsync();
        Task<MonitoredServiceDTO> AddServiceAsync(CreateServiceStatusRequest request);
        Task<bool> DeleteServiceAsync(Guid id);
        Task<bool> CheckServiceStatusAsync(Guid id);
    }
}
