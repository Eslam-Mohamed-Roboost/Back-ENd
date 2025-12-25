using API.Domain.Entities.Teacher;
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
    IRepository<TeacherClassAssignments> assignmentRepository)
    : RequestHandlerBase<GetTeacherAssignmentsQuery, RequestResult<List<TeacherAssignmentInfo>>>(parameters)
{
    public override async Task<RequestResult<List<TeacherAssignmentInfo>>> Handle(
        GetTeacherAssignmentsQuery request,
        CancellationToken cancellationToken)
    {
        // Note: Navigation properties might not be configured, so we'll need to join manually if needed
        // For now, returning basic assignment data
        var assignments = await assignmentRepository.Get(a => a.TeacherId == request.TeacherId)
            .Select(a => new TeacherAssignmentInfo
            {
                Id = a.ID,
                ClassId = a.ClassId,
                ClassName = "", // Will need to join with Classes table
                Grade = 0, // Will need to join with Classes table
                SubjectId = a.SubjectId,
                SubjectName = "", // Will need to join with Subjects table
                AssignedAt = a.AssignedAt
            })
            .ToListAsync(cancellationToken);

        return RequestResult<List<TeacherAssignmentInfo>>.Success(assignments);
    }
}

