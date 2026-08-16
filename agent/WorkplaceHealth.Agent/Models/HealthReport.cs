using WorkplaceHealth.Agent.Collectors;

namespace WorkplaceHealth.Agent.Models;

public class HealthReport
{
    public string DeviceName { get; set; } = string.Empty;

    public DateTime CollectedAtUtc { get; set; }

    public CpuInfo Cpu { get; set; } = new();

    public MemoryInfo Memory { get; set; } = new();

    public List<DiskInfo> Disks { get; set; } = [];

    public WindowsUpdateStatus WindowsUpdate { get; set; } = new();

    public List<ServiceStatus> Services { get; set; } = [];

    public RegistryStatus Registry { get; set; } = new();
}