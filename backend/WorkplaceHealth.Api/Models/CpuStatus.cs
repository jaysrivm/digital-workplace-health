namespace WorkplaceHealth.Api.Models;

public class CpuStatus
{
    public string Name { get; set; } = string.Empty;

    public int LogicalProcessors { get; set; }

    public int UsagePercent { get; set; }
}