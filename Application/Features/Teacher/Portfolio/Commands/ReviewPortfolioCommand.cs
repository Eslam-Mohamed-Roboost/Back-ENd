using API.Domain.Entities.Portfolio;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.Portfolio.Commands;

public record ReviewPortfolioCommand(
    long PortfolioFileId,
    string? FeedbackNotes = null) : IRequest<RequestResult<bool>>;

public class ReviewPortfolioCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<PortfolioFiles> portfolioRepository)
    : RequestHandlerBase<ReviewPortfolioCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(
        ReviewPortfolioCommand request,
        CancellationToken cancellationToken)
    {
        var portfolioFile = await portfolioRepository.Get(p => p.ID == request.PortfolioFileId)
            .FirstOrDefaultAsync(cancellationToken);

        if (portfolioFile == null)
        {
            return RequestResult<bool>.Failure(ErrorCode.NotFound, "Portfolio file not found");
        }

        portfolioFile.Status = "Reviewed";
        portfolioFile.ReviewedBy = _userState.UserID;
        portfolioFile.ReviewedAt = DateTime.UtcNow;
        portfolioFile.RevisionNotes = request.FeedbackNotes;
        portfolioFile.UpdatedAt = DateTime.UtcNow;

        portfolioRepository.Update(portfolioFile);
        await portfolioRepository.SaveChangesAsync();

        // TODO: Create notification for student (Phase 7)

        return RequestResult<bool>.Success(true, "Portfolio reviewed successfully");
    }
}

