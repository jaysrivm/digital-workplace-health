using System.Management;
using WorkplaceHealth.Agent.Models;

namespace WorkplaceHealth.Agent.Collectors;

public class WindowsUpdateCollector
{
    public WindowsUpdateStatus GetStatus()
    {
        var result = new WindowsUpdateStatus();

        try
        {
            // Check Windows Update service status
            using var serviceSearcher = new ManagementObjectSearcher(
                "SELECT Started FROM Win32_Service WHERE Name = 'wuauserv'");

            foreach (ManagementObject service in serviceSearcher.Get())
            {
                bool started = Convert.ToBoolean(service["Started"]);
                result.ServiceStatus = started ? "Running" : "Stopped";
                break;
            }

            // Query pending Windows Updates
            Type? updateSessionType = Type.GetTypeFromProgID(
                "Microsoft.Update.Session");

            if (updateSessionType == null)
            {
                result.CheckSucceeded = false;
                return result;
            }

            dynamic updateSession = Activator.CreateInstance(updateSessionType)!;
            dynamic updateSearcher = updateSession.CreateUpdateSearcher();

            dynamic searchResult = updateSearcher.Search(
                "IsInstalled=0 and IsHidden=0");

            result.PendingUpdateCount = searchResult.Updates.Count;
            result.CheckSucceeded = true;
        }
        catch (Exception)
        {
            result.ServiceStatus = "Unknown";
            result.PendingUpdateCount = 0;
            result.CheckSucceeded = false;
        }

        return result;
    }
}