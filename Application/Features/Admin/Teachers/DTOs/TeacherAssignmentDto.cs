namespace API.Application.Features.Admin.Teachers.DTOs;

public class TeacherAssignmentDto
{
    public long TeacherId { get; set; }
    public List<ClassSubjectAssignment> Assignments { get; set; } = new();
}

public class ClassSubjectAssignment
{
    public long ClassId { get; set; }
    public long SubjectId { get; set; }
}

public class TeacherAssignmentResponse
{
    public int AssignmentsCreated { get; set; }
    public List<string> Errors { get; set; } = new();
}

