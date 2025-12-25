namespace API.Application.Features.Admin.Settings.DTOs;

public class SystemSettingsDto
{
    public string SchoolName { get; set; } = string.Empty;
    public string? SchoolLogo { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty;
    public string SupportEmail { get; set; } = string.Empty;
    public string ItSupport { get; set; } = string.Empty;
    public string Timezone { get; set; } = "Asia/Dubai";
    public List<BadgeCategorySettingDto> BadgeCategories { get; set; } = new();
    public List<CPDTierDto> CpdTiers { get; set; } = new();
    public List<MissionSettingDto> Missions { get; set; } = new();
    public NotificationFrequencyDto NotificationFrequency { get; set; } = new();
    public BackupSettingsDto BackupSettings { get; set; } = new();
}

public class BadgeCategorySettingDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int BadgeCount { get; set; }
}

public class CPDTierDto
{
    public int Tier { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BadgeRange { get; set; } = string.Empty;
}

public class MissionSettingDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool Enabled { get; set; }
}

public class NotificationFrequencyDto
{
    public string BadgeSubmissions { get; set; } = "Immediate";
    public string ActivitySummary { get; set; } = "Daily";
}

public class BackupSettingsDto
{
    public bool Enabled { get; set; }
    public string Frequency { get; set; } = "Daily";
    public string Time { get; set; } = "02:00";
    public DateTime? LastBackup { get; set; }
    public string StorageLocation { get; set; } = string.Empty;
}

public class SystemLogDto
{
    public string Id { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Level { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
    public string? StackTrace { get; set; }
}
