using API.Application.Features.Admin.Missions.DTOs;
using API.Domain.Entities.Missions;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Admin.Missions.Commands;

public record CreateMissionResourceCommand(CreateMissionResourceDto Dto) : IRequest<RequestResult<long>>;
public record UploadMissionResourceCommand(UploadMissionResourceRequest Request) : IRequest<RequestResult<long>>;

public class CreateMissionResourceCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<MissionResources> repository,
    IRepository<Domain.Entities.Missions.Missions> missionsRepository)
    : RequestHandlerBase<CreateMissionResourceCommand, RequestResult<long>>(parameters)
{
    public override async Task<RequestResult<long>> Handle(CreateMissionResourceCommand request, CancellationToken cancellationToken)
    {
        // Verify mission exists
        var mission = await missionsRepository.Get(x => x.ID == request.Dto.MissionId)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (mission == null)
            return RequestResult<long>.Failure(ErrorCode.NotFound, "Mission not found");

        var resource = new MissionResources
        {
            MissionId = request.Dto.MissionId,
            Type = request.Dto.Type,
            Title = request.Dto.Title,
            Url = request.Dto.Url,
            Description = request.Dto.Description,
            Order = request.Dto.Order,
            IsRequired = request.Dto.IsRequired
        };

        repository.Add(resource);
        await repository.SaveChangesAsync();

        return RequestResult<long>.Success(resource.ID, "Resource created successfully");
    }
}

public class UploadMissionResourceCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<MissionResources> repository,
    IRepository<Domain.Entities.Missions.Missions> missionsRepository)
    : RequestHandlerBase<UploadMissionResourceCommand, RequestResult<long>>(parameters)
{
    public override async Task<RequestResult<long>> Handle(UploadMissionResourceCommand request, CancellationToken cancellationToken)
    {
        // Verify mission exists
        var mission = await missionsRepository.Get(x => x.ID == request.Request.MissionId)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (mission == null)
            return RequestResult<long>.Failure(ErrorCode.NotFound, "Mission not found");

        string resourceUrl = string.Empty;

        // Handle file upload
        if (request.Request.File != null && request.Request.File.Length > 0)
        {
            // Validate file size (100 MB max for videos, 50 MB for other files)
            var maxSizeBytes = request.Request.Type == "video" 
                ? 100L * 1024 * 1024  // 100 MB for videos
                : 50L * 1024 * 1024;   // 50 MB for other files

            if (request.Request.File.Length > maxSizeBytes)
            {
                return RequestResult<long>.Failure(ErrorCode.FileTooLarge, 
                    $"File size exceeds {maxSizeBytes / (1024 * 1024)} MB limit");
            }

            // Validate file type matches resource type
            var fileExtension = Path.GetExtension(request.Request.File.FileName).ToLowerInvariant();
            if (!IsValidFileType(request.Request.Type, fileExtension))
            {
                return RequestResult<long>.Failure(ErrorCode.ValidationError, 
                    $"File type {fileExtension} is not valid for resource type {request.Request.Type}");
            }

            // Generate storage path
            var storagePath = $"missions/{request.Request.MissionId}/resources/{Guid.NewGuid()}{fileExtension}";
            
            // TODO: In production, save file to cloud storage (Azure Blob, AWS S3, etc.)
            // For now, store the logical path. You'll need to implement actual file storage.
            // Example: await _fileStorageService.SaveFileAsync(request.Request.File, storagePath);
            
            // Store file to local wwwroot/uploads directory (for development)
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "missions", request.Request.MissionId.ToString(), "resources");
            Directory.CreateDirectory(uploadsPath);
            
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsPath, fileName);
            
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.Request.File.CopyToAsync(stream, cancellationToken);
            }

            // Set URL to the stored file path (relative to wwwroot)
            resourceUrl = $"/uploads/missions/{request.Request.MissionId}/resources/{fileName}";
        }
        else if (!string.IsNullOrEmpty(request.Request.Url))
        {
            // Use provided external URL
            resourceUrl = request.Request.Url;
        }
        else
        {
            return RequestResult<long>.Failure(ErrorCode.ValidationError, 
                "Either a file or URL must be provided");
        }

        var resource = new MissionResources
        {
            MissionId = request.Request.MissionId,
            Type = request.Request.Type,
            Title = request.Request.Title,
            Url = resourceUrl,
            Description = request.Request.Description,
            Order = request.Request.Order,
            IsRequired = request.Request.IsRequired
        };

        repository.Add(resource);
        await repository.SaveChangesAsync(cancellationToken);

        return RequestResult<long>.Success(resource.ID, "Resource uploaded successfully");
    }

    private static bool IsValidFileType(string resourceType, string fileExtension)
    {
        return resourceType.ToLowerInvariant() switch
        {
            "video" => new[] { ".mp4", ".webm", ".ogg", ".mov", ".avi", ".mkv", ".m4v" }.Contains(fileExtension),
            "pdf" => fileExtension == ".pdf",
            "article" => new[] { ".pdf", ".docx", ".txt", ".html" }.Contains(fileExtension),
            "interactive" => new[] { ".html", ".htm", ".zip" }.Contains(fileExtension),
            _ => true // Allow other types
        };
    }
}

