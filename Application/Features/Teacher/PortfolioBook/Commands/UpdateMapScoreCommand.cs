using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Teacher.PortfolioBook.Commands;

public record UpdateMapScoreCommand(UpdateMapScoreRequest Request) : IRequest<RequestResult<PortfolioMapScoreDto>>;

public class UpdateMapScoreCommandHandler(
    RequestHandlerBaseParameters parameters)
    : RequestHandlerBase<UpdateMapScoreCommand, RequestResult<PortfolioMapScoreDto>>(parameters)
{
    public override async Task<RequestResult<PortfolioMapScoreDto>> Handle(UpdateMapScoreCommand request, CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        // TODO: Implement database save when entity is created
        // Verify teacher has access to this student's data
        var mapScore = new PortfolioMapScoreDto
        {
            Id = 1, // Generate ID on create
            Term = request.Request.Term,
            Year = request.Request.Year,
            Score = request.Request.Score,
            DateTaken = request.Request.DateTaken,
            Percentile = request.Request.Percentile,
            Growth = null,
            GoalScore = null
        };

        return RequestResult<PortfolioMapScoreDto>.Success(mapScore);
    }
}
