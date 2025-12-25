namespace API.Application.Services;

public interface IStreakCalculationService
{
    /// <summary>
    /// Calculate current streak for a student based on activity logs
    /// </summary>
    Task<int> CalculateStudentStreakAsync(long studentId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Calculate current streak for a teacher based on CPD activity
    /// </summary>
    Task<int> CalculateTeacherStreakAsync(long teacherId, CancellationToken cancellationToken = default);
}

