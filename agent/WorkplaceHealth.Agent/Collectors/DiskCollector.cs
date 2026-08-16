namespace WorkplaceHealth.Agent.Collectors;

public class DiskCollector
{
    public List<DiskInfo> GetInfo()
    {
        var disks = new List<DiskInfo>();

        foreach (DriveInfo drive in DriveInfo.GetDrives())
        {
            if (!drive.IsReady)
            {
                continue;
            }

            double totalGb =
                drive.TotalSize / 1024.0 / 1024.0 / 1024.0;

            double freeGb =
                drive.AvailableFreeSpace / 1024.0 / 1024.0 / 1024.0;

            double usedGb = totalGb - freeGb;

            double freePercent =
                (freeGb / totalGb) * 100;

            disks.Add(new DiskInfo
            {
                Drive = drive.Name,
                TotalGb = totalGb,
                FreeGb = freeGb,
                UsedGb = usedGb,
                FreePercent = freePercent
            });
        }

        return disks;
    }
}

public class DiskInfo
{
    public string Drive { get; set; } = string.Empty;

    public double TotalGb { get; set; }

    public double FreeGb { get; set; }

    public double UsedGb { get; set; }

    public double FreePercent { get; set; }
}