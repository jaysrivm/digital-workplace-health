using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkplaceHealth.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeviceName = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastSeenAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HealthReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeviceId = table.Column<int>(type: "INTEGER", nullable: false),
                    CollectedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CpuName = table.Column<string>(type: "TEXT", nullable: false),
                    CpuLogicalProcessors = table.Column<int>(type: "INTEGER", nullable: false),
                    CpuUsagePercent = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalMemoryGb = table.Column<double>(type: "REAL", nullable: false),
                    AvailableMemoryGb = table.Column<double>(type: "REAL", nullable: false),
                    MemoryUsagePercent = table.Column<double>(type: "REAL", nullable: false),
                    WindowsUpdateServiceStatus = table.Column<string>(type: "TEXT", nullable: false),
                    PendingUpdateCount = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdateCheckSucceeded = table.Column<bool>(type: "INTEGER", nullable: false),
                    WindowsProductName = table.Column<string>(type: "TEXT", nullable: false),
                    WindowsDisplayVersion = table.Column<string>(type: "TEXT", nullable: false),
                    CurrentBuild = table.Column<string>(type: "TEXT", nullable: false),
                    RegistryCheckSucceeded = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HealthReports_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiskReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HealthReportId = table.Column<int>(type: "INTEGER", nullable: false),
                    Drive = table.Column<string>(type: "TEXT", nullable: false),
                    TotalGb = table.Column<double>(type: "REAL", nullable: false),
                    FreeGb = table.Column<double>(type: "REAL", nullable: false),
                    UsedGb = table.Column<double>(type: "REAL", nullable: false),
                    FreePercent = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiskReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiskReports_HealthReports_HealthReportId",
                        column: x => x.HealthReportId,
                        principalTable: "HealthReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HealthReportId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    StartType = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceReports_HealthReports_HealthReportId",
                        column: x => x.HealthReportId,
                        principalTable: "HealthReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiskReports_HealthReportId",
                table: "DiskReports",
                column: "HealthReportId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthReports_DeviceId",
                table: "HealthReports",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReports_HealthReportId",
                table: "ServiceReports",
                column: "HealthReportId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiskReports");

            migrationBuilder.DropTable(
                name: "ServiceReports");

            migrationBuilder.DropTable(
                name: "HealthReports");

            migrationBuilder.DropTable(
                name: "Devices");
        }
    }
}
