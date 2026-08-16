namespace WorkplaceHealth.Api.Models;

public class ServiceReport
{
    public int Id { get; set; }

    public int HealthReportId { get; set; }

    public HealthReport HealthReport { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string StartType { get; set; } = string.Empty;
}