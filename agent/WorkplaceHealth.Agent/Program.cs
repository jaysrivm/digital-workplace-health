using System.Management;
using System.ServiceProcess;
using WorkplaceHealth.Agent.Collectors;

// CPU information
using var cpuSearcher = new ManagementObjectSearcher(
    "SELECT Name, NumberOfLogicalProcessors, LoadPercentage FROM Win32_Processor");

foreach (ManagementObject processor in cpuSearcher.Get())
{
    Console.WriteLine($"CPU Name: {processor["Name"]}");
    Console.WriteLine($"Logical Processors: {processor["NumberOfLogicalProcessors"]}");
    Console.WriteLine($"CPU Usage: {processor["LoadPercentage"]}%");
}

// RAM information
using var memorySearcher = new ManagementObjectSearcher(
    "SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");

foreach (ManagementObject operatingSystem in memorySearcher.Get())
{
    double totalMemoryKb = Convert.ToDouble(operatingSystem["TotalVisibleMemorySize"]);
    double freeMemoryKb = Convert.ToDouble(operatingSystem["FreePhysicalMemory"]);

    double usedMemoryKb = totalMemoryKb - freeMemoryKb;
    double memoryUsagePercent = (usedMemoryKb / totalMemoryKb) * 100;

    double totalMemoryGb = totalMemoryKb / 1024 / 1024;
    double freeMemoryGb = freeMemoryKb / 1024 / 1024;

    Console.WriteLine($"Total RAM: {totalMemoryGb:F2} GB");
    Console.WriteLine($"Available RAM: {freeMemoryGb:F2} GB");
    Console.WriteLine($"RAM Usage: {memoryUsagePercent:F2}%");
}

// Disk information
DriveInfo[] drives = DriveInfo.GetDrives();

foreach (DriveInfo drive in drives)
{
    if (!drive.IsReady)
    {
        continue;
    }

    double totalSpaceGb = drive.TotalSize / 1024.0 / 1024.0 / 1024.0;
    double freeSpaceGb = drive.AvailableFreeSpace / 1024.0 / 1024.0 / 1024.0;
    double usedSpaceGb = totalSpaceGb - freeSpaceGb;

    double freeSpacePercent =
        (freeSpaceGb / totalSpaceGb) * 100;

    Console.WriteLine($"Drive: {drive.Name}");
    Console.WriteLine($"Total Space: {totalSpaceGb:F2} GB");
    Console.WriteLine($"Free Space: {freeSpaceGb:F2} GB");
    Console.WriteLine($"Used Space: {usedSpaceGb:F2} GB");
    Console.WriteLine($"Free Space: {freeSpacePercent:F2}%");
}

// Windows Update
var windowsUpdateCollector = new WindowsUpdateCollector();

var windowsUpdateStatus = windowsUpdateCollector.GetStatus();

Console.WriteLine($"Windows Update Service: {windowsUpdateStatus.ServiceStatus}");
Console.WriteLine($"Pending Updates: {windowsUpdateStatus.PendingUpdateCount}");
Console.WriteLine($"Update Check Succeeded: {windowsUpdateStatus.CheckSucceeded}");