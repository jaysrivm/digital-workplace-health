using System.Text.Json;
using WorkplaceHealth.Agent.Collectors;
using WorkplaceHealth.Agent.Models;

var cpuCollector = new CpuCollector();
var memoryCollector = new MemoryCollector();
var diskCollector = new DiskCollector();
var windowsUpdateCollector = new WindowsUpdateCollector();
var serviceCollector = new ServiceCollector();
var registryCollector = new RegistryCollector();

var healthReport = new HealthReport
{
    DeviceName = Environment.MachineName,

    CollectedAtUtc = DateTime.UtcNow,

    Cpu = cpuCollector.GetInfo(),

    Memory = memoryCollector.GetInfo(),

    Disks = diskCollector.GetInfo(),

    WindowsUpdate = windowsUpdateCollector.GetStatus(),

    Services = serviceCollector.GetImportantServices(),

    Registry = registryCollector.GetStatus()
};

var jsonOptions = new JsonSerializerOptions
{
    WriteIndented = true
};

string json = JsonSerializer.Serialize(
    healthReport,
    jsonOptions);

Console.WriteLine(json);