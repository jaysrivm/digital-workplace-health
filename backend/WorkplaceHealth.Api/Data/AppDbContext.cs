using Microsoft.EntityFrameworkCore;
using WorkplaceHealth.Api.Models;

namespace WorkplaceHealth.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Device> Devices => Set<Device>();

    public DbSet<HealthReport> HealthReports => Set<HealthReport>();

    public DbSet<DiskReport> DiskReports => Set<DiskReport>();

    public DbSet<ServiceReport> ServiceReports => Set<ServiceReport>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Device>()
            .HasMany(d => d.HealthReports)
            .WithOne(r => r.Device)
            .HasForeignKey(r => r.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<HealthReport>()
            .HasMany(r => r.Disks)
            .WithOne(d => d.HealthReport)
            .HasForeignKey(d => d.HealthReportId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<HealthReport>()
            .HasMany(r => r.Services)
            .WithOne(s => s.HealthReport)
            .HasForeignKey(s => s.HealthReportId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}