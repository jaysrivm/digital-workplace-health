using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WorkplaceHealth.Agent.Collectors;
using WorkplaceHealth.Agent.Models;

namespace WorkplaceHealth.Agent;

public class HealthReportWorker : BackgroundService
{
    private readonly ILogger<HealthReportWorker> _logger;

    private readonly CpuCollector _cpuCollector = new();
    private readonly MemoryCollector _memoryCollector = new();
    private readonly DiskCollector _diskCollector = new();
    private readonly WindowsUpdateCollector _windowsUpdateCollector = new();
    private readonly ServiceCollector _serviceCollector = new();
    private readonly RegistryCollector _registryCollector = new();

    public HealthReportWorker(ILogger<HealthReportWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var healthReport = new HealthReport
                {
                    DeviceName = Environment.MachineName,
                    CollectedAtUtc = DateTime.UtcNow,
                    Cpu = _cpuCollector.GetInfo(),
                    Memory = _memoryCollector.GetInfo(),
                    Disks = _diskCollector.GetInfo(),
                    WindowsUpdate = _windowsUpdateCollector.GetStatus(),
                    Services = _serviceCollector.GetImportantServices(),
                    Registry = _registryCollector.GetStatus()
                };

                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(
                    healthReport,
                    jsonOptions);

                _logger.LogInformation(
                    "Health report collected:\n{HealthReport}",
                    json);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to collect health report.");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(5),
                stoppingToken);
        }
    }
}