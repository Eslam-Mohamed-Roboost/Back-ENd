using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Student.PortfolioBook.Commands;

public record SavePortfolioProfileCommand(SavePortfolioProfileRequest Request) : IRequest<RequestResult<PortfolioProfileDto>>;

public class SavePortfolioProfileCommandHandler(
    RequestHandlerBaseParameters parameters)
    : RequestHandlerBase<SavePortfolioProfileCommand, RequestResult<PortfolioProfileDto>>(parameters)
{
    public override async Task<RequestResult<PortfolioProfileDto>> Handle(SavePortfolioProfileCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // TODO: Implement database save when entity is created
        var profile = new PortfolioProfileDto
        {
            FullName = request.Request.FullName,
            GradeSection = request.Request.GradeSection,
            FavoriteThings = request.Request.FavoriteThings,
            Uniqueness = request.Request.Uniqueness,
            FutureDream = request.Request.FutureDream
        };

        return RequestResult<PortfolioProfileDto>.Success(profile);
    }
}
