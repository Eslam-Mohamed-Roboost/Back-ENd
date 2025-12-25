using API.Application.Features.Student.PortfolioBook.DTOs;
using API.Shared.Models;
using MediatR;

namespace API.Application.Features.Teacher.PortfolioBook.Commands;

public record UpdateExactPathCommand(UpdateExactPathRequest Request) : IRequest<RequestResult<ExactPathProgressDto>>;

public class UpdateExactPathCommandHandler(
    RequestHandlerBaseParameters parameters)
    : RequestHandlerBase<UpdateExactPathCommand, RequestResult<ExactPathProgressDto>>(parameters)
{
    public override async Task<RequestResult<ExactPathProgressDto>> Handle(UpdateExactPathCommand request, CancellationToken cancellationToken)
    {
        var teacherId = _userState.UserID;

        // TODO: Implement database save when entity is created
        // Verify teacher has access to this student's data
        var exactPathProgress = new ExactPathProgressDto
        {
            Reading = request.Request.Reading,
            Vocabulary = request.Request.Vocabulary,
            Grammar = request.Request.Grammar
        };

        return RequestResult<ExactPathProgressDto>.Success(exactPathProgress);
    }
}
