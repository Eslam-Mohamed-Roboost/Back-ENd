using API.Application.Features.Teacher.Classes.DTOs;
using API.Domain.Entities.Teacher;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Portfolio.Queries;

public record GetMySubjectsQuery : IRequest<RequestResult<List<ClassSubjectInfo>>>;

public class GetMySubjectsQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<TeacherClassAssignments> assignmentRepository)
    : RequestHandlerBase<GetMySubjectsQuery, RequestResult<List<ClassSubjectInfo>>>(parameters)
{
    public override async Task<RequestResult<List<ClassSubjectInfo>>> Handle(
        GetMySubjectsQuery request,
        CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        // Get all unique subjects assigned to this teacher
        var subjects = await assignmentRepository.Get(a =>
            a.TeacherId == teacherId && !a.IsDeleted)
            .Include(a => a.Subject)
            .Where(a => a.Subject != null && !a.Subject.IsDeleted)
            .Select(a => new ClassSubjectInfo
            {
                SubjectId = a.SubjectId,
                SubjectName = a.Subject!.Name
            })
            .Distinct()
            .OrderBy(s => s.SubjectName)
            .ToListAsync(cancellationToken);

        return RequestResult<List<ClassSubjectInfo>>.Success(subjects);
    }
}


