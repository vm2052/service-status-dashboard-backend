using System;

namespace ServiceStatus.Application.DTO;

public class LatencyDataPointDTO
{
    public int Value { get; set; }
    public DateTime Timestamp { get; set; }
}
