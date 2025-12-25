using API.Application.Features.Student.Portfolio.DTOs;
using API.Domain.Entities.Portfolio;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Student.Portfolio.Queries;

public record GetStudentPortfolioFilesQuery : IRequest<RequestResult<List<PortfolioFileDto>>>;

public class GetStudentPortfolioFilesQueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<PortfolioFiles> portfolioFilesRepository)
    : RequestHandlerBase<GetStudentPortfolioFilesQuery, RequestResult<List<PortfolioFileDto>>>(parameters)
{
    public override async Task<RequestResult<List<PortfolioFileDto>>> Handle(GetStudentPortfolioFilesQuery request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;
        var files = await portfolioFilesRepository.Get(x => x.StudentId == studentId)
            .AsNoTracking()
            .Select(x => new PortfolioFileDto
            {
                Id = x.ID,
                FileName = x.FileName,
                FileType = x.FileType.ToString(),
                FileSize = x.FileSize,
                UploadDate = x.UploadedAt,
                SubjectId = x.SubjectId,
                ThumbnailUrl = x.ThumbnailUrl,
                PreviewUrl = x.PreviewUrl,
                DownloadUrl = x.DownloadUrl
            })
            .ToListAsync(cancellationToken);

        return RequestResult<List<PortfolioFileDto>>.Success(files);
    }
}
