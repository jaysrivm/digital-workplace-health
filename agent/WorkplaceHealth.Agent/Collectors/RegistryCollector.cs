using Microsoft.Win32;
using WorkplaceHealth.Agent.Models;

namespace WorkplaceHealth.Agent.Collectors;

public class RegistryCollector
{
    public RegistryStatus GetStatus()
    {
        var result = new RegistryStatus();

        try
        {
            using RegistryKey? key = Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

            if (key == null)
            {
                result.RegistryCheckSucceeded = false;
                return result;
            }

            result.WindowsProductName =
                key.GetValue("ProductName")?.ToString() ?? "Unknown";

            result.WindowsDisplayVersion =
                key.GetValue("DisplayVersion")?.ToString() ?? "Unknown";

            result.CurrentBuild =
                key.GetValue("CurrentBuild")?.ToString() ?? "Unknown";

            result.RegistryCheckSucceeded = true;
        }
        catch (Exception)
        {
            result.RegistryCheckSucceeded = false;
        }

        return result;
    }
}