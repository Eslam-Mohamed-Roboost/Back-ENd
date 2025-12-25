using API.Domain.Entities.Teacher;
using API.Domain.Entities.General;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.Teachers.Queries;

public record GetTeacherAssignmentsQuery(long TeacherId) : IRequest<RequestResult<List<TeacherAssignmentInfo>>>;

public class TeacherAssignmentInfo
{
    public long Id { get; set; }
    public long ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int Grade { get; set; }
    public long SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; }
}

public class GetTeacherAssignmentsQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<TeacherClassAssignments> assignmentRepository,
    IRepository<Classes> classRepository,
    IRepository<Subjects> subjectRepository)
    : RequestHandlerBase<GetTeacherAssignmentsQuery, RequestResult<List<TeacherAssignmentInfo>>>(parameters)
{
    public override async Task<RequestResult<List<TeacherAssignmentInfo>>> Handle(
        GetTeacherAssignmentsQuery request,
        CancellationToken cancellationToken)
    {
        // Get assignments with joins to Classes and Subjects tables
        var assignments = await assignmentRepository.Get(a => a.TeacherId == request.TeacherId && !a.IsDeleted)
            .Join(
                classRepository.Get(c => !c.IsDeleted),
                assignment => assignment.ClassId,
                classEntity => classEntity.ID,
                (assignment, classEntity) => new { Assignment = assignment, Class = classEntity }
            )
            .Join(
                subjectRepository.Get(s => !s.IsDeleted),
                combined => combined.Assignment.SubjectId,
                subject => subject.ID,
                (combined, subject) => new TeacherAssignmentInfo
                {
                    Id = combined.Assignment.ID,
                    ClassId = combined.Assignment.ClassId,
                    ClassName = combined.Class.Name,
                    Grade = combined.Class.Grade,
                    SubjectId = combined.Assignment.SubjectId,
                    SubjectName = subject.Name,
                    AssignedAt = combined.Assignment.AssignedAt
                }
            )
            .ToListAsync(cancellationToken);

        return RequestResult<List<TeacherAssignmentInfo>>.Success(assignments);
    }
}

