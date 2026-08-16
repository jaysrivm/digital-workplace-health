using System.Management;

namespace WorkplaceHealth.Agent.Collectors;

public class CpuCollector
{
    public CpuInfo GetInfo()
    {
        using var searcher = new ManagementObjectSearcher(
            "SELECT Name, NumberOfLogicalProcessors, LoadPercentage FROM Win32_Processor");

        foreach (ManagementObject processor in searcher.Get())
        {
            return new CpuInfo
            {
                Name = processor["Name"]?.ToString() ?? "Unknown",
                LogicalProcessors = Convert.ToInt32(
                    processor["NumberOfLogicalProcessors"]),
                UsagePercent = Convert.ToInt32(
                    processor["LoadPercentage"])
            };
        }

        return new CpuInfo();
    }
}

public class CpuInfo
{
    public string Name { get; set; } = "Unknown";

    public int LogicalProcessors { get; set; }

    public int UsagePercent { get; set; }
}