using API.Application.Features.Teacher.Portfolio.DTOs;
using API.Domain.Entities;
using API.Domain.Entities.General;
using API.Domain.Entities.Portfolio;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Portfolio.Queries;

public record GetSubjectStudentsQuery(long SubjectId) : IRequest<RequestResult<List<TeacherStudentSummaryDto>>>;

public class GetSubjectStudentsQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.User> userRepository,
     IRepository<PortfolioFiles> portfolioFilesRepository,
    IRepository<TeacherFeedback> feedbackRepository)
    : RequestHandlerBase<GetSubjectStudentsQuery, RequestResult<List<TeacherStudentSummaryDto>>>(parameters)
{
    public override async Task<RequestResult<List<TeacherStudentSummaryDto>>> Handle(GetSubjectStudentsQuery request, CancellationToken cancellationToken)
    {
        // For now, return all students who have at least one file or feedback in this subject.
        var studentIdsWithFiles = await portfolioFilesRepository.Get(x => x.SubjectId == request.SubjectId)
            .Select(x => x.StudentId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var studentIdsWithFeedback = await feedbackRepository.Get(x => x.SubjectId == request.SubjectId)
            .Select(x => x.StudentId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var studentIds = studentIdsWithFiles.Union(studentIdsWithFeedback).ToList();

        if (!studentIds.Any())
            return RequestResult<List<TeacherStudentSummaryDto>>.Success(new List<TeacherStudentSummaryDto>());

        var students = await userRepository.Get(x => studentIds.Contains(x.ID))
            .ToListAsync(cancellationToken);

        var latestSubmissions = await portfolioFilesRepository.Get(x => studentIds.Contains(x.StudentId) && x.SubjectId == request.SubjectId)
            .OrderByDescending(x => x.UploadedAt)
            .GroupBy(x => x.StudentId)
            .Select(g => g.First())
            .ToListAsync(cancellationToken);

        var latestMap = latestSubmissions.ToDictionary(x => x.StudentId, x => x);

        var result = students.Select(s =>
        {
            latestMap.TryGetValue(s.ID, out var latest);

            return new TeacherStudentSummaryDto
            {
                Id = s.ID,
                Name = s.Name,
                Email = s.Email,
                Avatar = string.Empty,
                PortfolioStatus = "reviewed", // placeholder, can be derived from PortfolioStatus table
                LatestSubmission = latest == null
                    ? null
                    : new TeacherSubmissionSummaryDto
                    {
                        Id = latest.ID,
                        Title = latest.FileName,
                        Content = string.Empty,
                        SubmittedAt = latest.UploadedAt,
                        Type = "file"
                    }
            };
        }).ToList();

        return RequestResult<List<TeacherStudentSummaryDto>>.Success(result);
    }
}


