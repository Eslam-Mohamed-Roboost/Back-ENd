using System.ComponentModel.DataAnnotations.Schema;
using API.Domain.Enums;

namespace API.Domain.Entities.System;

[Table("SystemSettings", Schema = "System")]
public class SystemSettings : BaseEntity
{
    public string SettingKey { get; set; } = string.Empty;
    public string SettingValue { get; set; } = string.Empty;
    public SettingDataType DataType { get; set; } = SettingDataType.String;
    public SettingCategory Category { get; set; } = SettingCategory.General;
    public string? Description { get; set; }
}
