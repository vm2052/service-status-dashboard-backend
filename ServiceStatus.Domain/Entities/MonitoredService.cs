

using System.ComponentModel.DataAnnotations.Schema;
using ServiceStatus.Domain.Entities;

public class MonitoredService
    {
	public Guid Id { get; private set; }
	public string Name { get; private set; }
	public string Url { get; private set; }
	public DateTime LastChecked { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsHealthy { get; private set; }
	public string? LastErrorMessage { get; private set; }
	public int CheckIntervalSeconds { get; private set; }
	public ICollection<ServiceLatency> Latencies { get; set; } = new List<ServiceLatency>();
	private MonitoredService() { } // For EF

	public MonitoredService(string name, string url, int checkIntervalSeconds = 60)
	{
		Name = name ?? throw new ArgumentNullException(nameof(name));
		Url = url ?? throw new ArgumentNullException(nameof(url));
		CheckIntervalSeconds = checkIntervalSeconds;
		LastChecked = DateTime.Now;
		
	}

	public void UpdateStatus(bool isHealthy, string? errorMessage = null, int latencyMs = 0)
	{
		IsHealthy = isHealthy;
		LastErrorMessage = errorMessage;
		LastChecked = DateTime.UtcNow;
 
	}

	public void UpdateCheckInterval(int seconds)
	{
		if (seconds <= 0)
			throw new ArgumentException("Check interval must be positive");
		CheckIntervalSeconds = seconds;
	}
}
 