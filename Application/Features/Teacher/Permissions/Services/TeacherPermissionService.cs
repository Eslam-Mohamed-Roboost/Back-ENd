using API.Domain.Entities.Teacher;
using API.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Permissions.Services;

public class TeacherPermissionService
{
    private readonly IRepository<TeacherPermissions> _permissionRepository;
    private readonly IRepository<TeacherClassAssignments> _assignmentRepository;

    public TeacherPermissionService(
        IRepository<TeacherPermissions> permissionRepository,
        IRepository<TeacherClassAssignments> assignmentRepository)
    {
        _permissionRepository = permissionRepository;
        _assignmentRepository = assignmentRepository;
    }

    /// <summary>
    /// Get teacher permissions, with defaults if no specific permissions exist
    /// </summary>
    public async Task<TeacherPermissions?> GetTeacherPermissionsAsync(
        long teacherId,
        long? classId = null,
        long? subjectId = null,
        CancellationToken cancellationToken = default)
    {
        // Try to find specific permission (class + subject)
        if (classId.HasValue && subjectId.HasValue)
        {
            var specific = await _permissionRepository.Get(p =>
                p.TeacherId == teacherId &&
                p.ClassId == classId.Value &&
                p.SubjectId == subjectId.Value &&
                !p.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (specific != null) return specific;
        }

        // Try to find class-level permission
        if (classId.HasValue)
        {
            var classLevel = await _permissionRepository.Get(p =>
                p.TeacherId == teacherId &&
                p.ClassId == classId.Value &&
                p.SubjectId == null &&
                !p.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (classLevel != null) return classLevel;
        }

        // Try to find subject-level permission
        if (subjectId.HasValue)
        {
            var subjectLevel = await _permissionRepository.Get(p =>
                p.TeacherId == teacherId &&
                p.ClassId == null &&
                p.SubjectId == subjectId.Value &&
                !p.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (subjectLevel != null) return subjectLevel;
        }

        // Try to find general teacher permission (no class/subject)
        var general = await _permissionRepository.Get(p =>
            p.TeacherId == teacherId &&
            p.ClassId == null &&
            p.SubjectId == null &&
            !p.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (general != null) return general;

        // Return default permissions if none found
        return new TeacherPermissions
        {
            TeacherId = teacherId,
            CanCreateExercises = true,
            CanCreateExaminations = true,
            CanGradeOwnClasses = true,
            CanGradeAllClasses = false,
            CanApproveGrades = false
        };
    }

    /// <summary>
    /// Check if teacher can create exercises for the given class/subject
    /// </summary>
    public async Task<bool> CanCreateExercisesAsync(
        long teacherId,
        long classId,
        long subjectId,
        CancellationToken cancellationToken = default)
    {
        // First check if teacher is assigned to this class/subject
        var isAssigned = await _assignmentRepository.Get(a =>
            a.TeacherId == teacherId &&
            a.ClassId == classId &&
            a.SubjectId == subjectId &&
            !a.IsDeleted)
            .AnyAsync(cancellationToken);

        if (!isAssigned) return false;

        var permissions = await GetTeacherPermissionsAsync(teacherId, classId, subjectId, cancellationToken);
        return permissions?.CanCreateExercises ?? true; // Default to true if no permissions set
    }

    /// <summary>
    /// Check if teacher can create examinations for the given class/subject
    /// </summary>
    public async Task<bool> CanCreateExaminationsAsync(
        long teacherId,
        long classId,
        long subjectId,
        CancellationToken cancellationToken = default)
    {
        // First check if teacher is assigned to this class/subject
        var isAssigned = await _assignmentRepository.Get(a =>
            a.TeacherId == teacherId &&
            a.ClassId == classId &&
            a.SubjectId == subjectId &&
            !a.IsDeleted)
            .AnyAsync(cancellationToken);

        if (!isAssigned) return false;

        var permissions = await GetTeacherPermissionsAsync(teacherId, classId, subjectId, cancellationToken);
        return permissions?.CanCreateExaminations ?? true; // Default to true if no permissions set
    }

    /// <summary>
    /// Check if teacher can grade for the given class/subject
    /// </summary>
    public async Task<bool> CanGradeAsync(
        long teacherId,
        long classId,
        long subjectId,
        CancellationToken cancellationToken = default)
    {
        var permissions = await GetTeacherPermissionsAsync(teacherId, classId, subjectId, cancellationToken);

        // If can grade all classes, return true
        if (permissions?.CanGradeAllClasses == true) return true;

        // If can grade own classes, check if teacher is assigned to this class/subject
        if (permissions?.CanGradeOwnClasses == true)
        {
            var isAssigned = await _assignmentRepository.Get(a =>
                a.TeacherId == teacherId &&
                a.ClassId == classId &&
                a.SubjectId == subjectId &&
                !a.IsDeleted)
                .AnyAsync(cancellationToken);

            return isAssigned;
        }

        return false;
    }

    /// <summary>
    /// Check if teacher can approve grades
    /// </summary>
    public async Task<bool> CanApproveGradesAsync(
        long teacherId,
        CancellationToken cancellationToken = default)
    {
        var permissions = await GetTeacherPermissionsAsync(teacherId, null, null, cancellationToken);
        return permissions?.CanApproveGrades ?? false; // Default to false for approval
    }
}

