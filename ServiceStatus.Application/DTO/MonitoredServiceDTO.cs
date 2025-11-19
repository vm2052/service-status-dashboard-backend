using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceStatus.Application.DTO
{
    public class MonitoredServiceDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public DateTime LastChecked { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsHealthy { get; set; }
        public string? LastErrorMessage { get; set; }
        public int CheckIntervalSeconds { get; set; }
    }
    public class CreateServiceStatusRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int CheckIntervalSeconds { get; set; } = 60;
        public DateTime LastChecked { get; set; }
        public bool IsHealthy { get; set; }
        public string? LastErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateServiceStatusRequest
    {
        public string? Name { get; set; }
        public string? Url { get; set; }
        public int? CheckIntervalSeconds { get; set; }
    }
}
