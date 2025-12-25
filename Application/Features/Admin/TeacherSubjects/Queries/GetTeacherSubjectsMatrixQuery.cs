using API.Application.Features.Admin.TeacherSubjects.DTOs;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.TeacherSubjects.Queries;

public record GetTeacherSubjectsMatrixQuery : IRequest<RequestResult<List<TeacherSubjectMatrixDto>>>;

public class GetTeacherSubjectsMatrixQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.User> userRepository,
    IRepository<Domain.Entities.Teacher.TeacherSubjects> teacherSubjectsRepository,
    IRepository<Domain.Entities.General.Subjects> subjectsRepository,
    IRepository<Domain.Entities.Teacher.TeacherCpdProgress> cpdProgressRepository,
    IRepository<Domain.Entities.Portfolio.TeacherFeedback> teacherFeedbackRepository)
    : RequestHandlerBase<GetTeacherSubjectsMatrixQuery, RequestResult<List<TeacherSubjectMatrixDto>>>(parameters)
{
    public override async Task<RequestResult<List<TeacherSubjectMatrixDto>>> Handle(GetTeacherSubjectsMatrixQuery request, CancellationToken cancellationToken)
    {
        // Get all teachers
        var teachers = await userRepository.Get()
            .Where(u => u.Role == ApplicationRole.Teacher)
            .ToListAsync(cancellationToken);

        var teacherIds = teachers.Select(t => t.ID).ToList();

        // Get teacher subjects
        var teacherSubjectsData = await (
            from ts in teacherSubjectsRepository.Get()
            join s in subjectsRepository.Get() on ts.SubjectId equals s.ID
            where teacherIds.Contains(ts.TeacherId)
            select new
            {
                ts.TeacherId,
                SubjectName = s.Name,
                ts.Grade
            })
            .ToListAsync(cancellationToken);

        // Get CPD progress - count completed modules per teacher
        var cpdBadges = await cpdProgressRepository.Get()
            .Where(cp => teacherIds.Contains(cp.TeacherId) && cp.Status == ProgressStatus.Completed)
            .GroupBy(cp => cp.TeacherId)
            .Select(g => new
            {
                TeacherId = g.Key,
                BadgeCount = g.Count()
            })
            .ToListAsync(cancellationToken);

        // Get portfolio activity - count feedback/reviews per teacher
        var portfolioActivity = await teacherFeedbackRepository.Get()
            .Where(tf => teacherIds.Contains(tf.TeacherId))
            .GroupBy(tf => tf.TeacherId)
            .Select(g => new
            {
                TeacherId = g.Key,
                ActivityCount = g.Count(),
                LastActive = g.Max(x => x.CreatedAt)
            })
            .ToListAsync(cancellationToken);

        // Build the result
        var result = teachers.Select(teacher =>
        {
            var subjects = teacherSubjectsData
                .Where(ts => ts.TeacherId == teacher.ID)
                .Select(ts => ts.SubjectName)
                .Distinct()
                .ToList();

            var grades = teacherSubjectsData
                .Where(ts => ts.TeacherId == teacher.ID)
                .Select(ts => ts.Grade.ToString())
                .Distinct()
                .ToList();

            var cpdCount = cpdBadges
                .FirstOrDefault(b => b.TeacherId == teacher.ID)?.BadgeCount ?? 0;

            var activity = portfolioActivity
                .FirstOrDefault(a => a.TeacherId == teacher.ID);

            return new TeacherSubjectMatrixDto
            {
                TeacherId = teacher.ID,
                TeacherName = teacher.Name,
                Email = teacher.Email,
                Subjects = subjects,
                Grades = grades,
                CpdBadgesEarned = cpdCount,
                PortfolioActivity = activity?.ActivityCount ?? 0,
                LastActive = activity?.LastActive ?? teacher.CreatedAt
            };
        }).ToList();

        return RequestResult<List<TeacherSubjectMatrixDto>>.Success(result);
    }
}
