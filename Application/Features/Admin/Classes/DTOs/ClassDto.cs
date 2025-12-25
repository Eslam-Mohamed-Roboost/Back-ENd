namespace API.Application.Features.Admin.Classes.DTOs;

public class ClassDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Grade { get; set; }
    public long? TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public int StudentCount { get; set; }
    public List<long> SubjectIds { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class CreateClassRequest
{
    public string Name { get; set; } = string.Empty;
    public int Grade { get; set; }
    public long? TeacherId { get; set; }
}

public class UpdateClassRequest
{
    public string Name { get; set; } = string.Empty;
    public int Grade { get; set; }
    public long? TeacherId { get; set; }
}

