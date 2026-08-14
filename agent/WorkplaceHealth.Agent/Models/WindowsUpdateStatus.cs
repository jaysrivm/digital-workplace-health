namespace WorkplaceHealth.Agent.Models;

public class WindowsUpdateStatus
{
    public string ServiceStatus { get; set; } = "Unknown";

    public int PendingUpdateCount { get; set; }

    public bool CheckSucceeded { get; set; }
}