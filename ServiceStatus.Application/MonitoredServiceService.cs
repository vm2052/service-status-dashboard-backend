using ServiceStatus.Application.DTO;
using ServiceStatus.Application.Interfaces;
using ServiceStatus.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceStatus.Application
{
    public class MonitoredServiceService : IMonitoredServiceService
    {
        private readonly IMonitoredServiceRepository _repository;

        public MonitoredServiceService(IMonitoredServiceRepository repository)
        {
            _repository = repository;
        }
        public async Task<MonitoredServiceDTO?> GetServiceStatusAsync(Guid id)
        {
            var service = await _repository.GetByIdAsync(id);
            return service == null ? null : MapToDto(service);
        }

        public async Task<IEnumerable<MonitoredServiceDTO>> GetAllServicesAsync()
        {
            var services = await _repository.GetAllAsync();
            return services.Select(MapToDto);
        }

        public async Task<MonitoredServiceDTO> AddServiceAsync(CreateServiceStatusRequest request)
        {
            if (await _repository.ExistsAsync(request.Name))
                throw new InvalidOperationException($"Service with name '{request.Name}' already exists");

            var serviceStatus = new MonitoredService(request.Name, request.Url, request.CheckIntervalSeconds);
            await _repository.AddAsync(serviceStatus);

            return MapToDto(serviceStatus);
        }

        public async Task<bool> DeleteServiceAsync(Guid id)
        {
            var service = await _repository.GetByIdAsync(id);
            if (service == null) return false;

            await _repository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> CheckServiceStatusAsync(Guid id)
        {
            var service = await _repository.GetByIdAsync(id);
            if (service == null) return false;

            // This would typically call an external service to check status
            var (isHealthy, errorMessage) = await CheckServiceHealth(service.Url);
            service.UpdateStatus(isHealthy, errorMessage);
            await _repository.UpdateAsync(service);

            return isHealthy;
        }

        private static async Task<(bool isHealthy, string? errorMessage)> CheckServiceHealth(string url)
        {
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(10);

                var response = await client.GetAsync(url);
                return (response.IsSuccessStatusCode, response.IsSuccessStatusCode ? null : $"HTTP {response.StatusCode}");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        private static MonitoredServiceDTO MapToDto(MonitoredService service)
        {
            return new MonitoredServiceDTO
            {
                Id = service.Id,
                Name = service.Name,
                Url = service.Url,
                LastChecked = service.LastChecked,
                IsHealthy = service.IsHealthy,
                LastErrorMessage = service.LastErrorMessage,
                CheckIntervalSeconds = service.CheckIntervalSeconds
            };
        }
    }
}
