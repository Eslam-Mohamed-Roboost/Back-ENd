using API.Helpers.Attributes;
using System.Text.Json.Serialization;

namespace API.Application.Features.Teacher.Classes.DTOs;

public class TeacherClassDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public int Grade { get; set; }
    public int StudentCount { get; set; }
    public List<ClassSubjectInfo> Subjects { get; set; } = new();
}

public class ClassSubjectInfo
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }

    public string SubjectName { get; set; } = string.Empty;
}

public class ClassStudentDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    [JsonConverter(typeof(LongAsStringConverter))]
    public long ClassId { get; set; }

    public string ClassName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? LastLogin { get; set; }
}

