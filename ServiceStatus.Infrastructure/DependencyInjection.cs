using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStatus.Application;
using ServiceStatus.Application.Interfaces;
using ServiceStatus.Domain.Interfaces;
using ServiceStatus.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceStatus.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IMonitoredServiceRepository, MonitoredServiceRepository>();
            services.AddScoped<IMonitoredServiceService, MonitoredServiceService>();

            return services;
        }
    }
}
