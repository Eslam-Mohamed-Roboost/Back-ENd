using API.Application.Features.Teacher.Portfolio.DTOs;
using API.Domain.Entities.Portfolio;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SubjectEntity = API.Domain.Entities.General.Subjects;

namespace API.Application.Features.Teacher.Portfolio.Queries;

public record GetStudentPortfolioDetailQuery(
    long StudentId, 
    long SubjectId) : IRequest<RequestResult<StudentPortfolioDetailDto>>;

public class GetStudentPortfolioDetailQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.User> userRepository,
    IRepository<SubjectEntity> subjectRepository,
    IRepository<PortfolioFiles> portfolioRepository)
    : RequestHandlerBase<GetStudentPortfolioDetailQuery, RequestResult<StudentPortfolioDetailDto>>(parameters)
{
    public override async Task<RequestResult<StudentPortfolioDetailDto>> Handle(
        GetStudentPortfolioDetailQuery request,
        CancellationToken cancellationToken)
    {
        // Get student info
        var student = await userRepository.Get(u => u.ID == request.StudentId)
            .FirstOrDefaultAsync(cancellationToken);

        if (student == null)
        {
            return RequestResult<StudentPortfolioDetailDto>.Failure(ErrorCode.NotFound, "Student not found");
        }

        // Get subject info
        var subject = await subjectRepository.Get(s => s.ID == request.SubjectId)
            .FirstOrDefaultAsync(cancellationToken);

        if (subject == null)
        {
            return RequestResult<StudentPortfolioDetailDto>.Failure(ErrorCode.NotFound, "Subject not found");
        }

        // Get portfolio files
        var portfolioFiles = await portfolioRepository.Get(p => 
            p.StudentId == request.StudentId && 
            p.SubjectId == request.SubjectId)
            .OrderByDescending(p => p.UploadedAt)
            .ToListAsync(cancellationToken);

        var fileDtos = new List<TeacherPortfolioFileDto>();

        foreach (var file in portfolioFiles)
        {
            string? reviewerName = null;
            if (file.ReviewedBy.HasValue)
            {
                var reviewer = await userRepository.Get(u => u.ID == file.ReviewedBy.Value)
                    .FirstOrDefaultAsync(cancellationToken);
                reviewerName = reviewer?.Name;
            }

            fileDtos.Add(new TeacherPortfolioFileDto
            {
                Id = file.ID,
                FileName = file.FileName,
                FileType = file.FileType.ToString(),
                FileSize = file.FileSize,
                DownloadUrl = file.DownloadUrl ?? string.Empty,
                UploadedAt = file.UploadedAt,
                Status = file.Status ?? "Pending",
                ReviewedBy = file.ReviewedBy,
                ReviewerName = reviewerName,
                ReviewedAt = file.ReviewedAt,
                RevisionNotes = file.RevisionNotes
            });
        }

        var detail = new StudentPortfolioDetailDto
        {
            StudentId = student.ID,
            StudentName = student.Name,
            SubjectId = subject.ID,
            SubjectName = subject.Name,
            Files = fileDtos
        };

        return RequestResult<StudentPortfolioDetailDto>.Success(detail);
    }
}

