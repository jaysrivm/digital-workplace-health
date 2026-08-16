namespace WorkplaceHealth.Agent.Models;

public class RegistryStatus
{
    public string WindowsProductName { get; set; } = "Unknown";

    public string WindowsDisplayVersion { get; set; } = "Unknown";

    public string CurrentBuild { get; set; } = "Unknown";

    public bool RegistryCheckSucceeded { get; set; }
}