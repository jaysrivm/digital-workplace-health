namespace WorkplaceHealth.Api.Models;

public class RegistryStatus
{
    public string WindowsProductName { get; set; } = string.Empty;

    public string WindowsDisplayVersion { get; set; } = string.Empty;

    public string CurrentBuild { get; set; } = string.Empty;

    public bool RegistryCheckSucceeded { get; set; }
}