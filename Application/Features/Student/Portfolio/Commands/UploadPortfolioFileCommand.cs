using API.Application.Features.Student.Portfolio.DTOs;
using API.Domain.Entities.Portfolio;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Student.Portfolio.Commands;

public record UploadPortfolioFileCommand(PortfolioUploadRequest Request) : IRequest<RequestResult<PortfolioFileDto>>;

public class UploadPortfolioFileCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<PortfolioFiles> portfolioFilesRepository)
    : RequestHandlerBase<UploadPortfolioFileCommand, RequestResult<PortfolioFileDto>>(parameters)
{
    public override async Task<RequestResult<PortfolioFileDto>> Handle(UploadPortfolioFileCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;
        var file = request.Request.File;

        if (file == null || file.Length == 0)
        {
            return RequestResult<PortfolioFileDto>.Failure(ErrorCode.ValidationError, "File is required");
        }

        // Basic size validation (50 MB), can be moved to config later
        const long maxSizeBytes = 50L * 1024 * 1024;
        if (file.Length > maxSizeBytes)
        {
            return RequestResult<PortfolioFileDto>.Failure(ErrorCode.FileTooLarge, "File size exceeds 50 MB limit");
        }

        // Map content type to FileType enum (fallback to Other)
        var fileType = GetFileTypeFromContentType(file.ContentType, Path.GetExtension(file.FileName));

        // For now, we don't have a storage service wired; store logical path only.
        var storagePath = $"portfolio/{studentId}/{request.Request.SubjectId}/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        var entity = new PortfolioFiles
        {
            StudentId = studentId,
            SubjectId = request.Request.SubjectId,
            FileName = file.FileName,
            FileType = fileType,
            FileSize = file.Length,
            StoragePath = storagePath,
            ThumbnailUrl = null,
            PreviewUrl = null,
            DownloadUrl = storagePath,
            UploadedAt = DateTime.UtcNow
        };

        portfolioFilesRepository.Add(entity);

        var dto = new PortfolioFileDto
        {
            Id = entity.ID,
            FileName = entity.FileName,
            FileType = entity.FileType.ToString(),
            FileSize = entity.FileSize,
            UploadDate = entity.UploadedAt,
            SubjectId = entity.SubjectId,
            ThumbnailUrl = entity.ThumbnailUrl,
            PreviewUrl = entity.PreviewUrl,
            DownloadUrl = entity.DownloadUrl
        };

        return RequestResult<PortfolioFileDto>.Success(dto);
    }

    private static FileType GetFileTypeFromContentType(string contentType, string extension)
    {
        extension = extension.ToLowerInvariant();

        return extension switch
        {
            ".pdf" => FileType.Pdf,
            ".docx" => FileType.Docx,
            ".pptx" => FileType.Pptx,
            ".jpg" or ".jpeg" => FileType.Jpg,
            ".png" => FileType.Png,
            ".mp4" => FileType.Mp4,
            _ => FileType.Other
        };
    }
}


