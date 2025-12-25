using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Student.PortfolioBook.Commands;

public record SavePortfolioAssignmentCommand(SavePortfolioAssignmentRequest Request) : IRequest<RequestResult<PortfolioAssignmentDto>>;

public class SavePortfolioAssignmentCommandHandler(
    RequestHandlerBaseParameters parameters)
    : RequestHandlerBase<SavePortfolioAssignmentCommand, RequestResult<PortfolioAssignmentDto>>(parameters)
{
    public override async Task<RequestResult<PortfolioAssignmentDto>> Handle(SavePortfolioAssignmentCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // TODO: Implement database save when entity is created
        // If Id is null, create new; otherwise update existing
        var assignment = new PortfolioAssignmentDto
        {
            Id = request.Request.Id ?? 1, // Generate ID on create
            Name = request.Request.Name,
            DueDate = request.Request.DueDate,
            Status = request.Request.Status,
            Notes = request.Request.Notes
        };

        return RequestResult<PortfolioAssignmentDto>.Success(assignment);
    }
}
