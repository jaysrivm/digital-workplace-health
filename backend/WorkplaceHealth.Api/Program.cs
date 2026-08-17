using Microsoft.EntityFrameworkCore;
using WorkplaceHealth.Api.Data;
using WorkplaceHealth.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("Frontend");

app.UseHttpsRedirection();

app.MapPost("/api/devices/report", async (
    DeviceHealthReport report,
    AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(report.DeviceName))
    {
        return Results.BadRequest(new
        {
            message = "DeviceName is required."
        });
    }

    var device = await db.Devices
        .FirstOrDefaultAsync(d =>
            d.DeviceName == report.DeviceName);

    if (device == null)
    {
        device = new Device
        {
            DeviceName = report.DeviceName,
            CreatedAtUtc = DateTime.UtcNow,
            LastSeenAtUtc = report.CollectedAtUtc
        };

        db.Devices.Add(device);
    }
    else
    {
        device.LastSeenAtUtc = report.CollectedAtUtc;
    }

    var healthReport = new HealthReport
    {
        Device = device,
        CollectedAtUtc = report.CollectedAtUtc,

        CpuName = report.Cpu.Name,
        CpuLogicalProcessors = report.Cpu.LogicalProcessors,
        CpuUsagePercent = report.Cpu.UsagePercent,

        TotalMemoryGb = report.Memory.TotalGb,
        AvailableMemoryGb = report.Memory.AvailableGb,
        MemoryUsagePercent = report.Memory.UsagePercent,

        WindowsUpdateServiceStatus =
            report.WindowsUpdate.ServiceStatus,

        PendingUpdateCount =
            report.WindowsUpdate.PendingUpdateCount,

        UpdateCheckSucceeded =
            report.WindowsUpdate.CheckSucceeded,

        WindowsProductName =
            report.Registry.WindowsProductName,

        WindowsDisplayVersion =
            report.Registry.WindowsDisplayVersion,

        CurrentBuild =
            report.Registry.CurrentBuild,

        RegistryCheckSucceeded =
            report.Registry.RegistryCheckSucceeded
    };

    foreach (var disk in report.Disks)
    {
        healthReport.Disks.Add(new DiskReport
        {
            Drive = disk.Drive,
            TotalGb = disk.TotalGb,
            FreeGb = disk.FreeGb,
            UsedGb = disk.UsedGb,
            FreePercent = disk.FreePercent
        });
    }

    foreach (var service in report.Services)
    {
        healthReport.Services.Add(new ServiceReport
        {
            Name = service.Name,
            DisplayName = service.DisplayName,
            Status = service.Status,
            StartType = service.StartType
        });
    }

    db.HealthReports.Add(healthReport);

    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        message = "Health report saved successfully.",
        deviceId = device.Id,
        healthReportId = healthReport.Id
    });
});
app.MapGet("/api/devices", async (AppDbContext db) =>
{
    var devices = await db.Devices
        .OrderBy(d => d.DeviceName)
        .Select(d => new
        {
            d.Id,
            d.DeviceName,
            d.CreatedAtUtc,
            d.LastSeenAtUtc
        })
        .ToListAsync();

    return Results.Ok(devices);
});
app.MapGet("/api/devices/{id:int}/history", async (
    int id,
    AppDbContext db) =>
{
    var deviceExists = await db.Devices
        .AnyAsync(d => d.Id == id);

    if (!deviceExists)
    {
        return Results.NotFound(new
        {
            message = "Device not found."
        });
    }

    var reports = await db.HealthReports
        .Where(r => r.DeviceId == id)
        .OrderByDescending(r => r.CollectedAtUtc)
        .Select(r => new
        {
            r.Id,
            r.CollectedAtUtc,

            Cpu = new
            {
                r.CpuName,
                r.CpuLogicalProcessors,
                r.CpuUsagePercent
            },

            Memory = new
            {
                r.TotalMemoryGb,
                r.AvailableMemoryGb,
                r.MemoryUsagePercent
            },

            WindowsUpdate = new
            {
                r.WindowsUpdateServiceStatus,
                r.PendingUpdateCount,
                r.UpdateCheckSucceeded
            },

            Registry = new
            {
                r.WindowsProductName,
                r.WindowsDisplayVersion,
                r.CurrentBuild,
                r.RegistryCheckSucceeded
            },

            Disks = r.Disks.Select(d => new
            {
                d.Drive,
                d.TotalGb,
                d.FreeGb,
                d.UsedGb,
                d.FreePercent
            }),

            Services = r.Services.Select(s => new
            {
                s.Name,
                s.DisplayName,
                s.Status,
                s.StartType
            })
        })
        .ToListAsync();

    return Results.Ok(reports);
});
app.Run();