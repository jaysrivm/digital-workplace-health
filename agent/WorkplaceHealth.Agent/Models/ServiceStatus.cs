namespace WorkplaceHealth.Agent.Models;

public class ServiceStatus
{
    public string Name { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Status { get; set; } = "Unknown";

    public string StartType { get; set; } = "Unknown";
}