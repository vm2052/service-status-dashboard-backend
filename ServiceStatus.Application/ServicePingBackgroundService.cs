using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceStatus.Application.Interfaces;
namespace ServiceStatus.Application;

public class ServicePingBackgroundService : BackgroundService
{
private readonly IServiceProvider _serviceProvider;

    public ServicePingBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<IMonitoredServiceService>();

                var services = await service.GetAllServicesAsync();
                var now = DateTime.Now;

                foreach (var s in services)
                {
                Console.WriteLine(s.CheckIntervalSeconds);
                Console.WriteLine((now - s.LastChecked).TotalSeconds);
                   if ((now - s.LastChecked).TotalSeconds >= s.CheckIntervalSeconds)
                    {
                        Console.WriteLine("0");
                        await PingService(service, s.Id, stoppingToken);
                    }
                }
            }

            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task PingService(IMonitoredServiceService service, Guid serviceId, CancellationToken token)
    {
        try
        {
            await service.CheckServiceStatusAsync(serviceId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error pinging service {serviceId}: {ex.Message}");
        }
    }
}