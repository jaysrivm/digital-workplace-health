namespace WorkplaceHealth.Api.Models;

public class Device
{
    public int Id { get; set; }

    public string DeviceName { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime LastSeenAtUtc { get; set; }

    public List<HealthReport> HealthReports { get; set; } = new();
}