namespace WorkplaceHealth.Api.Models;

public class DeviceHealthReport
{
    public string DeviceName { get; set; } = string.Empty;

    public DateTime CollectedAtUtc { get; set; }

    public CpuStatus Cpu { get; set; } = new();

    public MemoryStatus Memory { get; set; } = new();

    public List<DiskStatus> Disks { get; set; } = new();

    public WindowsUpdateStatus WindowsUpdate { get; set; } = new();

    public List<ServiceStatus> Services { get; set; } = new();

    public RegistryStatus Registry { get; set; } = new();
}