namespace WorkplaceHealth.Api.Models;

public class HealthReport
{
    public int Id { get; set; }

    public int DeviceId { get; set; }

    public Device Device { get; set; } = null!;

    public DateTime CollectedAtUtc { get; set; }

    public string CpuName { get; set; } = string.Empty;

    public int CpuLogicalProcessors { get; set; }

    public int CpuUsagePercent { get; set; }

    public double TotalMemoryGb { get; set; }

    public double AvailableMemoryGb { get; set; }

    public double MemoryUsagePercent { get; set; }

    public string WindowsUpdateServiceStatus { get; set; } = string.Empty;

    public int PendingUpdateCount { get; set; }

    public bool UpdateCheckSucceeded { get; set; }

    public string WindowsProductName { get; set; } = string.Empty;

    public string WindowsDisplayVersion { get; set; } = string.Empty;

    public string CurrentBuild { get; set; } = string.Empty;

    public bool RegistryCheckSucceeded { get; set; }

    public List<DiskReport> Disks { get; set; } = new();

    public List<ServiceReport> Services { get; set; } = new();
}