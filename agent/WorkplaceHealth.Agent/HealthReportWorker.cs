using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WorkplaceHealth.Agent.Collectors;
using WorkplaceHealth.Agent.Models;
namespace WorkplaceHealth.Agent;

public class HealthReportWorker : BackgroundService
{
    private readonly ILogger<HealthReportWorker> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    private readonly CpuCollector _cpuCollector = new();
    private readonly MemoryCollector _memoryCollector = new();
    private readonly DiskCollector _diskCollector = new();
    private readonly WindowsUpdateCollector _windowsUpdateCollector = new();
    private readonly ServiceCollector _serviceCollector = new();
    private readonly RegistryCollector _registryCollector = new();

    public HealthReportWorker(
        ILogger<HealthReportWorker> logger,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var baseUrl = _configuration["ApiSettings:BaseUrl"];

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            _logger.LogError(
                "API BaseUrl is not configured.");

            return;
        }

        var reportUrl = $"{baseUrl.TrimEnd('/')}/api/devices/report";

        _logger.LogInformation(
            "Workplace Health Agent started. API endpoint: {ReportUrl}",
            reportUrl);

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

                    WindowsUpdate =
                        _windowsUpdateCollector.GetStatus(),

                    Services =
                        _serviceCollector.GetImportantServices(),

                    Registry =
                        _registryCollector.GetStatus()
                };

                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                string json = JsonSerializer.Serialize(
                    healthReport,
                    jsonOptions);

                _logger.LogInformation(
                    "Health report collected for {DeviceName}.",
                    healthReport.DeviceName);

                var httpClient =
                    _httpClientFactory.CreateClient();

                using var content = new StringContent(
                    json,
                    System.Text.Encoding.UTF8,
                    "application/json");

                using var response = await httpClient.PostAsync(
                    reportUrl,
                    content,
                    stoppingToken);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody =
                        await response.Content.ReadAsStringAsync(
                            stoppingToken);

                    _logger.LogInformation(
                        "Health report sent successfully. API response: {Response}",
                        responseBody);
                }
                else
                {
                    var responseBody =
                        await response.Content.ReadAsStringAsync(
                            stoppingToken);

                    _logger.LogWarning(
                        "Failed to send health report. Status: {StatusCode}, Response: {Response}",
                        response.StatusCode,
                        responseBody);
                }
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to collect or send health report.");
            }

            try
            {
                await Task.Delay(
                    TimeSpan.FromSeconds(10),
                    stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        _logger.LogInformation(
            "Workplace Health Agent stopped.");
    }
}