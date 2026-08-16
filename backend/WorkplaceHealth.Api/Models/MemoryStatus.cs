namespace WorkplaceHealth.Api.Models;

public class MemoryStatus
{
    public double TotalGb { get; set; }

    public double AvailableGb { get; set; }

    public double UsagePercent { get; set; }
}