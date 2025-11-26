using ServiceStatus.Application.DTO;
using ServiceStatus.Application.Interfaces;
using ServiceStatus.Domain.Entities;
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
        private readonly IServiceLatencyRepository _latencyRepository;

        public MonitoredServiceService(IMonitoredServiceRepository repository,
         IServiceLatencyRepository latencyRepository)
        {
            _repository = repository;
            _latencyRepository = latencyRepository;
        }
        public async Task<MonitoredServiceDTO?> GetServiceStatusAsync(Guid id)
        {
            var service = await _repository.GetByIdAsync(id);
            return service == null ? null : await MapToDto(service);
        }

        public async Task<IEnumerable<MonitoredServiceDTO>> GetAllServicesAsync()
        {
            var services = await _repository.GetAllAsync();
            var tasks = services.Select(async service => await MapToDto(service));
            var dtos = await Task.WhenAll(tasks);
            return dtos;
        }

        public async Task<MonitoredServiceDTO> AddServiceAsync(CreateServiceStatusRequest request)
        {
            if (await _repository.ExistsAsync(request.Name))
                throw new InvalidOperationException($"Service with name '{request.Name}' already exists");

            var serviceStatus = new MonitoredService(request.Name, request.Url, request.CheckIntervalSeconds);
            await _repository.AddAsync(serviceStatus);

            return await MapToDto(serviceStatus);
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

            var (isHealthy, errorMessage, latency) = await CheckServiceHealth(service.Url);

            service.UpdateStatus(isHealthy, errorMessage);
            await _repository.UpdateAsync(service);

            // Save latency in DB
            var latencyEntry = new ServiceLatency
            {
                ServiceId = service.Id,
                LatencyMs = latency,
                Timestamp = DateTime.UtcNow
            };
            await _latencyRepository.AddAsync(latencyEntry);

            return isHealthy;
        }

        private static async Task<(bool isHealthy, string? errorMessage, int latencyMs)> CheckServiceHealth(string url)
        {
             try
                {
                    using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    var response = await client.GetAsync(url);
                    stopwatch.Stop();

                    int latencyMs = (int)stopwatch.ElapsedMilliseconds;
                    return (response.IsSuccessStatusCode, response.IsSuccessStatusCode ? null : $"HTTP {response.StatusCode}", latencyMs);
                }
                catch (Exception ex)
                {
                    return (false, ex.Message, 0);
                }
        }

        private async Task<MonitoredServiceDTO> MapToDto(MonitoredService service)
        { 
            var latencies = await _latencyRepository.GetLastNByServiceId(service.Id, 10);
            return new MonitoredServiceDTO
            {
                Id = service.Id,
                Name = service.Name,
                Url = service.Url,
                LastChecked = service.LastChecked,
                IsHealthy = service.IsHealthy,
                LastErrorMessage = service.LastErrorMessage,
                CheckIntervalSeconds = service.CheckIntervalSeconds,
                LatencyData = latencies.Select(t=>new LatencyDataPointDTO{Value = t.LatencyMs, Timestamp = t.Timestamp}).ToList()
            };
        }
    }
}
