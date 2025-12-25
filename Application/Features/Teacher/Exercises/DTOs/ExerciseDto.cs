using API.Helpers.Attributes;
using System.Text.Json.Serialization;

namespace API.Application.Features.Teacher.Exercises.DTOs;

public class ExerciseDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }

    [JsonConverter(typeof(LongAsStringConverter))]
    public long TeacherId { get; set; }

    public string TeacherName { get; set; } = string.Empty;

    [JsonConverter(typeof(LongAsStringConverter))]
    public long ClassId { get; set; }

    public string ClassName { get; set; } = string.Empty;

    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }

    public string SubjectName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty; // Homework, Classwork, Project
    public DateTime? DueDate { get; set; }
    public decimal MaxScore { get; set; }
    public string? Instructions { get; set; }
    public string? Attachments { get; set; } // JSON
    public string Status { get; set; } = "Draft"; // Draft, Published, Closed
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int SubmissionCount { get; set; }
    public int GradedCount { get; set; }
}

public class CreateExerciseRequest
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long ClassId { get; set; }

    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty; // Homework, Classwork, Project
    public DateTime? DueDate { get; set; }
    public decimal MaxScore { get; set; } = 100.00m;
    public string? Instructions { get; set; }
    public string? Attachments { get; set; } // JSON
    public string Status { get; set; } = "Draft"; // Draft, Published, Closed
}

public class UpdateExerciseRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal? MaxScore { get; set; }
    public string? Instructions { get; set; }
    public string? Attachments { get; set; }
    public string? Status { get; set; }
}

public class ExerciseSubmissionDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }

    [JsonConverter(typeof(LongAsStringConverter))]
    public long ExerciseId { get; set; }

    [JsonConverter(typeof(LongAsStringConverter))]
    public long StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public DateTime? SubmittedAt { get; set; }
    public string? Content { get; set; }
    public string? Attachments { get; set; } // JSON
    public string Status { get; set; } = "Submitted"; // Submitted, Late, Graded
    public decimal? Score { get; set; }
    public string? Feedback { get; set; }

    [JsonConverter(typeof(LongAsStringConverter))]
    public long? GradedBy { get; set; }

    public string? GraderName { get; set; }
    public DateTime? GradedAt { get; set; }
    public bool IsLate { get; set; }
}

public class GradeExerciseSubmissionRequest
{
    public decimal Score { get; set; }
    public string? Feedback { get; set; }
}

