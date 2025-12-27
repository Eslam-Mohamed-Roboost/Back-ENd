using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Domain.Entities.Portfolio;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Features.Teacher.PortfolioBook.Commands;

public record UpdateMapScoreCommand(UpdateMapScoreRequest Request) : IRequest<RequestResult<PortfolioMapScoreDto>>;

public class UpdateMapScoreCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<PortfolioMapScore> mapScoreRepository)
    : RequestHandlerBase<UpdateMapScoreCommand, RequestResult<PortfolioMapScoreDto>>(parameters)
{
    public override async Task<RequestResult<PortfolioMapScoreDto>> Handle(UpdateMapScoreCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;

        // Check if score already exists for this term/year
        var existing = await mapScoreRepository.Get(m =>
            m.StudentId == req.StudentId &&
            m.SubjectId == req.SubjectId &&
            m.Term == req.Term &&
            m.Year == req.Year)
            .FirstOrDefaultAsync(cancellationToken);

        // Calculate growth from previous score
        int? growth = null;
        var existingId = existing?.ID ?? 0;
        var previousScore = await mapScoreRepository.Get(m =>
            m.StudentId == req.StudentId &&
            m.SubjectId == req.SubjectId &&
            m.ID != existingId)
            .OrderByDescending(m => m.Year)
            .ThenByDescending(m => m.Term)
            .FirstOrDefaultAsync(cancellationToken);

        if (previousScore != null)
        {
            growth = req.Score - previousScore.Score;
        }

        PortfolioMapScore entity;
        if (existing != null)
        {
            // Update existing
            existing.Score = req.Score;
            existing.DateTaken = req.DateTaken;
            existing.Percentile = req.Percentile;
            existing.Growth = growth;
            entity = existing;
        }
        else
        {
            // Create new
            entity = new PortfolioMapScore
            {
                StudentId = req.StudentId,
                SubjectId = req.SubjectId,
                Term = req.Term,
                Year = req.Year,
                Score = req.Score,
                DateTaken = req.DateTaken,
                Percentile = req.Percentile,
                Growth = growth
            };
            mapScoreRepository.Add(entity);
        }

        await mapScoreRepository.SaveChangesAsync(cancellationToken);

        var dto = new PortfolioMapScoreDto
        {
            Id = entity.ID,
            Term = entity.Term,
            Year = entity.Year,
            Score = entity.Score,
            DateTaken = entity.DateTaken,
            Percentile = entity.Percentile,
            Growth = entity.Growth,
            GoalScore = entity.GoalScore
        };

        return RequestResult<PortfolioMapScoreDto>.Success(dto);
    }
}
