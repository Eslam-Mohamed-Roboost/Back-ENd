using API.Helpers.Attributes;
using System.Text.Json.Serialization;

namespace API.Application.Features.Teacher.Grades.DTOs;

public class GradeDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }

    [JsonConverter(typeof(LongAsStringConverter))]
    public long StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    [JsonConverter(typeof(LongAsStringConverter))]
    public long ClassId { get; set; }

    public string ClassName { get; set; } = string.Empty;

    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }

    public string SubjectName { get; set; } = string.Empty;

    [JsonConverter(typeof(LongAsStringConverter))]
    public long? ExerciseId { get; set; }

    public string? ExerciseTitle { get; set; }

    [JsonConverter(typeof(LongAsStringConverter))]
    public long? ExaminationId { get; set; }

    public string? ExaminationTitle { get; set; }

    public decimal Score { get; set; }
    public decimal MaxScore { get; set; }
    public decimal Percentage { get; set; }
    public string? LetterGrade { get; set; }
    public string? Term { get; set; }
    public int Year { get; set; }

    [JsonConverter(typeof(LongAsStringConverter))]
    public long GradedBy { get; set; }

    public string GraderName { get; set; } = string.Empty;
    public DateTime GradedAt { get; set; }
    public string Status { get; set; } = "Draft"; // Draft, PendingApproval, Approved, Rejected

    [JsonConverter(typeof(LongAsStringConverter))]
    public long? ApprovedBy { get; set; }

    public string? ApproverName { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? Notes { get; set; }
}

public class CreateGradeRequest
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long StudentId { get; set; }

    [JsonConverter(typeof(LongAsStringConverter))]
    public long ClassId { get; set; }

    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }

    [JsonConverter(typeof(LongAsStringConverter))]
    public long? ExerciseId { get; set; }

    [JsonConverter(typeof(LongAsStringConverter))]
    public long? ExaminationId { get; set; }

    public decimal Score { get; set; }
    public decimal MaxScore { get; set; }
    public string? Term { get; set; }
    public int Year { get; set; }
    public string? Notes { get; set; }
}

public class UpdateGradeRequest
{
    public decimal? Score { get; set; }
    public decimal? MaxScore { get; set; }
    public string? Term { get; set; }
    public int? Year { get; set; }
    public string? Notes { get; set; }
}

public class GradeApprovalRequest
{
    public bool Approve { get; set; }
    public string? Notes { get; set; }
}

public class GradeSummaryDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long ClassId { get; set; }

    public string ClassName { get; set; } = string.Empty;

    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }

    public string SubjectName { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
    public int GradedStudents { get; set; }
    public decimal AverageScore { get; set; }
    public decimal HighestScore { get; set; }
    public decimal LowestScore { get; set; }
    public Dictionary<string, int> GradeDistribution { get; set; } = new(); // LetterGrade -> Count
    public string? Term { get; set; }
    public int Year { get; set; }
}

