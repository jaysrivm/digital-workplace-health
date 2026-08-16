using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "WorkplaceHealth Agent";
});

builder.Services.AddHostedService<WorkplaceHealth.Agent.HealthReportWorker>();

var host = builder.Build();

host.Run();