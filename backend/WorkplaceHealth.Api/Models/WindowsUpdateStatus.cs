namespace WorkplaceHealth.Api.Models;

public class WindowsUpdateStatus
{
    public string ServiceStatus { get; set; } = string.Empty;

    public int PendingUpdateCount { get; set; }

    public bool CheckSucceeded { get; set; }
}