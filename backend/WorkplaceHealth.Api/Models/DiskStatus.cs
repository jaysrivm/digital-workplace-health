namespace WorkplaceHealth.Api.Models;

public class DiskStatus
{
    public string Drive { get; set; } = string.Empty;

    public double TotalGb { get; set; }

    public double FreeGb { get; set; }

    public double UsedGb { get; set; }

    public double FreePercent { get; set; }
}