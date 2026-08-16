namespace WorkplaceHealth.Api.Models;

public class DiskReport
{
    public int Id { get; set; }

    public int HealthReportId { get; set; }

    public HealthReport HealthReport { get; set; } = null!;

    public string Drive { get; set; } = string.Empty;

    public double TotalGb { get; set; }

    public double FreeGb { get; set; }

    public double UsedGb { get; set; }

    public double FreePercent { get; set; }
}