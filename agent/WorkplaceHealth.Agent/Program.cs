using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WorkplaceHealth.Agent;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Configuration
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(
                "appsettings.json",
                optional: false,
                reloadOnChange: true);

        builder.Services.AddWindowsService(options =>
        {
            options.ServiceName = "Workplace Health Agent";
        });

        builder.Services.AddHttpClient();

        builder.Services.AddHostedService<HealthReportWorker>();

        var host = builder.Build();

        host.Run();
    }
}