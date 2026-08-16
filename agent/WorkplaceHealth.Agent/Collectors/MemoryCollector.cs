using System.Management;

namespace WorkplaceHealth.Agent.Collectors;

public class MemoryCollector
{
    public MemoryInfo GetInfo()
    {
        using var searcher = new ManagementObjectSearcher(
            "SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");

        foreach (ManagementObject operatingSystem in searcher.Get())
        {
            double totalMemoryKb =
                Convert.ToDouble(operatingSystem["TotalVisibleMemorySize"]);

            double freeMemoryKb =
                Convert.ToDouble(operatingSystem["FreePhysicalMemory"]);

            double usedMemoryKb = totalMemoryKb - freeMemoryKb;

            double usagePercent =
                (usedMemoryKb / totalMemoryKb) * 100;

            return new MemoryInfo
            {
                TotalGb = totalMemoryKb / 1024 / 1024,
                AvailableGb = freeMemoryKb / 1024 / 1024,
                UsagePercent = usagePercent
            };
        }

        return new MemoryInfo();
    }
}

public class MemoryInfo
{
    public double TotalGb { get; set; }

    public double AvailableGb { get; set; }

    public double UsagePercent { get; set; }
}