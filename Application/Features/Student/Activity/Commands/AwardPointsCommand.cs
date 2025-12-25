using API.Application.Features.Student.Activity.DTOs;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Student.Activity.Commands;

public record AwardPointsCommand(AwardPointsRequest Request) : IRequest<RequestResult<bool>>;

public class AwardPointsCommandHandler(
    RequestHandlerBaseParameters parameters)
    : RequestHandlerBase<AwardPointsCommand, RequestResult<bool>>(parameters)
{
    public override async Task<RequestResult<bool>> Handle(AwardPointsCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement when PointsHistory entity is created
        // This is system-triggered to award points for various activities
        
        return RequestResult<bool>.Success(true);
    }
}
