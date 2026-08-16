using System.ServiceProcess;
using WorkplaceHealth.Agent.Models;

namespace WorkplaceHealth.Agent.Collectors;

public class ServiceCollector
{
    public List<ServiceStatus> GetImportantServices()
    {
        var servicesToCheck = new[]
        {
            "wuauserv",
            "BITS",
            "WinDefend",
            "Spooler"
        };

        var results = new List<ServiceStatus>();

        foreach (string serviceName in servicesToCheck)
        {
            try
            {
                using var service = new ServiceController(serviceName);

                results.Add(new ServiceStatus
                {
                    Name = service.ServiceName,
                    DisplayName = service.DisplayName,
                    Status = service.Status.ToString()
                });
            }
            catch (InvalidOperationException)
            {
                results.Add(new ServiceStatus
                {
                    Name = serviceName,
                    DisplayName = serviceName,
                    Status = "NotFound"
                });
            }
            catch (Exception)
            {
                results.Add(new ServiceStatus
                {
                    Name = serviceName,
                    DisplayName = serviceName,
                    Status = "Unknown"
                });
            }
        }

        return results;
    }
}