using API.Application.Services;
using API.Domain.Entities.Portfolio;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserEntity = API.Domain.Entities.User;

namespace API.Application.Features.Teacher.Portfolio.Commands;

public record RequestRevisionCommand(
    long PortfolioFileId,
    string RevisionNotes) : IRequest<RequestResult<bool>>;

public class RequestRevisionCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<PortfolioFiles> portfolioRepository,
    IRepository<UserEntity> userRepository,
    INotificationService notificationService)
    : RequestHandlerBase<RequestRevisionCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(
        RequestRevisionCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RevisionNotes))
        {
            return RequestResult<bool>.Failure(ErrorCode.ValidationError, "Revision notes are required");
        }

        var portfolioFile = await portfolioRepository.Get(p => p.ID == request.PortfolioFileId)
            .FirstOrDefaultAsync(cancellationToken);

        if (portfolioFile == null)
        {
            return RequestResult<bool>.Failure(ErrorCode.NotFound, "Portfolio file not found");
        }

        portfolioFile.Status = "NeedsRevision";
        portfolioFile.ReviewedBy = _userState.UserID;
        portfolioFile.ReviewedAt = DateTime.UtcNow;
        portfolioFile.RevisionNotes = request.RevisionNotes;
        portfolioFile.UpdatedAt = DateTime.UtcNow;

        portfolioRepository.Update(portfolioFile);
        await portfolioRepository.SaveChangesAsync();

        // Notify student
        var teacher = await userRepository.Get(u => u.ID == _userState.UserID).FirstOrDefaultAsync(cancellationToken);
        var teacherName = teacher?.Name ?? "Your teacher";

        await notificationService.SendPortfolioReviewNotificationAsync(
            portfolioFile.StudentId,
            teacherName,
            portfolioFile.FileName,
            true, // Needs revision
            request.RevisionNotes,
            portfolioFile.ID,
            cancellationToken);

        return RequestResult<bool>.Success(true, "Revision requested successfully");
    }
}

